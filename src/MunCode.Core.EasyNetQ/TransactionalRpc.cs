﻿namespace MunCode.Core
{
    using System;
    using System.Collections.Concurrent;

    using EasyNetQ;
    using EasyNetQ.Producer;
    using EasyNetQ.Topology;

    using MunCode.Core.Messaging.Messages;

    public class TransactionalRpc : Rpc
    {
        private readonly ITimeoutStrategy timeoutStrategy;
        private readonly ConcurrentDictionary<string, IQueue> declaredQueues = new ConcurrentDictionary<string, IQueue>();

        public TransactionalRpc(ConnectionConfiguration connectionConfiguration, IAdvancedBus advancedBus, IEventBus eventBus, IConventions conventions, IPublishExchangeDeclareStrategy publishExchangeDeclareStrategy, IMessageDeliveryModeStrategy messageDeliveryModeStrategy, ITimeoutStrategy timeoutStrategy, ITypeNameSerializer typeNameSerializer)
            : base(connectionConfiguration, advancedBus, eventBus, conventions, publishExchangeDeclareStrategy, messageDeliveryModeStrategy, timeoutStrategy, typeNameSerializer)
        {
            this.timeoutStrategy = timeoutStrategy;
        }

        protected override void RequestPublish<TRequest>(
            TRequest request,
            string routingKey,
            string returnQueueName,
            Guid correlationId)
        {
            this.declaredQueues.GetOrAdd(routingKey, key => this.advancedBus.QueueDeclare(routingKey));
            var messageType = typeof(TRequest);
            var timeout = request is ICommand ? null : (this.timeoutStrategy.GetTimeoutSeconds(messageType) * 1000UL).ToString();
            var message = new Message<TRequest>(request)
                                            {
                                                Properties =
                                                    {
                                                        ReplyTo = returnQueueName,
                                                        CorrelationId = correlationId.ToString(),
                                                        DeliveryMode = this.messageDeliveryModeStrategy.GetDeliveryMode(messageType)
                                                    }
                                            };
            if (timeout != null)
            {
                message.Properties.Expiration = timeout;
            }

            this.advancedBus.Publish(Exchange.GetDefault(), routingKey, false, message);
        }
    }
}