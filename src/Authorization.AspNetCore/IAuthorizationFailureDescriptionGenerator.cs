using GraphQL.Language.AST;
using Microsoft.AspNetCore.Authorization;

namespace GraphQL.Server.Authorization.AspNetCore
{
    /// <summary>
    /// The interface for an authorization failure description generator
    /// </summary>
    public interface IAuthorizationFailureDescriptionGenerator
    {
        /// <summary>
        /// Generates a failure description for the given <paramref name="result"/> 
        /// </summary>
        /// <param name="result">The ASP.NET Core authorization result</param>
        /// <param name="operationType">The GraphQL operation type. Not set if authorization occurs for the schema itself</param>
        /// <returns></returns>
        string GenerateFailureDescription(AuthorizationResult result, OperationType? operationType);
    }
}