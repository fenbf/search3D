using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Xml.Linq;
using System.Net;
using Google.GData.YouTube;
using Google.GData.Client;
using Google.GData.Extensions;

using search3D;

using s3dCore.SearchEngines;

namespace search3D.SearchEngines
{
    /** simple image from disk */
    public class YouTubeElement : ISearchElement
    {
        private ImageBrush brush = null;
        private ImageBrush fullBrush = null;
        private OnProgress onProgress = null;

        public string LinkUrl { get; set; }
        public string ThumnailUrl { get; set; }
        public string EmbedUrl { get; set; }

        public YouTubeElement() { }

        public void Load() {
            try
            {
                Uri uri = new Uri(ThumnailUrl, UriKind.RelativeOrAbsolute);
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

        public string GetLabel() { return ""; }

        public SearchElementType GetElementType() { return SearchElementType.etVideo; }

        /** returns ImageBrush */
        public Brush GetContentBrush()
        {
            return brush;
        }

        public Brush GetFullQualityBrush()
        {          
            return fullBrush;
        }

        public string GetFullContentUlr() { return EmbedUrl; }

        void bi_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            if (onProgress != null) onProgress(e.Progress);
        }

        public void DisposeFullQualityBrush() 
        {           
            
        }

        /** on progress delegate will be called when full quality image is loading */
        public void SetOnProgressDelegate(OnProgress p)
        {
            onProgress = p;
        }
    }

    /** engine for listing images from disk */
    public class YouTubeEngine : ISearchEngine
    {
        private List<YouTubeElement> elements;

        #region Data
        private const string MOST_POPULAR = "http://youtube.com/rss/global/top_viewed.rss";
        private const string SEARCH = "http://www.youtube.com/rss/tag/{0}.rss";
        #endregion

        public YouTubeEngine()
        {
            
        }

        public void Reset()
        {
            
        }

        public void OnSearch(ref SearchParams param, System.ComponentModel.BackgroundWorker bw)
        {
            if (param.Act == SearchParams.Action.aContinue)
            {
                SearchNext(bw);
                return;
            }

            try
            {
                var xraw = XElement.Load(string.Format(SEARCH, param.Pattern));
                var xroot = XElement.Parse(xraw.ToString());
                var links = (from item in xroot.Element("channel").Descendants("item")
                             select new YouTubeElement
                             {
                                 LinkUrl = item.Element("link").Value,
                                 EmbedUrl = GetEmbedUrlFromLink(item.Element("link").Value),
                                 ThumnailUrl =
                                    item.Elements().Where(
                                            child => child.Name.ToString().Contains("thumbnail")
                                        ).Single().Attribute("url").Value

                             }).Take(20);

                elements = links.ToList<YouTubeElement>();

                SearchNext(bw);
            }
            catch (Exception e)
            {

            }
        }

        private void SearchNext(System.ComponentModel.BackgroundWorker bw)
        {
            while (elements.Count > 0)
            {
                if (bw.CancellationPending)
                {
                    elements.Clear();
                    return;
                }

                YouTubeElement elem = elements.First();
                elem.Load();

                bw.ReportProgress(0, (object)elem);
                elements.RemoveAt(0);
            }
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
            return new SearchEngineDescription { EngineName = "YouTube", LogoUrl = "images/YouTubeIcon.png", Info = "search YouTube videos!" };
        }

        public string GetFoundImagesCount()
        {
            return "0";// elements.Count.ToString();
        }

        public string GetSearchButtonTitle() { return "Search"; }

        public bool IsSearchTextBoxVisible() { return true; }

        private static string GetEmbedUrlFromLink(string link)
        {
            try
            {
                string embedUrl = "http://www.";
                string startPart = link.Substring(link.IndexOf("you"));
                embedUrl += startPart.Substring(0, startPart.LastIndexOf(@"/"));
                embedUrl += "/v/";
                embedUrl += startPart.Substring(startPart.LastIndexOf("=") + 1);
                embedUrl += "&hl=en";
                return embedUrl;
            }
            catch
            {
                return link;
            }
        }

        public static void DoTest()
        {
            /*string url = "http://gdata.youtube.com/feeds/videos/-/" + "satriani";
            AtomFeed myFeed = GetFeed(url, 1, 20);
            DisplayFeed(myFeed);
             */
        }

        private static AtomFeed GetFeed(string url, int start, int number)
        {
            System.Diagnostics.Trace.Write("Conectando youtube at " + url);
            FeedQuery query = new FeedQuery("");
            Service service = new Service("youtube", "exampleCo");
            query.Uri = new Uri(url);
            query.StartIndex = start;
            query.NumberToRetrieve = number;

            AtomFeed myFeed = service.Query(query);
            return myFeed;
        }

        private static void DisplayFeed(AtomFeed myFeed)
        {
            foreach (AtomEntry entry in myFeed.Entries)
            {

                System.Diagnostics.Debug.WriteLine(entry.Id.AbsoluteUri + ", ");
            }
        }
    }
}
