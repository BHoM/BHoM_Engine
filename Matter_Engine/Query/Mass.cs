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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Evaluates the mass of an object based its VolumetricMaterialTakeoff and Density.")]
        [Input("elementM", "The element to evaluate the mass of.")]
        [Output("mass", "The physical mass of the element.", typeof(Mass))]
        public static double Mass(this IElementM elementM)
        {
            if(elementM == null)
            {
                Base.Compute.RecordError("Cannot query the mass of a null element.");
                return 0;
            }

            VolumetricMaterialTakeoff takeoff = elementM.IVolumetricMaterialTakeoff();
            if (takeoff.Materials.Any(x => double.IsNaN(x.Density)))
            {
                Base.Compute.RecordError($"Unable to compute the mass of the element due to unset density on {string.Join(",", takeoff.Materials.Where(x => double.IsNaN(x.Density)).Select(x => x.Name))}.");
                return double.NaN;
            }
            if (takeoff.Materials.Any(x => x.Density == 0))
            {
                Base.Compute.RecordWarning($"The following materials in the makeup of the element has zero density and are not acounted for in the mass calculation of the element: {string.Join(",", takeoff.Materials.Where(x => x.Density == 0).Select(x => x.Name))}.");
            }
            return takeoff.Materials.Zip(takeoff.Volumes, (m, v) => m.Density * v).Sum();
        }

        /******************************************/
    }
}



