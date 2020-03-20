/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Exchanges the internal IElement2Ds for the provided internal IElement2Ds. The internal IElement2Ds location is used and must align with the IElement2Ds geometry.")]
        [Input("element2D", "The IElement2D to exchange the internal IElement2D's of. This includes their location.")]
        [Input("newElements2D", "The IElement2Ds to set to the IElement2D. Must be of the correct type.")]
        [Output("element2D", "The modified IElement2D which has unchanged properties and exchanged internal IElement2Ds.")]
        public static IElement2D ISetInternalElements2D(this IElement2D element2D, List<IElement2D> newElements2D)
        {
            return Reflection.Compute.RunExtensionMethod(element2D, "SetInternalElements2D", new object[] { newElements2D }) as IElement2D;
        }

        /******************************************/
    }
}

