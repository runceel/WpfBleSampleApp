using System;
using System.Linq;
using System.Windows;
using Windows.Devices.Bluetooth.Advertisement;

namespace WpfBleSampleApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private BluetoothLEAdvertisementWatcher watcher;

        public MainWindow()
        {
            InitializeComponent();
            this.watcher = new BluetoothLEAdvertisementWatcher();

            // CompanyIDとかDataでフィルタリングしたいとき
            //var md = new BluetoothLEManufacturerData();
            //// company id 0xFFFF (多分これ https://www.bluetooth.com/specifications/assigned-numbers/company-Identifiers)
            //md.CompanyId = 0xFFFF; 

            //// data 0x1234
            //var w = new DataWriter();
            //w.WriteUInt16(0x1234);
            //md.Data = w.DetachBuffer();
            //this.watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(md);

            // rssi >= -60のとき受信開始するっぽい
            this.watcher.SignalStrengthFilter.InRangeThresholdInDBm = -60;
            // rssi <= -65が2秒続いたら受信終わるっぽい
            this.watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -65;
            this.watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(2000);
            this.watcher.Received += this.Watcher_Received;
        }

        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var md = args.Advertisement.ManufacturerData.FirstOrDefault();
                if (md != null)
                {
                    // ManufactureDataをもとにCompanyIDとったりできる
                }
                this.TextBlockRSSI.Text = $"{args.Timestamp:HH\\:mm\\:ss}, RSSI: {args.RawSignalStrengthInDBm}, Address: {args.BluetoothAddress.ToString("X")}, Type: {args.AdvertisementType}";
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.watcher.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.watcher.Start();
        }
    }
}
