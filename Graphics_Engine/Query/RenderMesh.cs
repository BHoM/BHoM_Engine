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

using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Graphics;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Returns a RenderMesh of the object, that is a mesh usable for graphical display.")]
        [Input("iobj", "Any object defined within BHoM.")]
        [Input("renderMeshOptions", "Options that regulate both the calculation of the Geometrical Representation and how it should be meshed.")]
        [Output("A RenderMesh, which is a geometrical mesh that can potentially have additional attributes like Colours.")]
        public static RenderMesh RenderMesh(this IObject iobj, RenderMeshOptions renderMeshOptions = null)
        {
            object result = null;

            RenderMeshOptions rmOpt = renderMeshOptions == null ? new RenderMeshOptions() : renderMeshOptions;

            // Try using the method that is defined in the TriangleNet_Toolkit. 
            Base.Compute.TryRunExtensionMethod(iobj, "IRenderMesh", new object[] { rmOpt }, out result);

            if (result != null)
                return result as RenderMesh;

            // If TriangleNet_Toolkit could not be found or didn't return anything, try looking for an ad-hoc extension method.
            Base.Compute.TryRunExtensionMethod(iobj, "RenderMesh", new object[] { rmOpt }, out result);

            return result as RenderMesh;
        }
        /***************************************************/
    }
}





