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

using BH.oM.Geometry;
using System;
using System.Collections.Generic;

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("This method is largely replaced by the Compute.ShearAreaPolyline() Method. \n" +
                     "Calculates the Shear area from a list of IntegrationSlices, based on the integral found in the documentation. \n" +
                     "To use this method for a closed set of Curves, first call Geometry.Create.IntegrationSlices() to generate the slices.")]
        [Input("slices", "The list of integration slices used to calculate the shear area. To generate the integration slices from as set of closed curves first call Geometry.Create.IntegrationSlices().")]
        [Input("momentOfInertia", "The moment of inertia around the axis orthogonal to the one being used to generate the slices.", typeof(SecondMomentOfArea))]
        [Input("centroid", "The centroid of the curves along the axis used to generate the slices.", typeof(Length))]
        [Output("shearArea", "The shear area calculated based on the slices.", typeof(Area))]
        [DocumentationURL("https://bhom.xyz/documentation/BHoM_oM/Structure_oM/Shear-Area-Derivation/", oM.Base.Attributes.Enums.DocumentationType.Documentation)]
        public static double ShearArea(List<IntegrationSlice> slices, double momentOfInertia, double centroid)
        {
            double sy = 0;
            double b = 0;
            double sum = 0;

            foreach (IntegrationSlice slice in slices)
            {
                sy += slice.Length * slice.Width * (centroid - slice.Centre);
                b = slice.Length;
                if (b > 0)
                {
                    sum += Math.Pow(sy, 2) / b * slice.Width;
                }

            }
            return Math.Pow(momentOfInertia, 2) / sum;
        }

        /***************************************************/
    }
}






