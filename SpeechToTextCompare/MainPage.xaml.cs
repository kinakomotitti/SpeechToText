using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using mic = Microsoft.CognitiveServices.Speech;
using win = Windows.Media.SpeechRecognition;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace SpeechToTextCompare
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region コンストラクタ

        public MainPage()
        {
            this.InitializeComponent();
        }

        #endregion

        #region OnNavigatedTo

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Windowsの音声認識機能を利用した音声認識機能の処理
            await this.WindowsSpeechRecognizerCore();

            //Cognitive Serviceを利用した音声認識機能の処理
            await this.MicrosoftSpeechRecofnizerCore();
        }

        #endregion


        #region Windows.Media.SpeechRecognition.SpeechRecognizer

        private win.SpeechRecognizer Recognizer { get; set; }

        private async Task WindowsSpeechRecognizerCore()
        {
            //Recognizerの作成
            this.Recognizer = new win.SpeechRecognizer();

            //継続して音声認識をするように設定。これがないと、一回きりの認識になる。
            await this.Recognizer.CompileConstraintsAsync();

            //音声認識中に発生する処理の登録
            this.Recognizer.HypothesisGenerated +=
                this.ContSpeechRecognizer_HypothesisGenerated;

            //音声認識処理が完了したときの処理の登録
            this.Recognizer.ContinuousRecognitionSession.ResultGenerated +=
                this.ContinuousRecognitionSession_ResultGenerated;

            //this.Recognizer.StateChanged += Recognizer_StateChanged;

            //ここから音声認識を開始する
            this.WinTextBox.Text = "Start\r\n ";
            await this.Recognizer.ContinuousRecognitionSession.StartAsync();
        }

        private async void Recognizer_StateChanged(win.SpeechRecognizer sender, win.SpeechRecognizerStateChangedEventArgs args)
        {
            await this.WinOutput(args.State.ToString(),false);
        }

        #region 認識中イベント
        private async void ContSpeechRecognizer_HypothesisGenerated(
        win.SpeechRecognizer sender, win.SpeechRecognitionHypothesisGeneratedEventArgs args)
        {
            //認識途中に画面表示
            await this.WinOutput(args.Hypothesis.Text, true);
        }
        #endregion

        #region 認識完了後イベント

        private async void ContinuousRecognitionSession_ResultGenerated(
            win.SpeechContinuousRecognitionSession sender, win.SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            await this.WinOutput(args.Result.Text, false);
        }

        #endregion

        #region private

        private async Task WinOutput(string text, bool isNotComplete)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var textlines = this.WinTextBox.Text.Split("\r\n");
                if (isNotComplete == false) textlines[textlines.Length - 1] = text + "\r\n ";
                else textlines[textlines.Length - 1] = text;

                this.WinTextBox.Text = string.Join("\r\n", textlines);
            });
        }

        #endregion

        #endregion

        #region Microsoft.CognitiveServices.Speech.SpeechRecognizer

        private mic.SpeechRecognizer MicRecognizer;

        private async Task MicrosoftSpeechRecofnizerCore()
        {
            //Cognitive Serviceを使うための設定
            var config = mic.SpeechConfig.FromSubscription("859b3e1bc5514b60819d09a4db722436", "eastus");
            config.SpeechRecognitionLanguage = "ja-jp";

            //Recognizerのインスタンスを生成。↑の設定を反映させる
            this.MicRecognizer = new mic.SpeechRecognizer(config);

            //認識中、認識完了時の処理を登録
            this.MicRecognizer.Recognizing += Recognizer_Recognizing;
            this.MicRecognizer.Recognized += Recognizer_Recognized;

            //ここから音声認識処理を開始する
            this.MicTextBox.Text = "Start\r\n ";
            await this.MicRecognizer.StartContinuousRecognitionAsync();
        }

        #region 認識中イベント

        private async void Recognizer_Recognized(object sender, mic.SpeechRecognitionEventArgs e)
        {
            await this.MicOutput(e.Result.Text, false);
        }

        #endregion
        
        #region 認識完了後イベント

        private async void Recognizer_Recognizing(object sender, mic.SpeechRecognitionEventArgs e)
        {
            await this.MicOutput(e.Result.Text, true);
        }

        #endregion

        #region private

        private async Task MicOutput(string text, bool isNotComplete)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var textlines = this.MicTextBox.Text.Split("\r\n");
                if (isNotComplete == false) textlines[textlines.Length - 1] = text + "\r\n ";
                else textlines[textlines.Length - 1] = text;

                this.MicTextBox.Text = string.Join("\r\n", textlines);
            });
        }

        #endregion

        #endregion
    }
}
