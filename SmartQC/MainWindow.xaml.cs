using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartQC.ViewModels;

namespace SmartQC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainViewModel();
            this.DataContext = vm;

            vm.AvailabilityPlotControl = this.AvailabilityPlotControl;
            vm.PlotControl = this.PlotControl;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is ProgressBar pb)
            {
                // Template에서 PART_Indicator를 찾아옴
                if (pb.Template.FindName("PART_Indicator", pb) is FrameworkElement indicator)
                {
                    double maxWidth = pb.ActualWidth; // ProgressBar 전체 너비
                    double newWidth = e.NewValue / pb.Maximum * maxWidth;

                    // 애니메이션 생성 (0.3초 동안 Width 변경)
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        To = newWidth,
                        Duration = TimeSpan.FromSeconds(0.3),
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    };

                    // 애니메이션 시작
                    indicator.BeginAnimation(FrameworkElement.WidthProperty, animation);
                }
            }
        }
    }
}