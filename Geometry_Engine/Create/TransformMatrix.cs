/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

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

        public static TransformMatrix TranslationMatrix(Vector vector)
        {
            return new TransformMatrix
            {
                Matrix = new double[,]
                {
                    {  1,  0,  0,  vector.X   },
                    {  0,  1,  0,  vector.Y   },
                    {  0,  0,  1,  vector.Z   },
                    {  0,  0,  0,  1   }
                }
            };
        }

        /***************************************************/

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

        public static TransformMatrix RotationMatrix(Point centre, Vector axis, double angle)
        {
            TransformMatrix rotation = TransformMatrix(Quaternion(axis.Normalise(), angle));
            TransformMatrix t1 = TranslationMatrix(centre - oM.Geometry.Point.Origin);
            TransformMatrix t2 = TranslationMatrix(oM.Geometry.Point.Origin - centre);

            return t1 * rotation * t2;
        }

        /***************************************************/

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

        public static TransformMatrix ProjectionMatrix(Plane plane, Vector vector = null)
        {
            Point x = new Point() { X = 1 };
            Point y = new Point() { Y = 1 };
            Point z = new Point() { Z = 1 };

            vector = vector ?? plane.Normal;

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

        public static TransformMatrix RandomMatrix(int seed = -1, double minVal = -1, double maxVal = 1)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomMatrix(rnd, minVal, maxVal);
        }

        /***************************************************/

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

        public static TransformMatrix OrientationMatrixLocalToGlobal(Cartesian csFrom)
        {
            return OrientationMatrixGlobalToLocal(csFrom).Invert();
        }
    
        /***************************************************/

        public static TransformMatrix OrientationMatrix(this Cartesian csFrom, Cartesian csTo)
        {
            return OrientationMatrixGlobalToLocal(csTo) * OrientationMatrixLocalToGlobal(csFrom);
        }

        /***************************************************/
    }
}

