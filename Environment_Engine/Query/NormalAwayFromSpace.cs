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

        public static bool NormalAwayFromSpace(this BHE.BuildingElement buildingElement, List<BHE.BuildingElement> space)
        {
            BHG.Polyline bound = new BHG.Polyline() { ControlPoints = buildingElement.PanelCurve.IControlPoints() };

            return NormalAwayFromSpace(bound, space);
        }

        /***************************************************/

        public static bool NormalAwayFromSpace(this BHG.Polyline pline, List<BHE.BuildingElement> space)
        {
            List<BHG.Point> centrePtList = new List<BHG.Point>();
            BHG.Point centrePt = pline.Centre();
            centrePtList.Add(centrePt);

            if (!pline.IsClosed()) return false; //Prevent failures of the clockwise check

            List<BHG.Point> pts = BH.Engine.Geometry.Query.DiscontinuityPoints(pline);
            if (pts.Count < 3) return false; //Protection in case there aren't enough points to make a plane
            BHG.Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            

            //The polyline can be locally concave. Check if the polyline is clockwise.
            if (!BH.Engine.Geometry.Query.IsClockwise(pline, plane.Normal))
                plane.Normal = -plane.Normal;

            if (!BH.Engine.Geometry.Query.IsContaining(pline, centrePtList, false))
            {
                BHG.Point pointOnLine = BH.Engine.Geometry.Query.ClosestPoint(pline, centrePt);
                BHG.Vector vector = new BHG.Vector();
                if (BH.Engine.Geometry.Query.Distance(pointOnLine, centrePt) > BH.oM.Geometry.Tolerance.MicroDistance)
                    vector = pointOnLine - centrePt;
                else
                {
                    BHG.Line line = BH.Engine.Geometry.Query.GetLineSegment(pline, pointOnLine);
                    vector = ((line.Start - line.End).Normalise()).CrossProduct(plane.Normal);
                }

                centrePt = BH.Engine.Geometry.Modify.Translate(pointOnLine, BH.Engine.Geometry.Modify.Normalise(vector) * 0.001);
            }

            //Move centrepoint along the normal.
            if (BH.Engine.Environment.Query.IsContaining(space, centrePt.Translate(plane.Normal * 0.01), pline))
                return false;
            else
                return true;
        }
    }
}
