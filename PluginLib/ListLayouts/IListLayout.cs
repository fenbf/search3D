using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows;

namespace s3dCore.ListLayout
{
    public class CameraParams
    {
        public Point3D Position;
        public Vector3D Rotations;

        public CameraParams() 
        {
            Position = new Point3D();
            Rotations = new Vector3D();
        }

        public CameraParams(double x, double y, double z)
        {
            Position = new Point3D(x, y, z);
            Rotations = new Vector3D();
        }

        public CameraParams(double x, double y, double z, double rx, double ry, double rz)
        {
            Position = new Point3D(x, y, z);
            Rotations = new Vector3D(rx, ry, rz);
        }

        public static void ClipRotationAngles(CameraParams param)
        {
            /*param.Rotations.X = ClipAngle(param.Rotations.X);
            param.Rotations.Y = ClipAngle(param.Rotations.Y);
            param.Rotations.Z = ClipAngle(param.Rotations.Z);*/
        }

        public static void ClipRotationAngles(ref Vector3D rot)
        {
            rot.X = ClipAngle(rot.X);
            rot.Y = ClipAngle(rot.Y);
            rot.Z = ClipAngle(rot.Z);
        }

        private static double ClipAngle(double ang)
        {
            int r = (int)ang;
            double f = ang - r;
            r %= 360;
            return r + f;
        }
    }

    public interface IListLayout
    {
        /** resets the data, prepares for new list */
        void ResetData();

        /** returns staring position to view the list - default position */
        CameraParams GetStartPosition();

        /** get camera's position to view specified model */
        CameraParams GetElementViewPosition(ModelUIElement3D model);

        /** creates specific transformation for the next element in the list */
        Transform3DGroup GetNextTransformation();

        /** creates animation when mouse is over the object */
        void SetupAnimationOnMouseEnter(ModelUIElement3D model);

        /** creates animation on mouse leve - inverse animation of mouse enter */
        void SetupAnimationOnMouseLeave(ModelUIElement3D model);

        /** vector used in moving formward - by mouse wheel - this vector will be multiplied by
         * Delta param from mouse wheel rotation */
        CameraParams GetFrontVectorParams();

        /** right direction, used when moving right - this will not be multiplied by any delta value! */
        CameraParams GetRightVectorParams();

        bool CanMoveHorizontally();

        /** returns true if Point p is in bounding box defined by the layout */
        bool IsInBoundingBox(Point3D p);

        /** image list fetches partially, when camera is viewing current end of the list
         * layout should decide wheather to download the next part of list */
        bool IsViewingEndReached(Point3D p, Vector3D rot);  
      
        /** return name of the layout so that it can be displayed in the layouts list in UI */
        string GetName();
    }
}
