using System;

namespace NewOrbit.Messaging.Timeouts
{
    public class TimeoutData
    {
        public string TargetId { get; set; }
        public string TargetMethod { get; set; }
        public DateTime Timeout { get; set; }
        public string TargetType { get; set; }
        public int NumberOfRetries { get; set; }
    }
}