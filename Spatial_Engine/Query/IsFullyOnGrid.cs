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
using System.Linq;

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
            ICurve curve = element1D.IGeometry();
            List <Point> ctrlPts = curve.ControlPoints();

            // This method, the way it currently works, can produce inaccurate results when checking linear elements against curved grids.
            // For now, support for curved grids has been disabled. A more sophisticated algorithm could fix it. A possible quicker fix could be
            // to also check grid curve control points against input element1D, though this may still leave some holes open.
            if (!grid.Curve.IIsLinear())
            {
                BH.Engine.Base.Compute.RecordError("IsFullyOnGrid does not support non-linear grid curves.");
                return false;
            }

            // All this method does is check if the control points are within tolerance from the grid, not if the actual curve is.
            // This works well for Lines and Arcs, but to properly support NurbsCurves a more complex algorithm would be required,
            // as control points of NurbsCurves are not usually on the curve itself. Simply using ClosestPoint to bring the control 
            // points onto the curve would not be enough, as they may often end up in completely the wrong place and may not accurately
            // reflect local maximi in distance from the curve. ClosestPoint does not yet support NurbsCurves anyway.
            if (curve.IsNurbsCurve())
            {
                BH.Engine.Base.Compute.RecordWarning("IsFullyOnGrid does not fully support NurbsCurves. Results may be inaccurate.");
            }

            foreach (Point pt in ctrlPts)
            {
                if (!IsFullyOnGrid(pt, grid, tolerance))
                    return false;
            }

            return true;
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
                if (!IsFullyOnGrid(e1D, grid, tolerance))
                    return false;
            }
            return true;
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
            Base.Compute.RecordError($"IsFullyOnGrid is not implemented for IElements of type: {element.GetType().Name}.");
            return false;
        }

        /******************************************/

    }
}




