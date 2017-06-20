using System;
using System.Collections.Generic;

namespace NewOrbit.Messaging.Timeouts
{
    public interface ITimeoutDatabase
    {
        void Save(TimeoutData timeoutData);
        void Delete(string id, string methodName);
        IEnumerable<TimeoutData> GetExpiredTimeoutsSince(DateTime dtm);
    }
}