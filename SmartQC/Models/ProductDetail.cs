﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQC.Models
{
    public class ProductDetail
    {
        [Key]
        public string? SerialNumber { get; set; }
        public string? ProductName { get; set; }
        public string? UserName { get; set; }
        public bool Defection { get; set; }
    }
}
