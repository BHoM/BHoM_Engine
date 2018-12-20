/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using System;
using System.Linq;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Architecture.Elements;

using System.Collections.Generic;

using BH.Engine.Geometry;

using BH.oM.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Level> Levels(this List<BHoMObject> bhomObjects)
        {
            List<Level> levels = new List<Level>();

            foreach (IBHoMObject obj in bhomObjects)
            {
                if (obj is Level)
                    levels.Add(obj as Level);
            }

            return levels;
        }

        public static double MinimumLevel(this BuildingElement bElement)
        {
            List<Point> crvPts = bElement.PanelCurve.IControlPoints();

            double min = 1e10;
            foreach (Point p in crvPts)
                min = Math.Min(min, p.Z);

            return min;
        }

        public static double MaximumLevel(this BuildingElement bElement)
        {
            List<Point> crvPts = bElement.PanelCurve.IControlPoints();

            double max = -1e10;
            foreach (Point p in crvPts)
                max = Math.Max(max, p.Z);

            return max;
        }

        public static Level Level(this BuildingElement bElement, IEnumerable<Level> levels)
        {
            double min = bElement.MinimumLevel();
            double max = bElement.MaximumLevel();

            return levels.Where(x => x.Elevation >= min && x.Elevation <= max).FirstOrDefault();
        }

        public static Level Level(this Space space, IEnumerable<Level> levels)
        {
            return levels.Where(x => x.Elevation >= space.Location.Z && x.Elevation <= space.Location.Z).FirstOrDefault();
        }

        public static Level Level(this List<BuildingElement> space, Level level)
        {
            Polyline floor = space.FloorGeometry();
            if (floor == null) return null;

            List<Point> floorPts = floor.IControlPoints();

            bool allPointsOnLevel = true;
            foreach(Point pt in floorPts)
                allPointsOnLevel &= (pt.Z > (level.Elevation - BH.oM.Geometry.Tolerance.Distance) && pt.Z < (level.Elevation + BH.oM.Geometry.Tolerance.Distance));

            if (!allPointsOnLevel) return null;
            return level;
        }

        public static Level Level(this List<BuildingElement> space, List<Level> levels)
        {
            foreach(Level l in levels)
            {
                Level match = space.Level(l);
                if (match != null) return match;
            }

            return null;
        }
    }
}
