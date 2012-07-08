using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace search3D
{
    class HelpSystem
    {
        public static HelpSystem GlobalSystem;

        public bool Enabled = true;

        private Dictionary<string, int> repetitions;

        private Label label;
        private string currentMsg;

        public HelpSystem(Label l) 
        {
            label = l;
            repetitions = new Dictionary<string, int>();
        }

        public void ShowHelp(string msg, int maxRepetitions)
        {
            if (Enabled == false) return;

            if (repetitions.ContainsKey(msg))
            {
                --repetitions[msg];
                if (repetitions[msg] <= 0)
                {
                    return;
                }
            }
            else
            {
                repetitions.Add(msg, maxRepetitions);
            }

            currentMsg = msg;

            label.Content = "Help: " + msg;
            label.Foreground = Brushes.Orange;
            label.Visibility = Visibility.Visible;
            DoubleAnimation fadein = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
            //fadein.RepeatBehavior = RepeatBehavior.Forever;
            label.BeginAnimation(UIElement.OpacityProperty, fadein);

            
        }

        public void HideHelp()
        {
            if (Enabled == false) return;

            Helper.HideAndFadeOut(label, 0.25);
        }
    }
}
