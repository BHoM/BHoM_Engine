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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Reinforcement;
using BH.oM.Physical.Reinforcement.BS8666;
using BH.oM.Quantities.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes the centreline for a Reinforcement using the standard as determined by the ShapeCode namespace. The curve will be oriented to the coordinate system.")]
        [Input("reinforcement", "The reinforcement containing the ShapeCode, reinforcement and bending radius to generate the centreline.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this Reinforcement reinforcement)
        {
            return reinforcement.IsValid() ? ICentreline(reinforcement.ShapeCode).Orient(new Cartesian(), reinforcement.CoordinateSystem) : null;
        }

        /***************************************************/

        [Description("Computes the centreline for a Reinforcement using the standard as determined by the ShapeCode namespace. The curve will be in the XY Plane - refer to the ShapeCode description for " +
            "specifics on the orientation.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(this IShapeCode shapeCode)
        {
            if (shapeCode.IsNull())
                return null;
            else if (!shapeCode.IIsCompliant())
                return null;

            return Centreline(shapeCode as dynamic);
        }

        /***************************************************/
        /****    Private Methods                    ********/
        /***************************************************/

        private static ICurve Centreline(this ShapeCode00 shapeCode)
        {
            return new Line() { Start = new Point() { X = -shapeCode.A / 2 }, End = new Point() { X = shapeCode.A / 2 } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode11 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point bEnd = new Point() { X = shapeCode.B - bendOffset - shapeCode.Diameter / 2 };
            Point arcCentre = bEnd + new Vector() { Y = bendOffset };
            Point aStart = arcCentre + new Vector() { X = bendOffset };

            Line b = new Line() { Start = new Point(), End = bEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart + new Vector { Y = shapeCode.A - bendOffset - shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { b, arc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode12 shapeCode)
        {
            double bendOffset = shapeCode.R + shapeCode.Diameter / 2;

            Point bEnd = new Point() { X = shapeCode.B - bendOffset - shapeCode.Diameter / 2 };
            Point arcCentre = bEnd + new Vector() { Y = bendOffset };
            Point aStart = arcCentre + new Vector() { X = bendOffset };

            Line b = new Line() { Start = new Point(), End = bEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart + new Vector { Y = shapeCode.A - bendOffset - shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { b, arc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode13 shapeCode)
        {
            double bendOffset = shapeCode.B / 2 - shapeCode.Diameter / 2;

            Point aEnd = new Point() { X = shapeCode.A - shapeCode.B / 2 };
            Point arcCentre = aEnd + new Vector() { Y = bendOffset };
            Point cStart = arcCentre + new Vector() { Y = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Circle circle = new Circle() { Centre = arcCentre, Radius = bendOffset, Normal = Vector.ZAxis };
            ICurve b = circle.SplitAtPoints(new List<Point>() { aEnd, cStart })[1];
            Line c = new Line() { Start = cStart, End = cStart + new Vector { X = -shapeCode.C + shapeCode.B / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, b, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode14 shapeCode)
        {
            double angle = Math.Atan((shapeCode.B - shapeCode.BendRadius) / (shapeCode.D));
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point cEnd = new Point() { X = shapeCode.C - bendOffset - shapeCode.Diameter / 2 };
            Point caCentre = cEnd + new Vector() { Y = bendOffset };
            Circle circle = new Circle() { Centre = caCentre, Normal = Vector.ZAxis, Radius = bendOffset };
            Point aEnd = new Point() { X = shapeCode.C - shapeCode.D + shapeCode.Diameter/2*Math.Cos(angle), Y = shapeCode.B - shapeCode.Diameter/2*(1 + Math.Sin(angle)) };

            Point aStart = caCentre.CircleTangentialPoint(aEnd, bendOffset)[1];
            //Point aStart = circle.ClosestPoint(aEnd).Rotate(arcCentre, Vector.ZAxis, -Math.PI / 2);

            Line c = new Line() { Start = new Point(), End = cEnd };
            ICurve arc = circle.SplitAtPoints(new List<Point>() { cEnd, aStart })[0];
            Line a = new Line() { Start = aStart, End = aEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { c, arc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode15 shapeCode)
        {
            double angle = Math.Atan((shapeCode.B - shapeCode.Diameter) / shapeCode.D);
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double lengthReduction = (shapeCode.BendRadius + shapeCode.Diameter) *Math.Sin(angle/2);

            Point cEnd = new Point() { X = -shapeCode.C + lengthReduction };
            Point acCentre = cEnd + new Vector() { Y = bendOffset };
            Circle circle = new Circle() { Centre = acCentre, Normal = Vector.ZAxis, Radius = bendOffset };
            Point aEnd = new Point() { X = -shapeCode.C - shapeCode.D + shapeCode.Diameter / 2 * Math.Cos(angle), Y = shapeCode.B - shapeCode.Diameter/2*( 1 + Math.Sin(angle))};

            Point aStart = acCentre.CircleTangentialPoint(aEnd, bendOffset)[0];

            Line c = new Line() { Start = new Point(), End = cEnd };
            ICurve arc = circle.SplitAtPoints(new List<Point>() { cEnd, aStart })[0].IFlip();
            Line a = new Line() { Start = aStart, End = aEnd};

            return new PolyCurve() { Curves = new List<ICurve>() { c, arc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode21 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point aEnd = new Point() { Y = -shapeCode.A + bendOffset + shapeCode.Diameter / 2 };
            Point abArcCentre = aEnd + new Vector() { X = bendOffset };
            Point bStart = abArcCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point bcArcCentre = bEnd + new Vector() { Y = bendOffset };
            Point cStart = bcArcCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcArcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { Y = shapeCode.C - bendOffset - shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode22 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point aEnd = new Point() { Y = shapeCode.A - shapeCode.BendRadius - shapeCode.Diameter };
            Point abArcCentre = aEnd + new Vector() { X = bendOffset };
            Point bStart = abArcCentre + new Vector() { Y = bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - shapeCode.BendRadius - shapeCode.Diameter - shapeCode.C / 2 };
            Point bdArcCentre = bEnd + new Vector() { Y = -shapeCode.C / 2 + shapeCode.Diameter / 2 };
            Point dStart = bdArcCentre + new Vector() { Y = -shapeCode.C / 2 + shapeCode.Diameter / 2 };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Circle circle = new Circle() { Centre = bdArcCentre, Radius = shapeCode.C / 2 - shapeCode.Diameter / 2, Normal = Vector.ZAxis };
            ICurve bdArc = circle.SplitAtPoints(new List<Point>() { bEnd, dStart })[1].IFlip();
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { X = -shapeCode.D + shapeCode.C / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bdArc, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode23 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point aEnd = new Point() { Y = -shapeCode.A + bendOffset + shapeCode.Diameter / 2 };
            Point abCentre = aEnd + new Vector() { X = bendOffset };
            Point bStart = abCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point bcCentre = bEnd + new Vector() { Y = -bendOffset };
            Point cStart = bcCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { Y = -shapeCode.C + bendOffset + shapeCode.Diameter / 2 } };

            PolyCurve polyCurve = new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };

            return shapeCode.ZBar ? polyCurve.Rotate(new Point(), Vector.ZAxis, Math.PI / 2) : polyCurve;
        }

        /***************************************************/

        [NotImplemented]
        private static ICurve Centreline(this ShapeCode24 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double angle = Math.Atan(shapeCode.D / shapeCode.E);
            double lengthReductionBottom = (bendOffset + shapeCode.Diameter / 2) * Math.Sin(angle)/2;

            Point aEnd = new Point() { X = shapeCode.A - lengthReductionBottom };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };

            Point cEnd = new Point() { X = shapeCode.A + shapeCode.E - shapeCode.Diameter / 2, Y = shapeCode.D + shapeCode.C - shapeCode.Diameter / 2 };
            Point cStart = cEnd.Translate(new Vector() { Y = -shapeCode.C + lengthReductionBottom });
            Point bcCentre = cStart.Translate(new Vector() { X = -bendOffset });
            Vector bDir = bcCentre - abCentre;
            Vector bPerp = bDir.CrossProduct(Vector.ZAxis);
            Point bMid = bcCentre.Translate(bDir*shapeCode.B).Translate(bPerp * bendOffset);
            Point bStart = abCentre.CircleTangentialPoint(bMid, bendOffset)[1];
            Point bEnd = bcCentre.CircleTangentialPoint(bStart, bendOffset)[0];

            Line a = new Line() { Start = new Point(), End = aEnd };
            ICurve abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            ICurve bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode25 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double aeAngle = Math.Acos((shapeCode.C - shapeCode.Diameter) / shapeCode.A);
            double ebAngle = Math.Asin((shapeCode.D - shapeCode.Diameter) / shapeCode.B);
            double lengthReductionLeft = ((shapeCode.BendRadius + shapeCode.Diameter) * (Math.PI/2 - aeAngle)) / 2;
            double lengthReductionRight = ((shapeCode.BendRadius + shapeCode.Diameter) * ebAngle) / 2;

            Line a = new Line() { Start = new Point(), End = new Point() { Y = -shapeCode.A + lengthReductionLeft } }.Rotate(new Point(), Vector.ZAxis, aeAngle);
            Line aeRadius = new Line() { Start = a.End, End = a.End + new Vector() { X = bendOffset } }.Rotate(a.End, Vector.ZAxis, aeAngle);
            Point eStart = aeRadius.End + new Vector() { Y = -bendOffset };
            Point eEnd = eStart + new Vector() { X = shapeCode.E - lengthReductionLeft - lengthReductionRight};
            Point ebCentre = eEnd + new Vector() { Y = bendOffset };
            Line ebRadius = new Line() { Start = ebCentre, End = eEnd }.Rotate(ebCentre, Vector.ZAxis, ebAngle);
            Point bStart = ebRadius.End;

            Arc aeArc = Engine.Geometry.Create.ArcByCentre(aeRadius.End, a.End, eStart);
            Line e = new Line() { Start = eStart, End = eEnd };
            Arc ebArc = Engine.Geometry.Create.ArcByCentre(ebCentre, eEnd, bStart);
            Line b = new Line() { Start = bStart, End = bStart + new Vector { X = shapeCode.B - lengthReductionRight } }.Rotate(bStart, Vector.ZAxis, ebAngle);

            return new PolyCurve() { Curves = new List<ICurve>() { a, aeArc, e, ebArc, b } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode26 shapeCode)
        {
            double d = shapeCode.D - shapeCode.Diameter;  //Height centreline
            double alpha = Math.Atan(shapeCode.E / d);  //Angle of the upper corner of the triangle F-d-B 
            double beta = alpha / 2 + Math.PI / 4;      //Angle of the bisector between A and B
            double arcAngle = Math.PI / 2 - alpha;      //Angle of the arc

            double r = shapeCode.BendRadius + shapeCode.Diameter / 2;   //Centreline bend radius
            double x = r / Math.Tan(beta);                              //Distance from AB corner with 0 radius to end of A
            double s = Math.Sqrt(r * r + x * x);                        //Distance from arc centre to AB corner
            double t = s - r;                                           //Hypotenous of triangle from AB corner to arc
            double addRed = x / s * t;                                  //Reduction in the leng reduction. This is X-distance from arc centre to AB using the fact that triangles are of the same shape

            double lengthReduction = x - addRed;

            //Corner AB
            Point aEnd = new Point() { X = shapeCode.A - lengthReduction };
            Point abCentre = aEnd + new Vector() { Y = r };
            Point bStart = aEnd.Rotate(abCentre, Vector.ZAxis, arcAngle);

            //Corner BC
            Point cStart = new Point { X = shapeCode.A + shapeCode.E + lengthReduction, Y = d };
            Point bcCentre = new Point { X = cStart.X, Y = cStart.Y - r };
            Point bEnd = cStart.Rotate(bcCentre, Vector.ZAxis, arcAngle);

            Point cEnd = new Point { X = shapeCode.A + shapeCode.E + shapeCode.C, Y = cStart.Y };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode27 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double angle = Math.Acos((shapeCode.E) / shapeCode.A);
            double lengthReduction = ((shapeCode.BendRadius + shapeCode.Diameter) * Math.Sin(angle/2));

            Point cStart = new Point() { Y = shapeCode.C - bendOffset - shapeCode.Diameter / 2 };
            Point bcCentre = cStart.Translate(new Vector() { X = -bendOffset });
            Point bEnd = bcCentre.Translate(new Vector() { Y = bendOffset });
            Point bStart = bEnd.Translate(new Vector() { X = -shapeCode.B + bendOffset + shapeCode.Diameter / 2 + lengthReduction });
            Point abCentre = bStart.Translate(new Vector() { Y = - bendOffset});
            Point aStart = new Point() { X = -shapeCode.B - shapeCode.E + + shapeCode.Diameter/2* (1 + Math.Cos(angle)), Y = shapeCode.C - shapeCode.D + shapeCode.Diameter / 2 * Math.Sin(angle) };
            Point aEnd = abCentre.CircleTangentialPoint(aStart, bendOffset)[1];

            Line a = new Line() { Start = aStart, End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = new Point() };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode28 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double angle = Math.Acos((shapeCode.E) / shapeCode.A);
            double lengthReduction = ((shapeCode.BendRadius + shapeCode.Diameter) * Math.Sin(angle / 2));

            Point cStart = new Point() { Y = -shapeCode.C + bendOffset + shapeCode.Diameter / 2 };
            Point bcCentre = cStart.Translate(new Vector() { X = -bendOffset });
            Point bEnd = bcCentre.Translate(new Vector() { Y = -bendOffset });
            Point bStart = bEnd.Translate(new Vector() { X = -shapeCode.B + bendOffset + shapeCode.Diameter / 2 + lengthReduction });
            Point abCentre = bStart.Translate(new Vector() { Y = -bendOffset });
            Point aStart = new Point() { X = -shapeCode.B - shapeCode.E + shapeCode.Diameter / 2 * (1 + Math.Cos(angle)), Y = - shapeCode.C - shapeCode.D + shapeCode.Diameter / 2* Math.Sin(angle) };
            Point aEnd = abCentre.CircleTangentialPoint(aStart, bendOffset)[1];

            Line a = new Line() { Start = aStart, End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = new Point() };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        [NotImplemented]
        private static ICurve Centreline(this ShapeCode29 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double angle = Math.Atan(shapeCode.E / (shapeCode.D - shapeCode.Diameter - 2*shapeCode.BendRadius));
            double lengthReduction = 2 * (bendOffset + shapeCode.Diameter / 2) * Math.Sin((Math.PI/2 - angle)/2 )*Math.Cos(Math.PI/4 - angle/2);

            Point aEnd = new Point() { X = shapeCode.A - bendOffset - shapeCode.Diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };
            Circle abCircle = new Circle() { Centre = abCentre, Normal = Vector.ZAxis, Radius = bendOffset };
            Point cEnd = new Point() { X = shapeCode.A - shapeCode.C - shapeCode.E, Y = shapeCode.D - shapeCode.Diameter };
            Point cStart = new Point() { X = shapeCode.A - shapeCode.E - lengthReduction, Y = shapeCode.D - shapeCode.Diameter };
            Point bcCentre = cStart.Translate(new Vector() { Y = -bendOffset });
            Circle bcCircle = new Circle() { Centre = bcCentre, Normal = Vector.ZAxis, Radius = bendOffset };
            Point bStart = abCircle.CurveProximity(bcCircle).Item2.Rotate(abCentre, Vector.ZAxis, -Math.PI/2);
            Point bEnd = bcCircle.CurveProximity(abCircle).Item2.Rotate(bcCentre, Vector.ZAxis, Math.PI / 2);

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode31 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point aEnd = new Point() { X = -shapeCode.A + bendOffset + shapeCode.Diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = -bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point cdCentre = cEnd + new Vector() { Y = bendOffset };
            Point dStart = cdCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { Y = shapeCode.D - bendOffset - shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode32 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point aEnd = new Point() { X = -shapeCode.A + bendOffset + shapeCode.Diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = -bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point cdCentre = cEnd + new Vector() { Y = -bendOffset };
            Point dStart = cdCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { Y = -shapeCode.D + bendOffset + shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode33 shapeCode)
        {
            double bendOffset = shapeCode.B / 2 - shapeCode.Diameter / 2;

            Point cBotStart = new Point() { X = shapeCode.A / 2 - shapeCode.C, Y = bendOffset };
            Point cBotEnd = cBotStart + new Vector() { X = shapeCode.C - shapeCode.B/2 };
            Point rightCentre = cBotEnd + new Vector() { Y = bendOffset };
            Point aTopStart = rightCentre + new Vector() { Y = bendOffset };
            Point aTopEnd = aTopStart + new Vector() { X = -shapeCode.A + shapeCode.B, Z = -shapeCode.Diameter };
            Point leftCentre = aTopEnd + new Vector() { Y = -bendOffset};
            Point aBotStart = leftCentre + new Vector() { Y = -bendOffset };
            Point cTopEnd = aTopStart + new Vector() { X = -shapeCode.C + shapeCode.B/2, Z = -shapeCode.Diameter };

            Line cBot = new Line() { Start = cBotStart, End = cBotEnd };
            Circle rightCircle = new Circle() { Centre = rightCentre, Radius = bendOffset, Normal = Vector.ZAxis };
            ICurve rightArc = rightCircle.SplitAtPoints(new List<Point>() { cBotEnd, aTopStart })[1];
            Line aTop = new Line() { Start = aTopStart, End = aTopEnd };
            Circle leftCircle = new Circle() { Centre = leftCentre, Radius = bendOffset };
            ICurve leftArc = leftCircle.SplitAtPoints(new List<Point>() { aTopEnd, aBotStart })[0];
            Line aBot = new Line() { Start = aBotStart, End = cBotEnd + new Vector() { Z = -shapeCode.Diameter } };
            Line cTop = new Line() { Start = aTopStart + new Vector() { Z = -shapeCode.Diameter }, End = cTopEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { cBot, rightArc, aTop, leftArc, aBot, rightArc.ITranslate(new Vector() { Z = -shapeCode.Diameter }), cTop } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode34 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double angle = Math.Asin(shapeCode.D / shapeCode.B);
            double lengthReductionLeft = ((shapeCode.BendRadius + shapeCode.Diameter) * angle) / 2;
            double lengthReductionRight = ((shapeCode.BendRadius + shapeCode.Diameter) * angle) / 2;

            Point aEnd = new Point() { X = shapeCode.A - lengthReductionLeft };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart + new Vector() { X = shapeCode.B - lengthReductionLeft - lengthReductionRight } }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd + new Vector() { X = bendOffset } }.Rotate(b.End, Vector.ZAxis, -Math.PI/2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre + new Vector() { Y = bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - lengthReductionRight - bendOffset - shapeCode.Diameter/2 };
            Point ceCentre = cEnd + new Vector() { Y = -bendOffset };
            Point eStart = ceCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc ceArc = Engine.Geometry.Create.ArcByCentre(ceCentre, cEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart + new Vector() { Y = -shapeCode.E + bendOffset + shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, ceArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode35 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double angle = Math.Asin(shapeCode.D / shapeCode.B);
            double lengthReductionLeft = ((shapeCode.BendRadius + shapeCode.Diameter) * angle) / 2;
            double lengthReductionRight = ((shapeCode.BendRadius + shapeCode.Diameter) * angle) / 2;

            Point aEnd = new Point() { X = shapeCode.A - lengthReductionLeft };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis,  angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart + new Vector() { X = shapeCode.B - lengthReductionLeft - lengthReductionLeft } }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd + new Vector() { X = bendOffset } }.Rotate(b.End, Vector.ZAxis, -Math.PI / 2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre + new Vector() { Y = bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - lengthReductionRight - bendOffset - shapeCode.Diameter/2 };
            Point ceCentre = cEnd + new Vector() { Y = bendOffset };
            Point eStart = ceCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc ceArc = Engine.Geometry.Create.ArcByCentre(ceCentre, cEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart + new Vector() { Y = shapeCode.E - bendOffset - shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, ceArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode36 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double angle = Math.Asin((shapeCode.E) / shapeCode.A);
            double lengthReduction = ((shapeCode.BendRadius + shapeCode.Diameter) * angle) / 2;

            Point dEnd = new Point() { X = -shapeCode.D + bendOffset + shapeCode.Diameter / 2 };
            Point dcCentre = dEnd + new Vector() { Y = -bendOffset };
            Point cStart = dcCentre + new Vector() { X = -bendOffset };
            Point cEnd = cStart + new Vector() { Y = -shapeCode.C + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point cbCentre = cEnd + new Vector() { X = bendOffset };
            Point bStart = cbCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - bendOffset - shapeCode.Diameter/2 - lengthReduction };
            Point baCentre = bEnd + new Vector() { Y = bendOffset };
            Point aEnd = new Point() { X = shapeCode.B + shapeCode.F - shapeCode.Diameter / 2,
                Y = -shapeCode.C + shapeCode.Diameter + shapeCode.E - shapeCode.Diameter - shapeCode.Diameter/2};
            Point aStart = baCentre.CircleTangentialPoint(aEnd, bendOffset)[1];



            Line d = new Line() { Start = new Point(), End = dEnd };
            Arc dcArc = Engine.Geometry.Create.ArcByCentre(dcCentre, dEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cbArc = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc baArc = Engine.Geometry.Create.ArcByCentre(baCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart + new Vector() { X = shapeCode.A - lengthReduction } }.Rotate(aStart, Vector.ZAxis, angle);

            return new PolyCurve() { Curves = new List<ICurve>() { d, dcArc, c, cbArc, b, baArc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode41 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point aEnd = new Point() { X = -shapeCode.A + bendOffset + shapeCode.Diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = -bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point cdCentre = cEnd + new Vector() { Y = bendOffset };
            Point dStart = cdCentre + new Vector() { X = bendOffset };
            Point dEnd = dStart + new Vector() { Y = shapeCode.D - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point deCentre = dEnd + new Vector() { X = -bendOffset };
            Point eStart = deCentre + new Vector() { Y = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dEnd };
            Arc deArc = Engine.Geometry.Create.ArcByCentre(deCentre, dEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart + new Vector { X = -shapeCode.E + bendOffset + shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d, deArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode44 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point aEnd = new Point() { X = shapeCode.A - bendOffset - shapeCode.Diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * shapeCode.Diameter + 2 * shapeCode.BendRadius };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * shapeCode.Diameter - 2 * shapeCode.BendRadius };
            Point cdCentre = cEnd + new Vector() { Y = bendOffset };
            Point dStart = cdCentre + new Vector() { X = bendOffset };
            Point dEnd = dStart + new Vector() { Y = shapeCode.D - 2 * shapeCode.Diameter - 2 * shapeCode.BendRadius };
            Point deCentre = dEnd + new Vector() { X = bendOffset };
            Point eStart = deCentre + new Vector() { Y = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dEnd };
            Arc deArc = Engine.Geometry.Create.ArcByCentre(deCentre, dEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart + new Vector { X = shapeCode.E - bendOffset - shapeCode.Diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d, deArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode46 shapeCode)
        {
            double d = shapeCode.D - shapeCode.Diameter;  //Height centreline
            double alpha = Math.Atan(shapeCode.F / d);  //Angle of the upper corner of the triangle F-d-B 
            double beta = alpha / 2 + Math.PI / 4;      //Angle of the bisector between A and B
            double arcAngle = Math.PI / 2 - alpha;      //Angle of the arc

            double r = shapeCode.BendRadius + shapeCode.Diameter / 2;   //Centreline bend radius
            double x = r / Math.Tan(beta);                              //Distance from AB corner with 0 radius to end of A
            double s = Math.Sqrt(r * r + x * x);                        //Distance from arc centre to AB corner
            double t = s - r;                                           //Hypotenous of triangle from AB corner to arc
            double addRed = x / s * t;                                  //Reduction in the leng reduction. This is X-distance from arc centre to AB using the fact that triangles are of the same shape

            double lengthReduction = x - addRed;
            //Corner AB
            Point aEnd = new Point() { X = shapeCode.A - lengthReduction };
            Point abCentre = aEnd + new Vector() { Y = -r };
            Point bLeftStart = aEnd.Rotate(abCentre, Vector.ZAxis, -arcAngle);

            //Corner BC left
            Point cStart = new Point { X = shapeCode.A + shapeCode.F + lengthReduction, Y = -d };
            Point bcCentre = new Point { X = cStart.X, Y = cStart.Y + r };
            Point bLeftEnd = cStart.Rotate(bcCentre, Vector.ZAxis, -arcAngle);

            //Corner BC right
            Point cEnd = new Point { X = cStart.X + shapeCode.C - 2 * lengthReduction, Y = cStart.Y };
            Point cbCentre = new Point { X = cEnd.X, Y = cEnd.Y + r };
            Point bRightStart = cEnd.Rotate(cbCentre, Vector.ZAxis, arcAngle);

            //Corner BE
            Point eStart = new Point { X = shapeCode.A + shapeCode.C + shapeCode.F * 2 + lengthReduction };
            Point deCentre = new Point { X = eStart.X, Y = eStart.Y - r };
            Point bRightEnd = eStart.Rotate(deCentre, Vector.ZAxis, arcAngle);
            Point eEnd = new Point { X = shapeCode.A + shapeCode.C + shapeCode.F * 2 + shapeCode.E };


            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bLeftStart);
            Line bLeft = new Line { Start = bLeftStart, End = bLeftEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bLeftEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bRightStart);
            Line bRight = new Line { Start = bRightStart, End = bRightEnd };
            Arc deArc = Engine.Geometry.Create.ArcByCentre(deCentre, bRightEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart + new Vector { X = shapeCode.E - lengthReduction } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, bLeft, bcArc, c, cdArc, bRight, deArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode47 shapeCode)
        {
            double hookDiameter = shapeCode.HookDiameter();
            double hookOffset = hookDiameter / 2 - shapeCode.Diameter / 2;
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point cStart = new Point()
            {
                X = -shapeCode.B / 2 + hookDiameter - shapeCode.Diameter / 2,
                Y = shapeCode.A / 2 - shapeCode.C
            };

            Point cEnd = cStart + new Vector() { Y = shapeCode.C - hookDiameter / 2 };
            Point cCentre = cEnd + new Vector() { X = -hookOffset };
            Point aLeftStart = cCentre + new Vector() { X = -hookOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + hookDiameter / 2 + bendOffset + shapeCode.Diameter/2 };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bStart = abCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point baCentre = bEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baCentre + new Vector() { X = bendOffset };
            Point aRightEnd = aRightStart + new Vector() { Y = shapeCode.A - hookDiameter / 2  - bendOffset - shapeCode.Diameter / 2 };
            Point dCentre = aRightEnd + new Vector() { X = -hookOffset };
            Point dStart = dCentre + new Vector() { X = -hookOffset };
            Point dEnd = dStart + new Vector() { Y = -shapeCode.D + hookDiameter / 2 };

            Line c = new Line() { Start = cStart, End = cEnd };
            Circle cCircle = new Circle() { Centre = cCentre, Normal = Vector.ZAxis, Radius = hookOffset };
            ICurve cHook = cCircle.SplitAtPoints(new List<Point>() { cEnd, aLeftStart })[1];
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc baArc = Engine.Geometry.Create.ArcByCentre(baCentre, bEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = aRightEnd };
            Circle dCircle = new Circle() { Centre = dCentre, Normal = Vector.ZAxis, Radius = hookOffset };
            ICurve dHook = dCircle.SplitAtPoints(new List<Point>() { aRightEnd, dStart })[1];
            Line d = new Line() { Start = dStart, End = dEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { c, cHook, aLeft, abArc, b, baArc, aRight, dHook, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode48 shapeCode)
        {
            double hookDiameter = shapeCode.HookDiameter();
            double hookOffset = hookDiameter / 2 - shapeCode.Diameter / 2;
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point cStart = new Point()
            {
                X = -shapeCode.B / 2 + (shapeCode.C - shapeCode.Diameter / 2) * Math.Cos(Math.PI / 4),
                Y = shapeCode.A / 2 - Math.Sin(Math.PI / 4) * shapeCode.C
            };

            Line c = new Line() { Start = cStart, End = cStart + new Vector() { X = -shapeCode.C + hookDiameter / 2 } }.Rotate(cStart, Vector.ZAxis, -Math.PI / 4);
            Point cEnd = c.End;
            Line cRadius = new Line() { Start = cEnd, End = cEnd + new Vector() { X = -hookOffset } }.Rotate(cEnd, Vector.ZAxis, Math.PI / 4);
            Point cCentre = cRadius.End;
            Point aLeftStart = cCentre + new Vector() { X = -hookOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + hookDiameter / 2 + bendOffset + shapeCode.Diameter/2 };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bStart = abCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point baCentre = bEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baCentre + new Vector() { X = bendOffset };
            Point aRightEnd = aRightStart + new Vector() { Y = shapeCode.A - hookDiameter / 2 - bendOffset - shapeCode.Diameter/2 };
            Point dCentre = aRightEnd + new Vector() { X = -hookOffset };
            Line dRadius = new Line() { Start = dCentre, End = dCentre + new Vector() { X = -hookOffset } }.Rotate(dCentre, Vector.ZAxis, -Math.PI / 4);
            Point dStart = dRadius.End;
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { X = -shapeCode.D + hookDiameter/2 } }.Rotate(dStart, Vector.ZAxis, Math.PI / 4);

            Arc cHook = Engine.Geometry.Create.ArcByCentre(cCentre, cEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc baArc = Engine.Geometry.Create.ArcByCentre(baCentre, bEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = aRightEnd };
            Arc dHook = Engine.Geometry.Create.ArcByCentre(dCentre, aRightEnd, dStart);

            return new PolyCurve() { Curves = new List<ICurve>() { c, cHook, aLeft, abArc, b, baArc, aRight, dHook, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode51 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point cStart = new Point() { X = shapeCode.B / 2 - shapeCode.Diameter / 2, Y = shapeCode.A / 2 - shapeCode.C };
            Point cEnd = cStart + new Vector() { Y = shapeCode.C - shapeCode.BendRadius - shapeCode.Diameter };
            Point cbCentre = cEnd + new Vector() { X = -bendOffset };
            Point bTopStart = cbCentre + new Vector() { Y = bendOffset };
            Point bTopEnd = bTopStart + new Vector() { X = -shapeCode.B + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point baLeftCentre = bTopEnd + new Vector() { Y = -bendOffset };
            Point aLeftStart = baLeftCentre + new Vector() { X = -bendOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bBotStart = abCentre + new Vector() { Y = -bendOffset };
            Point bBotEnd = bBotStart + new Vector() { X = shapeCode.B - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter, Z = -shapeCode.Diameter };
            Point baRightCentre = bBotEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baRightCentre + new Vector() { X = bendOffset };
            Point dEnd = bTopStart + new Vector() { X = -shapeCode.D + shapeCode.BendRadius + shapeCode.Diameter, Z = -shapeCode.Diameter };

            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bTopStart);
            Line bTop = new Line() { Start = bTopStart, End = bTopEnd };
            Arc baLeft = Engine.Geometry.Create.ArcByCentre(baLeftCentre, bTopEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bBotStart);
            Line bBot = new Line() { Start = bBotStart, End = bBotEnd };
            Arc baRight = Engine.Geometry.Create.ArcByCentre(baRightCentre, bBotEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = cEnd + new Vector() { Z = -shapeCode.Diameter } };
            Line d = new Line() { Start = bTopStart + new Vector() { Z = -shapeCode.Diameter }, End = dEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { c, cb, bTop, baLeft, aLeft, ab, bBot, baRight, aRight, cb.Translate(new Vector() { Z = -shapeCode.Diameter }), d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode52 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point cStart = new Point()
            {
                X = shapeCode.B / 2 - (shapeCode.C - shapeCode.Diameter / 2) * Math.Cos(Math.PI / 4),
                Y = shapeCode.A / 2 - shapeCode.C * Math.Sin(Math.PI / 4)
            };

            Line c = new Line() { Start = cStart, End = cStart + new Vector() { X = shapeCode.C - shapeCode.BendRadius - shapeCode.Diameter } }.Rotate(cStart, Vector.ZAxis, Math.PI / 4);
            Point cEnd = c.End;
            Line cbRadius = new Line() { Start = cEnd, End = cEnd + new Vector() { X = -bendOffset } }.Rotate(cEnd, Vector.ZAxis, -Math.PI / 4);
            Point cbCentre = cbRadius.End;
            Point bTopStart = cbCentre + new Vector() { Y = bendOffset };
            Point bTopEnd = bTopStart + new Vector() { X = -shapeCode.B + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point baLeftCentre = bTopEnd + new Vector() { Y = -bendOffset };
            Point aLeftStart = baLeftCentre + new Vector() { X = -bendOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bBotStart = abCentre + new Vector() { Y = -bendOffset };
            Point bBotEnd = bBotStart + new Vector() { X = shapeCode.B - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter, Z = -shapeCode.Diameter };
            Point baRightCentre = bBotEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baRightCentre + new Vector() { X = bendOffset };
            Point aRightEnd = aRightStart + new Vector() { Y = shapeCode.A - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point dStart = bTopStart.Translate(new Vector() { Z = -shapeCode.Diameter }).Rotate(cbCentre, Vector.ZAxis, Math.PI / 4);
            Point dEnd = dStart + new Vector() { X = -shapeCode.D + shapeCode.BendRadius + shapeCode.Diameter, Z = -shapeCode.Diameter };

            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bTopStart);
            Line bTop = new Line() { Start = bTopStart, End = bTopEnd };
            Arc baLeft = Engine.Geometry.Create.ArcByCentre(baLeftCentre, bTopEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bBotStart);
            Line bBot = new Line() { Start = bBotStart, End = bBotEnd };
            Arc baRight = Engine.Geometry.Create.ArcByCentre(baRightCentre, bBotEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = aRightEnd };
            Arc ad = Engine.Geometry.Create.ArcByCentre(cbCentre + new Vector() { Z = -shapeCode.Diameter }, aRightEnd, dStart);
            Line d = new Line() { Start = dStart, End = dEnd }.Rotate(dStart, Vector.ZAxis, Math.PI / 4);

            return new PolyCurve() { Curves = new List<ICurve>() { c, cb, bTop, baLeft, aLeft, ab, bBot, baRight, aRight, ad, d } };
        }

        /***************************************************/

        [NotImplemented]
        private static ICurve Centreline(this ShapeCode56 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;
            double angle = Math.Acos((shapeCode.A - shapeCode.C) / (shapeCode.B - shapeCode.Diameter - bendOffset - shapeCode.Diameter));
            double lengthReduction = (shapeCode.BendRadius + shapeCode.Diameter) * Math.Sin(angle)/2;

            Point fStart = new Point();
            Point fEnd = fStart + new Vector() { Y = -shapeCode.F + bendOffset + shapeCode.Diameter/2 };
            Point faCentre = fEnd + new Vector() { X = bendOffset };
            Point aStart = faCentre + new Vector() { Y = -bendOffset };
            Point aEnd = aStart + new Vector() { X = shapeCode.A - 2 * shapeCode.Diameter - 2 * shapeCode.BendRadius };
            Point adCentre = aEnd + new Vector() { Y = bendOffset };

            Point eEnd = fStart + new Vector() { X =  shapeCode.E - shapeCode.Diameter / 2, Y = -shapeCode.F + shapeCode.Diameter/2, Z = -shapeCode.Diameter };
            Point eStart = eEnd + new Vector() { X = -shapeCode.E + bendOffset + shapeCode.Diameter/2};
            Point bStart = fEnd + new Vector() { Y = shapeCode.B - 2*bendOffset - shapeCode.Diameter, Z = -shapeCode.Diameter };
            Point cbCentre = bStart + new Vector() { X = bendOffset };
            Point cEnd = cbCentre + new Vector() { Y = bendOffset };
            Point cStart = cEnd + new Vector() {X = shapeCode.C - shapeCode.BendRadius - shapeCode.Diameter - lengthReduction, Z= shapeCode.Diameter};
            Point dcCentre = cStart + new Vector() { Y = -bendOffset };

            Circle adCircle = new Circle() { Centre = adCentre, Normal = Vector.ZAxis, Radius = bendOffset };
            Circle dcCircle = new Circle() { Centre = dcCentre, Normal = Vector.ZAxis, Radius = bendOffset };

            Point dStart = adCircle.ClosestPoint(dcCentre).Rotate(adCentre, Vector.ZAxis, -Math.PI / 2);
            Point dEnd = dcCircle.ClosestPoint(adCentre).Rotate(dcCentre, Vector.ZAxis, Math.PI / 2);

            Line f = new Line() { Start = fStart, End = fEnd };
            Arc fa = Engine.Geometry.Create.ArcByCentre(faCentre, fEnd, aStart);
            Line a = new Line() { Start = aStart, End = aEnd };
            Arc ad = Engine.Geometry.Create.ArcByCentre(adCentre, aEnd, dStart);
            Line d = new Line() { Start = dStart, End = dEnd };
            Arc dc = Engine.Geometry.Create.ArcByCentre(dcCentre, dEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bStart);
            Line b = new Line() { Start = bStart, End = fEnd + new Vector() { Z = -shapeCode.Diameter } };
            Line e = new Line() { Start = eStart, End = eEnd};

            return new PolyCurve() { Curves = new List<ICurve>() { f, fa, a, ad, d, dc, c, cb, b, fa.Translate(new Vector() { Z = -shapeCode.Diameter }), e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode63 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point dStart = new Point() { X = shapeCode.B / 2 - shapeCode.Diameter / 2, Y = shapeCode.A / 2 - shapeCode.D };
            Point dEnd = dStart + new Vector() { Y = shapeCode.D - shapeCode.BendRadius - shapeCode.Diameter };
            Point dbCentre = dEnd + new Vector() { X = -bendOffset };
            Point bTopStart = dbCentre + new Vector() { Y = bendOffset };
            Point bTopEnd = bTopStart + new Vector() { X = -shapeCode.B + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point baLeftCentre = bTopEnd + new Vector() { Y = -bendOffset };
            Point aLeftStart = baLeftCentre + new Vector() { X = -bendOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bBotStart = abCentre + new Vector() { Y = -bendOffset };
            Point bBotEnd = bBotStart + new Vector() { X = shapeCode.B - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter, Z = -shapeCode.Diameter };
            Point baRightCentre = bBotEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baRightCentre + new Vector() { X = bendOffset };
            Point baRightEnd = dEnd + new Vector() { Z = -shapeCode.Diameter };
            Point cEnd = aLeftStart + new Vector() { Y = -shapeCode.C + shapeCode.BendRadius + shapeCode.Diameter, Z = -shapeCode.Diameter };

            Line d = new Line() { Start = dStart, End = dEnd };
            Arc db = Engine.Geometry.Create.ArcByCentre(dbCentre, dEnd, bTopStart);
            Line bTop = new Line() { Start = bTopStart, End = bTopEnd };
            Arc baLeft = Engine.Geometry.Create.ArcByCentre(baLeftCentre, bTopEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bBotStart);
            Line bBot = new Line() { Start = bBotStart, End = bBotEnd };
            Arc baRight = Engine.Geometry.Create.ArcByCentre(baRightCentre, bBotEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = baRightEnd };
            Line c = new Line() { Start = aLeftStart + new Vector() { Z = -shapeCode.Diameter }, End = cEnd };

            return new PolyCurve()
            {
                Curves = new List<ICurve>() { d, db, bTop, baLeft, aLeft, ab, bBot, baRight, aRight,
                db.Translate(new Vector() { Z = -shapeCode.Diameter }) , bTop.Translate(new Vector() { Z = -shapeCode.Diameter }),
                baLeft.Translate(new Vector(){Z = -shapeCode.Diameter }),c }
            };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode64 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point aEnd = new Point() { X = -shapeCode.A + shapeCode.Diameter + shapeCode.BendRadius };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = -bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * shapeCode.Diameter + 2 * shapeCode.BendRadius };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * shapeCode.BendRadius - 2 * shapeCode.Diameter };
            Point cdCentre = cEnd + new Vector() { Y = bendOffset };
            Point dRightStart = cdCentre + new Vector() { X = bendOffset };
            Point dRightEnd = dRightStart + new Vector() { Y = shapeCode.D - 2 * shapeCode.Diameter - 2 * shapeCode.BendRadius };
            Point deCentre = dRightEnd + new Vector() { X = -bendOffset };
            Point eStart = deCentre + new Vector() { Y = bendOffset };
            Point eEnd = eStart + new Vector() { X = -shapeCode.E + 2 * shapeCode.Diameter + 2 * shapeCode.BendRadius, Z = -shapeCode.Diameter };
            Point edCentre = eEnd + new Vector() { Y = -bendOffset };
            Point dLeftStart = edCentre + new Vector() { X = -bendOffset };
            Point dLeftEnd = dLeftStart + new Vector() { Y = -shapeCode.D + 2 * shapeCode.BendRadius + 2 * shapeCode.Diameter };
            Point dfCentre = dLeftEnd + new Vector() { X = bendOffset };
            Point fStart = dfCentre + new Vector() { Y = -bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cd = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dRightStart);
            Line dRight = new Line() { Start = dRightStart, End = dRightEnd };
            Arc de = Engine.Geometry.Create.ArcByCentre(deCentre, dRightEnd, eStart);
            Line e = new Line() { Start = eStart, End = eEnd };
            Arc ed = Engine.Geometry.Create.ArcByCentre(edCentre, eEnd, dLeftStart);
            Line dLeft = new Line() { Start = dLeftStart, End = dLeftEnd };
            Arc df = Engine.Geometry.Create.ArcByCentre(dfCentre, dLeftEnd, fStart);
            Line f = new Line() { Start = fStart, End = fStart + new Vector() { X = shapeCode.F - shapeCode.Diameter - shapeCode.BendRadius } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, ab, b, bc, c, cd, dRight, de, e, ed, dLeft, df, f } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode67 shapeCode)
        {
            Point aStart = new Point() { X = -shapeCode.B / 2 - shapeCode.Diameter / 2 * Math.Sin(Math.PI / 4), Y = shapeCode.Diameter / 2 * Math.Sin(Math.PI / 4) };
            Point aEnd = aStart + new Vector() { X = shapeCode.B + shapeCode.Diameter * Math.Sin(Math.PI / 4) };
            Point aCentre = new Point() { Y = shapeCode.C - shapeCode.R };

            return Engine.Geometry.Create.ArcByCentre(aCentre, aStart, aEnd);
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode75 shapeCode)
        {
            //Work out angle on outer circle for B start point as a proportion of the circumference
            double bAngle = shapeCode.B / shapeCode.A;
            double radius = shapeCode.A / 2 - shapeCode.Diameter / 2;

            Point origin = new Point();
            Point bStart = new Point() { X = radius * Math.Sin(bAngle), Y = -radius * Math.Cos(bAngle) };
            Point bEnd = bStart.Mirror(Plane.YZ);
            List<ICurve> splitCircle = new Circle() { Centre = origin, Normal = Vector.ZAxis, Radius = radius }.SplitAtPoints(new List<Point>() { bStart, bEnd });
            ICurve lap = splitCircle[0];
            ICurve arc = splitCircle[1];

            Polyline polyline = VerticalOffsetCurve(arc, 100, -shapeCode.Diameter);

            return new PolyCurve() { Curves = new List<ICurve>() { lap, polyline, lap.ITranslate(new Vector() { Z = -shapeCode.Diameter }) } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode77 shapeCode)
        {
            Point origin = new Point();
            double radius = shapeCode.A / 2 - shapeCode.Diameter / 2;
            double partialTurn = shapeCode.C - Math.Floor(shapeCode.C);
            double lapAngle = Math.PI*partialTurn;

            Point barStart = new Point() { Y = radius };

            List<ICurve> curves = new List<ICurve>();
            ICurve topLap = null;
            ICurve bottomLap = null;


            if (partialTurn != 0)
            {
                Point lapStart = barStart.Rotate(origin, Vector.ZAxis, lapAngle);
                Point lapEnd = barStart.Rotate(origin, Vector.ZAxis, -lapAngle);
                List<ICurve> laps = new Circle() { Centre = origin, Normal = Vector.ZAxis, Radius = radius }.SplitAtPoints(new List<Point>() { lapStart, barStart, lapEnd });
                topLap = laps[0].IFlip().VerticalOffsetCurve(25, -shapeCode.B * partialTurn / 2);
                bottomLap = laps[2].IFlip().ITranslate(new Vector() { Z = -shapeCode.B * (shapeCode.C - partialTurn / 2) }).VerticalOffsetCurve(25, -shapeCode.B * partialTurn / 2);
                curves.Add(topLap);
            }

            Point barOrigin = origin + new Vector() { Z = -shapeCode.B *partialTurn/2 };
            for (int i = 0; i < shapeCode.C - partialTurn; i++)
            {
                ICurve circle = new Circle() { Centre = barOrigin, Normal = Vector.ZAxis, Radius = radius }.SplitAtPoints(new List<Point>() {barOrigin + new Vector() {Y = radius } })[0].IFlip();
                Polyline polyline = circle.VerticalOffsetCurve(100, -shapeCode.B);
                curves.Add(polyline);
                barOrigin = barOrigin.Translate(new Vector() { Z = -shapeCode.B });
            }

            if (partialTurn != 0)
            {
                curves.Add(bottomLap);
            }

            return new PolyCurve() { Curves = curves };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode98 shapeCode)
        {
            double bendOffset = shapeCode.BendRadius + shapeCode.Diameter / 2;

            Point cEnd = new Point() { Z = shapeCode.C - shapeCode.Diameter - shapeCode.BendRadius };
            Point cbCentre = cEnd + new Vector() { Y = bendOffset };
            Point bLeftStart = cbCentre + new Vector() { Z = bendOffset };
            Point bLeftEnd = bLeftStart + new Vector() { Y = shapeCode.B - 2 * shapeCode.Diameter - 2 * shapeCode.BendRadius };
            Point baCentre = bLeftEnd + new Vector() { X = bendOffset };
            Point aStart = baCentre + new Vector() { Y = bendOffset };
            Point aEnd = aStart + new Vector() { X = shapeCode.A - 2 * shapeCode.Diameter - 2 * shapeCode.BendRadius };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bRightStart = abCentre + new Vector() { X = bendOffset };
            Point bRightEnd = bRightStart + new Vector() { Y = -shapeCode.B + 2 * shapeCode.Diameter + 2 * shapeCode.BendRadius };
            Point bdCentre = bRightEnd + new Vector() { Z = bendOffset };
            Point dStart = bdCentre + new Vector() { Y = -bendOffset };

            Line c = new Line() { Start = new Point(), End = cEnd };
            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bLeftStart);
            Line bLeft = new Line() { Start = bLeftStart, End = bLeftEnd };
            Arc ba = Engine.Geometry.Create.ArcByCentre(baCentre, bLeftEnd, aStart);
            Line a = new Line() { Start = aStart, End = aEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bRightStart);
            Line bRight = new Line() { Start = bRightStart, End = bRightEnd };
            Arc bd = Engine.Geometry.Create.ArcByCentre(bdCentre, bRightEnd, dStart);
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { Z = shapeCode.D - shapeCode.Diameter - shapeCode.BendRadius } };

            return new PolyCurve() { Curves = new List<ICurve>() { c, cb, bLeft, ba, a, ab, bRight, bd, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode99 shapeCode)
        {
            return shapeCode.Curve;
        }

        /***************************************************/

        private static List<Point> CircleTangentialPoint(this Point centre, Point endPoint, double radius)
        {
            double b = centre.Distance(endPoint);

            double th = Math.Acos(radius / b);
            double d = Math.Atan2(endPoint.Y - centre.Y, endPoint.X - centre.X);
            double d1 = d + th;
            double d2 = d - th;

            Point p1 = new Point() { X = centre.X + radius * Math.Cos(d1), Y = centre.Y + radius * Math.Sin(d1) };
            Point p2 = new Point() { X = centre.X + radius * Math.Cos(d2), Y = centre.Y + radius * Math.Sin(d2) };

            return new List<Point>() { p1, p2 };
        }

        /***************************************************/

        private static Polyline VerticalOffsetCurve(this ICurve curve, int divisions, double offset)
        {
            var parameters = Enumerable.Range(0, divisions + 1).Select(x => (double)x / (divisions));

            List<Point> points = new List<Point>();

            foreach (double parameter in parameters)
            {
                points.Add(curve.IPointAtParameter(parameter) + new Vector() { Z = parameter * offset });
            }

            return new Polyline() { ControlPoints = points };
        }

        /***************************************************/
        /****    Private Fallback Method            ********/
        /***************************************************/

        private static ICurve Centreline(IShapeCode shapeCode)
        {
            Base.Compute.RecordError("ShapeCode not recognised or supported.");
            return null;
        }

        /***************************************************/


    }
}


