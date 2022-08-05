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
using System;
using System.ComponentModel;

using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Calculates the angle between two vectors.")]
        [Input("v1", "First vector to compute the angle for.")]
        [Input("v2", "Second vector to compute the angle for.")]
        [Output("angle", "Angle between two vectors.")]
        public static double Angle(this Vector v1, Vector v2)
        {
            if (v1 == null || v2 == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute angle as one or both vectors are null.");
                return 0;
            }

            double dotProduct = v1.DotProduct(v2);
            double length = v1.Length() * v2.Length();

            return (Math.Abs(dotProduct) < length) ? Math.Acos(dotProduct / length) : (dotProduct < 0) ? Math.PI : 0;
        }

        /***************************************************/

        [Description("Calculates the counterclockwise angle between two vectors in a plane.")]
        [Input("v1", "First vector to compute the angle for.")]
        [Input("v2", "Second vector to compute the angle for.")]
        [Input("p", "Plane to compute the angle in.")]
        [Output("angle", "Angle between two vectors in given plane.")]
        public static double Angle(this Vector v1, Vector v2, Plane p)
        {
            v1 = v1.Project(p).Normalise();
            v2 = v2.Project(p).Normalise();

            double dotProduct = v1.DotProduct(v2);
            Vector n = p.Normal.Normalise();
           
            double det = v1.X * v2.Y * n.Z + v2.X * n.Y * v1.Z + n.X * v1.Y * v2.Z - v1.Z * v2.Y * n.X - v2.Z * n.Y * v1.X - n.Z * v1.Y * v2.X;

            double angle = Math.Atan2(det, dotProduct);
            return angle >= 0 ? angle : Math.PI * 2 + angle;
        }

        /***************************************************/
        [Description("Calculates the angle of the arc.")]
        [Input("arc", "Arc to compute the angle for.")]
        [Output("angle", "Angle of the arc.")]
        public static double Angle(this Arc arc)
        {
            return arc.EndAngle - arc.StartAngle;
        }

        /***************************************************/

        [Description("Signed angle between two vectors.")]
        [Input("a", "First vector to compute the angle for.")]
        [Input("b", "Second vector to compute the angle for.")]
        [Input("normal", "Normal vector to define the sign of the angle.")]
        [Output("angle", "Singed angle between two vectors.")]
        public static double SignedAngle(this Vector a, Vector b, Vector normal) // use normal vector to define the sign of the angle
        {
            double angle = Angle(a, b);

            Vector crossproduct = a.CrossProduct(b);
            if (crossproduct.DotProduct(normal) < 0)
                return -angle;
            else
                return angle;
        }

        /***************************************************/

        [Description("Signed angle between two lines.")]
        [Input("line1", "First line to compute the angle for.")]
        [Input("line2", "Second line to compute the angle for.")]
        [Input("normal", "Normal vector to define the sign of the angle.")]
        [Output("angle", "Singed angle between two lines.")]

        public static double SingedAngle(this Line line1, Line line2, Vector normal)
        {
            if (line1 == null || line2 == null || normal == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute angle as one or both lines are null.");
                return 0;
            }

            Vector line1Dir = line1.Direction();
            Vector line2Dir = line2.Direction();
            double angle = line1Dir.SignedAngle(line2Dir, normal);

            return angle;
        }

        /***************************************************/

        [Description("Gets the smallest angle between three points between 0 and pi radians. Angle is 0 if the three points form a straight line. The order of points is crucial to the calculation, as the points will imagine a line is connecting them in the order provided.")]
        [Input("firstPt", "The first Point of the three to calculate the angle between.")]
        [Input("secondPt", "The second point of the three to calculate the angle between.")]
        [Input("thirdPt", "The third point of the three to calculate the angle between.")]
        [Output("angle", "The smallest angle between the three points in radians.")]
        public static double Angle(this Point firstPt, Point secondPt, Point thirdPt)
        {
            double x1 = firstPt.X - secondPt.X; //Vector 1 - x
            double y1 = firstPt.Y - secondPt.Y; //Vector 1 - y
            double z1 = firstPt.Z - secondPt.Z; //Vector 1 - z
            double sqr1 = (x1 * x1) + (y1 * y1) + (z1 * z1); //Square of vector 1
            double x2 = thirdPt.X - secondPt.X; //Vector 2 - x
            double y2 = thirdPt.Y - secondPt.Y; //Vector 2 - y
            double z2 = thirdPt.Z - secondPt.Z; //Vector 2 - z
            double sqr2 = (x2 * x2) + (y2 * y2) + (z2 * z2); //Square of vector 2

            double xa = x1 * x2;
            double ya = y1 * y2;
            double za = z1 * z2;

            double costr = (xa + ya + za) / Math.Sqrt(Math.Abs(sqr1 * sqr2));
            double angle = Math.Abs(Math.Acos(costr)); //This produces a result in radians            
            return Math.PI - angle; //Convert so that a flat line is 0 angle through the points
        }

        /***************************************************/
        
    }
}


