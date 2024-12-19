/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Vector Basis with three orthogonal Vectors. Will ensure that x and y-vectors are orthogonal. This means the y-vector of the basis might be different than the one provided"
            +"/n x-vector is garantiued to stay the same. z-vector of basis will be calculated as x cross y. Method does not work for parallel vectors")]
        [Input("x", "x-vector of the basis. Basis is guaranteed to have a unit vector in this direction as its x-axis.")]
        [Input("y", "y-vector of the basis. Method will ensure that this vector in orthogonal to the x-axis.")]
        [Output("Basis", "An orthogonal vector basis with all unit vectors")]
        public static Basis Basis(Vector x, Vector y)
        {
            x = x.Normalise();
            y = y.Normalise();

            double dot = x.DotProduct(y);

            if (Math.Abs(1 - dot) < Tolerance.Angle)
                throw new ArgumentException("Can not create basis from parallell vectors");

            Vector z = x.CrossProduct(y);
            z.Normalise();

            if (Math.Abs(dot) > Tolerance.Angle)
            {
                Base.Compute.RecordWarning("x and y are not orthogonal. y will be made othogonal to x and z");
                y = z.CrossProduct(x).Normalise();
            }

            return new Basis(x, y, z);
        }

        /***************************************************/
    }
}






