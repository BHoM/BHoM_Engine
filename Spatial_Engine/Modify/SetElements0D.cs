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
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Assigns the provided IElement0Ds to the IElement1D. Points will always default the elements end. The IElement0Ds location is used and may change the IElement1Ds geometry.")]
        [Input("element1D", "The IElement1D to modify the IElement0D's properties of. This includes their location.")]
        [Input("newElements0D", "The IElement0Ds to assign to the IElement1D. Must be of a type assignable to the IElement1D or a Point which will default the elements end properties.")]
        [Output("element1D", "The modified IElement1D which has unchanged properties and new IElement0Ds.")]
        public static IElement1D ISetElements0D(this IElement1D element1D, List<IElement0D> newElements0D)
        {
            return Base.Compute.RunExtensionMethod(element1D, "SetElements0D", new object[] { newElements0D }) as IElement1D;
        }

        /******************************************/
    }
}



