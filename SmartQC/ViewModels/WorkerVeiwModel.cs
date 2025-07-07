using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartQC.Models;
using ScottPlot;
using ScottPlot.WPF;
using System.Reflection.Emit;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;


namespace SmartQC.ViewModels
{
    public partial class WorkerVeiwModel : ObservableObject
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private readonly Random random = new Random();
        private readonly List<double> _defectRates = new List<double>(); // Y축: 불량률 기록 리스트
        private readonly List<double> _timePoints = new List<double>(); // X축: 시간 기록 리스트
        private double _time = 0; // 현재 시간 (초)
        private DateTime _startTime = DateTime.Now; // 기준 시작 시간
        public WpfPlot? PlotControl { get; set; } // XAML에서 연결되는 Plot 객체
        [ObservableProperty]
        public double temperature;
        [ObservableProperty]
        public double humidity;
        [ObservableProperty]
        public double contamination_level;
        [ObservableProperty]
        public bool drability;
        [ObservableProperty]
        public int errorscount;
        [ObservableProperty]
        public int completecount;
        [ObservableProperty]
        public int needtocomplete = 40;
        [ObservableProperty]
        public int progressValue = 0;

        public WorkerVeiwModel()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Ontimer; 
            _timer.Start();
        }

        public void Ontimer(object sender, EventArgs e)
        { 
            MakeValue();
        }

        public void MakeValue()
        {
            Temperature = Math.Round(random.NextDouble() * 10 + 19.0, 1);       
            Humidity = Math.Round(random.NextDouble() * 30 + 35.0, 1);           
            Contamination_level = Math.Round(random.NextDouble() * 14 + 3.0, 1); 
            Drability = true; 
        }

        public void HandleOpenCVResult()
        {
            completecount++; // 생산 수량 증가

            // 10% 확률로 불량 발생
            bool isDefective = random.NextDouble() < 0.1;

            if (isDefective)
                errorscount++;

            // 불량률 계산 (0으로 나누기 방지)
            double defectRate = completecount > 0
                ? (double)errorscount / completecount * 100.0
                : 0;

            // 시간 경과
            _time = (DateTime.Now - _startTime).TotalSeconds;
            _timePoints.Add(_time);
            _defectRates.Add(defectRate);

            UpdatePlot();
        }

        private void UpdatePlot() // ScottPlot 갱신 메서드
        {
            if (PlotControl == null) return; // 연결된 Plot이 없으면 종료

            var plt = PlotControl.Plot; // 플롯 참조
            plt.Clear(); // 기존 그래프 초기화

            plt.Add.Scatter(xs:_timePoints.ToArray(),ys: _defectRates.ToArray()); // 점 그래프 추가
            plt.XLabel("Elapsed Time (s)"); // X축 라벨 설정
            plt.YLabel("Defect Rate (%)"); // Y축 라벨 설정
            plt.Title("Defect Rate Over Time"); // 그래프 제목
            plt.Axes.SetLimitsY(60,100); // Y축 최소값 고정

            PlotControl.Refresh(); // 그래프 새로 고침
        }


        [RelayCommand]
        private void Updateprogressbar() 
        {
            if (Needtocomplete <= 0) 
            {
                progressValue = 0;
                return;
            }
            progressValue = (completecount - errorscount)*100 / Needtocomplete;
        }
    }
}