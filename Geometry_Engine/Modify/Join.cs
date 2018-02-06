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

        public static List<PolyCurve> IJoin(this List<ICurve> curves)
        {
            List<PolyCurve> sections = curves.Select(c => new PolyCurve { Curves = new List<ICurve> { c.IClone() } }).ToList();

            int counter = 0;
            while (counter < sections.Count)
            {
                for (int j = counter + 1; j < sections.Count; j++)
                {
                    if (sections[j].IStartPoint().SquareDistance(sections[counter].IStartPoint()) <= Tolerance.SqrtDist)
                    {
                        sections[counter].Curves = sections[counter].Curves.Select(c => c.IFlip()).ToList();
                        sections[counter].Curves.Reverse();
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IStartPoint().SquareDistance(sections[counter].IEndPoint()) <= Tolerance.SqrtDist)
                    {
                        sections[j].Curves.InsertRange(0, sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IStartPoint()) <= Tolerance.SqrtDist)
                    {
                        sections[j].Curves.AddRange(sections[counter].Curves);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].IEndPoint().SquareDistance(sections[counter].IEndPoint()) <= Tolerance.SqrtDist)
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

        public static List<Polyline> Join(this List<Line> lines)
        {
            List<Polyline> sections = lines.Select(l => new Polyline { ControlPoints = l.ControlPoints() }).ToList();

            int counter = 0;
            while (counter < sections.Count)
            {
                for (int j = counter + 1; j < sections.Count; j++)
                {
                    if (sections[j].ControlPoints[0].SquareDistance(sections[counter].ControlPoints[0]) <= Tolerance.SqrtDist)
                    {
                        List<Point> aPts = sections[counter].ControlPoints.Skip(1).ToList();
                        aPts.Reverse();
                        sections[j].ControlPoints.InsertRange(0, aPts);
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints[0].SquareDistance(sections[counter].ControlPoints.Last()) <= Tolerance.SqrtDist)
                    {
                        List<Point> aPts = sections[counter].ControlPoints;
                        aPts.AddRange(sections[j].ControlPoints.Skip(1).ToList());
                        sections[j].ControlPoints = aPts;
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints.Last().SquareDistance(sections[counter].ControlPoints[0]) <= Tolerance.SqrtDist)
                    {
                        sections[j].ControlPoints.AddRange(sections[counter].ControlPoints.Skip(1).ToList()); ;
                        sections.RemoveAt(counter--);
                        break;
                    }
                    else if (sections[j].ControlPoints.Last().SquareDistance(sections[counter].ControlPoints.Last()) <= Tolerance.SqrtDist)
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
    }
}
