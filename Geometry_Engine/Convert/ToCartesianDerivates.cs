/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts the list of double[] curve derivatives in either 4d homogenous coordiantes or 3d cartesian coordinates to Vectors is cartesian coordinates.")]
        [Input("cwDers", "The list of curve derivatives to convert. Index in the list correspond to the derivative level.")]
        [Input("isRational", "If true, the controlPoints are assumed to be 4d in honogenous coordinates, if false assumed to be 3d in cartesian coordinates.")]
        [Output("derivatives", "The derivative Vectors resulting from the conversion from either cartesian or homogenous coordinates, depending on isRational.")]
        public static List<Vector> ToCartesianDerivatesCurve(this List<double[]> cwDers, bool isRational)
        {
            List<Vector> derivates = new List<Vector>();

            if (!isRational)
            {
                for (int i = 0; i < cwDers.Count; i++)
                {
                    double[] pt = cwDers[i];
                    derivates.Add(new Vector() { X = pt[0], Y = pt[1], Z = pt[2] });
                }
            }
            else
            {
                //Split into Aders containing the still scaled derivative vectors and wDers containing the derivatives of the weights
                List<Vector> aDers = new List<Vector>();
                List<double> wDers = new List<double>();

                for (int i = 0; i < cwDers.Count; i++)
                {
                    double[] cwDer = cwDers[i];
                    aDers.Add(new Vector { X = cwDer[0], Y = cwDer[1], Z = cwDer[2] });
                    wDers.Add(cwDer[3]);
                }


                //Compute the Derivatives in Cartesian coordinates
                for (int k = 0; k < aDers.Count; k++)
                {
                    Vector v = aDers[k];
                    for (int i = 1; i <= k; i++)
                    {
                        v -= Query.Binomal(k, i) * wDers[i] * derivates[k - i];
                    }
                    derivates.Add(v / wDers[0]);
                }
            }

            return derivates;
        }

        /***************************************************/
    }
}
