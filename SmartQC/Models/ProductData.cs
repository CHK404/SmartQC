using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQC.Models
{
    public class ProductData
    {
        [Key]
        public string? ProductName { get; set; }
        public int Quantity {  get; set; }
        public int Defective {  get; set; }
    }
}
