using GraphQL.Subscription;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GraphQL.Server.Transports.Subscriptions.Abstractions
{
    /// <summary>
    ///     Internal observer of the subscription
    /// </summary>
    public class Subscription
    {
        private readonly Action<Subscription> _completed;
        private readonly ILogger<Subscription> _logger;
        private readonly IWriterPipeline _writer;
        private IDisposable _unsubscribe;

        public Subscription(string id,
            OperationMessagePayload payload,
            SubscriptionExecutionResult result,
            IWriterPipeline writer,
            Action<Subscription> completed,
            ILogger<Subscription> logger)
        {
            _writer = writer;
            _completed = completed;
            _logger = logger;
            Id = id;
            OriginalPayload = payload;

            Subscribe(result);
        }

        public string Id { get; }

        public OperationMessagePayload OriginalPayload { get; }


        private async Task SendComplete()
        {
            _logger.LogDebug("Subscription: {subscriptionId} completing", Id);
            await _writer.SendAsync(new OperationMessage
            {
                Type = MessageType.GQL_COMPLETE,
                Id = Id
            });

            _completed?.Invoke(this);
            _unsubscribe?.Dispose();
        }

        private async Task SendData(ExecutionResult value)
        {
            _logger.LogDebug("Subscription: {subscriptionId} got data", Id);
            await _writer.SendAsync(new OperationMessage
            {
                Type = MessageType.GQL_DATA,
                Id = Id,
                Payload = value
            });
        }

        public Task UnsubscribeAsync()
        {
            _logger.LogDebug("Subscription: {subscriptionId} unsubscribing", Id);
            _unsubscribe.Dispose();
            return _writer.SendAsync(new OperationMessage
            {
                Type = MessageType.GQL_COMPLETE,
                Id = Id
            });
        }

        private void Subscribe(SubscriptionExecutionResult result)
        {
            var stream = result.Streams.Values.Single();
            _unsubscribe = stream.Synchronize()
                .Select(value => Observable.FromAsync(() => SendData(value)))
                .Merge(1)
                .Catch((Exception e) =>
                {
                    _logger.LogError(e, "Subscription: {subscriptionId} exception occurred", Id);
                    return Observable.Empty<Unit>();
                })
                .Concat(Observable.FromAsync(SendComplete))
                .Subscribe();

            _logger.LogDebug("Subscription: {subscriptionId} subscribed", Id);
        }
    }
}