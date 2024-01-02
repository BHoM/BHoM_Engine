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
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [ToBeRemoved("3.2", "Was used for incode purposes of defaulting elements, a task which is now achived by providing a ICurve to the SetOutlineElement1D.")]
        [Description("Creates a IElement1D of a type which can be assigned to the IElement2D at the location of the curve.")]
        [Input("element2D", "A IElement2D of which to get the correct IElement1D type of. The element is only used to identify the type of IElement1D to create, and will remain unchanged by this method.")]
        [Input("curve", "The curve location of which to assign to the new IElement1D.")]
        [Output("element1D", "A IElement1D which can be assigned to the IElement2D located at the curve's location.")]
        public static IElement1D INewElement1D(this IElement2D element2D, ICurve curve)
        {
            return curve;
        }

        /******************************************/
    }
}




