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
    public partial class MainViewModel : ObservableObject
    {


        private readonly DispatcherTimer _timer = new();
        private double elapsedSeconds = 0;
        [ObservableProperty]
        public int errorcounts;
        [ObservableProperty]
        public int totaldaycount = 40;
        [ObservableProperty]
        public int totalweekcount = 300;
        [ObservableProperty]
        public int totalmonthcount = 9000;
        [ObservableProperty]
        public int completedcount;
        [ObservableProperty]
        public TimeSpan totalWorkingTime;
        [ObservableProperty]
        private DateTime _sessionStartTime;
        [ObservableProperty]
        public int plannedTimeInMinutes;
        [ObservableProperty]
        public int dayprogressvalue;
        [ObservableProperty]
        public int weekprogressvalue; 
        [ObservableProperty]
        public int monthprogressvalue;
        public List<double> TimeHistory { get; set; } = new();
        public List<double> AvailabilityHistory { get; set; } = new();
        private readonly Random random = new Random();
        private readonly List<double> _defectRates = new List<double>(); // Y축: 불량률 기록 리스트
        private readonly List<double> _timePoints = new List<double>(); // X축: 시간 기록 리스트
        private double _time = 0; // 현재 시간 (초)
        private DateTime _startTime = DateTime.Now; // 기준 시작 시간
        public WpfPlot? AvailabilityPlotControl { get; set; }
        public WpfPlot? PlotControl { get; set; }
        [ObservableProperty]
        public double availabilityRate;


        public MainViewModel()
        {
            _sessionStartTime = DateTime.Now;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTick;
            _timer.Start();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            elapsedSeconds++;

            // 랜덤하게 작동/정지 상태를 시뮬레이션 (예: 90% 확률로 작동 중)
            bool isRunning = new Random().NextDouble() < 0.9;

            if (isRunning)
                totalWorkingTime = totalWorkingTime.Add(TimeSpan.FromSeconds(1));

            var plannedElapsedTime = DateTime.Now - _sessionStartTime;

            if (plannedElapsedTime.TotalSeconds == 0)
                AvailabilityRate = 0;
            else
                AvailabilityRate = totalWorkingTime.TotalSeconds / plannedElapsedTime.TotalSeconds * 100.0;

            TimeHistory.Add(elapsedSeconds);
            AvailabilityHistory.Add(AvailabilityRate);

            UpdatePlot();
        }

        private void UpdatePlot()
        {
            if (AvailabilityPlotControl == null) return;

            var plt = AvailabilityPlotControl.Plot;
            plt.Clear();
            plt.Add.Scatter(xs: TimeHistory.ToArray(), ys: AvailabilityHistory.ToArray());
            plt.XLabel("Elapsed Time (s)");
            plt.YLabel("Availability (%)");
            plt.Title("Real-Time Availability Simulation");
            plt.Axes.SetLimitsY(0, 100);
            AvailabilityPlotControl.Refresh();
        }

        public void HandleOpenCVResult()
        {
            completedcount++; // 생산 수량 증가

            // 10% 확률로 불량 발생
            bool isDefective = random.NextDouble() < 0.1;

            if (isDefective)
                errorcounts++;

            // 불량률 계산 (0으로 나누기 방지)
            double defectRate = completedcount > 0
                ? (double)errorcounts / completedcount * 100.0
                : 0;

            // 시간 경과
            _time = (DateTime.Now - _startTime).TotalSeconds;
            _timePoints.Add(_time);
            _defectRates.Add(defectRate);

            UpdatePlot();
        }

        private void UpdateErrorPlot() // ScottPlot 갱신 메서드
        {
            if (PlotControl == null) return; // 연결된 Plot이 없으면 종료

            var plt = PlotControl.Plot; // 플롯 참조
            plt.Clear(); // 기존 그래프 초기화

            plt.Add.Scatter(xs: _timePoints.ToArray(), ys: _defectRates.ToArray()); // 점 그래프 추가
            plt.XLabel("Elapsed Time (s)"); // X축 라벨 설정
            plt.YLabel("Defect Rate (%)"); // Y축 라벨 설정
            plt.Title("Defect Rate Over Time"); // 그래프 제목
            plt.Axes.SetLimitsY(60, 100); // Y축 최소값 고정

            PlotControl.Refresh(); // 그래프 새로 고침
        }

        private void Updateprogressbar()
        {
            if (Totaldaycount <= 0)
            {
                dayprogressvalue = 0;
                return;
            }
            dayprogressvalue = (completedcount - errorcounts) * 100 / Totaldaycount;
            if (Totalweekcount <= 0)
            {
                weekprogressvalue = 0;
                return;
            }
            weekprogressvalue = (completedcount - errorcounts) * 100 / Totalweekcount;
            if (Totalmonthcount <= 0)
            {
                monthprogressvalue = 0;
                return;
            }
            monthprogressvalue = (completedcount - errorcounts) * 100 / Totalmonthcount;


        }
    }
}



    

