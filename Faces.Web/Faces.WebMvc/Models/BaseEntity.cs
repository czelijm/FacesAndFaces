using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.Models
{
    public class BaseEntity
    {
        [Display(Name="Id")]
        public Guid Id { get; set; }
    }
}
