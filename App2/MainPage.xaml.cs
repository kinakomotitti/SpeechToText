using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace App2
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.Recognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            await this.Recognizer.CompileConstraintsAsync();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "en-us".
            var config = SpeechConfig.FromSubscription("859b3e1bc5514b60819d09a4db722436", "eastus");
            await this.Test2(config);
            
        }
        private async Task Test2(SpeechConfig config)
        {
            // Creates a speech recognizer using microphone as audio input.
            using (var recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(config))
            {
                // Starts recognizing.
                await Task.Run(async () =>
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        this.test2.Text += ("Say something...\r\n");
                    });
                });

                // Performs recognition. RecognizeOnceAsync() returns when the first utterance has been recognized,
                // so it is suitable only for single shot recognition like command or query. For long-running
                // recognition, use StartContinuousRecognitionAsync() instead.
                var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    await Task.Run(async () =>
                    {
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            this.test2.Text += ($"RECOGNIZED: Text={result.Text}\r\n");
                        });
                    });
                    await this.Test2(config);
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    await Task.Run(async () =>
                    {
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            this.test2.Text += ($"CANCELED: Reason={cancellation.Reason}\r\n");

                            if (cancellation.Reason == CancellationReason.Error)
                            {
                                this.test2.Text += ($"CANCELED: ErrorCode={cancellation.ErrorCode}\r\n");
                                this.test2.Text += ($"CANCELED: ErrorDetails={cancellation.ErrorDetails}\r\n");
                                this.test2.Text += ($"CANCELED: Did you update the subscription info?\r\n");
                            }

                        });
                    });
                }
            }
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await this.Test();
        }

        private async Task Test()
        {
            var result = await this.Recognizer.RecognizeAsync();
            this.test.Text += result.Text;
            this.test.Text += "\r\n";
            await this.Test();
        }

        private Windows.Media.SpeechRecognition.SpeechRecognizer Recognizer { get; set; }

    }
}
