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
    public static partial class Modify
    {
        /******************************************/
        /****          Public methods          ****/
        /******************************************/

        [Description("Sets the outline IElement1Ds of the IElement2D. ICurves will default the elements outline properties. This will maintain the IElement2D properties but exchange the outlines IElement1Ds.")]
        [Input("element2D", "The IElement2D to exchange the outline IElement1D's of. This includes their location.")]
        [Input("newOutline", "The outline IElement1Ds to set to the IElement2D. Must be planar and of a type assignable to the IElement2D or ICurve which will default the elements outline properties.")]
        [Output("element2D", "The modified IElement2D which has unchanged properties and exchanged outline IElement1Ds.")]
        public static IElement2D ISetOutlineElements1D(this IElement2D element2D, List<IElement1D> newOutline)
        {
            return Base.Compute.RunExtensionMethod(element2D, "SetOutlineElements1D", new object[] { newOutline }) as IElement2D;
        }

        /******************************************/
    }
}




