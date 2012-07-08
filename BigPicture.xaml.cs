using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace search3D
{
    /// <summary>
    /// Interaction logic for BigPicture.xaml
    /// </summary>
    public partial class BigPicture : UserControl
    {
        public delegate void CallbackAtEnd();

        s3dCore.SearchEngines.ISearchElement contentElement;
        BitmapImage imageToSave = null;
        RotateTransform imageRotation = null;

        public CallbackAtEnd AtEnd;

        public BigPicture()
        {
            InitializeComponent();

            this.Background = Brushes.Black;
            this.Visibility = Visibility.Collapsed;
            loadingProgress.Visibility = Visibility.Collapsed;

            imageRotation = new RotateTransform();
            imageRotation.CenterX = 0.5;
            imageRotation.CenterY = 0.5;
            imageRotation.Angle = 0;
        }

        public void SetContent(s3dCore.SearchEngines.ISearchElement element)
        {
            imageRotation.Angle = 0;
            Brush brush = element.GetFullQualityBrush();
            if (element.GetElementType() == s3dCore.SearchEngines.SearchElementType.etImage)
            {
                ImageBrush brushTemp = brush as ImageBrush;
                if (brushTemp.ImageSource is BitmapImage)
                {
                    imageToSave = brushTemp.ImageSource as BitmapImage;
                    imagePanel.Visibility = Visibility.Visible;
                }
                else
                {
                    throw new Exception("brush is not image!");
                }

                imagePanel.Visibility = Visibility.Visible;
                browser.Visibility = Visibility.Collapsed;
                content.Background = brush;
                element.SetOnProgressDelegate(OnImageProgress);
            }
            else
            {
                imagePanel.Visibility = Visibility.Collapsed;

                if (element.GetElementType() == s3dCore.SearchEngines.SearchElementType.etVideo)
                {
                    content.Visibility = Visibility.Collapsed;
                    browser.Visibility = Visibility.Visible;
                    browser.Navigate(new Uri(element.GetFullContentUlr(), UriKind.RelativeOrAbsolute));
                }
            }
            
            contentElement = element;

            labelContentTitle.Content = element.GetLabel();
        }

        public void FadeIn(double seconds)
        {
            this.Visibility = Visibility.Visible;
            errorLabel.Visibility = Visibility.Collapsed;

            Helper.ShowAndFadeIn(this, seconds);

            HelpSystem.GlobalSystem.ShowHelp("double click to return to search mode", 2);
        }

        public void FadeOut(double seconds)
        {
            if (browser.Visibility == Visibility.Visible)
            {
                browser.Source = null;
                browser.Visibility = Visibility.Collapsed;
            }

            if (contentElement != null)
            {
                imageToSave = null;
                contentElement.DisposeFullQualityBrush();
                contentElement.SetOnProgressDelegate(null);
            }

            Helper.HideAndFadeOut(this, seconds);
            HelpSystem.GlobalSystem.HideHelp();

            if (AtEnd != null) AtEnd();
        }

        void OnImageProgress(int p)
        {
            loadingProgress.Value = p;
            if (p >= 0 && loadingProgress.Visibility == Visibility.Collapsed)
            {
                Helper.ShowAndFadeIn(loadingProgress, 0.5);
                Helper.HideAndFadeOut(labelContentTitle, 0.5);
            }
            if (p > 99 && loadingProgress.Visibility == Visibility.Visible)
            {
                Helper.ShowAndFadeIn(labelContentTitle, 0.5);
                Helper.HideAndFadeOut(loadingProgress, 0.5);
            }
            // error?
            if (p < 0)
            {
                Helper.ShowAndFadeIn(errorLabel, 0.5);
            }
        }

        private void rotLeft_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();         
        }

        private void rotRight_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (contentElement.GetElementType() != s3dCore.SearchEngines.SearchElementType.etImage) return;

            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.InitialDirectory = @search3D.Properties.Settings.Default.StoreDirectory;                
            sfd.Filter = "Jpg files (*.jpg)|*.jpg|Jpg files (*.jpeg)|*.jpeg|Bmp files (*.bmp)|*.bmp" +
                         "|Png files (*.png)|*.png|Tif files (*.tif)|*.tif|Tif files (*.tiff)|*.tiff" +
                         "|Gif files (*.gif)|*.gif|Wmp files (*.wmp)|*.wmp|All files (*.*)|*.*";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (sfd.FileName != string.Empty)
                {
                    string errorMessage;
                    //TODO : need to make save path available in App.Config
                    if (SaveImageToDisk(sfd.FileName, out errorMessage))
                    {
                        MessageBox.Show("File was saved properly!", "Save Ok",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                        MessageBox.Show(errorMessage, "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("You need to enter a filename", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cropBtn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        bool SaveImageToDisk(string filename, out string errorMessage)
        {
            string extension = filename.LastIndexOf(".") > -1 ? filename.Substring(filename.LastIndexOf(".")) : ".jpg";

            BitmapEncoder encoder = null;
            switch (extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;

                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;

                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;

                case ".wmp":
                    encoder = new WmpBitmapEncoder();
                    break;
                default:
                    errorMessage = "extension is wrong!";
                    return false;
            }

            try
            {
                FileStream stream = new FileStream(filename, FileMode.Create);
                encoder.Frames.Add(BitmapFrame.Create(imageToSave));
                encoder.Save(stream);
                errorMessage = "";
                stream.Close();
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
            }

            return false;
        }

        private void goBackBtn_Click(object sender, RoutedEventArgs e)
        {
            FadeOut(0.5);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                FadeOut(0.5);
            }
        }
    }
}
