using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YahooSearchEngine
{
    /// <summary>
    /// Interaction logic for YahooControl.xaml
    /// </summary>
    public partial class YahooControl : UserControl
    {
        public string SelectedSize = "";
        public bool AdultContent = false;

        public YahooControl()
        {
            InitializeComponent();

            allBtn.IsChecked = true;
        }

        private void allBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedSize = "";   
        }

        private void smallBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedSize = "Small";
        }

        private void mediumBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedSize = "Medium";
        }

        private void largeBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedSize = "Large";
        }

        private void adult_Checked(object sender, RoutedEventArgs e)
        {
            AdultContent = (bool)adult.IsChecked;
        }

    }
}
