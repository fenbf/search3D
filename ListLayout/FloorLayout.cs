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
    public class FloorLayout : s3dCore.ListLayout.IListLayout
    {
        public static int Grids = 5;
        public double maxDist = 0.0;
        private int counter = 0;

        public void ResetData()
        {
            counter = 0;
            maxDist = 0.0;
        }

        public CameraParams GetStartPosition()
        {
            return new CameraParams(0.0f, 0.0f, 10.0);
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
            p.Z = -(double)column;
            p.X = (double)row - (double)Grids * 0.5;
            p.Z *= 2.5f;
            p.X *= 1.5f;
            p.Y = -1.5f;
            ++counter;

            if (p.Z < maxDist) maxDist = p.Z;

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
                    Angle = -70,
                    Axis = new Vector3D(1, 0, 0)
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

        public CameraParams GetElementViewPosition(ModelUIElement3D model)
        {
            Transform3DGroup group = model.Model.Transform as Transform3DGroup;
            TranslateTransform3D trans = group.Children[2] as TranslateTransform3D;
            return new CameraParams(trans.OffsetX, 0.0, trans.OffsetZ + 8);
        }

        /**
        * starts animation on mouse enter on specific element in the list
        * 
        * model's transformations must be in order [0] - scale, [1] - rotation, [2] - translation
        * 
        * \param model object that sould be animated 
        */
        public void SetupAnimationOnMouseEnter(ModelUIElement3D model)
        {
            Transform3DGroup group = model.Model.Transform as Transform3DGroup;
            /*ScaleTransform3D scale = group.Children[0] as ScaleTransform3D;
            DoubleAnimation anim = new DoubleAnimation(1.3, new Duration(TimeSpan.FromSeconds(0.25)));
            scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, anim);
            scale.BeginAnimation(ScaleTransform3D.ScaleZProperty, anim);*/

            TranslateTransform3D translate = group.Children[2] as TranslateTransform3D;
            DoubleAnimation tranAnim = new DoubleAnimation(-1.20, new Duration(TimeSpan.FromSeconds(0.1)));
            translate.BeginAnimation(TranslateTransform3D.OffsetYProperty, tranAnim);

            RotateTransform3D rotate = group.Children[1] as RotateTransform3D;
            AxisAngleRotation3D axis = rotate.Rotation as AxisAngleRotation3D;
            DoubleAnimation daRotate = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(0.25)));
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

            TranslateTransform3D translate = group.Children[2] as TranslateTransform3D;
            DoubleAnimation tranAnim = new DoubleAnimation(-1.5, new Duration(TimeSpan.FromSeconds(0.1)));
            translate.BeginAnimation(TranslateTransform3D.OffsetYProperty, tranAnim);

            RotateTransform3D rotate = group.Children[1] as RotateTransform3D;
            AxisAngleRotation3D axis = rotate.Rotation as AxisAngleRotation3D;
            DoubleAnimation daRotate = new DoubleAnimation(-70.0, new Duration(TimeSpan.FromSeconds(0.1)));
            axis.BeginAnimation(AxisAngleRotation3D.AngleProperty, daRotate);
        }

        public CameraParams GetFrontVectorParams()
        {
            return new CameraParams(0.0, 0.0, -1.0);
        }
        public CameraParams GetRightVectorParams()
        {
            return new CameraParams(0.5, 0.0, 0.0);
        }

        public bool CanMoveHorizontally()
        {
            return true;
        }

        public bool IsInBoundingBox(Point3D p)
        {
            return (p.X > -Grids*1.2 && p.X < Grids*1.2 &&
                p.Y < 1 && p.Y > -1.5 &&
                p.Z > maxDist && p.Z < 24);
        }

        /** return true if search engine should fetch more images */
        public bool IsViewingEndReached(Point3D p, Vector3D rot)
        {
            return p.Z < maxDist + 13;
        }

        public string GetName() { return "Floor"; }
    }
}
