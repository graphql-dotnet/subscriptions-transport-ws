using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Language.AST;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authorization;

namespace GraphQL.Server.Authorization.AspNetCore
{
    /// <summary>
    /// GraphQL authorization validation rule which integrates to ASP.NET Core authorization mechanism.
    /// For more information see https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction.
    /// </summary>
    public class AuthorizationValidationRule : IValidationRule
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IClaimsPrincipalAccessor _claimsPrincipalAccessor;

        /// <summary>
        /// Creates an instance of <see cref="AuthorizationValidationRule"/>.
        /// </summary>
        /// <param name="authorizationService"> ASP.NET Core <see cref="IAuthorizationService"/> to authorize against. </param>
        /// <param name="claimsPrincipalAccessor"> The <see cref="IClaimsPrincipalAccessor"/> which provides the <see cref="ClaimsPrincipal"/> for authorization. </param>
        public AuthorizationValidationRule(IAuthorizationService authorizationService, IClaimsPrincipalAccessor claimsPrincipalAccessor)
        {
            _authorizationService = authorizationService;
            _claimsPrincipalAccessor = claimsPrincipalAccessor;
        }

        /// <inheritdoc />
        public async Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            await AuthorizeAsync(null, context.Schema, context, null);
            var operationType = OperationType.Query;

            // this could leak info about hidden fields or types in error messages
            // it would be better to implement a filter on the Schema so it
            // acts as if they just don't exist vs. an auth denied error
            // - filtering the Schema is not currently supported
            // TODO: apply ISchemaFilter - context.Schema.Filter.AllowXXX

            return new NodeVisitors(
                new MatchingNodeVisitor<Operation>((astType, context) =>
                {
                    operationType = astType.OperationType;

                    var type = context.TypeInfo.GetLastType();
                    AuthorizeAsync(astType, type, context, operationType).GetAwaiter().GetResult(); // TODO: need to think of something to avoid this
                }),
                new MatchingNodeVisitor<ObjectField>((objectFieldAst, context) =>
                {
                    if (context.TypeInfo.GetArgument()?.ResolvedType.GetNamedType() is IComplexGraphType argumentType)
                    {
                        var fieldType = argumentType.GetField(objectFieldAst.Name);
                        AuthorizeAsync(objectFieldAst, fieldType, context, operationType).GetAwaiter().GetResult(); // TODO: need to think of something to avoid this
                    }
                }),
                new MatchingNodeVisitor<Field>((fieldAst, context) =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();

                    if (fieldDef == null)
                        return;

                    // check target field
                    AuthorizeAsync(fieldAst, fieldDef, context, operationType).GetAwaiter().GetResult(); // TODO: need to think of something to avoid this
                    // check returned graph type
                    AuthorizeAsync(fieldAst, fieldDef.ResolvedType.GetNamedType(), context, operationType).GetAwaiter().GetResult(); // TODO: need to think of something to avoid this
                })
            );
        }

        private async Task AuthorizeAsync(INode node, IProvideMetadata provider, ValidationContext context, OperationType? operationType)
        {
            var policyNames = provider?.GetPolicies();

            if (policyNames?.Count == 1)
            {
                // small optimization for the single policy - no 'new List<>()', no 'await Task.WhenAll()'
                var authorizationResult = await _authorizationService.AuthorizeAsync(_claimsPrincipalAccessor.GetClaimsPrincipal(context), policyNames[0]);
                if (!authorizationResult.Succeeded)
                    AddValidationError(node, context, operationType, authorizationResult);
            }
            else if (policyNames?.Count > 1)
            {
                var claimsPrincipal = _claimsPrincipalAccessor.GetClaimsPrincipal(context);
                var tasks = new List<Task<AuthorizationResult>>(policyNames.Count);
                foreach (string policyName in policyNames)
                {
                    var task = _authorizationService.AuthorizeAsync(claimsPrincipal, policyName);
                    tasks.Add(task);
                }

                var authorizationResults = await Task.WhenAll(tasks);

                foreach (var result in authorizationResults)
                {
                    if (!result.Succeeded)
                        AddValidationError(node, context, operationType, result);
                }
            }
        }

        /// <summary>
        /// Adds an authorization failure error to the document response
        /// </summary>
        protected virtual void AddValidationError(INode node, ValidationContext context, OperationType? operationType, AuthorizationResult result)
        {
            context.ReportError(new AuthorizationError(node, context, operationType, result));
        }
    }
}
