/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
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

        [Description("Computes the centreline for a Reinforcement using the ShapeCode provided according to Bs 8666:2020.")]
        [Input("reinforcement", "The reinforcement containing the ShapeCode, reinforcement and bending radius to generate the centreline.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this Reinforcement reinforcement)
        {
            return reinforcement.IsReinforcementValid() ? ICentreline(reinforcement.ShapeCode, reinforcement.Diameter, reinforcement.BendRadius).Orient(new Cartesian(), reinforcement.CoordinateSystem) : null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.", typeof(Length))]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.", typeof(Length))]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(this IShapeCode shapeCode, double diameter, double bendRadius = 0)
        {
            if (shapeCode.IsNull())
                return null;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return null;
            }

            return Centreline(shapeCode as dynamic, diameter, bendRadius);
        }

        /***************************************************/
        /****    Private Methods                    ********/
        /***************************************************/

        private static ICurve Centreline(this ShapeCode00 shapeCode, double diameter, double bendRadius = 0)
        {
            return new Line() { Start = new Point() { X = -shapeCode.A / 2 }, End = new Point() { X = shapeCode.A / 2 } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode11 shapeCode, double diameter, double bendRadius = 0)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point bEnd = new Point() { X = shapeCode.B - bendOffset - diameter / 2 };
            Point arcCentre = bEnd + new Vector() { Y = bendOffset };
            Point aStart = arcCentre + new Vector() { X = bendOffset };

            Line b = new Line() { Start = new Point(), End = bEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart + new Vector { Y = shapeCode.A - bendOffset - diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { b, arc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode12 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = shapeCode.R + diameter / 2;

            Point bEnd = new Point() { X = shapeCode.B - bendOffset - diameter / 2 };
            Point arcCentre = bEnd + new Vector() { Y = bendOffset };
            Point aStart = arcCentre + new Vector() { X = bendOffset };

            Line b = new Line() { Start = new Point(), End = bEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart + new Vector { Y = shapeCode.A - bendOffset - diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { b, arc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode13 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = shapeCode.B / 2 - diameter / 2;

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

        private static ICurve Centreline(this ShapeCode14 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.A);
            double bendOffset = bendRadius + diameter / 2;

            Point cEnd = new Point() { X = shapeCode.C - bendOffset - diameter / 2 };
            Point arcCentre = cEnd + new Vector() { Y = bendOffset };

            Line radius = new Line() { Start = arcCentre, End = cEnd }.Rotate(arcCentre, Vector.ZAxis, Math.PI - angle);
            Point aStart = radius.End;

            Line c = new Line() { Start = new Point(), End = cEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, cEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart + new Vector() { X = -shapeCode.A + bendOffset + diameter / 2 } }.Rotate(aStart, Vector.ZAxis, -angle);

            return new PolyCurve() { Curves = new List<ICurve>() { c, arc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode15 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Acos(shapeCode.D / shapeCode.A);
            double bendOffset = bendRadius + diameter / 2;
            double lengthReduction = ((bendRadius + diameter) * (angle))/2;

            Point cEnd = new Point() { X = -shapeCode.C + lengthReduction };
            Point arcCentre = cEnd + new Vector() { Y = bendOffset };

            Line radius = new Line() { Start = arcCentre, End = cEnd }.Rotate(arcCentre, Vector.ZAxis, -angle);
            Point aStart = radius.End;

            Line c = new Line() { Start = new Point(), End = cEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, cEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart + new Vector { X = -shapeCode.A + lengthReduction } }.Rotate(aStart, Vector.ZAxis, -angle);

            return new PolyCurve() { Curves = new List<ICurve>() { c, arc, a } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode21 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point aEnd = new Point() { Y = -shapeCode.A + bendOffset + diameter / 2 };
            Point abArcCentre = aEnd + new Vector() { X = bendOffset };
            Point bStart = abArcCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter };
            Point bcArcCentre = bEnd + new Vector() { Y = bendOffset };
            Point cStart = bcArcCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcArcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { Y = shapeCode.C - bendOffset - diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode22 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point aEnd = new Point() { Y = shapeCode.A - bendRadius - diameter };
            Point abArcCentre = aEnd + new Vector() { X = bendOffset };
            Point bStart = abArcCentre + new Vector() { Y = bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - bendRadius - diameter - shapeCode.C / 2 };
            Point bdArcCentre = bEnd + new Vector() { Y = -shapeCode.C / 2 + diameter / 2 };
            Point dStart = bdArcCentre + new Vector() { Y = -shapeCode.C / 2 + diameter / 2 };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Circle circle = new Circle() { Centre = bdArcCentre, Radius = shapeCode.C / 2 - diameter / 2, Normal = Vector.ZAxis };
            ICurve bdArc = circle.SplitAtPoints(new List<Point>() { bEnd, dStart })[1].IFlip();
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { X = -shapeCode.D + shapeCode.C / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bdArc, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode23 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point aEnd = new Point() { Y = -shapeCode.A + bendOffset + diameter / 2 };
            Point abCentre = aEnd + new Vector() { X = bendOffset };
            Point bStart = abCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter };
            Point bcCentre = bEnd + new Vector() { Y = -bendOffset };
            Point cStart = bcCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { Y = -shapeCode.C + bendOffset + diameter / 2 } };

            PolyCurve polyCurve = new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };

            return shapeCode.ZBar ? polyCurve.Rotate(new Point(), Vector.ZAxis, Math.PI / 2) : polyCurve;
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode24 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Asin(shapeCode.D / shapeCode.B);
            double lengthReductionBottom = ((bendRadius + diameter) * (angle)) / 2;
            double lengthReductionTop = ((bendRadius + diameter) * (Math.PI/2 - angle))/2;

            Point aEnd = new Point() { X = shapeCode.A - lengthReductionBottom };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart + new Vector() { X = shapeCode.B - lengthReductionBottom - lengthReductionTop } }.Rotate(bStart, Vector.ZAxis, angle) ;
            Line bcRadius = new Line() { Start = b.End, End = b.End + new Vector() { X = -bendOffset } }.Rotate(b.End, Vector.ZAxis, -Math.PI / 2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Circle circle = new Circle() { Centre = bcCentre, Radius = bendOffset, Normal = Vector.ZAxis };
            ICurve bcArc = circle.SplitAtPoints(new List<Point>() { b.End, cStart })[0];
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { Y = shapeCode.C - lengthReductionTop } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode25 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double aeAngle = Math.Acos(shapeCode.C / shapeCode.A);
            double ebAngle = Math.Asin(shapeCode.D / shapeCode.B);
            double lengthReductionLeft = ((bendRadius + diameter) * (Math.PI/2 - aeAngle)) / 2;
            double lengthReductionRight = ((bendRadius + diameter) * ebAngle) / 2;

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

        private static ICurve Centreline(this ShapeCode26 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Asin(shapeCode.D / shapeCode.B);
            double lengthReductionLeft = ((bendRadius + diameter) * angle) / 2;
            double lengthReductionRight = ((bendRadius + diameter) * angle) / 2;

            Point aEnd = new Point() { X = shapeCode.A - lengthReductionLeft };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart + new Vector() { X = shapeCode.B - lengthReductionLeft - lengthReductionRight } }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd + new Vector() { X = bendOffset } }.Rotate(b.End, Vector.ZAxis, -Math.PI / 2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre + new Vector() { Y = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcRadius.End, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { X = shapeCode.C - lengthReductionRight } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode27 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Asin(shapeCode.D / shapeCode.A);
            double lengthReduction = ((bendRadius + diameter) * angle) / 2;

            Line a = new Line() { Start = new Point(), End = new Point() { X = shapeCode.A - lengthReduction } }.Rotate(new Point(), Vector.ZAxis, angle);
            Point aEnd = a.End;
            Line abRadius = new Line() { Start = aEnd, End = aEnd + new Vector() { Y = -bendOffset } }.Rotate(aEnd, Vector.ZAxis, angle);
            Point bStart = abRadius.End + new Vector() { Y = bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - lengthReduction - bendOffset - diameter/2 };
            Line b = new Line() { Start = bStart, End = bEnd };
            Point bcCentre = bEnd + new Vector() { Y = -bendOffset };
            Point cStart = bcCentre + new Vector() { X = bendOffset };


            Arc abArc = Engine.Geometry.Create.ArcByCentre(abRadius.End, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { Y = -shapeCode.C + bendOffset + diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode28 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Asin(shapeCode.D / shapeCode.A);
            double lengthReduction = ((bendRadius + diameter) * angle) / 2;

            Line a = new Line() { Start = new Point(), End = new Point() { X = shapeCode.A - lengthReduction } }.Rotate(new Point(), Vector.ZAxis, angle);
            Point aEnd = a.End;
            Line abRadius = new Line() { Start = aEnd, End = aEnd + new Vector() { Y = -bendOffset } }.Rotate(aEnd, Vector.ZAxis, angle);
            Point bStart = abRadius.End + new Vector() { Y = bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - lengthReduction - bendOffset - diameter/2 };
            Line b = new Line() { Start = bStart, End = bEnd };
            Point bcCentre = bEnd + new Vector() { Y = bendOffset };
            Point cStart = bcCentre + new Vector() { X = bendOffset };


            Arc abArc = Engine.Geometry.Create.ArcByCentre(abRadius.End, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { Y = shapeCode.C - bendOffset - diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode29 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Asin(shapeCode.E / shapeCode.B);
            double lengthReduction = ((bendRadius + diameter) * (Math.PI/2 - angle)) / 2;

            Point aEnd = new Point() { X = shapeCode.A - bendOffset - diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, Math.PI/2 + angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart + new Vector() { Y = shapeCode.B - 2 * bendRadius - 2 * diameter } }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd + new Vector() { X = -bendOffset } }.Rotate(bEnd, Vector.ZAxis,angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre + new Vector() { Y = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart + new Vector() { X = -shapeCode.C + lengthReduction } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode31 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point aEnd = new Point() { X = -shapeCode.A + bendOffset + diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = -bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * bendRadius + 2 * diameter };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * bendRadius - 2 * diameter };
            Point cdCentre = cEnd + new Vector() { Y = bendOffset };
            Point dStart = cdCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { Y = shapeCode.D - bendOffset - diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode32 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point aEnd = new Point() { X = -shapeCode.A + bendOffset + diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = -bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * bendRadius + 2 * diameter };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * bendRadius - 2 * diameter };
            Point cdCentre = cEnd + new Vector() { Y = -bendOffset };
            Point dStart = cdCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { Y = -shapeCode.D + bendOffset + diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode33 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = shapeCode.B / 2 - diameter / 2;

            Point cBotStart = new Point() { X = shapeCode.A / 2 - shapeCode.C, Y = bendOffset };
            Point cBotEnd = cBotStart + new Vector() { X = shapeCode.C - shapeCode.B/2 };
            Point rightCentre = cBotEnd + new Vector() { Y = bendOffset };
            Point aTopStart = rightCentre + new Vector() { Y = bendOffset };
            Point aTopEnd = aTopStart + new Vector() { X = -shapeCode.A + shapeCode.B, Z = -diameter };
            Point leftCentre = aTopEnd + new Vector() { Y = -bendOffset};
            Point aBotStart = leftCentre + new Vector() { Y = -bendOffset };
            Point cTopEnd = aTopStart + new Vector() { X = -shapeCode.C + shapeCode.B/2, Z = -diameter };

            Line cBot = new Line() { Start = cBotStart, End = cBotEnd };
            Circle rightCircle = new Circle() { Centre = rightCentre, Radius = bendOffset, Normal = Vector.ZAxis };
            ICurve rightArc = rightCircle.SplitAtPoints(new List<Point>() { cBotEnd, aTopStart })[1];
            Line aTop = new Line() { Start = aTopStart, End = aTopEnd };
            Circle leftCircle = new Circle() { Centre = leftCentre, Radius = bendOffset };
            ICurve leftArc = leftCircle.SplitAtPoints(new List<Point>() { aTopEnd, aBotStart })[0];
            Line aBot = new Line() { Start = aBotStart, End = cBotEnd + new Vector() { Z = -diameter } };
            Line cTop = new Line() { Start = aTopStart + new Vector() { Z = -diameter }, End = cTopEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { cBot, rightArc, aTop, leftArc, aBot, rightArc.ITranslate(new Vector() { Z = -diameter }), cTop } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode34 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Asin(shapeCode.D / shapeCode.B);
            double lengthReductionLeft = ((bendRadius + diameter) * angle) / 2;
            double lengthReductionRight = ((bendRadius + diameter) * angle) / 2;

            Point aEnd = new Point() { X = shapeCode.A - lengthReductionLeft };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart + new Vector() { X = shapeCode.B - lengthReductionLeft - lengthReductionRight } }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd + new Vector() { X = bendOffset } }.Rotate(b.End, Vector.ZAxis, -Math.PI/2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre + new Vector() { Y = bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - lengthReductionRight - bendOffset - diameter/2 };
            Point ceCentre = cEnd + new Vector() { Y = -bendOffset };
            Point eStart = ceCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc ceArc = Engine.Geometry.Create.ArcByCentre(ceCentre, cEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart + new Vector() { Y = -shapeCode.E + bendOffset + diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, ceArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode35 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Asin(shapeCode.D / shapeCode.B);
            double lengthReductionLeft = ((bendRadius + diameter) * angle) / 2;
            double lengthReductionRight = ((bendRadius + diameter) * angle) / 2;

            Point aEnd = new Point() { X = shapeCode.A - lengthReductionLeft };
            Point abCentre = aEnd + new Vector() { Y = bendOffset };
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis,  angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart + new Vector() { X = shapeCode.B - lengthReductionLeft - lengthReductionLeft } }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd + new Vector() { X = bendOffset } }.Rotate(b.End, Vector.ZAxis, -Math.PI / 2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre + new Vector() { Y = bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - lengthReductionRight - bendOffset - diameter/2 };
            Point ceCentre = cEnd + new Vector() { Y = bendOffset };
            Point eStart = ceCentre + new Vector() { X = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc ceArc = Engine.Geometry.Create.ArcByCentre(ceCentre, cEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart + new Vector() { Y = shapeCode.E - bendOffset - diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, ceArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode36 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Asin(shapeCode.E / shapeCode.A);
            double lengthReduction = ((bendRadius + diameter) * angle) / 2;

            Point dEnd = new Point() { X = -shapeCode.D + bendOffset + diameter / 2 };
            Point dcCentre = dEnd + new Vector() { Y = -bendOffset };
            Point cStart = dcCentre + new Vector() { X = -bendOffset };
            Point cEnd = cStart + new Vector() { Y = -shapeCode.C + 2 * bendRadius + 2 * diameter };
            Point cbCentre = cEnd + new Vector() { X = bendOffset };
            Point bStart = cbCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - bendOffset - diameter/2 - lengthReduction };
            Point baCentre = bEnd + new Vector() { Y = bendOffset };
            Line baRadius = new Line() { Start = baCentre, End = bEnd }.Rotate(baCentre, Vector.ZAxis, angle);
            Point aStart = baRadius.End;

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

        private static ICurve Centreline(this ShapeCode41 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point aEnd = new Point() { X = -shapeCode.A + bendOffset + diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = -bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * bendRadius + 2 * diameter };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * bendRadius - 2 * diameter };
            Point cdCentre = cEnd + new Vector() { Y = bendOffset };
            Point dStart = cdCentre + new Vector() { X = bendOffset };
            Point dEnd = dStart + new Vector() { Y = shapeCode.D - 2 * bendRadius - 2 * diameter };
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
            Line e = new Line() { Start = eStart, End = eStart + new Vector { X = -shapeCode.E + bendOffset + diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d, deArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode44 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point aEnd = new Point() { X = shapeCode.A - bendOffset - diameter / 2 };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * diameter + 2 * bendRadius };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * diameter - 2 * bendRadius };
            Point cdCentre = cEnd + new Vector() { Y = bendOffset };
            Point dStart = cdCentre + new Vector() { X = bendOffset };
            Point dEnd = dStart + new Vector() { Y = shapeCode.D - 2 * diameter - 2 * bendRadius };
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
            Line e = new Line() { Start = eStart, End = eStart + new Vector { X = shapeCode.E - bendOffset - diameter / 2 } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d, deArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode46 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Acos(shapeCode.F / shapeCode.B);
            double lengthReduction = ((bendRadius + diameter) * angle) / 2;

            Point aEnd = new Point() { X = shapeCode.A - lengthReduction};
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, -angle);
            Point bLeftStart = abRadius.End;
            Line bLeft = new Line() { Start = bLeftStart, End = bLeftStart + new Vector() { X = shapeCode.B - 2*lengthReduction } }.Rotate(bLeftStart, Vector.ZAxis, -angle);
            Point bLeftEnd = bLeft.End;
            Line bcRadius = new Line() { Start = bLeftEnd, End = bLeftEnd + new Vector() { X = bendOffset } }.Rotate(bLeftEnd, Vector.ZAxis, Math.PI/2 - angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * lengthReduction };
            Point cbCentre = cEnd + new Vector() { Y = bendOffset };
            Line cbRadius = new Line() { Start = cbCentre, End = cEnd }.Rotate(cbCentre, Vector.ZAxis, angle);
            Point bRightStart = cbRadius.End;
            Line bRight = new Line() { Start = bRightStart, End = bRightStart + new Vector() { X = shapeCode.B - lengthReduction } }.Rotate(bRightStart, Vector.ZAxis, angle);
            Point bRightEnd = bRight.End;
            Line deRadius = new Line() { Start = bRightEnd, End = bRightEnd + new Vector() { X = bendOffset } }.Rotate(bRightEnd, Vector.ZAxis, -Math.PI / 2 + angle);
            Point deCentre = deRadius.End;
            Point eStart = deCentre + new Vector() { Y = bendOffset };

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bLeftStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bLeftEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bRightStart);
            Arc deArc = Engine.Geometry.Create.ArcByCentre(deCentre, bRightEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart + new Vector { X = shapeCode.E - lengthReduction } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, bLeft, bcArc, c, cdArc, bRight, deArc, e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode47 shapeCode, double diameter, double bendRadius)
        {
            double hookDiameter = shapeCode.HookDiameter(diameter, bendRadius);
            double hookOffset = hookDiameter / 2 - diameter / 2;
            double bendOffset = bendRadius + diameter / 2;

            Point cStart = new Point()
            {
                X = -shapeCode.B / 2 + hookDiameter - diameter / 2,
                Y = shapeCode.A / 2 - shapeCode.C
            };

            Point cEnd = cStart + new Vector() { Y = shapeCode.C - hookDiameter/2 };
            Point cCentre = cEnd + new Vector() { X = -hookOffset };
            Point aLeftStart = cCentre + new Vector() { X = -hookOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + hookDiameter/2 + bendOffset + diameter/2 };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bStart = abCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter };
            Point baCentre = bEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baCentre + new Vector() { X = bendOffset };
            Point aRightEnd = aRightStart + new Vector() { Y = shapeCode.A - hookDiameter/2  - bendOffset - diameter / 2 };
            Point dCentre = aRightEnd + new Vector() { X = -hookOffset };
            Point dStart = dCentre + new Vector() { X = -hookOffset };
            Point dEnd = dStart + new Vector() { Y = -shapeCode.D + hookDiameter/2 };

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

        private static ICurve Centreline(this ShapeCode48 shapeCode, double diameter, double bendRadius)
        {
            double hookDiameter = shapeCode.HookDiameter(diameter, bendRadius);
            double hookOffset = hookDiameter / 2 - diameter / 2;
            double bendOffset = bendRadius + diameter / 2;

            Point cStart = new Point()
            {
                X = -shapeCode.B / 2 + (shapeCode.C - diameter / 2) * Math.Cos(Math.PI / 4),
                Y = shapeCode.A / 2 - Math.Sin(Math.PI / 4) * shapeCode.C
            };

            Line c = new Line() { Start = cStart, End = cStart + new Vector() { X = -shapeCode.C + hookDiameter/2 } }.Rotate(cStart, Vector.ZAxis, -Math.PI / 4);
            Point cEnd = c.End;
            Line cRadius = new Line() { Start = cEnd, End = cEnd + new Vector() { X = -hookOffset } }.Rotate(cEnd, Vector.ZAxis, Math.PI / 4);
            Point cCentre = cRadius.End;
            Point aLeftStart = cCentre + new Vector() { X = -hookOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + hookDiameter/2 + bendOffset + diameter/2 };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bStart = abCentre + new Vector() { Y = -bendOffset };
            Point bEnd = bStart + new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter };
            Point baCentre = bEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baCentre + new Vector() { X = bendOffset };
            Point aRightEnd = aRightStart + new Vector() { Y = shapeCode.A - hookDiameter/2 - bendOffset - diameter/2 };
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

        private static ICurve Centreline(this ShapeCode51 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point cStart = new Point() { X = shapeCode.B / 2 - diameter / 2, Y = shapeCode.A / 2 - shapeCode.C };
            Point cEnd = cStart + new Vector() { Y = shapeCode.C - bendRadius - diameter };
            Point cbCentre = cEnd + new Vector() { X = -bendOffset };
            Point bTopStart = cbCentre + new Vector() { Y = bendOffset };
            Point bTopEnd = bTopStart + new Vector() { X = -shapeCode.B + 2 * bendRadius + 2 * diameter };
            Point baLeftCentre = bTopEnd + new Vector() { Y = -bendOffset };
            Point aLeftStart = baLeftCentre + new Vector() { X = -bendOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + 2 * bendRadius + 2 * diameter };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bBotStart = abCentre + new Vector() { Y = -bendOffset };
            Point bBotEnd = bBotStart + new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter, Z = -diameter };
            Point baRightCentre = bBotEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baRightCentre + new Vector() { X = bendOffset };
            Point dEnd = bTopStart + new Vector() { X = -shapeCode.D + bendRadius + diameter, Z = -diameter };

            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bTopStart);
            Line bTop = new Line() { Start = bTopStart, End = bTopEnd };
            Arc baLeft = Engine.Geometry.Create.ArcByCentre(baLeftCentre, bTopEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bBotStart);
            Line bBot = new Line() { Start = bBotStart, End = bBotEnd };
            Arc baRight = Engine.Geometry.Create.ArcByCentre(baRightCentre, bBotEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = cEnd + new Vector() { Z = -diameter } };
            Line d = new Line() { Start = bTopStart + new Vector() { Z = -diameter }, End = dEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { c, cb, bTop, baLeft, aLeft, ab, bBot, baRight, aRight, cb.Translate(new Vector() { Z = -diameter }), d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode52 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point cStart = new Point()
            {
                X = shapeCode.B / 2 - (shapeCode.C - diameter / 2) * Math.Cos(Math.PI / 4),
                Y = shapeCode.A / 2 - shapeCode.C * Math.Sin(Math.PI / 4)
            };

            Line c = new Line() { Start = cStart, End = cStart + new Vector() { X = shapeCode.C - bendRadius - diameter } }.Rotate(cStart, Vector.ZAxis, Math.PI / 4);
            Point cEnd = c.End;
            Line cbRadius = new Line() { Start = cEnd, End = cEnd + new Vector() { X = -bendOffset } }.Rotate(cEnd, Vector.ZAxis, -Math.PI / 4);
            Point cbCentre = cbRadius.End;
            Point bTopStart = cbCentre + new Vector() { Y = bendOffset };
            Point bTopEnd = bTopStart + new Vector() { X = -shapeCode.B + 2 * bendRadius + 2 * diameter };
            Point baLeftCentre = bTopEnd + new Vector() { Y = -bendOffset };
            Point aLeftStart = baLeftCentre + new Vector() { X = -bendOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + 2 * bendRadius + 2 * diameter };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bBotStart = abCentre + new Vector() { Y = -bendOffset };
            Point bBotEnd = bBotStart + new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter, Z = -diameter };
            Point baRightCentre = bBotEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baRightCentre + new Vector() { X = bendOffset };
            Point aRightEnd = aRightStart + new Vector() { Y = shapeCode.A - 2 * bendRadius - 2 * diameter };
            Point dStart = bTopStart.Translate(new Vector() { Z = -diameter }).Rotate(cbCentre, Vector.ZAxis, Math.PI / 4);
            Point dEnd = dStart + new Vector() { X = -shapeCode.D + bendRadius + diameter, Z = -diameter };

            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bTopStart);
            Line bTop = new Line() { Start = bTopStart, End = bTopEnd };
            Arc baLeft = Engine.Geometry.Create.ArcByCentre(baLeftCentre, bTopEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bBotStart);
            Line bBot = new Line() { Start = bBotStart, End = bBotEnd };
            Arc baRight = Engine.Geometry.Create.ArcByCentre(baRightCentre, bBotEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = aRightEnd };
            Arc ad = Engine.Geometry.Create.ArcByCentre(cbCentre + new Vector() { Z = -diameter }, aRightEnd, dStart);
            Line d = new Line() { Start = dStart, End = dEnd }.Rotate(dStart, Vector.ZAxis, Math.PI / 4);

            return new PolyCurve() { Curves = new List<ICurve>() { c, cb, bTop, baLeft, aLeft, ab, bBot, baRight, aRight, ad, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode56 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;
            double angle = Math.Acos((shapeCode.A - shapeCode.C) / shapeCode.D);
            double lengthReduction = (bendRadius + diameter) * Math.Sin(angle)/2l;

            Point fStart = new Point(); //{ X = -shapeCode.A / 2 + diameter / 2, Y = -shapeCode.B / 2 + shapeCode.F };
            Point fEnd = fStart + new Vector() { Y = -shapeCode.F + diameter + bendRadius };
            Point faCentre = fEnd + new Vector() { X = bendOffset };
            Point aStart = faCentre + new Vector() { Y = -bendOffset };
            Point aEnd = aStart + new Vector() { X = shapeCode.A - 2 * diameter - 2 * bendRadius };
            Point adCentre = aEnd + new Vector() { Y = bendOffset };
            Line adRadius = new Line() { Start = adCentre, End = adCentre + new Vector() { Y = -bendOffset } }.Rotate(adCentre, Vector.ZAxis, Math.PI - angle);
            Point dStart = adRadius.End;
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { X = -shapeCode.D + diameter + bendRadius + lengthReduction  } }.Rotate(dStart, Vector.ZAxis, -angle);
            Point dEnd = d.End;
            Line dcRadius = new Line() { Start = dEnd, End = dEnd + new Vector() { Y = -bendOffset } }.Rotate(dEnd, Vector.ZAxis, -angle);
            Point dcCentre = dcRadius.End;
            Point cStart = dcCentre + new Vector() { Y = bendOffset };
            Point cEnd = cStart + new Vector() { X = -shapeCode.C + bendRadius + diameter + lengthReduction, Z = -diameter };
            Point cbCenter = cEnd + new Vector() { Y = -bendOffset };
            Point bStart = cbCenter + new Vector() { X = -bendOffset };
            Point bEnd = fEnd + new Vector() { Z = -diameter };
            Point eStart = aStart + new Vector() { Z = -diameter };

            Line f = new Line() { Start = fStart, End = fEnd };
            Arc fa = Engine.Geometry.Create.ArcByCentre(faCentre, fEnd, aStart);
            Line a = new Line() { Start = aStart, End = aEnd };
            Arc ad = Engine.Geometry.Create.ArcByCentre(adCentre, aEnd, dStart);
            Arc dc = Engine.Geometry.Create.ArcByCentre(dcCentre, dEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCenter, cEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Line e = new Line() { Start = eStart, End = eStart + new Vector() { X = shapeCode.E - bendRadius - diameter } };

            return new PolyCurve() { Curves = new List<ICurve>() { f, fa, a, ad, d, dc, c, cb, b, fa.Translate(new Vector() { Z = -diameter }), e } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode63 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point dStart = new Point() { X = shapeCode.B / 2 - diameter / 2, Y = shapeCode.A / 2 - shapeCode.D };
            Point dEnd = dStart + new Vector() { Y = shapeCode.D - bendRadius - diameter };
            Point dbCentre = dEnd + new Vector() { X = -bendOffset };
            Point bTopStart = dbCentre + new Vector() { Y = bendOffset };
            Point bTopEnd = bTopStart + new Vector() { X = -shapeCode.B + 2 * bendRadius + 2 * diameter };
            Point baLeftCentre = bTopEnd + new Vector() { Y = -bendOffset };
            Point aLeftStart = baLeftCentre + new Vector() { X = -bendOffset };
            Point aLeftEnd = aLeftStart + new Vector() { Y = -shapeCode.A + 2 * bendRadius + 2 * diameter };
            Point abCentre = aLeftEnd + new Vector() { X = bendOffset };
            Point bBotStart = abCentre + new Vector() { Y = -bendOffset };
            Point bBotEnd = bBotStart + new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter, Z = -diameter };
            Point baRightCentre = bBotEnd + new Vector() { Y = bendOffset };
            Point aRightStart = baRightCentre + new Vector() { X = bendOffset };
            Point baRightEnd = dEnd + new Vector() { Z = -diameter };
            Point cEnd = aLeftStart + new Vector() { Y = -shapeCode.C + bendRadius + diameter, Z = -diameter };

            Line d = new Line() { Start = dStart, End = dEnd };
            Arc db = Engine.Geometry.Create.ArcByCentre(dbCentre, dEnd, bTopStart);
            Line bTop = new Line() { Start = bTopStart, End = bTopEnd };
            Arc baLeft = Engine.Geometry.Create.ArcByCentre(baLeftCentre, bTopEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bBotStart);
            Line bBot = new Line() { Start = bBotStart, End = bBotEnd };
            Arc baRight = Engine.Geometry.Create.ArcByCentre(baRightCentre, bBotEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = baRightEnd };
            Line c = new Line() { Start = aLeftStart + new Vector() { Z = -diameter }, End = cEnd };

            return new PolyCurve()
            {
                Curves = new List<ICurve>() { d, db, bTop, baLeft, aLeft, ab, bBot, baRight, aRight,
                db.Translate(new Vector() { Z = -diameter }) , bTop.Translate(new Vector() { Z = -diameter }),
                baLeft.Translate(new Vector(){Z = -diameter }),c }
            };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode64 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point aEnd = new Point() { X = -shapeCode.A + diameter + bendRadius };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bStart = abCentre + new Vector() { X = -bendOffset };
            Point bEnd = bStart + new Vector() { Y = -shapeCode.B + 2 * diameter + 2 * bendRadius };
            Point bcCentre = bEnd + new Vector() { X = bendOffset };
            Point cStart = bcCentre + new Vector() { Y = -bendOffset };
            Point cEnd = cStart + new Vector() { X = shapeCode.C - 2 * bendRadius - 2 * diameter };
            Point cdCentre = cEnd + new Vector() { Y = bendOffset };
            Point dRightStart = cdCentre + new Vector() { X = bendOffset };
            Point dRightEnd = dRightStart + new Vector() { Y = shapeCode.D - 2 * diameter - 2 * bendRadius };
            Point deCentre = dRightEnd + new Vector() { X = -bendOffset };
            Point eStart = deCentre + new Vector() { Y = bendOffset };
            Point eEnd = eStart + new Vector() { X = -shapeCode.E + 2 * diameter + 2 * bendRadius, Z = -diameter };
            Point edCentre = eEnd + new Vector() { Y = -bendOffset };
            Point dLeftStart = edCentre + new Vector() { X = -bendOffset };
            Point dLeftEnd = dLeftStart + new Vector() { Y = -shapeCode.D + 2 * bendRadius + 2 * diameter };
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
            Line f = new Line() { Start = fStart, End = fStart + new Vector() { X = shapeCode.F - diameter - bendRadius } };

            return new PolyCurve() { Curves = new List<ICurve>() { a, ab, b, bc, c, cd, dRight, de, e, ed, dLeft, df, f } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode67 shapeCode, double diameter, double bendRadius)
        {
            Point aStart = new Point() { X = -shapeCode.B / 2 - diameter / 2 * Math.Sin(Math.PI / 4), Y = diameter / 2 * Math.Sin(Math.PI / 4) };
            Point aEnd = aStart + new Vector() { X = shapeCode.B + diameter * Math.Sin(Math.PI / 4) };
            Point aCentre = new Point() { Y = shapeCode.C - shapeCode.R };

            return Engine.Geometry.Create.ArcByCentre(aCentre, aStart, aEnd);
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode75 shapeCode, double diameter, double bendRadius)
        {
            //Work out angle on outer circle for B start point as a proportion of the circumference
            double bAngle = shapeCode.B / shapeCode.A;
            double radius = shapeCode.A / 2 - diameter / 2;

            Point origin = new Point();
            Point bStart = new Point() { X = radius * Math.Sin(bAngle), Y = -radius * Math.Cos(bAngle) };
            Point bEnd = bStart.Mirror(Plane.YZ);
            List<ICurve> splitCircle = new Circle() { Centre = origin, Normal = Vector.ZAxis, Radius = radius }.SplitAtPoints(new List<Point>() { bStart, bEnd });
            ICurve lap = splitCircle[0];
            ICurve arc = splitCircle[1];

            Polyline polyline = VerticalOffsetCurve(arc, 100, -diameter);

            return new PolyCurve() { Curves = new List<ICurve>() { lap, polyline, lap.ITranslate(new Vector() { Z = -diameter }) } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode77 shapeCode, double diameter, double bendRadius)
        {
            Point origin = new Point();
            double radius = shapeCode.A / 2 - diameter / 2;
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

        private static ICurve Centreline(this ShapeCode98 shapeCode, double diameter, double bendRadius)
        {
            double bendOffset = bendRadius + diameter / 2;

            Point cEnd = new Point() { Z = shapeCode.C - diameter - bendRadius };
            Point cbCentre = cEnd + new Vector() { Y = bendOffset };
            Point bLeftStart = cbCentre + new Vector() { Z = bendOffset };
            Point bLeftEnd = bLeftStart + new Vector() { Y = shapeCode.B - 2 * diameter - 2 * bendRadius };
            Point baCentre = bLeftEnd + new Vector() { X = bendOffset };
            Point aStart = baCentre + new Vector() { Y = bendOffset };
            Point aEnd = aStart + new Vector() { X = shapeCode.A - 2 * diameter - 2 * bendRadius };
            Point abCentre = aEnd + new Vector() { Y = -bendOffset };
            Point bRightStart = abCentre + new Vector() { X = bendOffset };
            Point bRightEnd = bRightStart + new Vector() { Y = -shapeCode.B + 2 * diameter + 2 * bendRadius };
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
            Line d = new Line() { Start = dStart, End = dStart + new Vector() { Z = shapeCode.D - diameter - bendRadius } };

            return new PolyCurve() { Curves = new List<ICurve>() { c, cb, bLeft, ba, a, ab, bRight, bd, d } };
        }

        /***************************************************/

        private static ICurve Centreline(this ShapeCode99 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

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

        private static ICurve Centreline(IShapeCode shapeCode, double diameter, double bendRadius)
        {
            Reflection.Compute.RecordError("ShapeCode not recognised or supported.");
            return null;
        }

        /***************************************************/


    }
}

