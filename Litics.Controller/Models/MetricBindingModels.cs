using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Litics.Controller.Models
{
    public class PostDataBindigModel
    {
        [Required]
        [Display(Name = "Data Type")]
        public string Type { get; set; }
        [Required]
        [Display(Name = "Object Metric Value")]
        public Object Value { get; set; }
    }
    public class GetDataBindigModel
    {
        [Required]
        [Display(Name = "Data Type")]
        public string Type { get; set; }
        [Required]
        [Display(Name = "From Date")]
        public string FromDateMath { get; set; }
        [Display(Name = "To Date")]
        public string ToDateMath { get; set; }
    }
}