using System;
using NewOrbit.Messaging;
using NewOrbit.Messaging.Saga;
using SampleDomain.Messages;

namespace SampleDomain.Handlers
{
    public class OrderLifecycle : ISagaData
    {
        public bool Created { get; set; }
        public bool Cancelled { get; set; }
        public string Id { get; set; }
        public string Reference { get; set; }
        public string CustomerCode { get; set; }
    }

    public class OrderCreationSaga : Saga<OrderLifecycle>, IHandleCommandsOf<CreateOrder>
    {
        public OrderCreationSaga(IClientCommandBus commandBus, IEventBus eventBus) : base(commandBus, eventBus)
        {
        }

        protected override OrderLifecycle CreateData(string id)
        {
            return new OrderLifecycle
            {
                Cancelled = false,
                Created = false,
                Id = id
            };
        }

        public void HandleCommand(CreateOrder command)
        {
            // an order gets 24 hours cooling off period before its actually placed,
            this.Data.Created = true;
            this.Data.Reference = command.ReferenceCode;
            this.Data.CustomerCode = command.CustomerCode;
            this.RegisterTimeout(nameof(this.CooldownExpiry), TimeSpan.FromHours(24));
        }

        private void CooldownExpiry()
        {
            if (!Data.Cancelled)
            {
                this.SubmitCommand(new BillCustomer
                {
                    CorrelationId = this.SagaId,
                    Id = Guid.NewGuid().ToString(),
                    Reference = this.Data.Reference,
                    CustomerCode = this.Data.CustomerCode
                });
                // kill this saga as well no doubt.
            }
        }
    }
}
