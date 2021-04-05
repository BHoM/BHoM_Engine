/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.MEP.Enums;
using BH.oM.MEP.System;
using BH.oM.Reflection.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using System;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the Circular Equivalent Diameter for elements that are non-circular, equivalent in length, fluid resistance and airflow.")]
        [Input("profile", "Shape profile to query the Circular Equivalent Diameter.")]
        [Output("circularEquivalentDiameter", "Circular Equivalent Diameter for element section profiles that are non-circular, equivalent in length, fluid resistance and airflow.")]
        public static double ICircularEquivalentDiameter(this IProfile profile)
        {
            return CircularEquivalentDiameter(profile as dynamic);
        }

        /***************************************************/

        [Description("Returns the Circular Equivalent Diameter for elements that are non-circular, equivalent in length, fluid resistance and airflow.")]
        [Input("obj", "The Box shaped element to query the Circular Equivalent Diameter.")]
        [Output("circularEquivalentDiameter", "Circular Equivalent Diameter for element section profiles that are non-circular, equivalent in length, fluid resistance and airflow.")]
        public static double CircularEquivalentDiameter(this IFlow obj)
        {
            ShapeType elementShape = obj.ElementSize.Shape;

            if (elementShape == ShapeType.Box)
            {
                double thickness = obj.SectionProfile.Where(x => x.Type == ProfileType.Element).First().Layer.Select(x => x.Thickness).Sum();
                double a = 1000 * (obj.ElementSize.Height - 2 * thickness);
                double b = 1000 * (obj.ElementSize.Width - 2 * thickness);
                return (1.30 * Math.Pow(a * b, 0.625) / Math.Pow(a + b, 0.250)) / 1000;
            }
            BH.Engine.Reflection.Compute.RecordError("Circular Equivalent Diameter only applies to Box ShapeProfiles.");
            return 0;
        }

        /***************************************************/

        [Description("Returns the Circular Equivalent Diameter for elements that are non-circular, equivalent in length, fluid resistance and airflow.")]
        [Input("box", "Box Shape profile to query the Circular Equivalent Diameter.")]
        [Output("circularEquivalentDiameter", "Circular Equivalent Diameter for element section profiles that are non-circular, equivalent in length, fluid resistance and airflow.")]
        public static double CircularEquivalentDiameter(this BoxProfile box)
        {
            double a = 1000 * (box.Height - 2 * box.Thickness);
            double b = 1000 * (box.Width - 2 * box.Thickness);
            return (1.30 * Math.Pow(a * b, 0.625) / Math.Pow(a + b, 0.250)) / 1000;
        }

        /***************************************************/

        public static double CircularEquivalentDiameter(this object profile)
        {
            return 0; //To catch things that are not box profile.
        }
    }
}

