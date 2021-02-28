using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstants.Commnads
{
    public interface IRegisterOrderCommand
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FileUrl { get; set; }
        public byte[] FileData { get; set; }
    }
}
