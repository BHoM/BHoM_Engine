/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a TransformMatrix corresponding to the provided Quaternion. Used to create part of the Rotation matrix.")]
        [Input("q", "The Quaternion to create the TransformMatrix matrix from.")]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix TransformMatrix(Quaternion q)
        {
            return new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  1 - 2*Math.Pow(q.Y,2) - 2*Math.Pow(q.Z, 2), 2 * q.X*q.Y - 2 * q.W*q.Z, 2 * q.X*q.Z + 2 * q.Y*q.W,   0   },
                    {  2 * q.X*q.Y + 2 * q.W*q.Z,  1 - 2*Math.Pow(q.X,2) - 2*Math.Pow(q.Z, 2), 2 * q.Y*q.Z - 2 * q.W*q.X,  0   },
                    {  2 * q.X*q.Z - 2 * q.W*q.Y,  2 * q.Y*q.Z + 2 * q.W*q.X,  1 - 2*Math.Pow(q.X,2) - 2*Math.Pow(q.Y, 2), 0   },
                    {  0,                          0,                          0,                                          1   }
                }
            };
        }

        /***************************************************/

        [PreviousInputNames("translation", "vector")]
        [Description("Creates a Translation matrix from the provided Vector.")]
        [Input("translation", "The Vector corresponding to the seeked translation.")]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix TranslationMatrix(Vector translation)
        {
            if (translation == null)
                return null;
            return new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  1,  0,  0,  translation.X   },
                    {  0,  1,  0,  translation.Y   },
                    {  0,  0,  1,  translation.Z   },
                    {  0,  0,  0,  1   }
                }
            };
        }

        /***************************************************/

        [Description("Creates a Identity TransformMatrix, e.g. a Matrix with 1 values on the main diagonal and all other values 0.")]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix IdentityMatrix()
        {
            return new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  1,  0,  0,  0   },
                    {  0,  1,  0,  0   },
                    {  0,  0,  1,  0   },
                    {  0,  0,  0,  1   }
                }
            };
        }

        /***************************************************/

        [Description("Creates a RotationMatrix corresponding to a rotation with the specifiend angle around the provided vector axis and centre point.")]
        [Input("centre", "The centre of the rotation.")]
        [Input("axis", "The axis to rotate around.")]
        [Input("angle", "The angle of the rotation.", typeof(Angle))]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix RotationMatrix(Point centre, Vector axis, double angle)
        {
            TransformMatrix rotation = TransformMatrix(Quaternion(axis.Normalise(), angle));
            TransformMatrix t1 = TranslationMatrix(centre - oM.Geometry.Point.Origin);
            TransformMatrix t2 = TranslationMatrix(oM.Geometry.Point.Origin - centre);

            return t1 * rotation * t2;
        }

        /***************************************************/

        [Description("Creates a ScaleMatrix for a non-uniform scaling based on a scale vector and a reference point.")]
        [Input("refPoint", "Base point of the scaling.")]
        [Input("scaleVector", "The scale vector used to determine by how much the geometry should be scaled in each direction.")]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix ScaleMatrix(Point refPoint, Vector scaleVector)
        {
            TransformMatrix move1 = TranslationMatrix(oM.Geometry.Point.Origin - refPoint);
            TransformMatrix move2 = TranslationMatrix(refPoint - oM.Geometry.Point.Origin);
            TransformMatrix scale = new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  scaleVector.X,  0,  0,  0   },
                    {  0, scaleVector.Y,  0,  0   },
                    {  0,  0, scaleVector.Z,  0   },
                    {  0,  0,  0,  1   }
                }
            };

            return move2 * scale * move1;
        }

        /***************************************************/

        [Description("Creates a TransformMatrix corresponding to a projection of the geometry on to the plane along a vector. If no vector is provided, the projection will be along the normal of the plane.")]
        [Input("plane", "The plane to project the geometry to.")]
        [Input("vector", "Optional vector to project along. The normal of the plane will be used if nothing is provided.")]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix ProjectionMatrix(Plane plane, Vector vector = null)
        {
            Point x = new Point() { X = 1 };
            Point y = new Point() { Y = 1 };
            Point z = new Point() { Z = 1 };

            vector = vector == null || vector.SquareLength() < Tolerance.Distance * Tolerance.Distance ? plane.Normal : vector;

            Vector refVector = (new Point()).ProjectAlong(plane, vector) - new Point();

            // Set plane to origin for projection
            plane = new Plane() { Normal = plane.Normal };

            // Compute the projection for three controlPoints defining the transformation
            x = x.ProjectAlong(plane, vector);
            y = y.ProjectAlong(plane, vector);
            z = z.ProjectAlong(plane, vector);

            TransformMatrix project = new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  x.X, y.X, z.X, 0 },
                    {  x.Y, y.Y, z.Y, 0 },
                    {  x.Z, y.Z, z.Z, 0 },
                    {  0,   0,   0,   1 }
                }
            };

            // Move the projection out from the origin to the original plane
            TransformMatrix move = TranslationMatrix(refVector);

            return move * project;
        }

        /***************************************************/

        [Description("Creates a TransformMatrix corresponding to a orientation of the geometry from Global coordinates, with origin at {0,0,0} to the local system provided.")]
        [Input("csTo", "The cartesian coordinate system to orient the geometry to.")]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix OrientationMatrixGlobalToLocal(Cartesian csTo)
        {
            Vector XWorld = new Vector { X = 1, Y = 0, Z = 0 };
            Vector YWorld = new Vector { X = 0, Y = 1, Z = 0 };
            Vector ZWorld = new Vector { X = 0, Y = 0, Z = 1 };

            Vector XTo = csTo.X.Normalise();
            Vector YTo = csTo.Y.Normalise();
            Vector ZTo = csTo.Z.Normalise();

            TransformMatrix globalToLocal = new TransformMatrix
            {
                Matrix = new double[,]
                {
                    { XWorld.DotProduct(XTo), XWorld.DotProduct(YTo), XWorld.DotProduct(ZTo), csTo.Origin.X },
                    { YWorld.DotProduct(XTo), YWorld.DotProduct(YTo), YWorld.DotProduct(ZTo), csTo.Origin.Y },
                    { ZWorld.DotProduct(XTo), ZWorld.DotProduct(YTo), ZWorld.DotProduct(ZTo), csTo.Origin.Z },
                    { 0,                      0,                      0                     , 1  }
                }
            };

            return globalToLocal;
        }

        /***************************************************/

        [Description("Creates a TransformMatrix corresponding to a orientation of the geometry to Global coordinates, with origin at {0,0,0} from the local system provided.")]
        [Input("csFrom", "The cartesian coordinate system to orient the geometry from.")]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix OrientationMatrixLocalToGlobal(Cartesian csFrom)
        {
            return OrientationMatrixGlobalToLocal(csFrom).Invert();
        }

        /***************************************************/

        [Description("Creates a TransformMatrix corresponding to a orientation of the geometry from one local coordinate system to another.")]
        [Input("csFrom", "The cartesian coordinate system to orient the geometry from.")]
        [Input("csTo", "The cartesian coordinate system to orient the geometry to.")]
        [Output("transform", "The created TransformMatrix.")]
        public static TransformMatrix OrientationMatrix(this Cartesian csFrom, Cartesian csTo)
        {
            return OrientationMatrixGlobalToLocal(csTo) * OrientationMatrixLocalToGlobal(csFrom);
        }

        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [Description("Creates a random TransformMatrix based on a seed. If no seed is provided, a random one will be generated.")]
        [Input("seed", "Input seed for random generation. If -1 is provided, a random seed will be generated.")]
        [Input("minVal", "Minimum value to be used for each element in the created random matrix.")]
        [Input("maxVal", "Maximum value to be used for each element in the created random matrix.")]
        [Output("transform", "The generated random TransformMatrix.")]
        public static TransformMatrix RandomMatrix(int seed = -1, double minVal = -1, double maxVal = 1)
        {
            if (seed == -1)
                seed = NextRandomSeed();
            Random rnd = new Random(seed);
            return RandomMatrix(rnd, minVal, maxVal);
        }

        /***************************************************/

        [Description("Creates a random TransformMatrix using the provided Random class.")]
        [Input("rnd", "Random object to be used to generate the random geometry.")]
        [Input("minVal", "Minimum value to be used for each element in the created random matrix.")]
        [Input("maxVal", "Maximum value to be used for each element in the created random matrix.")]
        [Output("transform", "The generated random TransformMatrix.")]
        public static TransformMatrix RandomMatrix(Random rnd, double minVal = -1, double maxVal = 1)
        {
            double[,] matrix = new double[4, 4];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 4; j++)
                    matrix[i, j] = minVal + rnd.NextDouble() * (maxVal - minVal);
            matrix[3, 3] = 1;

            return new TransformMatrix { Matrix = matrix };
        }

        /***************************************************/
    }
}



