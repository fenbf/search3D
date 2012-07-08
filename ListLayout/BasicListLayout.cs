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
    /** controls layout of the element list in the 3D space.
     * setups positions, controls animations, and other basic params.
     * this class simply creates grid layout in XY orientation.
     **/
    public class BasicListLayout : s3dCore.ListLayout.IListLayout
    {
        protected int counter = 0;
        private static int Grids = 3;
        private double maxRight = 1;

        /**
         * call it when new list appears
         */
        public void ResetData()
        {
            counter = 0;
            maxRight = 1.0;
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
            // position:
            Point3D p = new Point3D();
            int row = counter % Grids;
            int column = counter / Grids;
            p.X = (double)column;
            p.Y = (double)row - (double)Grids * 0.35;
            p.X *= 1.5f;
            p.Y *= 1.5f;
            p.Z = 0.0f;
            ++counter;

            if (p.X > maxRight)
                maxRight = p.X;

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
                    Angle = 0,
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
            return new CameraParams(trans.OffsetX, trans.OffsetY, trans.OffsetZ + 5);
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

            TranslateTransform3D translate = group.Children[2] as TranslateTransform3D;
            DoubleAnimation tranAnim = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(0.25)));
            translate.BeginAnimation(TranslateTransform3D.OffsetZProperty, tranAnim);

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
            DoubleAnimation tranAnim = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(0.1)));
            translate.BeginAnimation(TranslateTransform3D.OffsetZProperty, tranAnim);
        }

        public CameraParams GetFrontVectorParams() 
        {
            return new CameraParams(0.0, 0.0, -0.25);
        }
        public CameraParams GetRightVectorParams()
        {
            return new CameraParams(1.0, 0.0, 0.0);
        }

        public bool CanMoveHorizontally()
        {
            return true;
        }

        public bool IsInBoundingBox(Point3D p)
        {
            return (p.X > -1 && p.X < maxRight &&
                p.Y < 4 && p.Y > -4 &&
                p.Z > -10 && p.Z < 24);
        }
        /** return true if search engine should fetch more images */
        public bool IsViewingEndReached(Point3D p, Vector3D rot)
        {
            return p.X > maxRight - 8;
        }

        public string GetName() { return "Basic"; }
    }
}
