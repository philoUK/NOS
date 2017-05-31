using System;
using NewOrbit.Messaging;

namespace MessagingFacts.Messages
{
    public class TestCommand : ICommand
    {
        public TestCommand()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
}
