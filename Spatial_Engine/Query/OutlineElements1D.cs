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
        /****            IElement2D            ****/
        /******************************************/

        [Description("Returns every IElement1D which makes up the exterior perimeter of the IElement2D.")]
        [Input("element2D", "The IElement2D of which to get the exterior sub IElement1Ds from.")]
        [Output("element1Ds", "The IElement1Ds which geometry make up the perimeter curve for the IElement2D.")]
        public static List<IElement1D> IOutlineElements1D(this IElement2D element2D)
        {
            return Base.Compute.RunExtensionMethod(element2D, "OutlineElements1D") as List<IElement1D>;
        }

        /******************************************/
    }
}






