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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Physical.Constructions;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Gets the average thickness of the property for the purpose of calculating solid volume.")]
        [Input("construction", "The property to evaluate the average thickness of.")]
        [Output("volumePerArea", "The average thickness of the property for the purpose of calculating solid volume.", typeof(Length))]
        public static double IVolumePerArea(this IConstruction construction)
        {
            if (construction == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the thickness of a null construction.");
                return 0;
            }

            return VolumePerArea(construction as dynamic);
        }

        [Description("Gets the average thickness of the property for the purpose of calculating solid volume.")]
        [Input("construction", "The property to evaluate the average thickness of.")]
        [Output("volumePerArea", "The average thickness of the property for the purpose of calculating solid volume.", typeof(Length))]
        public static double VolumePerArea(this Construction construction)
        {
            if (construction == null)
                return 0;
            if (construction.Layers.Any(x => x.Material == null))
                Base.Compute.RecordWarning("At least one Material in a Construction was null. VolumePerArea excludes this layer, assuming it is void space.");
            return construction.Layers.Where(x => x.Material != null).Sum(x => x.Thickness);
        }

        private static double VolumePerArea(this object construction)
        {
            return 0; //Fallback method
        }
    }
}