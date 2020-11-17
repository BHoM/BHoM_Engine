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

using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using BH.oM.Geometry.SettingOut;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****  Public Methods                  ****/
        /******************************************/

        [Description("Checks if the geometrical representation of an IElement0D, projected onto the XY Plane, is fully within a set tolerance from a grid line.")]
        [Input("element0D", "The IElement0D that will be checked for proximity to the grid.")]
        [Input("grid", "The Grid to use for evaulation.")]
        [Input("tolerance", "The maximum distance allowed from the Grid for this method to return true.", typeof(Length))]
        [Output("isFullyOnGrid", "A boolean which is true if the geometrical representation of the IElement0D is fully within a set tolerance from the grid line.")]
        public static bool IsFullyOnGrid(this IElement0D element0D, Grid grid, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            Point position = element0D.IGeometry().Project(Plane.XY);
            ICurve gridCurve = grid.Curve.IProject(Plane.XY);

            return position.IDistance(gridCurve) <= tolerance;
        }

        /******************************************/

        [Description("Checks if the geometrical representation of an IElement1D, projected onto the XY Plane, is fully within a set tolerance from a grid line.")]
        [Input("element1D", "The IElement1D that will be checked for proximity to the grid.")]
        [Input("grid", "The Grid to use for evaulation.")]
        [Input("tolerance", "The maximum distance allowed from the Grid for this method to return true.", typeof(Length))]
        [Output("isFullyOnGrid", "A boolean which is true if the geometrical representation of an IElement1D is fully within a set tolerance from the grid line.")]
        public static bool IsFullyOnGrid(this IElement1D element1D, Grid grid, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            ICurve curve = element1D.IGeometry().IProject(Plane.XY);
            ICurve gridCurve = grid.Curve.IProject(Plane.XY);

            return curve.Distance(gridCurve) <= tolerance;
        }

        /******************************************/

        [Description("Checks if the geometrical representation of an IElement2D, projected onto the XY Plane, is fully within a set tolerance from a grid line.")]
        [Input("element2D", "The IElement2D that will be checked for proximity to the grid.")]
        [Input("grid", "The Grid to use for evaulation.")]
        [Input("tolerance", "The maximum distance allowed from the Grid for this method to return true.", typeof(Length))]
        [Output("isFullyOnGrid", "A boolean which is true if the geometrical representation of an IElement2D is fully within a set tolerance from the grid line.")]
        public static bool IsFullyOnGrid(this IElement2D element2D, Grid grid, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            List<IElement1D> elements1D = element2D.IOutlineElements1D();

            foreach (IElement1D e1D in elements1D)
            {
                if (IsFullyOnGrid(e1D, grid, tolerance))
                    return true;
            }
            return false;
        }

        /******************************************/
        /****   Public Methods - Interfaces    ****/
        /******************************************/

        [Description("Checks if the geometrical representation of an IElement, projected onto the XY Plane, is fully within a set tolerance from a grid line.")]
        [Input("element", "The IElement that will be checked for proximity to the grid.")]
        [Input("grid", "The Grid to use for evaulation.")]
        [Input("tolerance", "The maximum distance allowed from the Grid for this method to return true.", typeof(Length))]
        [Output("isFullyOnGrid", "A boolean which is true if the geometrical representation of the IElement is fully within a set tolerance from the grid line.")]
        public static bool IIsFullyOnGrid(this IElement element, Grid grid, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            return IsFullyOnGrid(element as dynamic, grid, tolerance);
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsFullyOnGrid(this IElement element, Grid grid, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            Reflection.Compute.RecordError($"IsFullyOnGrid is not implemented for IElements of type: {element.GetType().Name}.");
            return false;
        }

        /******************************************/

    }
}
