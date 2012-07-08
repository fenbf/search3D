using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows;

namespace search3D
{
    public class Helper
    {
        private static KeySpline splineStart = new KeySpline(0.4, 0.85, 0.5, 0.0);
        private static KeySpline splineEnd = new KeySpline(0.25, 0.65, 0.25, 1);

        public static Point3DAnimationUsingKeyFrames GetSmoothAnimation(Point3D start, Point3D end, double duration)
        {
            Point3DAnimationUsingKeyFrames anim = new Point3DAnimationUsingKeyFrames();
            anim.Duration = new Duration(TimeSpan.FromSeconds(duration));
            SplinePoint3DKeyFrame p1 = new SplinePoint3DKeyFrame(start,
                                                                 KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0)),
                                                                 splineStart);
            anim.KeyFrames.Add(p1);
            SplinePoint3DKeyFrame p2 = new SplinePoint3DKeyFrame(end,
                                                                 KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration)),
                                                                 splineEnd);
            anim.KeyFrames.Add(p2);

            return anim;
        }
        public static DoubleAnimationUsingKeyFrames GetSmoothAnimation(double start, double end, double duration)
        {
            DoubleAnimationUsingKeyFrames anim = new DoubleAnimationUsingKeyFrames();
            anim.Duration = new Duration(TimeSpan.FromSeconds(duration));
            SplineDoubleKeyFrame p1 = new SplineDoubleKeyFrame(start,
                                                                 KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0)),
                                                                 splineStart);
            anim.KeyFrames.Add(p1);
            SplineDoubleKeyFrame p2 = new SplineDoubleKeyFrame(end,
                                                                 KeyTime.FromTimeSpan(TimeSpan.FromSeconds(duration)),
                                                                 splineEnd);
            anim.KeyFrames.Add(p2);

            return anim;
        }

        public static void ShowAndFadeIn(UIElement control, double seconds) 
        {
            control.Visibility = Visibility.Visible;
            DoubleAnimation fadein = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(seconds)));
            control.BeginAnimation(UIElement.OpacityProperty, fadein);
        }
        public static void HideAndFadeOut(UIElement control, double seconds)
        {
            DoubleAnimation fadeout = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(seconds)));
            fadeout.Completed += delegate(object sender,EventArgs e)
            {
                control.Visibility = Visibility.Collapsed;
            };
            control.BeginAnimation(UIElement.OpacityProperty, fadeout);
        }

    }
}
