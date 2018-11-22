using System;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace App1.Views
{
    public sealed partial class PivotPage : Page
    {
        public PivotPage()
        {
             // We use NavigationCacheMode.Required to keep track the selected item on navigation. For further information see the following links.
             // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/windows.ui.xaml.controls.page.navigationcachemode.aspx
             // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/Hh771188.aspx
             NavigationCacheMode = NavigationCacheMode.Required;
             InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.Recognizer = new SpeechRecognizer();
            await this.Recognizer.CompileConstraintsAsync();
        }

        private async void ButtonWithUI_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.Recognizer.RecognizeWithUIAsync();
            var dialog = new MessageDialog(result.Text);
            await dialog.ShowAsync();
        }

        private async void ButtonNoUI_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.Recognizer.RecognizeAsync();
            var dialog = new MessageDialog(result.Text);
            await dialog.ShowAsync();
        }
        private SpeechRecognizer Recognizer { get; set; }

    }
}
