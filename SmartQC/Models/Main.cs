using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQC.Models
{
    public class Main
    {
        public int Errorcounts {  get; set; }
        public int Totaldaycount { get; set; }
        public int Totalweekcount { get; set; }
        public int Totalmonthcount { get; set; }
        public int Completedcount { get; set; }
        public TimeSpan TotalWorkingTime { get; set; } = TimeSpan.Zero;

        public int PlannedTimeInMinutes { get; set; }
        public double AvailabilityRate {  get; set; }





    }
}
