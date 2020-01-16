using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using GraphQL.Subscription;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace GraphQL.Server.Transports.Subscriptions.Abstractions.Tests
{
    public class SubscriptionFacts
    {
        public SubscriptionFacts()
        {
            _writer = Substitute.For<IWriterPipeline>();
        }

        private readonly IWriterPipeline _writer;

        [Fact]
        public void On_data_from_stream()
        {
            /* Given */
            var id = "1";
            var payload = new OperationMessagePayload();
            var stream = new ReplaySubject<ExecutionResult>(1);
            var result = new SubscriptionExecutionResult
            {
                Streams = new Dictionary<string, IObservable<ExecutionResult>>
                {
                    { "op", stream }
                }
            };
            var expected = new ExecutionResult();
            var sut = new Subscription(id, payload, result, _writer, null, new NullLogger<Subscription>());

            /* When */
            stream.OnNext(expected);

            /* Then */
            _writer.Received().SendAsync(
                Arg.Is<OperationMessage>(
                    message => message.Id == id
                               && message.Type == MessageType.GQL_DATA));
        }

        [Fact]
        public void On_stream_complete()
        {
            /* Given */
            var id = "1";
            var payload = new OperationMessagePayload();
            var stream = new ReplaySubject<ExecutionResult>(1);
            var result = new SubscriptionExecutionResult
            {
                Streams = new Dictionary<string, IObservable<ExecutionResult>>
                {
                    { "op", stream }
                }
            };

            var completed = Substitute.For<Action<Subscription>>();
            var sut = new Subscription(id, payload, result, _writer, completed, new NullLogger<Subscription>());

            /* When */
            stream.OnCompleted();

            /* Then */
            Assert.False(stream.HasObservers);
            completed.Received().Invoke(sut);
            _writer.Received().SendAsync(
                Arg.Is<OperationMessage>(
                    message => message.Id == id
                               && message.Type == MessageType.GQL_COMPLETE));
        }

        [Fact]
        public void Subscribe_to_stream()
        {
            /* Given */
            var id = "1";
            var payload = new OperationMessagePayload();
            var stream = new Subject<ExecutionResult>();
            var result = new SubscriptionExecutionResult
            {
                Streams = new Dictionary<string, IObservable<ExecutionResult>>
                {
                    { "op", stream }
                }
            };

            /* When */
            var sut = new Subscription(id, payload, result, _writer, null, new NullLogger<Subscription>());

            /* Then */
            Assert.True(stream.HasObservers);
        }

        [Fact]
        public void Subscribe_to_completed_stream_should_not_throw()
        {
            /* Given */
            var id = "1";
            var payload = new OperationMessagePayload();
            var subject = new Subject<ExecutionResult>();
            subject.OnCompleted();
            var stream = subject;
            var result = new SubscriptionExecutionResult
            {
                Streams = new Dictionary<string, IObservable<ExecutionResult>>
                {
                    { "op", stream }
                }
            };

            /* When */
            /* Then */
            var sut = new Subscription(id, payload, result, _writer, null, new NullLogger<Subscription>());
        }

        [Fact]
        public async Task Unsubscribe_from_stream()
        {
            /* Given */
            var id = "1";
            var payload = new OperationMessagePayload();
            var unsubscribe = Substitute.For<IDisposable>();
            var stream = Substitute.For<IObservable<ExecutionResult>>();
            stream.Subscribe(null).ReturnsForAnyArgs(unsubscribe);
            var result = new SubscriptionExecutionResult
            {
                Streams = new Dictionary<string, IObservable<ExecutionResult>>
                {
                    { "op", stream }
                }
            };
            var sut = new Subscription(id, payload, result, _writer, null, new NullLogger<Subscription>());

            /* When */
            await sut.UnsubscribeAsync();

            /* Then */
            unsubscribe.Received().Dispose();
        }

        [Fact]
        public async Task Write_Complete_on_unsubscribe()
        {
            /* Given */
            var id = "1";
            var payload = new OperationMessagePayload();
            var result = new SubscriptionExecutionResult
            {
                Streams = new Dictionary<string, IObservable<ExecutionResult>>
                {
                    { "1", Substitute.For<IObservable<ExecutionResult>>() }
                }
            };

            var sut = new Subscription(id, payload, result, _writer, null, new NullLogger<Subscription>());

            /* When */
            await sut.UnsubscribeAsync();

            /* Then */
            await _writer.Received().SendAsync(
                Arg.Is<OperationMessage>(
                    message => message.Id == id
                               && message.Type == MessageType.GQL_COMPLETE));
        }
    }
}