using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.IO;

using s3dCore.SearchEngines;

namespace search3D.SearchEngines
{
    class FlickrImage : ISearchElement
    {
        public string url;
        public string urlBig;
        private ImageBrush brush = null;
        private ImageBrush fullBrush = null;
        private OnProgress onProgress = null;

        public void Load()
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
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("cannot load image! " + e);
            }
        }

        public string GetLabel() { return url; }

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

        void bi_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            if (onProgress != null) onProgress(e.Progress);
        }

        public void DisposeFullQualityBrush()
        {
            GC.SuppressFinalize(fullBrush);
            fullBrush = null;
        }

        /** on progress delegate will be called when full quality image is loading */
        public void SetOnProgressDelegate(OnProgress p)
        {
            onProgress = p;
        }
    }

    class FlickrEngine : ISearchEngine
    {
        #region Data
        private const string FLICKR_API_KEY = "2c9cae43031e99b6b5e62a2bb2a1edbf";
        private const string MOST_RECENT = "http://www.flickr.com/services/rest/?method=flickr.photos.getRecent&api_key=" + FLICKR_API_KEY;
        private const string INTERESTING = "http://www.flickr.com/services/rest/?method=flickr.interestingness.getList&api_key=" + FLICKR_API_KEY;
        private const string SEARCH = "http://www.flickr.com/services/rest/?method=flickr.photos.search&api_key=" + FLICKR_API_KEY + "&text={0}";
        #endregion

        private IEnumerable<FlickrImage> photosFound;

        private const int maxCount = 300;
        private const int maxRoundCount = 30;
        private int currentStopIndex = 0;
        private int photosCount;
        private bool searchEnd = true;

        public FlickrEngine()
        {
            photosFound = null;
        }

        public void Reset()
        {
            photosCount = 0;  
        }

        public void OnSearch(ref SearchParams param, System.ComponentModel.BackgroundWorker bw)
        {
            if (param.Act == SearchParams.Action.aContinue)
            {
                SearchNext(bw);
                return;
            } 

            searchEnd = false;
            try
            {
                XElement xraw = XElement.Load(string.Format(SEARCH, param.Pattern));
                XElement xroot = XElement.Parse(xraw.ToString());
                var photos = (from photo in xroot.Element("photos").Elements("photo")
                              select new FlickrImage
                              {
                                  url =
                                  string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}_t.jpg",
                                                (string)photo.Attribute("farm"),
                                                (string)photo.Attribute("server"),
                                                (string)photo.Attribute("id"),
                                                (string)photo.Attribute("secret")),
                                  urlBig =
                                  string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}_b.jpg",
                                                (string)photo.Attribute("farm"),
                                                (string)photo.Attribute("server"),
                                                (string)photo.Attribute("id"),
                                                (string)photo.Attribute("secret"))
                              }).Take(maxCount);

                photosFound = photos;
                currentStopIndex = 0;

                photosCount = photosFound.Count();
                SearchNext(bw);

            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message, "ERROR");
            }
        }

        public void SearchNext(System.ComponentModel.BackgroundWorker bw)
        {
            int max = photosFound.Count();
            int counter = 0;
            foreach (FlickrImage img in photosFound)
            {
                if (bw.CancellationPending)
                {
                    searchEnd = true;
                    return;
                }

                ++counter;
                if (counter < currentStopIndex) continue;

                img.Load();
                //images.Add(img);

                ++currentStopIndex;
                bw.ReportProgress(currentStopIndex / max, (object)img);

                if (currentStopIndex % maxRoundCount == 0) break;
            }

            if (currentStopIndex >= maxCount || currentStopIndex >= max) searchEnd = true;
        }

        public bool PreSearch()
        {
            return true;
        }

        public UIElement GetSearchControl()
        {
            return null;
        }

        public SearchEngineDescription GetDescription()
        {
            return new SearchEngineDescription { EngineName = "Flickr", LogoUrl = "images/flickrIcon.png", Info = "searches the Flickr gallery" };
        }

        public string GetFoundImagesCount()
        {
            return photosCount.ToString();
        }

        public string GetSearchButtonTitle() { return "Search"; }

        public bool IsSearchTextBoxVisible() { return true; }
    }
}
