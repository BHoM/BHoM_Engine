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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Queries the IElement0Ds from a IElement1D. Returns null if no IElement0Ds are defined for the type of IElement1D.")]
        [Input("element1D", "The IElement1D of which to get the IElement0Ds.")]
        [Output("element0Ds", "The IElement0Ds from the IElement1D. Returns null if no IElement0Ds are defined for the type of IElement1D.")]
        public static List<IElement0D> IElements0D(this IElement1D element1D)
        {
            return Base.Compute.RunExtensionMethod(element1D, "Elements0D") as List<IElement0D>;
        }

        /******************************************/
    }
}





