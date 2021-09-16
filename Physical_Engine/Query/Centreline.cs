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
            if (reinforcement.IsNull())
                return null;

            return ICentreline(reinforcement.ShapeCode, reinforcement.Diameter, reinforcement.BendRadius).Orient(new Cartesian(), reinforcement.CoordinateSystem);
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
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
            else if (bendRadius < diameter.SchedulingRadius())
                bendRadius = diameter.SchedulingRadius();

            return Centreline(shapeCode as dynamic, diameter, bendRadius);
        }

        /***************************************************/
        /****    Private Methods                    ********/
        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode00 shapeCode, double diameter, double bendRadius = 0)
        {
            return new Line() { Start = new Point() { X = -shapeCode.A / 2 }, End = new Point() { X = shapeCode.A / 2 } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode11 shapeCode, double diameter, double bendRadius = 0)
        {
            Point bEnd = new Point() { X = shapeCode.B - bendRadius };
            Point arcCentre = bEnd.Translate(new Vector() { Y = bendRadius });
            Point aStart = arcCentre.Translate(new Vector() { X = bendRadius });
            Line b = new Line() { Start = new Point(), End = bEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector { Y = shapeCode.A - bendRadius }) };

            return new PolyCurve() { Curves = new List<ICurve>() { b, arc, a } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode12 shapeCode, double diameter, double bendRadius)
        {
            Point bEnd = new Point() { X = shapeCode.B - shapeCode.R };
            Point arcCentre = bEnd.Translate(new Vector() { Y = shapeCode.R });
            Point aStart = arcCentre.Translate(new Vector() { X = shapeCode.R });
            Line b = new Line() { Start = new Point(), End = bEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector { Y = shapeCode.A - shapeCode.R }) };

            return new PolyCurve() { Curves = new List<ICurve>() { b, arc, a } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode13 shapeCode, double diameter, double bendRadius)
        {
            Point aEnd = new Point() { X = shapeCode.A - shapeCode.B / 2 - diameter / 2 };
            Point arcCentre = aEnd.Translate(new Vector() { Y = shapeCode.B / 2 + diameter / 2 });
            Point cStart = aEnd.Translate(new Vector() { Y = shapeCode.B + diameter });
            Line a = new Line() { Start = new Point(), End = aEnd };
            Circle circle = new Circle() { Centre = arcCentre, Radius = shapeCode.B / 2 + diameter / 2, Normal = Vector.ZAxis };
            ICurve b = circle.SplitAtPoints(new List<Point>() { aEnd, cStart })[1];
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector { X = -shapeCode.C + shapeCode.B / 2 + diameter / 2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, b, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode14 shapeCode, double diameter, double bendRadius)
        {

            double angle = Math.Asin(shapeCode.D / shapeCode.A);

            Point cEnd = new Point() { X = shapeCode.C - bendRadius };
            Point arcCentre = cEnd.Translate(new Vector() { Y = bendRadius });

            Line radius = new Line() { Start = arcCentre, End = cEnd }.Rotate(arcCentre, Vector.ZAxis, Math.PI - angle);
            Point aStart = radius.End;

            Line c = new Line() { Start = new Point(), End = cEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, cEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector() { X = -shapeCode.A + bendRadius }) }.Rotate(aStart, Vector.ZAxis, -angle);

            return new PolyCurve() { Curves = new List<ICurve>() { c, arc, a } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode15 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.A);

            Point cEnd = new Point() { X = -shapeCode.C + bendRadius };
            Point arcCentre = cEnd.Translate(new Vector() { Y = bendRadius });

            Line radius = new Line() { Start = arcCentre, End = cEnd }.Rotate(arcCentre, Vector.ZAxis, -Math.PI / 2 + angle);
            Point aStart = radius.End;

            Line c = new Line() { Start = new Point(), End = cEnd };
            Arc arc = Engine.Geometry.Create.ArcByCentre(arcCentre, cEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector { X = -shapeCode.A + bendRadius }) }.Rotate(aStart, Vector.ZAxis, -angle);

            return new PolyCurve() { Curves = new List<ICurve>() { c, arc, a } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode21 shapeCode, double diameter, double bendRadius)
        {
            Point aEnd = new Point() { Y = -shapeCode.A + bendRadius };
            Point abArcCentre = aEnd.Translate(new Vector() { X = bendRadius });
            Point bStart = abArcCentre.Translate(new Vector() { Y = -bendRadius });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius });
            Point bcArcCentre = bEnd.Translate(new Vector() { Y = bendRadius });
            Point cStart = bcArcCentre.Translate(new Vector() { X = bendRadius });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcArcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { Y = shapeCode.C }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode22 shapeCode, double diameter, double bendRadius)
        {
            Point aEnd = new Point() { Y = shapeCode.A - bendRadius - diameter };
            Point abArcCentre = aEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point bStart = abArcCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - bendRadius - shapeCode.C / 2 - 2 * diameter });
            Point bdArcCentre = bEnd.Translate(new Vector() { Y = -shapeCode.C / 2 - diameter / 2 });
            Point dStart = bdArcCentre.Translate(new Vector() { Y = -shapeCode.C / 2 - diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Circle circle = new Circle() { Centre = bdArcCentre, Radius = shapeCode.C / 2 + diameter / 2, Normal = Vector.ZAxis };
            ICurve bdArc = circle.SplitAtPoints(new List<Point>() { bEnd, dStart })[1].IFlip();
            Line d = new Line() { Start = dStart, End = dStart.Translate(new Vector() { X = -shapeCode.D + shapeCode.C / 2 + diameter }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bdArc, d } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode23 shapeCode, double diameter, double bendRadius)
        {
            Point aEnd = new Point() { Y = -shapeCode.A + bendRadius };
            Point abCentre = aEnd.Translate(new Vector() { X = bendRadius });
            Point bStart = abCentre.Translate(new Vector() { Y = -bendRadius });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius });
            Point bcCentre = bEnd.Translate(new Vector() { Y = -bendRadius });
            Point cStart = bcCentre.Translate(new Vector() { X = bendRadius });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { Y = -shapeCode.C }) };

            PolyCurve polyCurve = new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };

            return shapeCode.ZBar ? polyCurve.Rotate(new Point(), Vector.ZAxis, Math.PI / 2) : polyCurve;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode24 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.B);

            Point aEnd = new Point() { X = shapeCode.A - bendRadius };
            Point abCentre = aEnd.Translate(new Vector() { Y = bendRadius });
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, Math.PI / 2 - angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector() { X = shapeCode.B }) }.Rotate(bStart, Vector.ZAxis, angle);
            Line bcRadius = new Line() { Start = b.End, End = b.End.Translate(new Vector() { X = -bendRadius - diameter / 2 }) }.Rotate(b.End, Vector.ZAxis, -Math.PI / 2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Circle circle = new Circle() { Centre = bcCentre, Radius = bendRadius + diameter / 2, Normal = Vector.ZAxis };
            ICurve bcArc = circle.SplitAtPoints(new List<Point>() { b.End, cStart })[0];


            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { Y = shapeCode.C }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode25 shapeCode, double diameter, double bendRadius)
        {
            double aeAngle = Math.Acos(shapeCode.C / shapeCode.A);
            double ebAngle = Math.Asin(shapeCode.D / shapeCode.B);

            Line a = new Line() { Start = new Point(), End = new Point() { Y = -shapeCode.A + bendRadius + diameter / 2 } }.Rotate(new Point(), Vector.ZAxis, aeAngle);
            Line aeRadius = new Line() { Start = a.End, End = a.End.Translate(new Vector() { X = bendRadius + diameter / 2 }) }.Rotate(a.End, Vector.ZAxis, Math.PI / 2 - aeAngle);
            Point eStart = aeRadius.End.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point eEnd = eStart.Translate(new Vector() { X = shapeCode.E - 2 * bendRadius - diameter });
            Point ebCentre = eEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Line ebRadius = new Line() { Start = ebCentre, End = eEnd }.Rotate(ebCentre, Vector.ZAxis, Math.PI / 2 - ebAngle);
            Point bStart = ebRadius.End;

            Arc aeArc = Engine.Geometry.Create.ArcByCentre(aeRadius.End, a.End, eStart);
            Line e = new Line() { Start = eStart, End = eEnd };
            Arc ebArc = Engine.Geometry.Create.ArcByCentre(ebCentre, eEnd, bStart);
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector { X = shapeCode.B - bendRadius }) }.Rotate(bStart, Vector.ZAxis, ebAngle);

            return new PolyCurve() { Curves = new List<ICurve>() { a, aeArc, e, ebArc, b } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode26 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.B);

            Point aEnd = new Point() { X = shapeCode.A - bendRadius - diameter };
            Point abCentre = aEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, Math.PI / 2 - angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - diameter }) }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd.Translate(new Vector() { X = bendRadius + diameter / 2 }) }.Rotate(b.End, Vector.ZAxis, - Math.PI/2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcRadius.End, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { X = shapeCode.C - bendRadius }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode27 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.A);

            Line a = new Line() { Start = new Point(), End = new Point() { X = shapeCode.A - bendRadius } }.Rotate(new Point(), Vector.ZAxis, angle);
            Point aEnd = a.End;
            Line abRadius = new Line() { Start = aEnd, End = aEnd.Translate(new Vector() { X = bendRadius + diameter / 2 }) }.Rotate(aEnd, Vector.ZAxis, -angle);
            Point bStart = abRadius.End.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - diameter / 2 });
            Line b = new Line() { Start = bStart, End = bEnd };
            Point bcCentre = bEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point cStart = bcCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });


            Arc abArc = Engine.Geometry.Create.ArcByCentre(abRadius.End, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { Y = -shapeCode.C + bendRadius + diameter / 2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode28 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.A);

            Line a = new Line() { Start = new Point(), End = new Point() { X = shapeCode.A - bendRadius } }.Rotate(new Point(), Vector.ZAxis, angle);
            Point aEnd = a.End;
            Line abRadius = new Line() { Start = aEnd, End = aEnd.Translate(new Vector() { X = bendRadius + diameter / 2 }) }.Rotate(aEnd, Vector.ZAxis, -angle);
            Point bStart = abRadius.End.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - diameter / 2 });
            Line b = new Line() { Start = bStart, End = bEnd };
            Point bcCentre = bEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point cStart = bcCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });


            Arc abArc = Engine.Geometry.Create.ArcByCentre(abRadius.End, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { Y = shapeCode.C - bendRadius - diameter / 2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode29 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.E / shapeCode.B);

            Point aEnd = new Point() { X = shapeCode.A - diameter - bendRadius };
            Point abCentre = aEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, Math.PI - angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector() { Y = shapeCode.B - 2 * bendRadius - 2 * diameter }) }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd.Translate(new Vector() { X = -bendRadius - diameter / 2 }) }.Rotate(bEnd, Vector.ZAxis, Math.PI/2 - angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { X = -shapeCode.C + diameter / 2 + bendRadius }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode31 shapeCode, double diameter, double bendRadius)
        {
            Point aEnd = new Point() { X = -shapeCode.A + bendRadius + diameter };
            Point abCentre = aEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bStart = abCentre.Translate(new Vector() { X = -bendRadius - diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() {Y = -shapeCode.B + 2*bendRadius - 2*diameter });
            Point bcCentre = bEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point cStart = bcCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point cEnd = cStart.Translate(new Vector() { X = shapeCode.C - 2 * bendRadius - 2 * diameter });
            Point cdCentre = cEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point dStart = cdCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dStart.Translate(new Vector() { Y = shapeCode.D - bendRadius - diameter/2}) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode32 shapeCode, double diameter, double bendRadius)
        {
            Point aEnd = new Point() { X = -shapeCode.A + bendRadius + diameter / 2 };
            Point abCentre = aEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bStart = abCentre.Translate(new Vector() { X = -bendRadius - diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { Y = -shapeCode.B + 2 * bendRadius - diameter });
            Point bcCentre = bEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point cStart = bcCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point cEnd = cStart.Translate(new Vector() { X = shapeCode.C - 2 * bendRadius - 2 * diameter });
            Point cdCentre = cEnd.Translate(new Vector() { Y = - bendRadius - diameter / 2 });
            Point dStart = cdCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dStart.Translate(new Vector() { Y = - shapeCode.D + bendRadius + diameter / 2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode33 shapeCode, double diameter, double bendRadius)
        {
            Point cBotStart = new Point() { X = shapeCode.A/2 - shapeCode.C, Y = shapeCode.B / 2 + diameter / 2 };
            Point cBotEnd = cBotStart.Translate(new Vector() {X = shapeCode.C - shapeCode.B/2 -diameter/2 });
            Point rightCentre = cBotEnd.Translate(new Vector() {Y = shapeCode.B/2 - diameter/2 });
            Point aTopStart = rightCentre.Translate(new Vector() { Y = shapeCode.B / 2 - diameter / 2 });
            Point aTopEnd = aTopStart.Translate(new Vector() { X = -shapeCode.A + shapeCode.B + 2 * diameter, Z = - diameter });
            Point leftCentre = aTopEnd.Translate(new Vector() { Y = -shapeCode.B / 2 + diameter / 2 });
            Point aBotStart = leftCentre.Translate(new Vector() { Y = -shapeCode.B / 2 + diameter / 2 });
            Point cTopEnd = aTopStart.Translate(new Vector() { X = - shapeCode.C + shapeCode.B/2 + diameter/2, Z = -diameter });

            Line cBot = new Line() { Start = cBotStart, End = cBotEnd };
            Circle rightCircle = new Circle() { Centre = rightCentre, Radius = shapeCode.B / 2 - diameter / 2, Normal = Vector.ZAxis };
            ICurve rightArc = rightCircle.SplitAtPoints(new List<Point>() { cBotEnd, aTopStart })[1];
            Line aTop = new Line() { Start = aTopStart, End = aTopEnd };
            Circle leftCircle = new Circle() { Centre = leftCentre, Radius = shapeCode.B / 2 - diameter/2 };
            ICurve leftArc = leftCircle.SplitAtPoints(new List<Point>() { aTopEnd, aBotStart })[0];
            Line aBot = new Line() { Start = aBotStart, End = cBotEnd.Translate(new Vector() { Z = -diameter }) };
            Line cTop = new Line() { Start = aTopStart.Translate(new Vector() {Z = -diameter }), End = cTopEnd }.Translate(new Vector() { Z = -diameter });

            return new PolyCurve() { Curves = new List<ICurve>() { cBot, rightArc, aTop, leftArc, aBot, rightArc.ITranslate(new Vector() { Z = -diameter}), cTop } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode34 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.B);

            Point aEnd = new Point() { X = shapeCode.A - bendRadius - diameter / 2 };
            Point abCentre = aEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, Math.PI / 2 - angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius }) }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd.Translate(new Vector() { X = bendRadius + diameter / 2 }) }.Rotate(b.End, Vector.ZAxis, -angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point cEnd = cStart.Translate(new Vector() { X = shapeCode.C - 2 * bendRadius - diameter });
            Point ceCentre = cEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point eStart = ceCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc ceArc = Engine.Geometry.Create.ArcByCentre(ceCentre, cEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart.Translate(new Vector() { Y = -shapeCode.E + bendRadius + diameter / 2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, ceArc, e } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode35 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.B);

            Point aEnd = new Point() { X = shapeCode.A - bendRadius - diameter };
            Point abCentre = aEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, Math.PI / 2 - angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - diameter }) }.Rotate(bStart, Vector.ZAxis, angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd.Translate(new Vector() { X = bendRadius + diameter / 2 }) }.Rotate(b.End, Vector.ZAxis, -Math.PI / 2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point cEnd = cStart.Translate(new Vector() { X = shapeCode.C - 2 * bendRadius - diameter });
            Point ceCentre = cEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point eStart = ceCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc ceArc = Engine.Geometry.Create.ArcByCentre(ceCentre, cEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart.Translate(new Vector() { Y = shapeCode.E - bendRadius - diameter / 2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, ceArc, e } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode36 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.E / shapeCode.A);

            Point dEnd = new Point() { X = -shapeCode.D + bendRadius + diameter };
            Point dcCentre = dEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point cStart = dcCentre.Translate(new Vector() { X = -bendRadius - diameter / 2 });
            Point cEnd = cStart.Translate(new Vector() { Y = -shapeCode.C + 2 * bendRadius - 2*diameter });
            Point cbCentre = cEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point bStart = cbCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter });
            Point baCentre = bEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Line baRadius = new Line() { Start = baCentre, End = bEnd }.Rotate(baCentre, Vector.ZAxis, Math.PI / 2 - angle);
            Point aStart = baRadius.End;

            Line d = new Line() { Start = new Point(), End = dEnd };
            Arc dcArc = Engine.Geometry.Create.ArcByCentre(dcCentre, dEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cbArc = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc baArc = Engine.Geometry.Create.ArcByCentre(baCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector() { X = shapeCode.A - bendRadius - diameter }) }.Rotate(aStart,Vector.ZAxis,angle);

            return new PolyCurve() { Curves = new List<ICurve>() { d, dcArc, c, cbArc, b, baArc, a  } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode41 shapeCode, double diameter, double bendRadius)
        {
            Point aEnd = new Point() { X = -shapeCode.A + bendRadius + diameter };
            Point abCentre = aEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bStart = abCentre.Translate(new Vector() { X = -bendRadius - diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { Y = -shapeCode.B + 2 * bendRadius - 2*diameter });
            Point bcCentre = bEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point cStart = bcCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point cEnd = cStart.Translate(new Vector() { X = shapeCode.C - 2 * bendRadius - 2 * diameter });
            Point cdCentre = cEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point dStart = cdCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point dEnd = dStart.Translate(new Vector() { Y = shapeCode.D - 2 * bendRadius - 2 * diameter });
            Point deCentre = dEnd.Translate(new Vector() { X = -bendRadius - diameter / 2 });
            Point eStart = deCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dEnd };
            Arc deArc = Engine.Geometry.Create.ArcByCentre(deCentre, dEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart.Translate( new Vector{ X = -shapeCode.E + diameter + bendRadius }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d, deArc, e } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode44 shapeCode, double diameter, double bendRadius)
        {
            Point aEnd = new Point() { X = shapeCode.A - diameter - bendRadius };
            Point abCentre = aEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bStart = abCentre.Translate(new Vector() { X = diameter / 2 + bendRadius });
            Point bEnd = bStart.Translate(new Vector() { Y = -shapeCode.B + 2 * diameter + 2 * bendRadius });
            Point bcCentre = bEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point cStart = bcCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point cEnd = cStart.Translate(new Vector() { X = shapeCode.C - 2 * diameter - 2 * bendRadius });
            Point cdCentre = cEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point dStart = cdCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point dEnd = dStart.Translate(new Vector() { Y = shapeCode.D - 2 * diameter - 2 * bendRadius });
            Point deCentre = dEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point eStart = deCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Line d = new Line() { Start = dStart, End = dEnd };
            Arc deArc = Engine.Geometry.Create.ArcByCentre(deCentre, dEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart.Translate(new Vector { X = shapeCode.E - diameter - bendRadius }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d, deArc, e } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode46 shapeCode, double diameter, double bendRadius)
        {
            double angle = Math.Asin(shapeCode.D / shapeCode.B);

            Point aEnd = new Point() { X = shapeCode.A - diameter - bendRadius };
            Point abCentre = aEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, -angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector() { X = shapeCode.B - 2 * diameter - 2 * bendRadius }) }.Rotate(bStart, Vector.ZAxis, -angle);
            Point bEnd = b.End;
            Line bcRadius = new Line() { Start = bEnd, End = bEnd.Translate(new Vector() { X = bendRadius + diameter / 2 }) }.Rotate(bEnd, Vector.ZAxis, angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point cEnd = cStart.Translate(new Vector() { X = shapeCode.C - 2 * diameter - 2 * bendRadius });
            Point cdCentre = cEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Line cdRadius = new Line() { Start = cdCentre, End = cEnd }.Rotate(cdCentre, Vector.ZAxis, Math.PI / 2 - angle);
            Point dStart = cdRadius.End;
            Line d = new Line() { Start = dStart, End = dStart.Translate(new Vector() { X = shapeCode.B - 2 * diameter - 2 * bendRadius }) }.Rotate(dStart, Vector.ZAxis, angle);
            Point dEnd = d.End;
            Line deRadius = new Line() { Start = dEnd, End = dEnd.Translate(new Vector() { X = bendRadius + diameter / 2 }) }.Rotate(dEnd, Vector.ZAxis, -Math.PI / 2 + angle);
            Point deCentre = deRadius.End;
            Point eStart = deCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Engine.Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cdArc = Engine.Geometry.Create.ArcByCentre(cdCentre, cEnd, dStart);
            Arc deArc = Engine.Geometry.Create.ArcByCentre(deCentre, dEnd, eStart);
            Line e = new Line() { Start = eStart, End = eStart.Translate(new Vector { X = shapeCode.E - diameter - bendRadius }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c, cdArc, d, deArc, e } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode47 shapeCode, double diameter, double bendRadius)
        {
            Point cStart = new Point()
            {
                X = -shapeCode.B / 2 + diameter.HookDiameter() - diameter / 2,
                Y = shapeCode.A / 2 - shapeCode.C
            };

            double hookDiameter = diameter.HookDiameter();

            Point cEnd = cStart.Translate(new Vector() { Y = shapeCode.C - hookDiameter });
            Point cCentre = cEnd.Translate(new Vector() { X = -hookDiameter/2 + diameter/2 });
            Point aLeftStart = cCentre.Translate(new Vector() { X = -hookDiameter / 2 + diameter / 2 });
            Point aLeftEnd = aLeftStart.Translate(new Vector() { Y = -shapeCode.A + hookDiameter + bendRadius + diameter / 2 });
            Point abCentre = aLeftEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point bStart = abCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter });
            Point baCentre = bEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point aRightStart = baCentre.Translate(new Vector(){ X = bendRadius + diameter/2});
            Point aRightEnd = aRightStart.Translate(new Vector() { Y = shapeCode.A - hookDiameter - bendRadius - diameter / 2 });
            Point dCentre = aRightEnd.Translate(new Vector() { X = -hookDiameter/2 + diameter / 2 });
            Point dStart = dCentre.Translate(new Vector() { X = -hookDiameter/2 + diameter / 2 });
            Point dEnd = dStart.Translate(new Vector() { Y = -shapeCode.D + hookDiameter  });

            Line c = new Line() { Start = cStart, End = cEnd };
            Circle cCircle = new Circle() { Centre = cCentre, Normal = Vector.ZAxis, Radius = hookDiameter / 2 - diameter/2 };
            ICurve cHook = cCircle.SplitAtPoints(new List<Point>() { cEnd, aLeftStart })[1];
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc abArc = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc baArc = Engine.Geometry.Create.ArcByCentre(baCentre, bEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = aRightEnd };
            Circle dCircle = new Circle() { Centre = dCentre, Normal = Vector.ZAxis, Radius = hookDiameter / 2 - diameter/2};
            ICurve dHook = dCircle.SplitAtPoints(new List<Point>() { aRightEnd, dStart })[1];
            Line d = new Line() { Start = dStart, End = dEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { c, cHook, aLeft, abArc, b, baArc, aRight, dHook, d } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode48 shapeCode, double diameter, double bendRadius)
        {
            Point cStart = new Point()
            {
                X = -shapeCode.B / 2 + (shapeCode.C - diameter / 2)*Math.Cos(Math.PI/4),
                Y = shapeCode.A / 2 - Math.Sin(Math.PI / 4) * shapeCode.C
            };

            double hookDiameter = diameter.HookDiameter();

            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { X = -shapeCode.C + hookDiameter }) }.Rotate(cStart, Vector.ZAxis, -Math.PI/4);
            Point cEnd = c.End;
            Line cRadius = new Line() { Start = cEnd, End = cEnd.Translate(new Vector() { X = -hookDiameter/2 + diameter / 2 }) }.Rotate(cEnd,Vector.ZAxis,Math.PI/4);
            Point cCentre = cRadius.End;
            Point aLeftStart = cCentre.Translate(new Vector() { X = -hookDiameter / 2 + diameter / 2 });
            Point aLeftEnd = aLeftStart.Translate(new Vector() { Y = -shapeCode.A + hookDiameter + bendRadius + diameter / 2 });
            Point abCentre = aLeftEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point bStart = abCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter });
            Point baCentre = bEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point aRightStart = baCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point aRightEnd = aRightStart.Translate(new Vector() { Y = shapeCode.A - hookDiameter - bendRadius - diameter / 2 });
            Point dCentre = aRightEnd.Translate(new Vector() { X = -hookDiameter/2 + diameter / 2 });
            Line dRadius = new Line() { Start = dCentre, End = dCentre.Translate(new Vector() { X = -hookDiameter/2 + diameter / 2 }) }.Rotate(dCentre, Vector.ZAxis, -Math.PI / 4);
            Point dStart = dRadius.End;
            Line d = new Line() { Start = dStart, End = dStart.Translate(new Vector() { X = -shapeCode.D + hookDiameter }) }.Rotate(dStart,Vector.ZAxis,Math.PI/4);

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

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode51 shapeCode, double diameter, double bendRadius)
        {
            Point cStart = new Point() { X = shapeCode.B / 2 - diameter / 2, Y = shapeCode.A / 2 - shapeCode.C };
            Point cEnd = cStart.Translate(new Vector() { Y = shapeCode.C - bendRadius - diameter });
            Point cbCentre = cEnd.Translate(new Vector() { X = -bendRadius - diameter / 2 });
            Point bTopStart = cbCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point bTopEnd = bTopStart.Translate(new Vector() { X = -shapeCode.B - 2 * bendRadius - 2*diameter });
            Point baLeftCentre = bTopEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point aLeftStart = baLeftCentre.Translate(new Vector() { X = -bendRadius - diameter / 2 });
            Point aLeftEnd = aLeftStart.Translate(new Vector() { Y = shapeCode.A - 2 * bendRadius - 2*diameter });
            Point abCentre = aLeftEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point bBotStart = abCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bBotEnd = bBotStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - 2*diameter, Z = -diameter });
            Point baRightCentre = bBotEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point aRightStart = baRightCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point dEnd = bTopStart.Translate(new Vector() { X = -shapeCode.D + bendRadius + diameter, Z = -diameter });

            Line c = new Line() { Start = cStart, End = cEnd };
            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bTopStart);
            Line bTop = new Line() { Start = bTopStart, End = bTopEnd };
            Arc baLeft = Engine.Geometry.Create.ArcByCentre(baLeftCentre, bTopEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre , aLeftEnd, bBotStart);
            Line bBot = new Line() { Start = bBotStart, End = bBotEnd };
            Arc baRight = Engine.Geometry.Create.ArcByCentre(baRightCentre, bBotEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = cEnd.Translate(new Vector() { Z = -diameter }) };
            Line d = new Line() { Start = bTopStart.Translate(new Vector() { Z = -diameter }), End = dEnd };

            return new PolyCurve() { Curves = new List<ICurve>() { c, cb, bTop, baLeft, aLeft, ab, bBot, baRight, aRight, cb.Translate(new Vector() { Z = -diameter}), d} };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode52 shapeCode, double diameter, double bendRadius)
        {
            Point cStart = new Point()
            {
                X = shapeCode.B / 2 - (shapeCode.C - diameter / 2) * Math.Cos(Math.PI / 4),
                Y = shapeCode.A / 2 - shapeCode.C * Math.Sin(Math.PI / 4)
            };

            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { X = shapeCode.C - bendRadius - diameter }) }.Rotate(cStart, Vector.ZAxis, Math.PI / 4);
            Point cEnd = c.End;
            Line cbRadius = new Line() { Start = cEnd, End = cEnd.Translate(new Vector() { X = -bendRadius - diameter / 2 }) }.Rotate(cEnd, Vector.ZAxis, -Math.PI / 4);
            Point cbCentre = cbRadius.End;
            Point bTopStart = cbCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point bTopEnd = bTopStart.Translate(new Vector() { X = -shapeCode.B - 2 * bendRadius - 2 * diameter });
            Point baLeftCentre = bTopEnd.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point aLeftStart = baLeftCentre.Translate(new Vector() { X = -bendRadius - diameter / 2 });
            Point aLeftEnd = aLeftStart.Translate(new Vector() { Y = shapeCode.A - 2 * bendRadius - 2 * diameter });
            Point abCentre = aLeftEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point bBotStart = abCentre.Translate(new Vector() { Y = -bendRadius - diameter / 2 });
            Point bBotEnd = bBotStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius - 2 * diameter, Z = -diameter });
            Point baRightCentre = bBotEnd.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point aRightStart = baRightCentre.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point aRightEnd = aRightStart.Translate(new Vector() { Y = shapeCode.A - 2 * bendRadius - 2 * diameter });
            Point dStart = bTopStart.Translate(new Vector() { Z = -diameter });
            Point dEnd = bTopStart.Translate(new Vector() { X = -shapeCode.D + bendRadius + diameter, Z = -diameter });

            Arc cb = Engine.Geometry.Create.ArcByCentre(cbCentre, cEnd, bTopStart);
            Line bTop = new Line() { Start = bTopStart, End = bTopEnd };
            Arc baLeft = Engine.Geometry.Create.ArcByCentre(baLeftCentre, bTopEnd, aLeftStart);
            Line aLeft = new Line() { Start = aLeftStart, End = aLeftEnd };
            Arc ab = Engine.Geometry.Create.ArcByCentre(abCentre, aLeftEnd, bBotStart);
            Line bBot = new Line() { Start = bBotStart, End = bBotEnd };
            Arc baRight = Engine.Geometry.Create.ArcByCentre(baRightCentre, bBotEnd, aRightStart);
            Line aRight = new Line() { Start = aRightStart, End = aRightEnd};
            Arc ad = Engine.Geometry.Create.ArcByCentre(cbCentre.Translate(new Vector() { Z = -diameter }), aRightEnd, dStart);
            Line d = new Line() { Start = dStart, End = dEnd }.Rotate(dStart, Vector.ZAxis, Math.PI/4);

            return new PolyCurve() { Curves = new List<ICurve>() { c, cb, bTop, baLeft, aLeft, ab, bBot, baRight, aRight, ad, d } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode56 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode63 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode64 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode67 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode75 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode77 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode98 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(this ShapeCode99 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/
        /****    Private Fallback Method            ********/
        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        private static ICurve Centreline(IShapeCode shapeCode, double diameter, double bendRadius)
        {
            Reflection.Compute.RecordError("ShapeCode not recognised or supported.");
            return null;
        }

        /***************************************************/


    }
}

