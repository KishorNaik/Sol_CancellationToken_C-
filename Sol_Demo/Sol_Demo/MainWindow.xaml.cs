using Sol_Demo.Contexts;
using Sol_Demo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sol_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WebSiteContext websiteContext = null;

        private CancellationTokenSource cancellationTokenSource = null;

        public MainWindow()
        {
            InitializeComponent();

            websiteContext = new WebSiteContext();
            cancellationTokenSource = new CancellationTokenSource();
        }

        // 4
        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tbResult.Text = "";
                pbDownload.Value = 0;
                // Create an insatnce of Progress Oject
                Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();

                // Wire up Progress Change Event
                progress.ProgressChanged += Progress_ProgressChanged;

                try
                {
                    // Start the Job for Download Website Content and Pass as parameter of progress and cancellation Token Object
                    await websiteContext?.RunDownloadAsync(progress, cancellationTokenSource.Token);
                }
                catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
                {
                    tbResult.Text += $"The async download was cancelled. { Environment.NewLine }";
                }
                finally
                {
                    // if cancellation request true then dispose cancelllation Token
                    if (cancellationTokenSource.IsCancellationRequested == true)
                    {
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = new CancellationTokenSource();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //5
        private async Task BindResultTask(ProgressReportModel progressReportModel)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                tbResult.Text += $"{ progressReportModel.SiteDownloaded.Url } downloaded: { progressReportModel.SiteDownloaded.Data.Length } characters long.{ Environment.NewLine }";
            });
        }

        //6
        private async Task BindProgressBarTask(int value)
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                pbDownload.Value = value;
            });
        }

        // 7
        private async void Progress_ProgressChanged(object sender, ProgressReportModel e)
        {
            await BindResultTask(e);
            await BindProgressBarTask(e.PrecentageComplete);
        }

        private void btnCancl_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }

        private async void btnParallel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tbResult.Text = "";
                pbDownload.Value = 0;

                // Create an insatnce of Progress Oject
                Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
                ParallelOptions parallelOptions = new ParallelOptions() { CancellationToken = cancellationTokenSource.Token };

                // Wire up Progress Change Event
                progress.ProgressChanged += Progress_ProgressChanged;

                try
                {
                    // Start the Job for Download Website Content and Pass as parameter of progress and cancellation Token Object
                    await websiteContext?.RunDownloadParallelAsync(progress, parallelOptions);
                }
                catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
                {
                    tbResult.Text += $"The async download was cancelled. { Environment.NewLine }";
                }
                finally
                {
                    // if cancellation request true then dispose cancelllation Token
                    if (cancellationTokenSource.IsCancellationRequested == true)
                    {
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = new CancellationTokenSource();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}