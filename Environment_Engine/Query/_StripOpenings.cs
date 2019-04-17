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
using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.Engine.Environment;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<BuildingElement>> StripOpenings(this List<List<BuildingElement>> elementsAsSpaces)
        {
            List<List<BuildingElement>> elements = new List<List<BuildingElement>>();

            foreach (List<BuildingElement> lst in elementsAsSpaces)
                elements.Add(lst.StripOpenings());

            return elements;
        }

        public static List<BuildingElement> StripOpenings(this List<BuildingElement> elements)
        {
            List<BuildingElement> rtnElements = new List<BuildingElement>();

            List<Opening> openings = elements.OpeningsFromElements();

            foreach (BuildingElement be in elements)
            {
                //Check centre and curve points of the opening and elements
                List<Opening> matchingOpenings = openings.Where(x => x.OpeningCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).Centre().Distance(be.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).Centre()) <= BH.oM.Geometry.Tolerance.Distance).ToList();

                if (matchingOpenings.Count == 0)
                {
                    rtnElements.Add(be);
                    continue; //This BE centre did not match any opening centres
                }

                //Centre did match so now check all edge points
                List<Point> bePts = be.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IControlPoints();

                bool match = false;

                foreach (Opening o in matchingOpenings)
                {
                    List<Point> openPts = o.OpeningCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IControlPoints();

                    match = bePts.PointsMatch(openPts);
                    if (match) break;
                }

                if (!match)
                    rtnElements.Add(be); //This element did not match an opening
            }

            return rtnElements;
        }

       /* public static List<BuildingElement> GetOpenings(this List<BuildingElement> elements)
        {
            List<BuildingElement> rtnElements = new List<BuildingElement>();

            List<Opening> openings = elements.Openings();

            foreach (BuildingElement be in elements)
            {
                //Check centre and curve points of the opening and elements
                List<Opening> matchingOpenings = openings.Where(x => x.OpeningCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).Centre().Distance(be.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).Centre()) <= BH.oM.Geometry.Tolerance.Distance).ToList();

                if (matchingOpenings.Count == 0)
                {
                    rtnElements.Add(be);
                    continue; //This BE centre did not match any opening centres
                }

                //Centre did match so now check all edge points
                List<Point> bePts = be.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IControlPoints();

                bool match = false;

                foreach (Opening o in matchingOpenings)
                {
                    List<Point> openPts = o.OpeningCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IControlPoints();

                    match = bePts.PointsMatch(openPts);
                    if (match) break;
                }

                if (match)
                    rtnElements.Add(be); //This element did not match an opening
            }

            return rtnElements;
        }*/
    }
}