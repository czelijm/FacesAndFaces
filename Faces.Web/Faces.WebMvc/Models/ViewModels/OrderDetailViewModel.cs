using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        //public Guid OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public byte[] FileData { get; set; }
        public string FileUrl { get; set; }
    }
}
