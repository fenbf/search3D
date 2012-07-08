using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows;
using s3dCore.ListLayout;

namespace CylinderLayout
{
    public class CylinderLayout : IListLayout
    {
        private double maxHeight = 0.0;
        private int counter = 0;
        private const int Grids = 1;

        public void ResetData()
        {
            counter = 0;
            maxHeight = -1.0;
        }

        public CameraParams GetStartPosition()
        {
            return new CameraParams(0.0f, 0.0f, 7.0);
        }

        public CameraParams GetElementViewPosition(ModelUIElement3D model)
        {
            Transform3DGroup group = model.Model.Transform as Transform3DGroup;
            RotateTransform3D rot = group.Children[2] as RotateTransform3D;
            AxisAngleRotation3D ang = rot.Rotation as AxisAngleRotation3D;

            TranslateTransform3D trans = group.Children[1] as TranslateTransform3D;
            return new CameraParams(0.0, trans.OffsetY, 7.0, 0.0, ang.Angle, 0.0);
        }

        /**
         * Creates transformation group for the object in the list, it can be rotated, translated, scaled, as layout class wish :)
         * \return transformation group for the object in the list
         */
        public Transform3DGroup GetNextTransformation()
        {
            Point3D p = new Point3D();
            int row = counter % Grids;
            int column = counter / Grids;
            p.Z = -7.0;// (double)column;
            p.Y = (double)row - (double)Grids * 0.5;
            p.Y *= 1.5f;
            p.Y += column * 2.5 / 36.0;
            p.X = 0.0f;

            if (maxHeight < p.Y) maxHeight = p.Y;

            Transform3DGroup group = new Transform3DGroup();
            group.Children.Add(new ScaleTransform3D
            {
                ScaleX = 1.0,
                ScaleY = 1.0,
                ScaleZ = 1.0
            });
            group.Children.Add(new TranslateTransform3D
            {
                OffsetX = p.X,
                OffsetY = p.Y,
                OffsetZ = p.Z
            });
            int angle = counter * 10;
            angle %= 360;
            group.Children.Add(new RotateTransform3D
            {
                Rotation = new AxisAngleRotation3D
                {
                    Angle = angle,
                    Axis = new Vector3D(0, 1, 0)
                }
            });

            ++counter;

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

            /*TranslateTransform3D translate = group.Children[1] as TranslateTransform3D;
            DoubleAnimation tranAnim = new DoubleAnimation(-8, new Duration(TimeSpan.FromSeconds(0.25)));
            translate.BeginAnimation(TranslateTransform3D.OffsetZProperty, tranAnim);
             */
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

            /*TranslateTransform3D translate = group.Children[1] as TranslateTransform3D;
            DoubleAnimation tranAnim = new DoubleAnimation(-10, new Duration(TimeSpan.FromSeconds(0.1)));
            translate.BeginAnimation(TranslateTransform3D.OffsetZProperty, tranAnim);*/
        }

        public CameraParams GetFrontVectorParams()
        {
            return new CameraParams(0.0, 0.5 * 2.5 / 36.0, 0.0, 0.0, 5, 0.0);
        }

        /** movement in right direction is blocked */
        public CameraParams GetRightVectorParams()
        {
            return new CameraParams(0.0, -0.5 * 2.5 / 36.0, 0.0, 0.0, 5, 0.0);
        }

        public bool CanMoveHorizontally()
        {
            return true;
        }

        public bool IsInBoundingBox(Point3D p)
        {
            return (p.X > -10 && p.X < 20 &&
                p.Y < maxHeight + 2 && p.Y > -1 &&
                p.Z > -10 && p.Z < 10);
        }

        /** return true if search engine should fetch more images */
        public bool IsViewingEndReached(Point3D p, Vector3D rot)
        {
            return rot.Y >= 300 || p.Y > maxHeight;
        }

        public string GetName() { return "Cylinder"; }
    }
}
