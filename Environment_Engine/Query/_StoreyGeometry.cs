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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;

using BHE = BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BHG.Polyline StoreyGeometry(this BH.oM.Architecture.Elements.Level level, List<BHE.Space> spaces)
        {
            /*List<BHE.Space> spacesAtLevel = spaces.FindAll(x => x.Level.Elevation == level.Elevation).ToList();

            if (spacesAtLevel.Count == 0)
                return null;

            List<BHE.BuildingElement> bHoMBuildingElement = spacesAtLevel.SelectMany(x => x.BuildingElements).ToList();
            List<BHG.Point> ctrlPoints = new List<BHG.Point>();

            foreach (BHE.BuildingElement element in bHoMBuildingElement)
            {
                foreach (BHG.Point pt in element.BuildingElementGeometry.ICurve().IControlPoints())
                {
                    if (pt.Z > (level.Elevation - BH.oM.Geometry.Tolerance.Distance) && pt.Z < (level.Elevation + BH.oM.Geometry.Tolerance.Distance))
                        ctrlPoints.Add(pt);

                }
            }

            return BH.Engine.Geometry.Create.ConvexHull(ctrlPoints.CullDuplicates());*/

            return new oM.Geometry.Polyline();
        }

        public static BHG.Polyline StoreyGeometry(this BH.oM.Architecture.Elements.Level level, List<List<BHE.BuildingElement>> spaces)
        {
            List<List<BHE.BuildingElement>> spacesAtLevel = spaces.FindAll(x => x.Level(level) != null).ToList();

            if (spacesAtLevel.Count == 0) return null;

            List<BHG.Point> ctrlPoints = new List<BHG.Point>();

            foreach (List<BHE.BuildingElement> space in spacesAtLevel)
            {
                foreach (BHE.BuildingElement element in space)
                {
                    foreach (BHG.Point pt in element.PanelCurve.IControlPoints())
                    {
                        if (pt.Z > (level.Elevation - BH.oM.Geometry.Tolerance.Distance) && pt.Z < (level.Elevation + BH.oM.Geometry.Tolerance.Distance))
                            ctrlPoints.Add(pt);
                    }
                }
            }

            return BH.Engine.Geometry.Create.ConvexHull(ctrlPoints.CullDuplicates());
        }
    }
}
