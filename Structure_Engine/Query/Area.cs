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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.Reinforcement;
using System.ComponentModel;
using BH.Engine.Analytical;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the area of a FEMesh as the sum of the area of all faces. Quad faces will be triangulated to perform the area calculation.")]
        [Input("mesh", "The FEMesh to calculate the area for.")]
        [Output("area", "The area of the FEMesh.", typeof(Area))]
        public static double Area(this FEMesh mesh)
        {
            return mesh.IsNull() ? 0 : mesh.Area();
        }

        /***************************************************/

        [Description("Calculates the area of a Surface based on the area of the geometrical surface stored in Extents.")]
        [Input("surface", "The Surface to calculate the area for.")]
        [Output("area", "The area of the Surface.", typeof(Area))]
        public static double Area(this Surface surface)
        {
            return surface.IsNull() ? 0 : surface.Extents.IArea();
        }

        /***************************************************/

        [Description("Calculates the total section area of all rebars in the reinforcement object.")]
        [Input("reinforcement", "The LongitudinalReinforcement to calculate the area for.")]
        [Output("area", "The area of the LongitudinalReinforcement.", typeof(Area))]
        public static double Area(this LongitudinalReinforcement reinforcement)
        {
            return reinforcement.IsNull() ? 0 : reinforcement.ReinforcingBarCount() * reinforcement.Diameter * reinforcement.Diameter / 4 * Math.PI;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Calculates the area of an IAreaElement.")]
        [Input("element", "The element to calculate the area for.")]
        [Output("area", "The area of the element.", typeof(Area))]
        public static double IArea(this IAreaElement element)
        {
            return element.IIsNull() ? 0 : Area(element as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double Area(this BH.oM.Dimensional.IElement2D element)
        {
            return Engine.Spatial.Query.Area(element);
        }

        /***************************************************/
    }

}




