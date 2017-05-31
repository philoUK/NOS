using System;

namespace NewOrbit.Messaging.Command.Azure
{
    [Serializable]
    public class MessageUnpackingException : Exception
    {
        public MessageUnpackingException(string message, Exception inner) : base(message,inner)
        {
        }

    }
}