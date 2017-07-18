using System;
using NewOrbit.Messaging.Shared;

namespace NewOrbit.Messaging.Timeouts
{
    public class TimeoutData : IMessage
    {
        public string TargetId { get; set; }
        public string TargetMethod { get; set; }
        public DateTime Timeout { get; set; }
        public string TargetType { get; set; }
        public int NumberOfRetries { get; set; }

        public string CorrelationId => this.TargetId;

        public Type ExtractTargetType()
        {
            return Type.GetType(this.TargetType ?? typeof(object).AssemblyQualifiedName);
        }
    }
}