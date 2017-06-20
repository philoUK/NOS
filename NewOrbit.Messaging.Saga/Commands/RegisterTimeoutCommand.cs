using System;
using System.Collections.Generic;

namespace NewOrbit.Messaging.Saga.Commands
{
    public class RegisterTimeoutCommand : ICommand
    {
        public string CorrelationId { get; set; }
        public string Id { get; set; }
        public string MethodName { get; set; }
        public DateTime Timeout { get; set; }
        public string OwnerType { get; set; }
    }
}
