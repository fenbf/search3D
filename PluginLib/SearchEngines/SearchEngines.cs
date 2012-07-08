using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;

namespace s3dCore.SearchEngines
{
    /** -1 indicates an error! */
    public delegate void OnProgress(int percentage);

    public enum SearchElementType { etImage, etVideo, etSimple };

    /** interface for object can be searched, like images, news, etc */
    public interface ISearchElement
    {
        /** label that will be displayed as a title of the object */
        string GetLabel();

        /** returns type of the element - this will help when the object will be displayed using full quality brush */
        SearchElementType GetElementType();

        /** brush for the element, it will be used as a texture for object in the search list. 
         * Note that ImageBrush is faster than VisualBrush, so use as simple brush as possible
         */
        Brush GetContentBrush();

        /** brush that will be displayed in the center of the screen, it should be full quality image or video */
        Brush GetFullQualityBrush();

        /** returns url to the movie or web page */
        string GetFullContentUlr();

        void DisposeFullQualityBrush();

        /** on progress delegate will be called when full quality image is loading */
        void SetOnProgressDelegate(OnProgress p);
    }

    /** desription of the search engine, used in UI */
    public class SearchEngineDescription
    {
        public string EngineName { get; set; }
        public string LogoUrl { get; set; }
        public string Info { get; set; }
    }

    public class SearchEngineDescriptionList : List<SearchEngineDescription> { }

    public struct SearchParams
    {
        public enum Action { aStart, aContinue, aStop };
        
        public string Pattern;
        public Action Act;
    }

    /** interface for the search engine */
    public interface ISearchEngine
    {
        /** resets the engine, used fe. when switching between two different engines */
        void Reset();

        /** performs searching based on the pattern string argument. 
         * Note that it is done on the worker thread. The main task of this method is to fetch the data
         * and create list of elements so that it can be obtained by Elements() method and use to create 3D list
         */
        void OnSearch(ref SearchParams param, System.ComponentModel.BackgroundWorker bw);

        /** called before search occured on UI thread 
         * /return true if OnSearch can be called
         */
        bool PreSearch();

        /** each search engine can have custom search options, by returning UIelement user is able to use those options */
        UIElement GetSearchControl();

        /** returns simple description of the search engine */
        SearchEngineDescription GetDescription();

        /** returns info about number of found images - it can be correct value or even "100 and more" info */
        string GetFoundImagesCount();

        /** default is "Search" but other engines may name it in some other way */
        string GetSearchButtonTitle();

        /** is the search text box visible, for instance search engine based on the file system do not need this text box */
        bool IsSearchTextBoxVisible();
    }
}
