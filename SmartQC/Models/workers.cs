using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQC.Models
{
    public class workers
    {
        public int errorscount { get; set; } //OpenCV or SQL 에서 받아오는 값
        public int completecount { get; set; } //Opencv or SQL 에서 받아오는 값
        public double Temperature { get; set; } //랜덤 수
        public double Humidity { get; set; } //랜덤 수
        public bool Repare_status { get; set; } //PLC CPU정보에서 받아오기
        public string RepareText => Repare_status ? "불량" : "양호";
        public bool Durability { get; set; } //랜덤 확률 줘서
        public string DurationText => Durability ? "양호" : "불량";
        public double Contamination_level { get; set; } //랜덤 수

    }
}
