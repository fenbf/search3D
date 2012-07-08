using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using search3D;

using s3dCore.SearchEngines;

namespace search3D.SearchEngines
{
    /** simple image from disk */
    public class FileImageElement : ISearchElement
    {
        private const int ThumbSize = 128;

        private FileInfo info = null;
        private ImageBrush brush = null;
        private ImageBrush fullBrush = null;
        private OnProgress onProgress = null;

        public FileImageElement() { }
        public FileImageElement(FileInfo i) { info = i; }

        public void SetInfo(FileInfo i)
        {
            info = i;
        }

        public void Load() {
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.DecodePixelWidth = ThumbSize;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                //bi.DecodePixelHeight = 8;

                bi.UriSource = new Uri(info.FullName, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                if (bi.CanFreeze)
                    bi.Freeze();

                brush = new ImageBrush(bi);
                brush.Stretch = Stretch.UniformToFill;

                if (brush.CanFreeze)
                    brush.Freeze();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("cannot load image! " + e);
            }
        }

        public string GetLabel() { return info.Name; }

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
                bi.DownloadProgress += new EventHandler<DownloadProgressEventArgs>(bi_DownloadProgress);
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                //bi.DecodePixelHeight = 8;

                bi.UriSource = new Uri(info.FullName, UriKind.RelativeOrAbsolute);
                bi.EndInit();

                fullBrush = new ImageBrush(bi);
                fullBrush.Stretch = Stretch.Uniform;
                fullBrush.Freeze();
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

    /** engine for listing images from disk */
    public class FileEngine : ISearchEngine
    {
        private Queue<FileImageElement> images;

        private string[] imagesExtensions = new string[] { "*.jpg", "*.jpeg", "*.bmp", "*.png" };

        private const int maxRoundCount = 50;

        private string selectedPath;

        private int imagesFound;

        public FileEngine()
        {
            images = new Queue<FileImageElement>();
        }

        public void Reset()
        {
            selectedPath = "";
            images.Clear();
            imagesFound = 0;
        }

        public void OnSearch(ref SearchParams param, System.ComponentModel.BackgroundWorker bw)
        {
            if (param.Act == SearchParams.Action.aContinue)
            {
                SearchNext(bw);
                return;
            }            

            if (selectedPath == null) return;

            images.Clear();
            int counter = 0;
            DirectoryInfo di = new DirectoryInfo(selectedPath);
            imagesFound = 0;
            foreach (string imageExtension in imagesExtensions)
            {                
                FileInfo[] rgFiles = di.GetFiles(imageExtension);                
                foreach (FileInfo fi in rgFiles)
                {
                    if (bw.CancellationPending)
                    {
                        images.Clear();
                        return;
                    }

                    FileImageElement img = new FileImageElement();
                    img.SetInfo(fi);
                    images.Enqueue(img);

                    ++counter;
                    ++imagesFound;
                }                
            }

            SearchNext(bw);

            selectedPath = null;
        }

        private void SearchNext(System.ComponentModel.BackgroundWorker bw)
        {
            int counter = 0;
            while (counter < maxRoundCount && images.Count > 0)
            {
                if (bw.CancellationPending)
                {
                    images.Clear();
                    return;
                }

                ++counter;

                FileImageElement img = images.Dequeue();
                img.Load();

                bw.ReportProgress(0, (object)img);
            }
        }

        public bool PreSearch()
        {
            System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = ofd.SelectedPath;
                return true;
            }

            return false;
        }

        public UIElement GetSearchControl()
        {
            return null;
        }

        public SearchEngineDescription GetDescription()
        {
            return new SearchEngineDescription { EngineName = "Files", LogoUrl = "images/fileIcon.png", Info = "loads images from your disk" };
        }

        public string GetFoundImagesCount()
        {
            return imagesFound.ToString();
        }

        public string GetSearchButtonTitle() { return "Browse"; }

        public bool IsSearchTextBoxVisible() { return false; }
    }
}
