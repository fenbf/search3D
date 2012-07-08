using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows;

using s3dCore.ListLayout;

namespace search3D.ListLayout
{
    public class SpiralLayout : s3dCore.ListLayout.IListLayout
    {
        private double maxDist = 0.0;
        private int counter = 0;

        public void ResetData()
        {
            counter = 0;
            maxDist = -1.0;
        }

        public CameraParams GetStartPosition()
        {
            return new CameraParams(0.0f, 0.0f, 10.0);
        }

        public CameraParams GetElementViewPosition(ModelUIElement3D model)
        {
            Transform3DGroup group = model.Model.Transform as Transform3DGroup;
            TranslateTransform3D trans = group.Children[2] as TranslateTransform3D;
            return new CameraParams(0.0, 0.0, trans.OffsetZ + 7, 0.0, 0.0, 0);
        }

        /**
         * Creates transformation group for the object in the list, it can be rotated, translated, scaled, as layout class wish :)
         * \return transformation group for the object in the list
         */
        public Transform3DGroup GetNextTransformation()
        {
            Point3D p = new Point3D();
            p.X = 1.8f * Math.Cos(counter * 0.17 * 30.0);
            p.Y = 1.2f * Math.Sin(counter * 0.17 * 30.0);
            p.Z = -counter;

            if (p.Z < maxDist) maxDist = p.Z;

            ++counter;

            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(new ScaleTransform3D
            {
                ScaleX = 1.0,
                ScaleY = 1.0,
                ScaleZ = 1.0
            });
            group.Children.Add(new RotateTransform3D
            {
                Rotation = new AxisAngleRotation3D
                {
                    Angle = 35,
                    Axis = new Vector3D(1, 1, 1)
                }
            });
            group.Children.Add(new TranslateTransform3D
            {
                OffsetX = p.X,
                OffsetY = p.Y,
                OffsetZ = p.Z
            });

            return group;
        }

        /**
         * starts animation on mouse enter on specific element in the list
         * 
         * \param model object that sould be animated 
         */
        public void SetupAnimationOnMouseEnter(ModelUIElement3D model)
        {
            Transform3DGroup group = model.Model.Transform as Transform3DGroup;
            ScaleTransform3D scale = group.Children[0] as ScaleTransform3D;
            DoubleAnimation anim = new DoubleAnimation(1.3, new Duration(TimeSpan.FromSeconds(0.25)));
            scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, anim);
            scale.BeginAnimation(ScaleTransform3D.ScaleZProperty, anim);

            RotateTransform3D rotate = group.Children[1] as RotateTransform3D;
            AxisAngleRotation3D axis = rotate.Rotation as AxisAngleRotation3D;
            double rot = 0;
            DoubleAnimation daRotate = new DoubleAnimation(rot, new Duration(TimeSpan.FromSeconds(0.45)));
            axis.BeginAnimation(AxisAngleRotation3D.AngleProperty, daRotate);
        }

        /**
         * starts animation on mouse leave on specific element in the list
         * 
         * \param model object that sould be animated 
         */
        public void SetupAnimationOnMouseLeave(ModelUIElement3D model)
        {
            Transform3DGroup group = model.Model.Transform as Transform3DGroup;
            ScaleTransform3D scale = group.Children[0] as ScaleTransform3D;
            DoubleAnimation anim = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(0.1)));
            scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, anim);
            scale.BeginAnimation(ScaleTransform3D.ScaleZProperty, anim);

            /*TranslateTransform3D translate = group.Children[2] as TranslateTransform3D;
            DoubleAnimation tranAnimX = new DoubleAnimation(tempPos.X, new Duration(TimeSpan.FromSeconds(0.1)));
            translate.BeginAnimation(TranslateTransform3D.OffsetXProperty, tranAnimX);
            DoubleAnimation tranAnimY = new DoubleAnimation(tempPos.Y, new Duration(TimeSpan.FromSeconds(0.1)));
            translate.BeginAnimation(TranslateTransform3D.OffsetYProperty, tranAnimY);*/

            RotateTransform3D rotate = group.Children[1] as RotateTransform3D;
            AxisAngleRotation3D axis = rotate.Rotation as AxisAngleRotation3D;
            double rot = 35;
            DoubleAnimation daRotate = new DoubleAnimation(rot, new Duration(TimeSpan.FromSeconds(0.25)));
            axis.BeginAnimation(AxisAngleRotation3D.AngleProperty, daRotate);
        }

        public CameraParams GetFrontVectorParams()
        {
            return new CameraParams(0.0, 0.0, -1.0, 0.0, 0.0, 0.0);
        }

        /** movement in right direction is blocked */
        public CameraParams GetRightVectorParams()
        {
            return new CameraParams(0.0, 0.0, 0.0);
        }

        public bool CanMoveHorizontally()
        {
            return false;
        }

        public bool IsInBoundingBox(Point3D p)
        {
            return (p.X > -2 && p.X < 2 &&
                p.Y < 2 && p.Y > -2 &&
                p.Z > maxDist && p.Z < 11);
        }

        /** return true if search engine should fetch more images */
        public bool IsViewingEndReached(Point3D p, Vector3D rot)
        {
            return p.Z <= maxDist+12;
        }

        public string GetName() { return "Spiral"; }
    }
}
