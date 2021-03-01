using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstants.Events
{
    public interface IOrderDispatchedEvent
    {
        Guid Id { get; set; }
        DateTime DispatchDataTime { get; }
    }
}
