using NewOrbit.Messaging;

namespace SampleDomain.Messages
{
    public class BillCustomer : ICommand
    {
        public string CorrelationId { get; set; }
        public string Id { get; set; }
        public string Reference { get; set; }
        public string CustomerCode { get; set; }
    }
}
