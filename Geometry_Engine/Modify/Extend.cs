/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                  ****/
        /***************************************************/

        [Description("Extends curve by given lengths")]
        [Input("curve", "Curve to extend")]
        [Input("start", "Length of extension on the begining of a curve. Negative value will trim the curve")]
        [Input("end", "Length of extension on the end of a curve. Negative value will trim the curve")]
        [Input("tangentExtensions", "True - extends on tangent lines\nFalse - extends by curve's shape")]
        [Output("curve", "Extended curve")]
        public static Line Extend(this Line curve, double start = 0.0, double end = 0.0, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            if (start + end + curve.Length() < tolerance)
            {
                Base.Compute.RecordError("Extend values too small");
                return null;
            }
            Vector dir = curve.Direction(tolerance);
            return new Line { Start = curve.Start - dir * start, End = curve.End + dir * end };
        }

        /***************************************************/

        [Description("Extends curve by given lengths")]
        [Input("curve", "Curve to extend")]
        [Input("start", "Length of extension on the begining of a curve. Negative value will trim the curve")]
        [Input("end", "Length of extension on the end of a curve. Negative value will trim the curve")]
        [Input("tangentExtensions", "True - extends on tangent lines\nFalse - extends by curve's shape")]
        [Output("curve", "Extended curve")]
        public static ICurve Extend(this Arc curve, double start = 0.0, double end = 0.0, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            if (tangentExtensions)
                return curve.ExtendTangent(start, end, tolerance);

            if (curve.StartAngle != 0)
            {
                curve.CoordinateSystem = curve.CoordinateSystem.Rotate(curve.CoordinateSystem.Origin, curve.CoordinateSystem.Z, curve.StartAngle);
                curve.EndAngle = -curve.StartAngle;
                curve.StartAngle = 0;
            }

            double startAngleExt = start / curve.Radius;
            double endAngleExt = end / curve.Radius;

            if (startAngleExt + endAngleExt + curve.EndAngle < tolerance)
            {
                Base.Compute.RecordError("Negative extend values are smaller than curve length.");
                return null;
            }

            if (startAngleExt + endAngleExt + curve.EndAngle - (2 * Math.PI) > tolerance)
            {
                Base.Compute.RecordError("Extension values to great.");
                return null;
            }
            Cartesian oldCS = curve.CoordinateSystem;
            Cartesian newCS = oldCS.Rotate(oldCS.Origin, oldCS.Z, -startAngleExt);

            return new Arc
            {
                CoordinateSystem = newCS,
                Radius = curve.Radius,
                StartAngle = 0,
                EndAngle = curve.EndAngle + startAngleExt + endAngleExt
            };
        }

        /***************************************************/

        [Description("Extends curve by given lengths")]
        [Input("curve", "Curve to extend")]
        [Input("start", "Length of extension on the begining of a curve. Negative value will trim the curve")]
        [Input("end", "Length of extension on the end of a curve. Negative value will trim the curve")]
        [Input("tangentExtensions", "True - extends on tangent lines\nFalse - extends by curve's shape")]
        [Output("curve", "Extended curve")]
        public static Circle Extend(this Circle curve, double start = 0.0, double end = 0.0, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordNote("Cannot Trim or Extend closed curves.");
            return curve;
        }

        /***************************************************/

        [Description("Extends curve by given lengths")]
        [Input("curve", "Curve to extend")]
        [Input("start", "Length of extension on the begining of a curve. Negative value will trim the curve")]
        [Input("end", "Length of extension on the end of a curve. Negative value will trim the curve")]
        [Input("tangentExtensions", "True - extends on tangent lines\nFalse - extends by curve's shape")]
        [Output("curve", "Extended curve")]
        public static Ellipse Extend(this Ellipse curve, double start = 0.0, double end = 0.0, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordNote("Cannot Trim or Extend closed curves.");
            return curve;
        }

        /***************************************************/

        [Description("Extends curve by given lengths")]
        [Input("curve", "Curve to extend")]
        [Input("start", "Length of extension on the begining of a curve. Negative value will trim the curve")]
        [Input("end", "Length of extension on the end of a curve. Negative value will trim the curve")]
        [Input("tangentExtensions", "True - extends on tangent lines\nFalse - extends by curve's shape")]
        [Output("curve", "Extended curve")]
        public static Polyline Extend(this Polyline curve, double start = 0.0, double end = 0.0, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            if (curve.IsClosed(tolerance))
            {
                Base.Compute.RecordNote("Cannot Trim or Extend closed curves.");
                return curve;
            }

            if (start + end + curve.Length() < tolerance)
            {
                Base.Compute.RecordError("Negative extend values are smaller than curve length.");
                return null;
            }

            Polyline result = new Polyline();
            List<Line> lines = curve.SubParts();

            if (start < 0 && -start > lines[0].ILength())
            {
                double startCut = -lines[0].ILength();

                while (startCut > start && lines.Count > 1)
                {
                    lines.RemoveAt(0);
                    startCut -= lines[0].ILength();
                }

                startCut += lines[0].ILength();

                if (lines.Count > 1)
                    lines[0] = lines[0].Extend(start - startCut, 0, false, tolerance);
                else
                {
                    lines[0] = lines[0].Extend(start - startCut, end, tangentExtensions, tolerance);
                    result = Create.Polyline(lines);
                    return result;
                }
            }
            else
                lines[0] = lines[0].Extend(start, 0, tangentExtensions, tolerance);

            if (end < 0 && -end > lines[lines.Count - 1].ILength())
            {
                double endCut = -lines[lines.Count - 1].ILength();
                while (endCut > end)
                {
                    lines.RemoveAt(lines.Count - 1);
                    endCut -= lines[lines.Count - 1].ILength();
                }
                endCut += lines[lines.Count - 1].ILength();
                lines[lines.Count - 1] = lines[lines.Count - 1].Extend(0, end - endCut, tangentExtensions, tolerance);
            }
            else
                lines[lines.Count - 1] = lines[lines.Count - 1].Extend(0, end, tangentExtensions, tolerance);

            result = Create.Polyline(lines);
            return result;
        }

        /***************************************************/

        [Description("Extends curve by given lengths")]
        [Input("curve", "Curve to extend")]
        [Input("start", "Length of extension on the begining of a curve. Negative value will trim the curve")]
        [Input("end", "Length of extension on the end of a curve. Negative value will trim the curve")]
        [Input("tangentExtensions", "True - extends on tangent lines\nFalse - extends by curve's shape")]
        [Output("curve", "Extended curve")]
        public static PolyCurve Extend(this PolyCurve curve, double start = 0.0, double end = 0.0, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            if (tangentExtensions)
                return curve.ExtendTangent(start, end, tolerance);

            if (curve.IsClosed(tolerance))
            {
                Base.Compute.RecordNote("Cannot Trim or Extend closed curves.");
                return curve;
            }

            if (start + end + curve.Length() < tolerance)
            {
                Base.Compute.RecordError("Negative extend values are smaller than curve length.");
                return null;
            }

            List<ICurve> curves = curve.SubParts();
            if (start < 0 && -start > curves[0].ILength())
            {
                double startCut = -curves[0].ILength();
                while (startCut > start && curves.Count > 1)
                {
                    curves.RemoveAt(0);
                    startCut -= curves[0].ILength();
                }

                startCut += curves[0].ILength();

                if (curves.Count > 1)
                    curves[0] = curves[0].IExtend(start - startCut, 0, tangentExtensions, tolerance);
                else
                {
                    curves[0] = curves[0].IExtend(start - startCut, end, tangentExtensions, tolerance);
                    return new PolyCurve { Curves = curves };
                }
            }
            else
                curves[0] = curves[0].IExtend(start, 0, tangentExtensions, tolerance);

            if (end < 0 && -end > curves[curves.Count - 1].ILength())
            {
                double endCut = -curves[curves.Count - 1].ILength();
                while (endCut > end)
                {
                    curves.RemoveAt(curves.Count - 1);
                    endCut -= curves[curves.Count - 1].ILength();
                }
                endCut += curves[curves.Count - 1].ILength();
                curves[curves.Count - 1] = curves[curves.Count - 1].IExtend(0, end - endCut, tangentExtensions, tolerance);
            }
            else
                curves[curves.Count - 1] = curves[curves.Count - 1].IExtend(0, end, tangentExtensions, tolerance);

            return new PolyCurve { Curves = curves };
        }


        /***************************************************/
        /***   Public Methods - Interfaces               ***/
        /***************************************************/

        [Description("Extends curve by given lengths")]
        [Input("curve", "Curve to extend")]
        [Input("start", "Length of extension on the begining of a curve. Negative value will trim the curve")]
        [Input("end", "Length of extension on the end of a curve. Negative value will trim the curve")]
        [Input("tangentExtensions", "True - extends on tangent lines\nFalse - extends by curve's shape")]
        [Output("curve", "Extended curve")]
        public static ICurve IExtend(this ICurve curve, double start = 0.0, double end = 0.0, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            return Extend(curve as dynamic, start, end, tangentExtensions, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static ICurve Extend(this ICurve curve, double start = 0.0, double end = 0.0, bool tangentExtensions = false, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"Extend is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }


        /***************************************************/
        /***   Private Methods - Tangent extensions      ***/
        /***************************************************/

        private static ICurve ExtendTangent(this Arc curve, double start = 0.0, double end = 0.0, double tolerance = Tolerance.Distance)
        {
            if (-start > curve.Length() || -end > curve.Length()) //ExtendTangent allows to trim the curve but only to the limit of it's length. 
            {
                Base.Compute.RecordError("Extension value too small");
                return null;
            }

            if (start < 0 && end < 0)
                return curve.Extend(start, end, false, tolerance);

            PolyCurve polyCurve = new PolyCurve();

            if (start < 0 && !(end < 0))
            {
                ICurve iCurve = curve.Extend(start, 0, false, tolerance);
                return iCurve.IExtend(0, end, true, tolerance);
            }

            if (!(start < 0) && end < 0)
            {
                ICurve iCurve = curve.Extend(0, end, false, tolerance);
                return iCurve.IExtend(start, 0, true, tolerance);
            }

            Point stPt = curve.StartPoint();
            Point enPt = curve.EndPoint();
            Vector startTan = curve.TangentAtParameter(0, tolerance);
            Vector endTan = curve.TangentAtParameter(1, tolerance);
            List<ICurve> resultList = new List<ICurve>();

            if (start != 0)
                resultList.Add(new Line { Start = stPt - startTan * start, End = stPt, Infinite = false });

            resultList.Add(curve);

            if (end != 0)
                resultList.Add(new Line { Start = enPt, End = enPt + endTan * end, Infinite = false });

            if (resultList.Count == 1)
                return resultList[0];
            else
                return new PolyCurve { Curves = resultList };
        }

        /***************************************************/

        private static PolyCurve ExtendTangent(this PolyCurve curve, double start = 0.0, double end = 0.0, double tolerance = Tolerance.Distance)
        {
            if (-start > curve.Length() || -end > curve.Length()) //ExtendTangent allows to trim the curve but only to the limit of it's length. 
            {
                Base.Compute.RecordError("Extension value too small");
                return null;
            }

            if (start < 0 && end < 0)
                return curve.Extend(start, end, false, tolerance);

            if (start < 0 && !(end < 0))
            {
                curve = curve.Extend(start, 0, false, tolerance);
                return curve.ExtendTangent(0, end, tolerance);
            }

            if (!(start < 0) && end < 0)
            {
                curve = curve.Extend(0, end, false, tolerance);
                return curve.ExtendTangent(start, 0, tolerance);
            }

            List<ICurve> curves = curve.SubParts();
            if (curves.Count == 1)
                return new PolyCurve { Curves = curves[0].IExtend(start, end, true, tolerance).ISubParts().ToList() };
            else
            {
                curves[0] = curves[0].IExtend(start, 0, true, tolerance);
                curves[curves.Count - 1] = curves[curves.Count - 1].IExtend(0, end, true, tolerance);
                return new PolyCurve { Curves = curves };
            }
        }

        /***************************************************/
    }
}


