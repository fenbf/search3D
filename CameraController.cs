using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows;

using s3dCore.ListLayout;

namespace search3D
{
    class CameraController
    {
        private ProjectionCamera camera;
        private IListLayout layout;
        private PointLight light;
        private AxisAngleRotation3D cameraRotX;
        private AxisAngleRotation3D cameraRotY;
        private AxisAngleRotation3D cameraRotZ;
        private AxisAngleRotation3D lightRotX;
        private AxisAngleRotation3D lightRotY;
        private AxisAngleRotation3D lightRotZ;

        /** move to list element, to start position, on mouse wheel, etc */
        public double MoveDuration = 1.25;
        /** move left, right, etc */
        public double SmallMoveDuration = 0.5;

        private bool viewingEndReached;
        public bool IsViewingEndReached
        {
            get { return viewingEndReached; }
        }

        public CameraController(Camera c, IListLayout l, PointLight p)
        {
            camera = (ProjectionCamera)c;
            layout = l;
            light = p;

            try
            {
                Transform3DGroup group = camera.Transform as Transform3DGroup;
                RotateTransform3D rot = group.Children[0] as RotateTransform3D;
                cameraRotX = rot.Rotation as AxisAngleRotation3D;
                rot = group.Children[1] as RotateTransform3D;
                cameraRotY = rot.Rotation as AxisAngleRotation3D;
                rot = group.Children[2] as RotateTransform3D;
                cameraRotZ = rot.Rotation as AxisAngleRotation3D;
            }
            catch (Exception )
            {
                System.Diagnostics.Debug.WriteLine("camera transformations are wrong!!!");
            }
            try
            {
                Transform3DGroup group = light.Transform as Transform3DGroup;
                RotateTransform3D rot = group.Children[0] as RotateTransform3D;
                lightRotX = rot.Rotation as AxisAngleRotation3D;
                rot = group.Children[1] as RotateTransform3D;
                lightRotY = rot.Rotation as AxisAngleRotation3D;
                rot = group.Children[2] as RotateTransform3D;
                lightRotZ = rot.Rotation as AxisAngleRotation3D;
            }
            catch (Exception )
            {
                System.Diagnostics.Debug.WriteLine("light transformations are wrong!!!");
            }
        }

        public void ChangeLayout(IListLayout newLayout)
        {
            layout = newLayout;
        }

        public void MoveForward(double param)
        {
            CameraParams camParams = layout.GetFrontVectorParams();
            Point3D dest = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z);
            Vector3D offset = (Vector3D)camParams.Position;
            dest += offset * param;

            Vector3D rot = new Vector3D(cameraRotX.Angle, cameraRotY.Angle, cameraRotZ.Angle);
            rot += param * camParams.Rotations;
            CameraParams.ClipRotationAngles(ref rot);

            // is moving in that direcion blocked?
            viewingEndReached = layout.IsViewingEndReached(dest, rot);

            if (layout.IsInBoundingBox(dest) == false)
            {
                return;
            }

            

            SetupAnimations(dest, rot, MoveDuration);
        }

        public void MoveHorizontally(double delta)
        {
            CameraParams param = layout.GetRightVectorParams();
            Point3D dest = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z);
            Vector3D offset = (Vector3D)param.Position;
            dest += delta * offset;

            Vector3D rot = new Vector3D(cameraRotX.Angle, cameraRotY.Angle, cameraRotZ.Angle);
            rot += delta * param.Rotations;
            CameraParams.ClipRotationAngles(ref rot);

            viewingEndReached = layout.IsViewingEndReached(dest, rot);

            if (layout.IsInBoundingBox(dest) == false)
            {
                return;
            }

            SetupAnimations(dest, rot, SmallMoveDuration);
        }

        public void MoveToStartPosition()
        {
            CameraParams dest = layout.GetStartPosition();

            SetupAnimations(dest.Position, dest.Rotations, MoveDuration); 
        }

        public void MoveToSelectedElement(CameraParams param)
        {
            SetupAnimations(param.Position, param.Rotations, MoveDuration);
        }

        private void SetupAnimations(Point3D p, Vector3D rot, double time)
        {
            if (camera.Position.Equals(p) == false)
            {
                Point3DAnimationUsingKeyFrames anim = Helper.GetSmoothAnimation(camera.Position, p, time);

                camera.BeginAnimation(PerspectiveCamera.PositionProperty, anim);
                light.BeginAnimation(PointLight.PositionProperty, anim);
            }
            s3dCore.ListLayout.CameraParams.ClipRotationAngles(ref rot);
            if (rot.X != cameraRotX.Angle)
            {
                cameraRotX.BeginAnimation(AxisAngleRotation3D.AngleProperty, Helper.GetSmoothAnimation(cameraRotX.Angle, rot.X, time));
                lightRotX.BeginAnimation(AxisAngleRotation3D.AngleProperty, Helper.GetSmoothAnimation(lightRotX.Angle, rot.X, time));
            }
            if (rot.Y != cameraRotY.Angle)
            {
                double start = cameraRotY.Angle;
                double start2 = 360.0 + cameraRotY.Angle;
                //if (Math.Abs(start - rot.Y) > Math.Abs(start2 - rot.Y)) start = start2;

                double end = rot.Y;
                //double end2 = 360.0 + end;
                //if (Math.Abs(start - end) > Math.Abs(start - end2)) end = end2;
                //if (Math.Abs(start2 - end) > Math.Abs(start2 - end2)) end = end2;
                //if (rot.Y - cameraRotY.Angle > 180.0) start = 360.0-cameraRotY.Angle;
                //if (rot.Y - cameraRotY.Angle < -180.0) rot.Y = 360.0 - rot.Y;
                cameraRotY.BeginAnimation(AxisAngleRotation3D.AngleProperty, Helper.GetSmoothAnimation(start, end, time));
                lightRotY.BeginAnimation(AxisAngleRotation3D.AngleProperty, Helper.GetSmoothAnimation(start, end, time));
            }
            if (rot.Z != cameraRotZ.Angle)
            {
                cameraRotZ.BeginAnimation(AxisAngleRotation3D.AngleProperty, Helper.GetSmoothAnimation(cameraRotZ.Angle, rot.Z, time));
                lightRotZ.BeginAnimation(AxisAngleRotation3D.AngleProperty, Helper.GetSmoothAnimation(lightRotZ.Angle, rot.Z, time));
            }
        }
    }
}
