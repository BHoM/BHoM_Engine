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

using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;

using BH.oM.Environment.Elements;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        /*public static List<List<BuildingElement>> JoinBuildingElements(this List<List<BuildingElement>> elementsAsSpaces)
        {
            List<List<BuildingElement>> spaces = new List<List<BuildingElement>>();

            foreach(List<BuildingElement> space in elementsAsSpaces)
            {
                List<BuildingElement> sp = space.JoinBuildingElements();
                spaces.Add(sp);
            }

            return spaces;
        }

        public static List<BuildingElement> JoinBuildingElements(this List<BuildingElement> elements)
        {
            List<BuildingElement> rtnElements = new List<BuildingElement>();
            rtnElements.AddRange(elements.Where(x => x.Area() >= 0.1).ToList());

            elements = elements.Where(x => x.Area() < 0.1).ToList();

            while(elements.Count > 0)
            {
                BuildingElement currentElement = elements[0];

                List<BuildingElement> allElements = new List<BuildingElement>(elements);
                allElements.AddRange(rtnElements);

                List<BuildingElement> connected = currentElement.ConnectedElementsByEdge(allElements).Where(x => x.MatchAdjacencies(currentElement)).Where(x => x.IsCoPlanar(currentElement)).ToList();

                if (connected.Count > 0)
                {
                    BuildingElement replace = connected.JoinBuildingElement();
                    foreach (BuildingElement be in connected)
                        elements.Remove(be);
                    rtnElements.Add(replace);
                }
                else rtnElements.Add(currentElement);

                if(elements.Count > 0)
                    elements.RemoveAt(0);
            }

            return rtnElements;
        }

        public static BuildingElement JoinBuildingElement(this List<BuildingElement> elementsToJoin)
        {
            List<Line> outlines = new List<Line>();
            foreach (BuildingElement be in elementsToJoin)
                outlines.AddRange(be.Edges());

            List<Point> pnts = new List<Point>();
            foreach (Line l in outlines)
            {
                pnts.Add(l.Start);
                pnts.Add(l.End);
            }

            List<Line> toJoin = new List<Line>();
            while (outlines.Count > 0)
            {
                Line l = outlines[0];

                List<Line> cuts = l.SplitAtPoints(pnts);
                if (cuts.Count > 1)
                    outlines.AddRange(cuts);
                else
                    toJoin.Add(l);

                outlines.RemoveAt(0);
            }

            List<Line> toJoin2 = new List<Line>();

            pnts = new List<Point>();
            foreach (Line l in toJoin)
                pnts.AddRange(l.ControlPoints());

            foreach (Line l in toJoin)
            {
                int startConnections = pnts.Where(x => x == l.Start).ToList().Count;
                int endConnections = pnts.Where(x => x == l.End).ToList().Count;

                if (startConnections <= 2 || endConnections <= 2)
                    toJoin2.Add(l);
            }

            while (toJoin2.Join().Count != 1 || !toJoin2.Join()[0].IsClosed())
            {
                //Got something missing...
                List<Point> pnts2 = new List<Point>();
                foreach (Line l in toJoin2)
                    pnts2.AddRange(l.ControlPoints());

                //Check each point has a match
                List<Point> unmatched = new List<Point>();
                foreach (Point p in pnts2)
                {
                    if (pnts2.Where(x => x == p).ToList().Count == 1)
                        unmatched.Add(p); //Only one instance in the list
                }

                if (unmatched.Count < 2) break;

                bool match = false;
                for (int x = 0; x < unmatched.Count; x++)
                {
                    for (int y = x + 1; y < unmatched.Count; y++)
                    {
                        Line l = toJoin.Where(a => (a.Start == unmatched[x] || a.End == unmatched[x]) && (a.Start == unmatched[y] || a.End == unmatched[y])).FirstOrDefault();
                        if (l != null)
                        {
                            toJoin2.Add(l);
                            match = true;
                            break;
                        }
                    }
                    if (match) break;
                }

                if (!match) break; //No match found - problem perhaps?
            }

            Polyline joined = toJoin2.Join()[0];

            BuildingElement rtn = elementsToJoin[0];
            rtn.PanelCurve = joined;

            return rtn;
        }

        public static double Angle(Point endPt1, Point connectingPt, Point endPt2)
        {
            double x1 = endPt1.X - connectingPt.X; //Vector 1 - x
            double y1 = endPt1.Y - connectingPt.Y; //Vector 1 - y

            double x2 = endPt2.X - connectingPt.X; //Vector 2 - x
            double y2 = endPt2.Y - connectingPt.Y; //Vector 2 - y

            double angle = System.Math.Atan2(y1, x1) - System.Math.Atan2(y2, x2);
            angle = angle * 360 / (2 * System.Math.PI);

            if (angle < 0)
                angle += 360;

            return angle;
        }*/
    }
}