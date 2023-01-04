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
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [ToBeRemoved("3.2", "Was used for incode purposes of defaulting elements, a task which is now achived by providing a Point to the SetElement0D.")]
        [Description("Creates a IElement0D of a type which can be assigned to the IElement1D at the location of the point.")]
        [Input("element1D", "A IElement1D with a IElement0D type defined. The element is only used to identify the type of IElement0D to create, and will remain unchanged by this method.")]
        [Input("point", "The point location of which to assign to the new IElement0D.")]
        [Output("element0D", "A IElement0D which can be assigned to the IElement1D located at the point's location. Returns null if the IElement1D does not have a IElement0D type defined.")]
        public static IElement0D INewElement0D(this IElement1D element1D, Point point)
        {
            return point;
        }

        /******************************************/
    }
}



