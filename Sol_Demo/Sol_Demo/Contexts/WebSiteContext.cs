using Sol_Demo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sol_Demo.Contexts
{
    public class WebSiteContext
    {
        #region Private Method

        // 1
        private Task<List<String>> GetWebUrlListAsync()
        {
            return Task.Run(() =>
            {
                List<string> listWebUrl = new List<string>();

                listWebUrl.Add("https://www.yahoo.com");
                listWebUrl.Add("https://www.google.com");
                listWebUrl.Add("https://www.microsoft.com");
                listWebUrl.Add("https://www.cnn.com");
                listWebUrl.Add("https://www.amazon.com");
                listWebUrl.Add("https://www.facebook.com");
                listWebUrl.Add("https://www.twitter.com");
                listWebUrl.Add("https://www.codeproject.com");
                listWebUrl.Add("https://www.stackoverflow.com");
                listWebUrl.Add("https://en.wikipedia.org/wiki/.NET_Framework");

                return listWebUrl;
            });
        }

        //2
        private async Task<WebSiteModel> DownloadWebsiteAsync(string url)
        {
            WebClient webClient = new WebClient();
            WebSiteModel webSiteModel = new WebSiteModel()
            {
                Url = url,
                Data = await webClient?.DownloadStringTaskAsync(url)
            };

            return webSiteModel;
        }

        private WebSiteModel DownloadWebsite(string url)
        {
            WebClient webClient = new WebClient();
            WebSiteModel webSiteModel = new WebSiteModel()
            {
                Url = url,
                Data = webClient.DownloadString(url)
            };

            return webSiteModel;
        }

        #endregion Private Method

        #region Public Method

        // 3
        public async Task RunDownloadAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            int counter = 0;
            try
            {
                //Create an instance of ProgressReport Model where we can keep List Progress Report Data
                var progressReportModel = new ProgressReportModel();

                // get list of url Which we want to download the content
                var listWebsiteUrl = await this.GetWebUrlListAsync();

                foreach (var url in listWebsiteUrl)
                {
                    // Download Web site Content
                    var webSiteModel = await this.DownloadWebsiteAsync(url);

                    // Request for Cancellation Task
                    cancellationToken.ThrowIfCancellationRequested();

                    counter++;

                    // Store Data in Progress Model
                    progressReportModel.PrecentageComplete = (counter * 100) / listWebsiteUrl.Count;
                    progressReportModel.SiteDownloaded = webSiteModel;

                    //Report Data to Progress Object
                    progress.Report(progressReportModel);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task RunDownloadParallelAsync(IProgress<ProgressReportModel> progress, ParallelOptions parallelOptions)
        {
            int counter = 0;
            try
            {
                //Create an instance of ProgressReport Model where we can keep List Progress Report Data
                var progressReportModel = new ProgressReportModel();

                // get list of url Which we want to download the content
                var listWebsiteUrl = await this.GetWebUrlListAsync();

                await Task.Run(() =>
                {
                    Parallel.ForEach<String>(listWebsiteUrl, parallelOptions, (url) =>
                    {
                        // Download Web site Content
                        var webSiteModel = this.DownloadWebsite(url);

                        // Request for Cancellation Task
                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();

                        counter++;

                        // Store Data in Progress Model
                        progressReportModel.PrecentageComplete = (counter * 100) / listWebsiteUrl.Count;
                        progressReportModel.SiteDownloaded = webSiteModel;

                        //Report Data to Progress Object
                        progress.Report(progressReportModel);
                    });
                });
            }
            catch
            {
                throw;
            }
        }

        #endregion Public Method
    }
}