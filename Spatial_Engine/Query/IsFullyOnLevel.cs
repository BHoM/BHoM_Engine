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
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using BH.oM.Spatial.SettingOut;
using System.ComponentModel;
using System;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****  Public Methods                  ****/
        /******************************************/

        [Description("Checks if the geometrical representation of an IElement0D is fully within a set tolerance from a level elevation.")]
        [Input("element0D", "The IElement0D that will be checked for proximity to the Level.")]
        [Input("level", "The Level to use for evaulation.")]
        [Input("tolerance", "The maximum distance allowed from the Level for this method to return true.", typeof(Length))]
        [Output("isFullyOnLevel", "A boolean which is true if the geometrical representation of the IElement0D is fully within a set tolerance from the level elevation.")]
        public static bool IsFullyOnLevel(this IElement0D element0D, Level level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            Point position = element0D.IGeometry();

            return Math.Abs(level.Elevation - position.Z) <= tolerance;
        }

        /******************************************/

        [Description("Checks if the geometrical representation of an IElement1D is fully within a set tolerance from a level elevation.")]
        [Input("element1D", "The IElement1D that will be checked for proximity to the Level.")]
        [Input("level", "The Level to use for evaulation.")]
        [Input("tolerance", "The maximum distance allowed from the Level elevation for this method to return true.", typeof(Length))]
        [Output("isFullyOnLevel", "A boolean which is true if the geometrical representation of an IElement1D is fully within a set tolerance from the level elevation.")]
        public static bool IsFullyOnLevel(this IElement1D element1D, Level level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            BoundingBox bBox = element1D.Bounds();

            return level.Elevation - tolerance <= bBox.Min.Z && bBox.Max.Z <= level.Elevation + tolerance;
        }

        /******************************************/

        [Description("Checks if the geometrical representation of an IElement2D is fully within a set tolerance from a level elevation.")]
        [Input("element2D", "The IElement2D that will be checked for proximity to the Level.")]
        [Input("level", "The Level to use for evaulation.")]
        [Input("tolerance", "The maximum distance allowed from the Level for this method to return true.", typeof(Length))]
        [Output("isFullyOnLevel", "A boolean which is true if the geometrical representation of an IElement2D is fully within a set tolerance from the level elevation.")]
        public static bool IsFullyOnLevel(this IElement2D element2D, Level level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            BoundingBox bBox = element2D.Bounds();

            return level.Elevation - tolerance <= bBox.Min.Z && bBox.Max.Z <= level.Elevation + tolerance;
        }

        /******************************************/
        /****   Public Methods - Interfaces    ****/
        /******************************************/

        [Description("Checks if the geometrical representation of an IElement is fully within a set tolerance from a level elevation.")]
        [Input("element", "The IElement that will be checked for proximity to the Level.")]
        [Input("level", "The Level to use for evaulation.")]
        [Input("tolerance", "The maximum distance allowed from the Level for this method to return true.", typeof(Length))]
        [Output("isFullyOnLevel", "A boolean which is true if the geometrical representation of the IElement is fully within a set tolerance from the level elevation.")]
        public static bool IIsFullyOnLevel(this IElement element, Level level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            return IsFullyOnLevel(element as dynamic, level, tolerance);
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsFullyOnLevel(this IElement element, Level level, double tolerance = BH.oM.Geometry.Tolerance.Distance)
        {
            Base.Compute.RecordError($"IsFullyOnLevel is not implemented for IElements of type: {element.GetType().Name}.");
            return false;
        }

        /******************************************/

    }
}




