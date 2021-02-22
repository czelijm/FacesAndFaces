using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.Models.ViewModels
{
    public class OrderViewModel : BaseEntity
    {
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name ="Image File")]
        public IFormFile File { get; set; }
        [Display(Name ="Image Url")]
        public string FileUrl { get; set; }
        [Display(Name ="Order Status")]
        public string Status { get; set; }
        public byte[] FileData { get; set; }


    }
}
