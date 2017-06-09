using System;
using System.Collections.Generic;
using System.Text;

namespace NewOrbit.Messaging.Saga
{
    public interface ISaga
    {
        void Initialise();
        void Load(ISagaData data);
    }
}
