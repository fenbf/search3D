using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace search3D
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        #region Data
        private s3dCore.SearchEngines.ISearchEngine currentSearchEngine;
        private List<s3dCore.SearchEngines.ISearchEngine> searchEngines;
        private s3dCore.SearchEngines.SearchEngineDescriptionList engineDescs;

        private BackgroundWorker workerThread;

        private Dictionary<ModelUIElement3D, s3dCore.SearchEngines.ISearchElement> currentList;
        private ModelUIElement3D currentlySelected = null;
        private Brush tempBrush;

        private CameraController camControler;

        private List<s3dCore.ListLayout.IListLayout> layouts;
        private s3dCore.ListLayout.IListLayout currentLayout;

        private Storyboard onSearching = null;

        private HelpSystem help;
        #endregion

        #region Contructor
        public Window1()
        {
            InitializeComponent();

            mainCanvas.Background = Brushes.Black;

            // worker thread:
            workerThread = new BackgroundWorker();
            workerThread.DoWork += new DoWorkEventHandler(PerformSearch);
            workerThread.ProgressChanged += new ProgressChangedEventHandler(workerThread_ProgressChanged);
            workerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerThread_RunWorkerCompleted);
            workerThread.WorkerReportsProgress = true;
            workerThread.WorkerSupportsCancellation = true;

            // search engines:
            {
                searchEngines = new List<s3dCore.SearchEngines.ISearchEngine>();
                searchEngines.Add(new SearchEngines.FileEngine());
                searchEngines.Add(new SearchEngines.FlickrEngine());
                //searchEngines.Add(new SearchEngines.YouTubeEngine());

                //SearchEngines.YouTubeEngine.DoTest();

                List<s3dCore.SearchEngines.ISearchEngine> plugins = PluginHelper.GetEnginePlugins();
                if (plugins != null)
                {
                    searchEngines.AddRange(plugins);
                }

                engineDescs = new s3dCore.SearchEngines.SearchEngineDescriptionList();
                foreach (s3dCore.SearchEngines.ISearchEngine eng in searchEngines)
                {
                    s3dCore.SearchEngines.SearchEngineDescription desc = eng.GetDescription();
                    engineDescs.Add(desc);
                    enginesCombo.Items.Add(desc);
                }
                currentSearchEngine = null;
                ChangeSearchEngine(searchEngines[searchEngines.Count-1]);
                enginesCombo.SelectedIndex = searchEngines.Count - 1;
            }

            // layouts:
            {
                layouts = new List<s3dCore.ListLayout.IListLayout>();
                layouts.Add(new ListLayout.BasicListLayout());
                layouts.Add(new ListLayout.SpiralLayout());
                layouts.Add(new ListLayout.FloorLayout());

                List<s3dCore.ListLayout.IListLayout> plugins = PluginHelper.GetLayoutPlugins();
                if (plugins != null)
                {
                    layouts.AddRange(plugins);
                }

                foreach (s3dCore.ListLayout.IListLayout layout in layouts)
                {
                    layoutCombo.Items.Add(layout.GetName());
                }

                currentList = new Dictionary<ModelUIElement3D, s3dCore.SearchEngines.ISearchElement>();
                camControler = new CameraController(mainCamera, currentLayout, mainLight);

                ChangeLayout(layouts[1]);
                layoutCombo.SelectedIndex = 1;
            }            

            onSearching = this.TryFindResource("OnSearching") as Storyboard;

            help = new HelpSystem(statusLabel);
            HelpSystem.GlobalSystem = help;


            bigPicture.AtEnd = new BigPicture.CallbackAtEnd(AtEndBigPicture);

            if (Properties.Settings.Default.FirstTimeUse == true)
            {
                Properties.Settings.Default.FirstTimeUse = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                HelpSystem.GlobalSystem.Enabled = false;
            }

            HelpSystem.GlobalSystem.ShowHelp("Start searching and move using mouse - drag and mouse roll - in the 3d space...", 1);
        }
        #endregion

        #region Worker Thread
        void workerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowSearching(false);
        }

        void workerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            AddElementToList(e.UserState as s3dCore.SearchEngines.ISearchElement);
        }

        void PerformSearch(object sender, DoWorkEventArgs e)
        {
            //Thread t = Thread.CurrentThread;
            //System.Diagnostics.Debug.WriteLine("thread priority: " + t.Priority.ToString());
            s3dCore.SearchEngines.SearchParams param = (s3dCore.SearchEngines.SearchParams)e.Argument;
            currentSearchEngine.OnSearch(ref param, workerThread);
        }
        #endregion

        #region Models and 3D layout creation
        private void AddElementToList(s3dCore.SearchEngines.ISearchElement element)
        {
            if (element == null) throw new Exception("element is null!");
            
            Brush brush = element.GetContentBrush();

            //Create the model
            ModelUIElement3D model3D = CreateModel(brush);

            //hook up mouse events, and add to lookup and return the ModelUIElement3D
            mainViewport.Children.Add(model3D);
            currentList.Add(model3D, element);
        }

        ModelUIElement3D CreateModel(Brush brush)
        {
            ModelUIElement3D model3D = new ModelUIElement3D
            {
                Model = new GeometryModel3D
                {
                    Geometry = new MeshGeometry3D
                    {
                        TriangleIndices = new Int32Collection(
                            new int[] { 0, 1, 2, 2, 3, 0 }),
                        TextureCoordinates = new PointCollection(
                            new Point[] 
                    { 
                        new Point(0, 1), 
                        new Point(1, 1), 
                        new Point(1, 0), 
                        new Point(0, 0) 
                    }),
                        Positions = new Point3DCollection(
                            new Point3D[] 
                    { 
                        new Point3D(-0.5, -0.5, 0), 
                        new Point3D(0.5, -0.5, 0), 
                        new Point3D(0.5, 0.5, 0), 
                        new Point3D(-0.5, 0.5, 0) 
                    })
                    },
                    Material = new DiffuseMaterial
                    {
                        Brush = brush
                    },
                    BackMaterial = new DiffuseMaterial
                    {
                        Brush = Brushes.White
                    },
                    Transform = currentLayout.GetNextTransformation()
                }
            };

            model3D.MouseEnter += new MouseEventHandler(model3D_MouseEnter);
            model3D.MouseLeave += new MouseEventHandler(model3D_MouseLeave);
            model3D.MouseLeftButtonDown += new MouseButtonEventHandler(model3D_MouseLeftButtonDown);
            return model3D;
        }

        private void layoutCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeLayout(layouts[layoutCombo.SelectedIndex]);
        }

        void ChangeLayout(s3dCore.ListLayout.IListLayout newLayout)
        {
            if (currentLayout != null && currentLayout.GetType() == newLayout.GetType())
            {
                return;
            }

            currentLayout = newLayout;
            currentLayout.ResetData();
            camControler.ChangeLayout(currentLayout);

            if (currentLayout.CanMoveHorizontally() == false)
            {
                viewLeftBtn.Visibility = Visibility.Collapsed;
                viewRightBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                viewLeftBtn.Visibility = Visibility.Visible;
                viewRightBtn.Visibility = Visibility.Visible;
            }

            // copy elements 
            List<s3dCore.SearchEngines.ISearchElement> tempList = new List<s3dCore.SearchEngines.ISearchElement>(currentList.Count);
            foreach (s3dCore.SearchEngines.ISearchElement elem in currentList.Values)
            {
                tempList.Add(elem);
            }
            currentList.Clear();
            mainViewport.Children.Clear();
            foreach (s3dCore.SearchEngines.ISearchElement element in tempList)
            {
                ModelUIElement3D model3D = CreateModel(element.GetContentBrush());

                mainViewport.Children.Add(model3D);
                currentList.Add(model3D, element);
            }
            camControler.MoveToStartPosition();
        }
        #endregion

        #region mouse enter/leave/click on images in the list
        void model3D_MouseLeave(object sender, MouseEventArgs e)
        {
            currentLayout.SetupAnimationOnMouseLeave(sender as ModelUIElement3D);
            //HelpSystem.GlobalSystem.HideHelp();

            if (tempBrush != null)
            {
                GeometryModel3D geo = currentlySelected.Model as GeometryModel3D;
                DiffuseMaterial mat = geo.Material as DiffuseMaterial;
                mat.Brush = tempBrush;
                tempBrush = null;
            }
            currentlySelected = null;
        }

        void model3D_MouseEnter(object sender, MouseEventArgs e)
        {
            currentLayout.SetupAnimationOnMouseEnter(sender as ModelUIElement3D);
            HelpSystem.GlobalSystem.ShowHelp("double click to view element in full qulity", 20);

            if (tempBrush != null)
            {
                GeometryModel3D geo = currentlySelected.Model as GeometryModel3D;
                DiffuseMaterial mat = geo.Material as DiffuseMaterial;
                mat.Brush = tempBrush;
                tempBrush = null;
            }
            currentlySelected = null;
        }

        void model3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ModelUIElement3D model = sender as ModelUIElement3D;

            // double click?
            if (e.ClickCount > 1)
            {
                viewport.Visibility = Visibility.Collapsed;

                bigPicture.SetContent(currentList[model]);
                bigPicture.FadeIn(0.5);

                return;
            }

            {
                s3dCore.ListLayout.CameraParams dest = currentLayout.GetElementViewPosition(model);

                camControler.MoveToSelectedElement(dest);

                currentlySelected = model;
            }
        }

        void AtEndBigPicture()
        {
            viewport.Visibility = Visibility.Visible;
        }

        #endregion

        #region controller - move forward, left, right, reset...
        private void mainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            camControler.MoveForward(e.Delta * 0.1);
            if (camControler.IsViewingEndReached == true)
            {
                SearchNext();
            }
        }

        private void viewLeftBtn_Click(object sender, RoutedEventArgs e)
        {
            camControler.MoveHorizontally(-1);
            if (camControler.IsViewingEndReached == true)
            {
                SearchNext();
            }

        }

        private void viewRightBtn_Click(object sender, RoutedEventArgs e)
        {
            camControler.MoveHorizontally(1);
            if (camControler.IsViewingEndReached == true)
            {
                SearchNext();
            }
        }

        private void resetViewBtn_Click(object sender, RoutedEventArgs e)
        {
            camControler.MoveToStartPosition();
        }

        #endregion

        #region app top menu
        private void fullsccreenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowStyle == WindowStyle.None)
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.Topmost = false;
                fullsccreenBtn.Content = "Fullscreen";
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Maximized;
                this.Topmost = true;
                fullsccreenBtn.Content = "Windowed";
            }
        }

        private void closeAppBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region engine selection/change
        private void enginesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            s3dCore.SearchEngines.ISearchEngine engine = searchEngines[enginesCombo.SelectedIndex];
            ChangeSearchEngine(engine);
        }

        void ChangeSearchEngine(s3dCore.SearchEngines.ISearchEngine newEngine)
        {
            if (currentSearchEngine != null && newEngine.GetType() == currentSearchEngine.GetType()) return;

            if (workerThread.IsBusy)
            {
                searchBtn.Content = "Cancelling...";
                workerThread.CancelAsync();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, ((Action)delegate { ChangeSearchEngine(newEngine); }));
            }

            currentSearchEngine = newEngine;
            customUI.Children.Clear();
            if (newEngine.GetSearchControl() != null)
            {
                customUI.Children.Add(newEngine.GetSearchControl());
            }
            currentSearchEngine.Reset();
            searchBtn.Content = currentSearchEngine.GetSearchButtonTitle();

            if (currentSearchEngine.IsSearchTextBoxVisible())
            {
                patternTb.Visibility = Visibility.Visible;
            }
            else
            {
                patternTb.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Searching
        void ShowSearching(bool enable)
        {
            if (enable)
            {
                labelFoundImagesCount.Visibility = Visibility.Visible;
                labelFoundImagesCount.Content = "images found " + currentSearchEngine.GetFoundImagesCount();
                onSearching.Begin(searchBtn, true);                
                searchBtn.Content += "...";
            }
            else
            {
                labelFoundImagesCount.Visibility = Visibility.Collapsed;
                onSearching.Stop(searchBtn);
                searchBtn.Content = currentSearchEngine.GetSearchButtonTitle();
            }
        }

        void SearchNext()
        {
            if (workerThread.IsBusy == false)
            {
                ShowSearching(true);
                workerThread.RunWorkerAsync((object)new s3dCore.SearchEngines.SearchParams { Pattern = "", Act = s3dCore.SearchEngines.SearchParams.Action.aContinue });
            }
        }

        private void patternTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoSearch();
            }
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void DoSearch()
        {
            if (workerThread.IsBusy)
            {
                searchBtn.Content = "Cancelling...";
                workerThread.CancelAsync();
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, ((Action)delegate { DoSearch(); }));

                return;
            }

            if (currentSearchEngine.PreSearch())
            {
                workerThread.RunWorkerAsync((object)new s3dCore.SearchEngines.SearchParams { Pattern = patternTb.Text, Act = s3dCore.SearchEngines.SearchParams.Action.aStart });
            }
            else
            {
                return;
            }

            mainViewport.Children.Clear();
            currentLayout.ResetData();
            currentList.Clear();
            camControler.MoveToStartPosition();

            ShowSearching(true);
        }
        #endregion

        bool drag = false;
        Point offset;
        private void mainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            drag = true;
            offset = e.GetPosition(this);
        }

        private void mainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (drag)
            {
                Point dist = new Point();
                dist.X = e.GetPosition(this).X - offset.X;
                dist.Y = e.GetPosition(this).Y - offset.Y;
                //closeAppBtn.Content = dist;

                double moveX = 10 * -dist.X / this.ActualWidth;

                camControler.MoveHorizontally(moveX);
                if (camControler.IsViewingEndReached == true)
                {
                    SearchNext();
                }
            }

            drag = false;
        }

        private void mainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
               
        }
    }
}
