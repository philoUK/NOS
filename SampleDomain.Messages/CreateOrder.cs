using NewOrbit.Messaging;

namespace SampleDomain.Messages
{
    public class CreateOrder : ICommand
    {
        public string CorrelationId { get; set; }
        public string Id { get; set; }
        public string ReferenceCode { get; set; }
        public string CustomerCode { get; set; }
    }
}
