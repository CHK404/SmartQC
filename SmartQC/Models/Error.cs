using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQC.Models
{
    public class Error
    {
        [Key]
        public string? ErrorCode { get; set; }
        public string? ErrorType { get; set; }
        public string? SerialNumber { get; set; }
    }
}
