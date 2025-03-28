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

using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Spatial.SettingOut;
using BH.oM.Base;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the Setting Out Level that the Environment Panel resides on")]
        [Input("panel", "An Environment Panel to find the level from")]
        [Input("levels", "A collection of Setting Out Levels to search from")]
        [Output("level", "The Setting Out Level of the panel")]
        public static Level Level(this Panel panel, IEnumerable<Level> levels)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the level of a null panel.");
                return null;
            }

            double min = panel.MinimumLevel();
            double max = panel.MaximumLevel();

            return levels.Where(x => x.Elevation >= min && x.Elevation <= max).FirstOrDefault();
        }

        [Description("Returns the Setting Out Level that the space (represented by a collection of Environment Panels) resides on")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a single space to find the level from")]
        [Input("level", "The Setting Out Level to check against")]
        [Output("level", "The Setting Out Level of the space if the space resides on this level, otherwise returns null if the space does not reside on this level")]
        public static Level Level(this List<Panel> panelsAsSpace, Level level)
        {
            Polyline floor = panelsAsSpace.FloorGeometry();
            if (floor == null)
                return null;

            List<Point> floorPts = floor.IControlPoints();

            bool allPointsOnLevel = true;
            foreach (Point pt in floorPts)
                allPointsOnLevel &= (pt.Z > (level.Elevation - BH.oM.Geometry.Tolerance.Distance) && pt.Z < (level.Elevation + BH.oM.Geometry.Tolerance.Distance));

            if (!allPointsOnLevel)
                return null;
            
            return level;
        }

        [Description("Returns the Setting Out Level that the space (represented by a collection of Environment Panels) resides on")]
        [Input("panelsAsSpace", "A collection of Environment Panels that represent a single space to find the level from")]
        [Input("levels", "A collection of Setting Out Levels to search from")]
        [Output("level", "The Setting Out Level of the space")]
        public static Level Level(this List<Panel> panelsAsSpace, List<Level> levels)
        {
            foreach (Level l in levels)
            {
                Level match = panelsAsSpace.Level(l);
                if (match != null) return match;
            }

            return null;
        }

        [Description("Returns a collection of Setting Out Levels from a list of generic BHoM objects")]
        [Input("bhomObjects", "A collection of generic BHoM objects")]
        [Output("levels", "A collection of Setting Out Level objects")]
        public static List<Level> Levels(this List<IBHoMObject> bhomObjects)
        {
            bhomObjects = bhomObjects.ObjectsByType(typeof(Level));
            List<Level> levels = new List<Level>();

            foreach (IBHoMObject o in bhomObjects)
                levels.Add(o as Level);

            return levels;
        }
    }
}






