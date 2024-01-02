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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Description("Creates a IElement2D of a type which can be assigned to the IElement2D.")]
        [Input("element2D", "A IElement2D with a internal IElement2D type defined. The element is only used to identify the type of internal IElement2D to create, and will remain unchanged by this method.")]
        [Output("element2D", "A internal IElement2D which can be assigned to the IElement2D. Returns null if the IElement2D does not have a type of internal IElement2D.")]
        public static IElement2D INewInternalElement2D(this IElement2D element2D)
        {
            return Base.Compute.RunExtensionMethod(element2D, "NewInternalElement2D") as IElement2D;
        }

        /******************************************/
    }
}




