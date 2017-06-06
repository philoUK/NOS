using System;

namespace NewOrbit.Messaging.Shared
{
    [Serializable]
    public class MessageUnpackingException : Exception
    {
        public MessageUnpackingException(string message, Exception inner) : base(message,inner)
        {
        }

    }
}