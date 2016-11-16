using Windows.UI.Xaml.Controls;

namespace FlipperControl
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void PrevBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var index = this.FlipperControl.DisplayIndex - 1;
            if (index < 0) index = this.FlipperControl.Views.Count - 1;
            this.FlipperControl.DisplayIndex = index;
        }

        private void NextBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var index = this.FlipperControl.DisplayIndex + 1;
            if (index > this.FlipperControl.Views.Count) index = 0;
            this.FlipperControl.DisplayIndex = index;
        }
    }
}
