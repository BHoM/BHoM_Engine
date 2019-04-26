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
using System;
using System.Linq;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****                Join curves                ****/
        /***************************************************/

        public static List<PolyCurve> IJoin(this List<ICurve> curves, double tolerance = Tolerance.Distance)
        {
            List<PolyCurve> sections = curves.Select(c => new PolyCurve { Curves = new List<ICurve> { c.IClone() } }).ToList();

            double sqTol = tolerance * tolerance;
            int counter = 0;
            while (counter < sections.Count)
            {
                for (int j = counter + 1; j < sections.Count; j++)
                {
                    if (sections[j].IStartPoint().SquareDistance(sections[counter].IStartPoint()) <= sqTol)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IStartPoint().SquareDistance(sections[counter].IEndPoint()) <= sqTol)
                    {
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IStartPoint()) <= sqTol)
                    {
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IEndPoint()) <= sqTol)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                }
                counter++;
            }
            return sections;
        }

        /***************************************************/

        public static List<PolyCurve> Join(this List<PolyCurve> curves, double tolerance = Tolerance.Distance)
        {

            List<PolyCurve> sections = new List<PolyCurve>();

            foreach(PolyCurve crv in curves)
            {
                foreach(ICurve icrv in crv.ISubParts())
                {
                    sections.Add(new PolyCurve { Curves = new List<ICurve> { icrv } });
                }
            }

            double sqTol = tolerance * tolerance;
            int counter = 0;
            while (counter < sections.Count)
            {
                for (int j = counter + 1; j < sections.Count; j++)
                {
                    if (sections[j].IStartPoint().SquareDistance(sections[counter].IStartPoint()) <= sqTol)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IStartPoint().SquareDistance(sections[counter].IEndPoint()) <= sqTol)
                    {
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IStartPoint()) <= sqTol)
                    {
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IEndPoint()) <= sqTol)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                }
                counter++;
            }
            return sections;
        }

        /***************************************************/

        public static List<Polyline> Join(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            List<Polyline> sections = lines.Select(l => new Polyline { ControlPoints = l.ControlPoints() }).ToList();
            return sections.Join(tolerance);
        }

        /***************************************************/

        public static List<Polyline> Join(this List<Polyline> curves, double tolerance = Tolerance.Distance)
        {
            List<Polyline> sections = curves.Select(l => new Polyline { ControlPoints = l.ControlPoints }).ToList();

            double sqTol = tolerance * tolerance;
            int counter = 0;
            while (counter < sections.Count)
            {
                for (int j = counter + 1; j < sections.Count; j++)
                {
                    if (sections[j].ControlPoints[0].SquareDistance(sections[counter].ControlPoints[0]) <= sqTol)
                    {
                        List<Point> aPts = sections[counter].ControlPoints.Skip(1).ToList();
                        aPts.Reverse();
                        sections[j].ControlPoints.InsertRange(0, aPts);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints[0].SquareDistance(sections[counter].ControlPoints.Last()) <= sqTol)
                    {
                        List<Point> aPts = sections[counter].ControlPoints;
                        aPts.AddRange(sections[j].ControlPoints.Skip(1).ToList());
                        sections[j].ControlPoints = aPts;
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints.Last().SquareDistance(sections[counter].ControlPoints[0]) <= sqTol)
                    {
                        sections[j].ControlPoints.AddRange(sections[counter].ControlPoints.Skip(1).ToList()); ;
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints.Last().SquareDistance(sections[counter].ControlPoints.Last()) <= sqTol)
                    {
                        sections[counter].ControlPoints.Reverse();
                        sections[j].ControlPoints.AddRange(sections[counter].ControlPoints.Skip(1).ToList()); ;
                        sections.RemoveAt(counter--);
                        break;
                    }
                }
                counter++;
            }
            return sections;
        }

        /***************************************************/
    }
}
