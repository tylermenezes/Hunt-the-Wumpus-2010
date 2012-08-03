using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HuntTheWumpus.GameEngineCore;

namespace HuntTheWumpus.Utilities
{
    public static class MathUtil
    {
        /// <summary>
        /// random number, chosen by universal dice roll. literally though, UNIVERSAL dice roll.
        /// </summary>
        /// <returns>the answer</returns>
        public static float GetQuantumRandomNumber()
        {
            return float.NaN;
        }

        public static string ToPrettyString(this Vector3 vector)
        {
            //
            // {X:10.66667 Y:0.25 Z:0}
            // to
            // 10.66667,0.25,0
            //

            return String.Format("{0},{1},{2}", vector.X, vector.Y, vector.Z);
        }

        public static float PixelsPerMeter{
            get; private set;
        }

        public static float Height
        {
            get;
            private set;
        }

        static MathUtil()
        {
            Height = ((Size)DefaultSettings.Settings["WindowSize"]).Y;
        }

        public static void Init(float pixelsPerMeter)
        {
            PixelsPerMeter = pixelsPerMeter;
            
        }
        /// <summary>
        /// physics to client transform\ntransforms vectors from physics cooerdinates to graphics coordinates
        /// </summary>
        public static Vector2 P2C(this Vector2 a)//physics to client transform
        {
            return PixelsPerMeter * Vector2.Reflect(a, Vector2.UnitY) + new Vector2(0, Height);
        }

        public static Vector2 C2P(this Vector2 a)
        {
            return Vector2.Reflect((a - new Vector2(0, Height)) / PixelsPerMeter, Vector2.UnitY);
        }

        public static Vector2 Transpose(this Vector2 v)
        {
            return new Vector2(v.Y, v.X);
        }
        public static Vector2 Perpen(this Vector2 v)
        {
            return new Vector2(v.Y, -v.X);
        }
        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.X, v.Y, 0);
        }
        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector4 ToVector4(this Vector2 v)
        {
            return new Vector4(v, 0, 0);
        }
        public static Vector4 ToVector4(this Vector3 v)
        {
            return new Vector4(v, 0);
        }
        public static Vector2 ToVector2(this Vector4 v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector3 ToVector3(this Vector4 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector3 TransformToLocal(this Vector3 v, Matrix worldToLocal)
        {
            return Vector3.TransformNormal(v, worldToLocal);
        }
        public static Vector3 TransformToWorld(this Vector3 v, Matrix localToWorld)
        {
            return Vector3.TransformNormal(v, localToWorld);
        }
        public static float getPhase(this Vector2 vector)
        {
            return
                (float)((vector.X == 0) ? Math.PI / 2 : Math.Atan2(vector.Y, vector.X));
        }


        public static float GetMagnitude(this Vector2 vector)
        {
            return (float)Math.Sqrt(
                    Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2));
        }

        /// <summary>
        /// Create Vector from Phase and Magnitude
        /// </summary>
        public static Vector2 CreateVector(float phase, float magnitude)
        {
            return
                new Vector2(magnitude * (float)Math.Cos(phase),
                    magnitude * (float)Math.Sin(phase));
        }
        /// <summary>
        /// Calculates Angle Between Lines
        /// </summary>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="pointC"></param>
        public static float GetAngleABC(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            if (pointA == pointC) return 0f;
            else if (pointA == pointB) return float.NaN;
            else
            {
                return (float)Math.Abs(
                (pointA.X - pointB.X == 0) ? GetAngleABC(pointC, pointB, pointA) :
                    Math.Atan2(pointA.Y - pointB.Y, pointA.X - pointB.X) - Math.Atan2(pointC.Y - pointB.Y, pointC.X - pointB.X));
            }
        }

        public static void PrintOut(this Matrix m)
        {
            string sep = ", ";
            string str = "{{" + m.M11 + sep + m.M12 + sep + m.M13 + "},"
                        + "{" + m.M21 + sep + m.M22 + sep + m.M23 + "},"
                        + "{" + m.M31 + sep + m.M32 + sep + m.M33 + "}}";
            Debug.Trace("\r\n" + str);
        }

        public static Matrix ToXNA(this MathNet.Numerics.LinearAlgebra.Matrix m)
        {
            if (m.RowCount < 3 || m.ColumnCount < 3) throw new Exception("only 3x3 or higher mats supported. try your local Persian rug dealer.");

            Matrix xm = new Matrix();
            xm.M11 = (float)m[0, 0];
            xm.M12 = (float)m[0, 1];
            xm.M13 = (float)m[0, 2];

            xm.M21 = (float)m[1, 0];
            xm.M22 = (float)m[1, 1];
            xm.M23 = (float)m[1, 2];

            xm.M31 = (float)m[2, 0];
            xm.M32 = (float)m[2, 1];
            xm.M33 = (float)m[2, 2];

            return xm;
        }
        public static MathNet.Numerics.LinearAlgebra.Matrix ToMathNet( this Matrix m)
        {
            double[][] mdata = MathNet.Numerics.LinearAlgebra.Matrix.CreateMatrixData(3, 3);
            mdata[0][0] = m.M11;
            mdata[0][1] = m.M12;
            mdata[0][2] = m.M13;

            mdata[1][0] = m.M21;
            mdata[1][1] = m.M22;
            mdata[1][2] = m.M23;

            mdata[2][0] = m.M31;
            mdata[2][1] = m.M32;
            mdata[2][2] = m.M33;
            MathNet.Numerics.LinearAlgebra.Matrix mnet = new MathNet.Numerics.LinearAlgebra.Matrix(mdata);
            return mnet;
        }

        /// <summary>
        /// finds the velocity of a point rotating about another point
        /// </summary>
        /// <param name="relPos">the relative position of the point (from its center of rotation)</param>
        /// <param name="angVel">the angular velocity of the point</param>
        /// <returns></returns>
        public static Vector2 VelOfPoint(Vector2 relPos, float angVel)
        {
            return Vector3.Cross(angVel * Vector3.UnitZ, relPos.ToVector3()).ToVector2();
        }

        /// <summary>
        /// rotates a point around the orgin
        /// </summary>
        /// <param name="pt">point</param>
        /// <param name="rot">angle to rotate</param>
        /// <returns></returns>
        public static Vector2 RotatePt(Vector2 pt, float rot)
        {
            Matrix rotM = Matrix.CreateRotationZ(rot);
            return Vector2.Transform(pt, rotM);
        }
    }
}
