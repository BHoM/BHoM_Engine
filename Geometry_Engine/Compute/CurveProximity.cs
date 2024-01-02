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

using System;
using BH.oM.Geometry;
 
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.oM.Base;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Line curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> cIntersections = curve1.CurveIntersections(curve2);
            if (cIntersections.Count > 0)
                return new Output<Point, Point> { Item1 = cIntersections[0], Item2 = cIntersections[0] };

            Point min1 = new Point();
            Point min2 = new Point();
            min1 = curve1.Start;
            min2 = curve2.Start;

            if (curve1.End.Distance(curve2) < min1.Distance(curve2))
                min1 = curve1.End;

            if (curve2.End.Distance(curve1) < min2.Distance(curve1))
                min2 = curve2.End;

            if (min2.Distance(curve1) < min1.Distance(curve2))
                min1 = curve1.ClosestPoint(min2);
            else
                min2 = curve2.ClosestPoint(min1);

            if (curve1.IsCoplanar(curve2))
                return new Output<Point, Point> { Item1 = min1, Item2 = min2 };

            Output<double, double> skewLineProximity = curve1.SkewLineProximity(curve2);
            double[] t = new double[] { skewLineProximity.Item1, skewLineProximity.Item2 };
            double t1 = Math.Max(Math.Min(t[0], 1), 0);
            double t2 = Math.Max(Math.Min(t[1], 1), 0);
            Vector e1 = curve1.End - curve1.Start;
            Vector e2 = curve2.End - curve2.Start;

            if ((curve1.Start + e1 * t1).Distance(curve2.Start + e2 * t2) < min1.Distance(min2))
                return new Output<Point, Point> { Item1 = curve1.Start + e1 * t1, Item2 = curve2.Start + e2 * t2 };
            else
                return new Output<Point, Point> { Item1 = min1, Item2 = min2 };

        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Line curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> cIntersections = curve1.CurveIntersections(curve2);
            if (cIntersections.Count > 0)
                return new Output<Point, Point> { Item1 = cIntersections[0], Item2 = cIntersections[0] };

            List<Point> plnPts = new List<Point>();
            plnPts.Add(curve1.Start);
            plnPts.Add(curve1.End);
            plnPts.Add(curve2.Centre());
            Point tmp;
            Plane ftPln = plnPts.FitPlane();

            if (ftPln != null)
            {
                List<Point> intersecting = curve2.PlaneIntersections(ftPln);

                if (intersecting.Count > 1)
                {
                    if (intersecting[0].Distance(curve1) < intersecting[1].Distance(curve1))
                        tmp = intersecting[0];
                    else
                        tmp = intersecting[1];
                }
                else if (intersecting.Count == 1)
                    tmp = intersecting[0];
                else
                    tmp = curve2.StartPoint();

            }
            else
                tmp = curve2.StartPoint();

            if (curve2.StartPoint().Distance(curve1) < tmp.Distance(curve1))
                tmp = curve2.StartPoint();

            if (curve2.EndPoint().Distance(curve1) < tmp.Distance(curve1))
                tmp = curve2.EndPoint();

            Line prLn = curve1.Project(curve2.FitPlane());
            List<Point> lnInt = prLn.CurveIntersections(curve2);

            if (lnInt.Count > 0)
            {
                if (lnInt.Count > 1)
                {
                    if (lnInt[0].Distance(curve1) > lnInt[0].Distance(curve1))
                        lnInt[0] = lnInt[1];
                }

                if (lnInt[0].Distance(curve1) < tmp.Distance(curve1))
                    tmp = lnInt[0];
            }

            Output<Point, Point> result = new Output<Point, Point>();
            result.Item1 = curve1.ClosestPoint(tmp);
            result.Item2 = curve2.ClosestPoint(result.Item1);
            result.Item1 = curve1.ClosestPoint(result.Item2);
            Output<Point, Point> oldfinal = new Output<Point, Point>();
            Output<Point, Point> result2 = new Output<Point, Point>();

            if (curve1.Start.Distance(curve2) < curve1.End.Distance(curve2))
                result2.Item1 = curve1.Start;
            else
                result2.Item1 = curve1.End;

            result2.Item2 = curve2.ClosestPoint(result2.Item1);
            result2.Item1 = curve1.ClosestPoint(result2.Item2);
            Output<Point, Point> final = new Output<Point, Point>();

            if (result.Item1.Distance(result.Item2) < result2.Item1.Distance(result2.Item2))
                final = result;
            else
                final = result2;

            do
            {
                oldfinal.Item1 = final.Item1;
                oldfinal.Item2 = final.Item2;
                final.Item2 = curve2.ClosestPoint(final.Item1);
                final.Item1 = curve1.ClosestPoint(final.Item2);
            }
            while (Math.Abs(oldfinal.Item1.Distance(oldfinal.Item2) - final.Item1.Distance(final.Item2)) > tolerance * tolerance);

            return final;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Line curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> cIntersections = curve1.CurveIntersections(curve2);
            if (cIntersections.Count > 0)
                return new Output<Point, Point> { Item1 = cIntersections[0], Item2 = cIntersections[0] };

            List<Point> plnPts = new List<Point>();
            plnPts.Add(curve1.Start);
            plnPts.Add(curve1.End);
            plnPts.Add(curve2.Centre);
            Point tmp;
            Plane ftPln = plnPts.FitPlane();
            Output<Point, Point> result = new Output<Point, Point>();

            if (Math.Abs(curve2.Normal.DotProduct(curve1.Direction())) < Tolerance.Angle) // 2D solution
            {
                double startSqDist = curve2.Centre.SquareDistance(curve1.Start);
                if (startSqDist < Math.Pow(curve2.Radius, 2)) // within circle
                    result.Item1 = startSqDist < curve2.Centre.SquareDistance(curve1.End) ? curve1.End : curve1.Start;
                else      // outside circle
                    result.Item1 = curve1.ClosestPoint(curve2.Centre);
                result.Item2 = curve2.ClosestPoint(result.Item1);
                return result;
            }

            if (ftPln != null)
            {
                List<Point> intersecting = curve2.PlaneIntersections(ftPln);

                if (intersecting[0].Distance(curve1) < intersecting[1].Distance(curve1))
                    tmp = intersecting[0];
                else
                    tmp = intersecting[1];
            }
            else
                tmp = curve1.ClosestPoint(curve2.Centre);

            Line prLn = curve1.Project(curve2.FitPlane());
            List<Point> lnInt = prLn.CurveIntersections(curve2);

            if (lnInt.Count > 0)
            {
                if (lnInt.Count > 1)
                {
                    if (lnInt[0].Distance(curve1) > lnInt[1].Distance(curve1))
                        lnInt[0] = lnInt[1];
                }

                if (lnInt[0].Distance(curve1) < tmp.Distance(curve1) || lnInt[0].Distance(curve1) < tmp.Distance(curve2))
                    tmp = lnInt[0];
            }

            result.Item1 = curve1.ClosestPoint(tmp);
            result.Item2 = curve2.ClosestPoint(result.Item1);
            Output<Point, Point> oldresult = new Output<Point, Point>();

            do
            {
                oldresult.Item1 = result.Item1;
                oldresult.Item2 = result.Item2;
                result.Item1 = curve1.ClosestPoint(result.Item2);
                result.Item2 = curve2.ClosestPoint(result.Item1);
            }
            while (Math.Abs(oldresult.Item1.Distance(oldresult.Item2) - result.Item1.Distance(result.Item2)) > tolerance * tolerance);

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Line curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Line curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Arc curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Arc curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> cIntersections = curve1.CurveIntersections(curve2);
            if (cIntersections.Count > 0)
                return new Output<Point, Point> { Item1 = cIntersections[0], Item2 = cIntersections[0] };

            Output<Point, Point> result = new Output<Point, Point>();
            Output<Point, Point> oldresult = new Output<Point, Point>();
            Output<Point, Point> result2 = new Output<Point, Point>();
            Output<Point, Point> result3 = new Output<Point, Point>();
            Output<Point, Point> result4 = new Output<Point, Point>();
            Output<Point, Point> result5 = new Output<Point, Point>();
            Output<Point, Point> result6 = new Output<Point, Point>();
            Plane ftPln1 = curve1.FitPlane();
            Plane ftPln2 = curve2.FitPlane();
            List<Point> fitPoints = new List<Point>();
            bool check = false;

            if ((fitPoints = curve1.PlaneIntersections(ftPln2)).Count > 0)
            {
                if (fitPoints.Count == 1)
                {
                    result.Item1 = fitPoints[0];
                    result.Item2 = curve2.ClosestPoint(fitPoints[0]);
                    check = true;
                }
                else if (fitPoints.Count > 1)
                {
                    if (fitPoints[0].Distance(curve2) < fitPoints[1].Distance(curve2))
                    {
                        result.Item1 = fitPoints[0];
                        result.Item2 = curve2.ClosestPoint(fitPoints[0]);
                        check = true;
                    }
                    else
                    {
                        result.Item1 = fitPoints[1];
                        result.Item2 = curve2.ClosestPoint(fitPoints[1]);
                        check = true;
                    }
                }
            }

            if ((fitPoints = curve2.PlaneIntersections(ftPln1)).Count > 0)
            {
                if (check)
                {
                    if (fitPoints.Count == 1 && (fitPoints[0].Distance(curve1) < result.Item1.Distance(curve2)))
                    {
                        result.Item2 = fitPoints[0];
                        result.Item1 = curve1.ClosestPoint(fitPoints[0]);
                    }
                    else if (fitPoints.Count > 1)
                    {

                        if (fitPoints[0].Distance(curve1) < fitPoints[1].Distance(curve1))
                        {
                            if (fitPoints[0].Distance(curve1) < result.Item1.Distance(curve2))
                            {
                                result.Item2 = fitPoints[0];
                                result.Item1 = curve1.ClosestPoint(fitPoints[0]);
                            }
                        }
                        else
                        {

                            if (fitPoints[1].Distance(curve1) < result.Item1.Distance(curve2))
                            {
                                result.Item2 = fitPoints[1];
                                result.Item1 = curve1.ClosestPoint(fitPoints[1]);
                            }
                        }
                    }
                }
                else if (fitPoints.Count == 1)
                {
                    result.Item2 = fitPoints[0];
                    result.Item1 = curve1.ClosestPoint(fitPoints[0]);
                    check = true;
                }
                else if (fitPoints.Count > 1)
                {

                    if (fitPoints[0].Distance(curve1) < fitPoints[1].Distance(curve1))
                    {
                        result.Item2 = fitPoints[0];
                        result.Item1 = curve1.ClosestPoint(fitPoints[0]);
                        check = true;
                    }
                    else
                    {
                        result.Item2 = fitPoints[1];
                        result.Item1 = curve1.ClosestPoint(fitPoints[1]);
                        check = true;
                    }
                }
            }

            if (check)
            {
                do
                {
                    oldresult.Item1 = result.Item1;
                    oldresult.Item2 = result.Item2;
                    result.Item1 = curve2.ClosestPoint(result.Item2);
                    result.Item2 = curve1.ClosestPoint(result.Item1);
                }
                while (oldresult.Item2.Distance(result.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result.Item1) > tolerance * tolerance);
            }

            check = false;
            Line intersect = new Line { Start = curve1.Centre(), End = curve2.Centre() };
            Point tmp1 = intersect.CurveProximity(curve1).Item2;
            Point tmp2 = intersect.CurveProximity(curve2).Item2;

            if (tmp1.Distance(curve2) < tmp2.Distance(curve1))
            {
                result2.Item1 = tmp1;
                result2.Item2 = curve2.ClosestPoint(tmp1);
            }
            else
            {
                result2.Item2 = tmp2;
                result2.Item1 = curve1.ClosestPoint(tmp2);
            }

            do
            {
                oldresult.Item1 = result2.Item1;
                oldresult.Item2 = result2.Item2;
                result2.Item1 = curve2.ClosestPoint(result2.Item2);
                result2.Item2 = curve1.ClosestPoint(result2.Item1);
            }
            while (oldresult.Item2.Distance(result2.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result2.Item1) > tolerance * tolerance);

            if (curve1.EndPoint().Distance(curve2.ClosestPoint(curve1.EndPoint())) < curve1.StartPoint().Distance(curve2.ClosestPoint(curve1.StartPoint())))
            {
                result3.Item1 = curve1.EndPoint();
                result3.Item2 = curve2.ClosestPoint(result3.Item1);
            }
            else
            {
                result3.Item1 = curve1.StartPoint();
                result3.Item2 = curve2.ClosestPoint(result3.Item1);
            }

            if (curve2.EndPoint().Distance(curve1.ClosestPoint(curve2.EndPoint())) < curve2.StartPoint().Distance(curve1.ClosestPoint(curve2.StartPoint())))
            {
                result4.Item2 = curve2.EndPoint();
                result4.Item1 = curve1.ClosestPoint(result4.Item2);
            }
            else
            {
                result4.Item2 = curve2.StartPoint();
                result4.Item1 = curve1.ClosestPoint(result4.Item2);
            }

            do
            {
                oldresult.Item1 = result3.Item1;
                oldresult.Item2 = result3.Item2;
                result3.Item1 = curve2.ClosestPoint(result3.Item2);
                result3.Item2 = curve1.ClosestPoint(result3.Item1);
            }
            while (oldresult.Item2.Distance(result3.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result3.Item1) > tolerance * tolerance);

            do
            {
                oldresult.Item1 = result4.Item1;
                oldresult.Item2 = result4.Item2;
                result4.Item1 = curve2.ClosestPoint(result4.Item2);
                result4.Item2 = curve1.ClosestPoint(result4.Item1);
            }
            while (oldresult.Item2.Distance(result4.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result4.Item1) > tolerance * tolerance);

            result5.Item2 = curve2.PointAtParameter(0.5);
            result5.Item1 = curve1.ClosestPoint(result5.Item2);
            result6.Item1 = curve1.PointAtParameter(0.5);
            result6.Item2 = curve2.ClosestPoint(result6.Item1);

            do
            {
                oldresult.Item1 = result5.Item1;
                oldresult.Item2 = result5.Item2;
                result5.Item1 = curve2.ClosestPoint(result5.Item2);
                result5.Item2 = curve1.ClosestPoint(result5.Item1);
            }
            while (oldresult.Item2.Distance(result5.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result5.Item1) > tolerance * tolerance);

            do
            {
                oldresult.Item1 = result6.Item1;
                oldresult.Item2 = result6.Item2;
                result6.Item1 = curve2.ClosestPoint(result6.Item2);
                result6.Item2 = curve1.ClosestPoint(result6.Item1);
            }
            while (oldresult.Item2.Distance(result6.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result6.Item1) > tolerance * tolerance);

            if (result.Item1 == null || result.Item2 == null)
                result = result2;

            if (result2.Item2.Distance(result2.Item1) < result.Item1.Distance(result.Item2))
                result = result2;

            if (result3.Item2.Distance(result3.Item1) < result.Item1.Distance(result.Item2))
                result = result3;

            if (result4.Item2.Distance(result4.Item1) < result.Item1.Distance(result.Item2))
                result = result4;

            if (result5.Item2.Distance(result5.Item1) < result.Item1.Distance(result.Item2))
                result = result5;

            if (result6.Item2.Distance(result6.Item1) < result.Item1.Distance(result.Item2))
                result = result6;

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Arc curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> cIntersections = curve1.CurveIntersections(curve2);
            if (cIntersections.Count > 0)
                return new Output<Point, Point> { Item1 = cIntersections[0], Item2 = cIntersections[0] };

            Output<Point, Point> result = new Output<Point, Point>();
            Plane ftPln1 = curve1.FitPlane();
            Plane ftPln2 = curve2.FitPlane();
            List<Point> intPts = new List<Point>();
            Point tmp = null;

            if ((intPts = curve1.PlaneIntersections(ftPln2)).Count != 0)
            {
                if (intPts.Count == 1)
                    tmp = intPts[0];
                else
                {
                    if (intPts[0].Distance(curve2) < intPts[1].Distance(curve2))
                    {
                        tmp = intPts[0];
                    }
                    else
                    {
                        tmp = intPts[1];
                    }
                }
            }
            else if ((intPts = curve2.PlaneIntersections(ftPln1)).Count != 0)
            {
                if (intPts.Count == 1)
                {
                    if (tmp == null)
                        tmp = intPts[0];
                    else if (tmp.Distance(curve2) > intPts[0].Distance(curve1))
                        tmp = intPts[0];
                }
                else
                {
                    if (intPts[0].Distance(curve1) < intPts[1].Distance(curve1))
                    {
                        if (tmp == null)
                            tmp = intPts[0];
                        else if (tmp.Distance(curve2) > intPts[0].Distance(curve1))
                            tmp = intPts[0];
                    }
                    else
                    {
                        if (tmp == null)
                            tmp = intPts[1];
                        else if (tmp.Distance(curve2) > intPts[1].Distance(curve1))
                            tmp = intPts[1];
                    }
                }
            }

            Output<Point, Point> oldresult = new Output<Point, Point>();
            Output<Point, Point> oldresult2 = new Output<Point, Point>();
            Output<Point, Point> result2 = new Output<Point, Point>();

            if (tmp != null)
            {
                if (tmp.IsOnCurve(curve1))
                {
                    result.Item1 = tmp;
                    result.Item2 = curve2.ClosestPoint(result.Item1);
                }
                else
                {
                    result.Item2 = tmp;
                    result.Item1 = curve1.ClosestPoint(result.Item2);
                }

                do
                {
                    oldresult.Item1 = result.Item1;
                    oldresult.Item2 = result.Item2;
                    result.Item1 = curve2.ClosestPoint(result.Item2);
                    result.Item2 = curve1.ClosestPoint(result.Item1);
                }
                while (oldresult.Item2.Distance(result.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result.Item1) > tolerance * tolerance);
            }
            else
            {
                Line intersect = new Line { Start = curve1.Centre(), End = curve2.Centre };
                Point tmp1 = intersect.CurveProximity(curve1).Item1;
                Point tmp2 = intersect.CurveProximity(curve2).Item1;
                if (tmp == null)
                    tmp = tmp1;

                if (tmp1.Distance(curve2) < tmp.Distance(curve1) || tmp1.Distance(curve2) < tmp.Distance(curve2))
                    tmp = tmp1;

                if (tmp2.Distance(curve1) < tmp.Distance(curve1) || tmp2.Distance(curve1) < tmp.Distance(curve2))
                    tmp = tmp2;

                if (tmp.IsOnCurve(curve1))
                {
                    result.Item1 = tmp;
                    result.Item2 = curve2.ClosestPoint(tmp);
                }
                else
                {
                    result.Item2 = tmp;
                    result.Item1 = curve1.ClosestPoint(tmp);
                }

                do
                {
                    oldresult.Item1 = result.Item1;
                    oldresult.Item2 = result.Item2;
                    result.Item1 = curve2.ClosestPoint(result.Item2);
                    result.Item2 = curve1.ClosestPoint(result.Item1);
                }
                while (oldresult.Item2.Distance(result.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result.Item1) > tolerance * tolerance);
            }

            if (curve1.EndPoint().Distance(curve2.ClosestPoint(curve1.EndPoint())) < result.Item1.Distance(result.Item2))
            {
                result.Item1 = curve1.EndPoint();
                result.Item2 = curve2.ClosestPoint(result.Item1);
            }

            if (curve1.StartPoint().Distance(curve2.ClosestPoint(curve1.StartPoint())) < result.Item1.Distance(result.Item2))
            {
                result.Item1 = curve1.StartPoint();
                result.Item2 = curve2.ClosestPoint(result.Item1);
            }

            tmp = curve1.PointAtParameter((curve1.ParameterAtPoint(result.Item1) + 0.6) % 1);
            result2.Item1 = tmp;
            result2.Item2 = curve2.ClosestPoint(tmp);

            do
            {
                oldresult2.Item1 = result2.Item1;
                oldresult2.Item2 = result2.Item2;
                result2.Item1 = curve2.ClosestPoint(result2.Item2);
                result2.Item2 = curve1.ClosestPoint(result2.Item1);
            }
            while (oldresult2.Item2.Distance(result2.Item2) > tolerance * tolerance && oldresult2.Item1.Distance(result2.Item1) > tolerance * tolerance);

            if (result.Item1.Distance(result.Item2) > result2.Item1.Distance(result2.Item2))
                result = result2;

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Arc curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Arc curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Circle curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Circle curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Circle curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> cIntersections = curve1.CurveIntersections(curve2);
            if (cIntersections.Count > 0)
                return new Output<Point, Point> { Item1 = cIntersections[0], Item2 = cIntersections[0] };

            Output<Point, Point> result = new Output<Point, Point>();

            //if (Math.Abs(curve1.Normal.IsParallel(curve2.Normal)) == 1) //TODO find solution for special case
            //{}

            Plane ftPln1 = curve1.FitPlane();
            Plane ftPln2 = curve2.FitPlane();
            List<Point> intPts = new List<Point>();
            Point tmp = null;

            if ((intPts = curve1.PlaneIntersections(ftPln2)).Count != 0)
            {
                if (intPts.Count == 1)
                    tmp = intPts[0];
                else
                {
                    if (intPts[0].Distance(curve2) < intPts[1].Distance(curve2))
                    {
                        tmp = intPts[0];
                    }
                    else
                    {
                        tmp = intPts[1];
                    }
                }
            }
            else if ((intPts = curve2.PlaneIntersections(ftPln1)).Count != 0)
            {
                if (intPts.Count == 1)
                {
                    if (tmp == null)
                        tmp = intPts[0];
                    else if (tmp.Distance(curve2) > intPts[0].Distance(curve1))
                        tmp = intPts[0];
                }
                else
                {
                    if (intPts[0].Distance(curve1) < intPts[1].Distance(curve1))
                    {
                        if (tmp == null)
                            tmp = intPts[0];
                        else if (tmp.Distance(curve2) > intPts[0].Distance(curve1))
                            tmp = intPts[0];
                    }
                    else
                    {
                        if (tmp == null)
                            tmp = intPts[1];
                        else if (tmp.Distance(curve2) > intPts[1].Distance(curve1))
                            tmp = intPts[1];
                    }
                }
            }

            Output<Point, Point> oldresult2 = new Output<Point, Point>();
            Output<Point, Point> result2 = new Output<Point, Point>();
            Output<Point, Point> oldresult = new Output<Point, Point>();

            if (tmp != null)
            {
                if (tmp.IsOnCurve(curve1))
                {
                    result.Item1 = curve1.PointAtParameter(curve1.ParameterAtPoint(tmp) + 0.25);
                    result.Item2 = curve2.ClosestPoint(tmp);
                    result2.Item1 = curve1.PointAtParameter((curve1.ParameterAtPoint(tmp) + 0.75) % 1);
                    result2.Item2 = curve2.ClosestPoint(result2.Item1);
                }
                else
                {
                    result.Item2 = curve2.PointAtParameter(curve2.ParameterAtPoint(tmp) + 0.25);
                    result.Item1 = curve1.ClosestPoint(tmp);
                    result2.Item2 = curve2.PointAtParameter((curve2.ParameterAtPoint(tmp) + 0.75) % 1);
                    result2.Item1 = curve1.ClosestPoint(result2.Item2);
                }

                do
                {
                    oldresult.Item1 = result.Item1;
                    oldresult.Item2 = result.Item2;
                    result.Item1 = curve2.ClosestPoint(result.Item2);
                    result.Item2 = curve1.ClosestPoint(result.Item1);
                }
                while (oldresult.Item2.Distance(result.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result.Item1) > tolerance * tolerance);

                do
                {
                    oldresult2.Item1 = result2.Item1;
                    oldresult2.Item2 = result2.Item2;
                    result2.Item1 = curve2.ClosestPoint(result2.Item2);
                    result2.Item2 = curve1.ClosestPoint(result2.Item1);
                }
                while (oldresult2.Item2.Distance(result2.Item2) > tolerance * tolerance && oldresult2.Item1.Distance(result2.Item1) > tolerance * tolerance);

                if (result.Item1.Distance(result.Item2) < result2.Item1.Distance(result2.Item2))
                    return result;
                else
                    return result2;
            }

            Line intersect = new Line { Start = curve1.Centre, End = curve2.Centre };
            Point tmp1 = intersect.CurveProximity(curve1).Item1;
            Point tmp2 = intersect.CurveProximity(curve2).Item1;
            if (tmp == null)
                tmp = tmp1;
            if (tmp1.Distance(curve2) < tmp.Distance(curve1) || tmp1.Distance(curve2) < tmp.Distance(curve2))
                tmp = tmp1;

            if (tmp2.Distance(curve1) < tmp.Distance(curve1) || tmp2.Distance(curve1) < tmp.Distance(curve2))
                tmp = tmp2;

            if (tmp.IsOnCurve(curve1))
            {
                result.Item1 = tmp;
                result.Item2 = curve2.ClosestPoint(tmp);
            }
            else
            {
                result.Item2 = tmp;
                result.Item1 = curve1.ClosestPoint(tmp);
            }

            do
            {
                oldresult.Item1 = result.Item1;
                oldresult.Item2 = result.Item2;
                result.Item1 = curve2.ClosestPoint(result.Item2);
                result.Item2 = curve1.ClosestPoint(result.Item1);
            }
            while (oldresult.Item2.Distance(result.Item2) > tolerance * tolerance && oldresult.Item1.Distance(result.Item1) > tolerance * tolerance);

            tmp = curve1.PointAtParameter((curve1.ParameterAtPoint(result.Item1) + 0.6) % 1);
            result2.Item1 = tmp;
            result2.Item2 = curve2.ClosestPoint(tmp);

            do
            {
                oldresult2.Item1 = result2.Item1;
                oldresult2.Item2 = result2.Item2;
                result2.Item1 = curve2.ClosestPoint(result2.Item2);
                result2.Item2 = curve1.ClosestPoint(result2.Item1);
            }
            while (oldresult2.Item2.Distance(result2.Item2) > tolerance * tolerance && oldresult2.Item1.Distance(result2.Item1) > tolerance * tolerance);

            if (result.Item1.Distance(result.Item2) > result2.Item1.Distance(result2.Item2))
                result = result2;

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Circle curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Circle curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this PolyCurve curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.ICurveProximity(curve1.Curves[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < curve1.Curves.Count; i++)
            {
                cp = curve1.Curves[i].ICurveProximity(curve2);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this PolyCurve curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.ICurveProximity(curve1.Curves[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < curve1.Curves.Count; i++)
            {
                cp = curve1.Curves[i].ICurveProximity(curve2);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this PolyCurve curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.ICurveProximity(curve1.Curves[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < curve1.Curves.Count; i++)
            {
                cp = curve1.Curves[i].ICurveProximity(curve2);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this PolyCurve curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve2.ControlPoints.Count - 1; i++)
            {
                temp.Add(new Line { Start = curve2.ControlPoints[i], End = curve2.ControlPoints[i + 1] });
            }

            if (curve2.IsClosed())
                temp.Add(new Line { Start = curve2.ControlPoints[curve2.ControlPoints.Count - 1], End = curve2.ControlPoints[0] });

            Output<Point, Point> result = curve2.ICurveProximity(curve1.Curves[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < curve1.Curves.Count; i++)
            {
                cp = curve1.Curves[i].ICurveProximity(curve2);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this PolyCurve curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.ICurveProximity(curve1.Curves[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < curve1.Curves.Count; i++)
            {
                cp = curve1.Curves[i].ICurveProximity(curve2);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Polyline curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
            {
                temp.Add(new Line { Start = curve1.ControlPoints[i], End = curve1.ControlPoints[i + 1] });
            }

            Output<Point, Point> result = curve2.CurveProximity(temp[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < temp.Count; i++)
            {
                cp = curve2.CurveProximity(temp[i]);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Polyline curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
            {
                temp.Add(new Line { Start = curve1.ControlPoints[i], End = curve1.ControlPoints[i + 1] });
            }

            Output<Point, Point> result = curve2.CurveProximity(temp[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < temp.Count; i++)
            {
                cp = temp[i].CurveProximity(curve2);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }
            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Polyline curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
            {
                temp.Add(new Line { Start = curve1.ControlPoints[i], End = curve1.ControlPoints[i + 1] });
            }

            Output<Point, Point> result = curve2.CurveProximity(temp[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < temp.Count; i++)
            {
                cp = temp[i].CurveProximity(curve2);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }

            return result;
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Polyline curve1, PolyCurve curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = curve2.CurveProximity(curve1);
            return new Output<Point, Point> { Item1 = result.Item2, Item2 = result.Item1 };
        }

        /***************************************************/

        public static Output<Point, Point> CurveProximity(this Polyline curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            List<Line> temp = new List<Line>();
            for (int i = 0; i < curve1.ControlPoints.Count - 1; i++)
            {
                temp.Add(new Line { Start = curve1.ControlPoints[i], End = curve1.ControlPoints[i + 1] });
            }

            Output<Point, Point> result = curve2.CurveProximity(temp[0]);
            Output<Point, Point> cp = new Output<Point, Point>();

            for (int i = 1; i < temp.Count; i++)
            {
                cp = temp[i].CurveProximity(curve2);

                if (cp.Item1.Distance(cp.Item2) < result.Item1.Distance(result.Item2))
                    result = cp;
            }

            return result;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Output<Point, Point> ICurveProximity(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            Output<Point, Point> result = CurveProximity(curve1 as dynamic, curve2 as dynamic);

            if (result.Item1.IIsOnCurve(curve1))
                return result;
            else
            {
                return new Output<Point, Point>
                {
                    Item1 = result.Item2,
                    Item2 = result.Item1
                };
            }
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static Output<Point, Point> CurveProximity(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"CurveProximity is not implemented for a combination of {curve1.GetType().Name} and {curve2.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}



