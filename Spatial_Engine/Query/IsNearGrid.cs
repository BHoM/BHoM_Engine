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

using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using BH.oM.Spatial.SettingOut;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****  Public Methods                  ****/
        /******************************************/

        [Description("Checks if the geometrical representation of an IElement0D, projected onto the XY Plane, is within a set distance from a grid line.")]
        [Input("element0D", "The IElement0D that will be checked for proximity to the grid.")]
        [Input("grid", "The Grid to use for evaulation.")]
        [Input("maxDistance", "The maximum distance allowed from the Grid for this method to return true.", typeof(Length))]
        [Output("isNearGrid", "A boolean which is true if the geometrical representation of the IElement0D is within a set distance from the grid line.")]
        public static bool IsNearGrid(this IElement0D element0D, Grid grid, double maxDistance)
        {
            Point position = element0D.IGeometry().Project(Plane.XY);
            ICurve gridCurve = grid.Curve.IProject(Plane.XY);

            return position.IDistance(gridCurve) <= maxDistance;
        }

        /******************************************/

        [Description("Checks if the geometrical representation of an IElement1D, projected onto the XY Plane, is within a set distance from a grid line.")]
        [Input("element1D", "The IElement1D that will be checked for proximity to the grid.")]
        [Input("grid", "The Grid to use for evaulation.")]
        [Input("maxDistance", "The maximum distance allowed from the Grid for this method to return true.", typeof(Length))]
        [Output("isNearGrid", "A boolean which is true if the geometrical representation of an IElement1D is within a set distance from the grid line.")]
        public static bool IsNearGrid(this IElement1D element1D, Grid grid, double maxDistance)
        {
            ICurve curve = element1D.IGeometry().IProject(Plane.XY);
            ICurve gridCurve = grid.Curve.IProject(Plane.XY);

            return curve.Distance(gridCurve) <= maxDistance;
        }

        /******************************************/

        [Description("Checks if the geometrical representation of an IElement2D, projected onto the XY Plane, is within a set distance from a grid line.")]
        [Input("element2D", "The IElement2D that will be checked for proximity to the grid.")]
        [Input("grid", "The Grid to use for evaulation.")]
        [Input("maxDistance", "The maximum distance allowed from the Grid for this method to return true.", typeof(Length))]
        [Output("isNearGrid", "A boolean which is true if the geometrical representation of an IElement2D is within a set distance from the grid line.")]
        public static bool IsNearGrid(this IElement2D element2D, Grid grid, double maxDistance)
        {
            List<IElement1D> elements1D = element2D.IOutlineElements1D();

            foreach (IElement1D e1D in elements1D)
            {
                if (IsNearGrid(e1D, grid, maxDistance))
                    return true;
            }
            return false;
        }

        /******************************************/
        /****   Public Methods - Interfaces    ****/
        /******************************************/

        [Description("Checks if the geometrical representation of an IElement, projected onto the XY Plane, is within a set distance from a grid line.")]
        [Input("element", "The IElement that will be checked for proximity to the grid.")]
        [Input("grid", "The Grid to use for evaulation.")]
        [Input("maxDistance", "The maximum distance allowed from the Grid for this method to return true.", typeof(Length))]
        [Output("isNearGrid", "A boolean which is true if the geometrical representation of the IElement is within a set distance from the grid line.")]
        public static bool IIsNearGrid(this IElement element, Grid grid, double maxDistance)
        {
            return IsNearGrid(element as dynamic, grid, maxDistance);
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsNearGrid(this IElement element, Grid grid, double maxDistance)
        {
            Base.Compute.RecordError($"IsNearGrid is not implemented for IElements of type: {element.GetType().Name}.");
            return false;
        }

        /******************************************/

    }
}





