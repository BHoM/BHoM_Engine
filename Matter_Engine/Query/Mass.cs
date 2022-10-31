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
        [Input("elementM", "The element to evaluate the mass of")]
        [Output("mass", "The physical mass of the element", typeof(Mass))]
        public static double Mass(this IElementM elementM)
        {
            if(elementM == null)
            {
                Base.Compute.RecordError("Cannot query the mass of a null element.");
                return 0;
            }

            VolumetricMaterialTakeoff mat = elementM.IVolumetricMaterialTakeoff();
            if (mat.Materials.Any(x => double.IsNaN(x.Density)))
            {
                Base.Compute.RecordError($"Unable to compute the mass of the element due to unset density on {string.Join(",", mat.Materials.Where(x => double.IsNaN(x.Density)).Select(x => x.Name))}.");
                return double.NaN;
            }
            return mat.Materials.Zip(mat.Volumes, (m, v) => m.Density * v).Sum();
        }

        /******************************************/
    }
}


