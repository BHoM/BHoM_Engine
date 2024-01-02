/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Principal curvature is the max and min curvature of the intersection curve between the surface and a plane formed by the span of the normal vector and the principal  direction.")]
        [Input("surface", "Surface to evaluate.")]
        [Input("u", "The parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [Input("v", "The parameter to evaluate at. Should be between 0 and 1. For values outside the range, the closest value will be used.")]
        [MultiOutput(0, "minC", "Minimum principal curvature. Curvature of the intersection of the surface and a plane spanned by the normal and minimum principal direction.")]
        [MultiOutput(1, "maxC", "Maximum principal curvature. Curvature of the intersection of the surface and a plane spanned by the normal and maximum principal  direction.")]
        [MultiOutput(2, "minK", "Minimum principal direction. Tangent of the curve on the surface with the least curvature.")]
        [MultiOutput(3, "maxK", "Maximum principal direction. Tangent of the curve on the surface with the most curvature.")]
        public static Output<double, double, Vector,Vector> PrincipalCurvatureAtParameter(this NurbsSurface surface, double u, double v)
        {
            // Vector entries for a "Hessian" with regard to u and v
            Vector dU2 = DerivativeAtParameter(surface, u, v, 2, 0);
            Vector dUV = DerivativeAtParameter(surface, u, v, 1, 1);
            Vector dV2 = DerivativeAtParameter(surface, u, v, 0, 2);

            // Get the local space, dU and dV are the basis vectors for the Hessian above.
            Vector dU = DerivativeAtParameter(surface, u, v, 1, 0);
            Vector dV = DerivativeAtParameter(surface, u, v, 0, 1);
            Vector normal = dU.CrossProduct(dV).Normalise();

            // We reduce the vector valued Hessian to a scalar valued Hessian, where the value (of the imagined original function) is the distance from the point we evaluate along the normal
            // | a b |
            // | c d |
            double a = dU2.DotProduct(normal);
            double b = dUV.DotProduct(normal);
            double c = b;
            double d = dV2.DotProduct(normal);
            // That's the Hessian for a system where the derivative vectors are the local coordinate systems xy-axis, i.e very poorly scaled and skewed for most purposes

            double phi = dU.SignedAngle(dV, normal);
            // Change of basis matrix, to get the transform for the Hessian to the world space
            // | e f |
            // | g h |
            double e = 1/ dU.Length();
            double g = 0;
            double h = 1 / (Math.Sin(phi)*dV.Length());

            double f = -(Math.Cos(phi)*dV.Length())*h*e;

            // A^T * Hess(x) * A gives the Hessian in the world space scaled/skewed system, i.e a system we care about 
            double a1 = a*e*e + b*g*e + c*e*g + d*g*g;
            double b1 = e*a*f + e*b*h + g*c*f + g*d*h;
            double c1 = f*a*e + b*g*f + c*e*h + d*g*h;
            double d1 = a*f*f + b*h*f + c*f*h + d*h*h;

            // Eigenvalues of the Hessian gives the min and max curvature of the function
            double p = (a1 + d1) * 0.5;
            double sqrt = Math.Sqrt((a1 + d1) * (a1 + d1) * 0.25 - (a1 * d1 - b1 * c1));

            if (double.IsNaN(sqrt))
                sqrt = 0;

            double eigenMin = (p - sqrt);
            double eigenMax = (p + sqrt);

            // The eigenvectors of the Hessian are the principled directions
            Vector eigMin = new Vector();
            Vector eigMax = new Vector();
            if (Math.Abs(c1) > Tolerance.Distance)
            {
                eigMin.X = eigenMin - d1;
                eigMin.Y = c1;

                eigMax.X = eigenMax - d1;
                eigMax.Y = c1;
            }
            else if (Math.Abs(b1) > Tolerance.Distance)
            {
                eigMin.X = b1;
                eigMin.Y = eigenMin - a1;

                eigMax.X = b1;
                eigMax.Y = eigenMax - a1;
            }
            else
            {
                eigMin.X = 1;

                eigMax.Y = 1;
            }

            // Orient to world space, as in we've been working on the origin with the normal as z-axis, this places the vectors back to the surface.
            // would be nice to suppress the warning in some manner as it is very much intended.
            TransformMatrix matrix = Create.OrientationMatrixGlobalToLocal(Create.CartesianCoordinateSystem(new Point(), dU, dV));

            return new Output<double, double, Vector, Vector>()
            {
                Item1 = eigenMin,
                Item2 = eigenMax,
                Item3 = eigMin.Transform(matrix).Normalise(),
                Item4 = eigMax.Transform(matrix).Normalise(),
            };
        }

        /***************************************************/

    }
}




