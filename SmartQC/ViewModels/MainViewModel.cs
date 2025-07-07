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
        public int totaldaycount = 20;
        [ObservableProperty]
        public int totalweekcount = 140;
        [ObservableProperty]
        public int totalmonthcount = 4200;
        [ObservableProperty]
        public int completedcount;
        [ObservableProperty]
        public TimeSpan totalWorkingTime;
        [ObservableProperty]
        private DateTime _sessionStartTime;
        [ObservableProperty]
        public int plannedTimeInMinutes;
        [ObservableProperty]
        public double dayprogressvalue;
        [ObservableProperty]
        public double weekprogressvalue; 
        [ObservableProperty]
        public double monthprogressvalue;
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
        [ObservableProperty]
        private bool isRunning = false;
        [ObservableProperty]
        private bool plcRunning = false;

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

            // 랜덤하게 작동/정지 상태를 시뮬레이션 (예: 95% 확률로 작동 중)
            plcRunning = random.NextDouble() < 0.95;
            if (!plcRunning)
            {
                Stop(); // 수동으로 메서드 호출
                return;
            }
            if (IsRunning&&plcRunning)
                totalWorkingTime = totalWorkingTime.Add(TimeSpan.FromSeconds(1));

            var plannedElapsedTime = DateTime.Now - _sessionStartTime;

            if (plannedElapsedTime.TotalSeconds == 0)
                AvailabilityRate = 0;
            else
                AvailabilityRate = totalWorkingTime.TotalSeconds / plannedElapsedTime.TotalSeconds * 100.0;

            TimeHistory.Add(elapsedSeconds);
            AvailabilityHistory.Add(AvailabilityRate);

            UpdateAvailabiltyPlot();
            HandleOpenCVResult();
        }

        private void UpdateAvailabiltyPlot()
        {
            if (AvailabilityPlotControl == null) return;

            var plt = AvailabilityPlotControl.Plot;
            plt.Clear();
            plt.Add.Scatter(xs: TimeHistory.ToArray(), ys: AvailabilityHistory.ToArray());
            plt.XLabel("Elapsed Time (s)");
            plt.YLabel("Availability (%)");
            plt.Title("Real-Time Availability Simulation");
            plt.Axes.SetLimitsY(20, 100);
            AvailabilityPlotControl.Refresh();
        }

        public void HandleOpenCVResult()
        {
            if (!IsRunning||!plcRunning) {return;}
            completedcount++; // 생산 수량 증가

            // 10% 확률로 불량 발생
            bool isDefective = random.NextDouble() < 0.10;

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

            UpdateErrorPlot();
            Updateprogressbar();
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
            plt.Axes.SetLimitsY(-5, 30); // Y축 최소값 고정

            PlotControl.Refresh(); // 그래프 새로 고침
        }

        private void Updateprogressbar()
        {
            if (Totaldaycount <= 0)
            {
                Dayprogressvalue = 0;
                return;
            }
            Dayprogressvalue = ((double)(completedcount - errorcounts) / Totaldaycount) * 100.0;
            if (Totalweekcount <= 0)
            {
                Weekprogressvalue = 0;
                return;
            }
            Weekprogressvalue = ((double)(completedcount - errorcounts) / Totalweekcount) * 100.0;
            if (Totalmonthcount <= 0)
            {
                Monthprogressvalue = 0;
                return;
            }
            Monthprogressvalue = ((double)(completedcount - errorcounts) / Totalmonthcount) * 100.0;


        }

        [RelayCommand]
        private void Start()
        {
            IsRunning = true;
            plcRunning = true;
        }

        [RelayCommand]
        private void Stop()
        {
            IsRunning = false;
            plcRunning = false;
        }
    }
}



    

