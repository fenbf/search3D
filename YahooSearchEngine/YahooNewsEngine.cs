using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Net;
using Yahoo.API;

using s3dCore.SearchEngines;

namespace YahooSearchEngine
{
    public class YahooNews : ISearchElement
    {
        public string url;
        public string urlBig;
        public string title;
        private ImageBrush brush = null;
        private ImageBrush fullBrush = null;
        private OnProgress onProgress = null;

        public bool Load()
        {
            try
            {
                Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
                WebClient web = new WebClient();
                Stream imgData = web.OpenRead(uri);
                MemoryStream memory = new MemoryStream();

                int data = imgData.ReadByte();
                while (data != -1)
                {
                    memory.WriteByte((byte)data);
                    data = imgData.ReadByte();
                }
                memory.Seek(0, SeekOrigin.Begin);

                BitmapImage bi = new BitmapImage();
                //bi.DecodePixelWidth = AppConfig.ThumbSize;
                bi.BeginInit();
                bi.StreamSource = memory;
                bi.EndInit();
                bi.Freeze();

                brush = new ImageBrush(bi);
                brush.Stretch = Stretch.UniformToFill;

                brush.Freeze();
                return true;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("cannot load news! " + e);
            }
            return false;
        }

        public string GetLabel() { return title; }

        public SearchElementType GetElementType() { return SearchElementType.etImage; }

        /** returns ImageBrush */
        public Brush GetContentBrush()
        {
            return brush;
        }

        public Brush GetFullQualityBrush()
        {
            if (fullBrush != null) return fullBrush;

            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                //bi.DecodePixelHeight = 8;

                bi.UriSource = new Uri(urlBig, UriKind.RelativeOrAbsolute);
                bi.EndInit();

                bi.DownloadProgress += new EventHandler<DownloadProgressEventArgs>(bi_DownloadProgress);
                bi.DownloadFailed += new EventHandler<ExceptionEventArgs>(bi_DownloadFailed);
                bi.DecodeFailed += new EventHandler<ExceptionEventArgs>(bi_DecodeFailed);

                fullBrush = new ImageBrush(bi);
                fullBrush.Stretch = Stretch.Uniform;
                //fullBrush.Freeze();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("cannot load big image! " + e);
            }

            return fullBrush;
        }

        public string GetFullContentUlr() { return ""; }

        void bi_DecodeFailed(object sender, ExceptionEventArgs e)
        {
            if (onProgress != null) onProgress(-1);
        }

        void bi_DownloadFailed(object sender, ExceptionEventArgs e)
        {
            if (onProgress != null) onProgress(-1);
        }

        void bi_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            if (onProgress != null) onProgress(e.Progress);
        }

        public void DisposeFullQualityBrush()
        {
            if (fullBrush != null)
            {
                GC.SuppressFinalize(fullBrush);
                fullBrush = null;
            }
        }

        /** on progress delegate will be called when full quality image is loading */
        public void SetOnProgressDelegate(OnProgress p)
        {
            onProgress = p;
        }
    }

    /*
    public class YahooNewsEngine : ISearchEngine
    {
        // http://developer.yahoo.com/search/image/V1/imageSearch.html

        private const string YAHOO_API_KEY = "SYOaYuvV34FDp8F2XhCwKOGbjUQUNEcT9KNh6tdgUQsqP2etA4iNsK3LH12L36ShqOvXq1SVLxNo";

        private const int maxResult = 500;
        private const int maxRoundCount = 50;
        private const int maxYahooResults = 50;
        private Queue<YahooNews> imagesToLoad;
        private int resultCount;
        private int start;
        private string pattern;
        private YahooSearchService yahoo;
        private YahooControl control;
        private int imagesFound;

        public YahooNewsEngine() 
        {
            imagesToLoad = new Queue<YahooNews>();
            pattern = null;
            control = null;// new YahooControl();
        }

        public void Reset()
        {
            imagesToLoad.Clear();
            imagesFound = 0;
        }

        public void OnSearch(ref SearchParams param, System.ComponentModel.BackgroundWorker bw)
        {
            int i = 0;
            if (param.Act == SearchParams.Action.aContinue)
            {
                SearchNext(bw);
                return;
            } 

            try
            {
                if (yahoo == null) yahoo = new YahooSearchService();

                pattern = param.Pattern;

                resultCount = 0;
                start = 0;
                imagesFound = 0;
                imagesToLoad.Clear();
                    

                SearchNext(bw);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error in search: " + e);
            }
        }

        public void SearchNext(System.ComponentModel.BackgroundWorker bw) 
        {
            if (start < maxResult)
            {
                try
                {
                    //Yahoo.API.ImageSearchResponse.ResultSet resultSet = yahoo.ImageSearch("YahooExample", pattern, "all", maxResult, 1, "any", true);
                    //Yahoo.API.ImageSearchResponse.ResultSet resultSet = yahoo.ImageSearch(YAHOO_API_KEY, pattern, "all", maxYahooResults, start, "any", control.AdultContent);

                    Yahoo.API.NewsSearchResponse.ResultSet resultSet = yahoo.NewsSearch(YAHOO_API_KEY, pattern, "all", maxYahooResults, start, "rank", "en");

                    foreach (Yahoo.API.NewsSearchResponse.ResultType result in resultSet.Result)
                    {
                        if (bw.CancellationPending == true)
                        {
                            imagesToLoad.Clear();
                            pattern = null;
                            return;
                        }

                        YahooNews img = new YahooNews();
                        img.url = result.Thumbnail.Url;
                        img.urlBig = result.Thumbnail.Url;
                        img.title = result.Title;
                        imagesToLoad.Enqueue(img);
                        ++resultCount;
                    }
                    start += maxYahooResults;
                    imagesFound += maxYahooResults;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("yahoo error: " + e);
                    System.Windows.MessageBox.Show(e.ToString());
                }
            }

            int counter = 0;
            while (counter < maxRoundCount && imagesToLoad.Count > 0) 
            {
                if (bw.CancellationPending == true) 
                {
                    imagesToLoad.Clear();
                    pattern = null;
                    return;
                }

                YahooNews img = imagesToLoad.Dequeue();
                if (img.Load() == true)
                {
                    bw.ReportProgress(resultCount - imagesToLoad.Count / resultCount, (object)img);
                }
                ++counter;
            }
        }

        public bool PreSearch() { return true; }

        public UIElement GetSearchControl()
        {
            return null;
        }

        public SearchEngineDescription GetDescription()
        {
            return new SearchEngineDescription { EngineName = "Yahoo", LogoUrl = "images/yahooIcon.png", Info = "uses Yahoo image search engine" };
        }

        public string GetFoundImagesCount()
        {
            return "more than " + imagesFound.ToString();
        }

        public string GetSearchButtonTitle() { return "Search"; }

        public bool IsSearchTextBoxVisible() { return true; }

        public static void Test()
        {
            YahooSearchService yahoo = new YahooSearchService();

            Yahoo.API.ImageSearchResponse.ResultSet resultSet = yahoo.ImageSearch("YahooExample", "satriani", "all", 10, 1, "any", true);

            StringWriter sw = new StringWriter();
            foreach (Yahoo.API.ImageSearchResponse.ResultType result in resultSet.Result)
            {
                System.Diagnostics.Debug.WriteLine("Title: {0}", result.Title);
                System.Diagnostics.Debug.WriteLine("Summary: {0}", result.Summary);
                System.Diagnostics.Debug.WriteLine("URL: {0}", result.Url);
                System.Diagnostics.Debug.WriteLine("==============================================================");
            }
        }
    }
     */
}
