using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQC.Models
{
    public class UserData
    {
        [Key]
        public string? UserName { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? FaceURL { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
