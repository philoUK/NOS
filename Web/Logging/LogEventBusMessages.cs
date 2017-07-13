using NewOrbit.Messaging;
using NewOrbit.Messaging.Event;

namespace Web.Logging
{
    public class LogEventBusMessages : ILogEventBusMessages
    {
        public void NoSubscribersFoundForEvent(IEvent @event)
        {
        }
    }
}
