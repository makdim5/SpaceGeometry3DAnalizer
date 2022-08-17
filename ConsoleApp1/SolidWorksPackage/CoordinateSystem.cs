using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;
using App2.util.mathutils;

namespace App2.SolidWorksPackage
{
    public class CoordinateSystem
    {
        //public double[,] matrix;

        MathUtility swMathUtils;

        public CoordinateSystem(MathUtility swMathUtils)
        {

            this.swMathUtils = swMathUtils;
            //this.matrix = GetDefaultCoordinateSystem();

        }

        public CoordinateSystem()
        {

        }

        public double[,] Multiply(double[,] matrix1, double[,] matrix2)
        {

            Matrix<double> m1 = Matrix<double>.Build.DenseOfArray(matrix1);
            Matrix<double> m2 = Matrix<double>.Build.DenseOfArray(matrix2);

            Matrix<double> result = m1 * m2;


            return result.ToArray();
        }

        public double[,] CreateCoordinateSystem(Vector3D vectorX, Vector3D vectorY, Vector3D vectorZ)
        {
            double[,] matrix = new double[,] {
                { vectorX.x, vectorX.y, vectorX.z, 0 },
                { vectorY.x, vectorY.y, vectorY.z, 0 },
                { vectorZ.x, vectorZ.y, vectorZ.z, 0 },
                { 0,         0,         0,         1 }
            };

            return matrix;
        }

        public double[,] GetDefaultCoordinateSystem()
        {
            return new double[,] {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
        }

        public double[,] MoveTo(double[,] matrix, Point3D point)
        {

            double[,] DCS = GetDefaultCoordinateSystem();

            DCS[0, 3] = point.x;
            DCS[1, 3] = point.y;
            DCS[2, 3] = point.z;

            Matrix<double> mDCS = Matrix<double>.Build.DenseOfArray(DCS);
            Matrix<double> mCDCS = Matrix<double>.Build.DenseOfArray(matrix);

            Matrix<double> result = mDCS * mCDCS;

            return result.ToArray();
        }

        public double[,] RotateAroundX(double[,] matrix, double radians)
        {

            MathTransform mathTransform = swMathUtils.CreateTransformRotateAxis(
                swMathUtils.CreatePoint(new double[] { matrix[0, 3], matrix[1, 3], matrix[2, 3] }) ,
                swMathUtils.CreateVector(new double[] { matrix[0, 0], matrix[0, 1], matrix[0, 2] }),
                radians) as MathTransform;

            return GetMatrix(mathTransform.ArrayData as double[]);

        }

        public double[,] RotateAroundY(double[,] matrix, double radians)
        {

            MathTransform mathTransform = swMathUtils.CreateTransformRotateAxis(
                swMathUtils.CreatePoint(new double[] { matrix[0, 3], matrix[1, 3], matrix[2, 3] }),
                swMathUtils.CreateVector(new double[] { matrix[1, 0], matrix[1, 1], matrix[1, 2] }),
                radians) as MathTransform;

            return GetMatrix(mathTransform.ArrayData as double[]);

        }

        public double[,] RotateAroundZ(double[,] matrix, double radians)
        {

            MathTransform mathTransform = swMathUtils.CreateTransformRotateAxis(
                swMathUtils.CreatePoint(new double[] { matrix[0, 3], matrix[1, 3], matrix[2, 3] }),
                swMathUtils.CreateVector(new double[] { matrix[2, 0], matrix[2, 1], matrix[2, 2] }),
                radians) as MathTransform;

            return GetMatrix(mathTransform.ArrayData as double[]);

        }

        public Vector3D TransformVector(double[,] matrix, Vector3D vector)
        {

            Vector3D result = new Vector3D();

            result.x = vector.x * matrix[0, 0] + vector.y * matrix[0, 1] + vector.z * matrix[0, 2] + matrix[0, 3];
            result.y = vector.x * matrix[1, 0] + vector.y * matrix[1, 1] + vector.z * matrix[1, 2] + matrix[1, 3];
            result.z = vector.x * matrix[2, 0] + vector.y * matrix[2, 1] + vector.z * matrix[2, 2] + matrix[2, 3];

            return result;

        }

        public Point3D TransformPoint(double[,] matrix, Point3D point)
        {

            Point3D result = new Point3D();

            result.x = point.x * matrix[0, 0] + point.y * matrix[0, 1] + point.z * matrix[0, 2] + matrix[0, 3];
            result.y = point.x * matrix[1, 0] + point.y * matrix[1, 1] + point.z * matrix[1, 2] + matrix[1, 3];
            result.z = point.x * matrix[2, 0] + point.y * matrix[2, 1] + point.z * matrix[2, 2] + matrix[2, 3];


            return result;

        }


        public double[,] GetMatrix(double[] transformMatrix)
        {

            double[,] matrix = new double[,] {
                { transformMatrix[0], transformMatrix[1], transformMatrix[2], transformMatrix[9] },
                { transformMatrix[3], transformMatrix[4], transformMatrix[5], transformMatrix[10] },
                { transformMatrix[6], transformMatrix[7], transformMatrix[8], transformMatrix[11] },
                { 0,                  0,                  0,                  1 }
            };

            return matrix;

        }

        public double[] GetTransformMatrix(double[,] matrix)
        {

            double[] transformMatrix = new double[] {
                 matrix[0,0], matrix[0,1], matrix[0,2],
                 matrix[1,0], matrix[1,1], matrix[1,2],
                 matrix[2,0], matrix[2,1], matrix[2,2],
                 matrix[0,3], matrix[1,3], matrix[2,3],
                 0,           0,           0,
                 1
            };

            return transformMatrix;

        }
    }
}
