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
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes the centreline for a Reinforcement using the ShapeCode provided according to Bs 8666:2020.")]
        [Input("reinforcement", "The reinforcement containing the ShapeCode, reinforcement and bending radius to generate the centreline.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(Reinforcement reinforcement)
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
        public static ICurve ICentreline(IShapeCode shapeCode, double diameter, double bendRadius = 0)
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

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode00 shapeCode, double diameter, double bendRadius = 0)
        {
            if (shapeCode.IsNull())
                return null;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return null;
            }

            return new Line() { Start = new Point() { X = -shapeCode.A / 2 }, End = new Point() { X = shapeCode.A / 2 } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode11 shapeCode, double diameter, double bendRadius = 0)
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

            Point bEnd = new Point() { X = shapeCode.B - bendRadius };
            Point arcCentre = bEnd.Translate(new Vector() { Y = bendRadius });
            Point aStart = arcCentre.Translate(new Vector() { X = bendRadius });
            Line b = new Line() { Start = new Point(), End = bEnd };
            Arc arc = Geometry.Create.ArcByCentre(arcCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector { Y = shapeCode.A - bendRadius }) };

            return new PolyCurve() { Curves = new List<ICurve>() { b, arc, a } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode12 shapeCode, double diameter, double bendRadius)
        {
            if (shapeCode.IsNull())
                return null;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return null;
            }
            else if (shapeCode.R < diameter.SchedulingRadius())
                shapeCode.R = diameter.SchedulingRadius();

            Point bEnd = new Point() { X = shapeCode.B - shapeCode.R };
            Point arcCentre = bEnd.Translate(new Vector() { Y = shapeCode.R });
            Point aStart = arcCentre.Translate(new Vector() { X = shapeCode.R });
            Line b = new Line() { Start = new Point(), End = bEnd };
            Arc arc = Geometry.Create.ArcByCentre(arcCentre, bEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector { Y = shapeCode.A - shapeCode.R }) };

            return new PolyCurve() { Curves = new List<ICurve>() { b, arc, a } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode13 shapeCode, double diameter, double bendRadius)
        {
            if (shapeCode.IsNull())
                return null;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return null;
            }

            Point aEnd = new Point() { X = shapeCode.A - shapeCode.B / 2 };
            Point arcCentre = aEnd.Translate(new Vector() { Y = shapeCode.B / 2 });
            Point cStart = aEnd.Translate(new Vector() { Y = shapeCode.B });
            Line a = new Line() { Start = new Point(), End = aEnd };
            Circle circle = new Circle() { Centre = arcCentre, Radius = bendRadius, Normal = Vector.ZAxis };
            ICurve b = circle.SplitAtPoints(new List<Point>() { aEnd, cStart })[0];
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector { X = -shapeCode.C + shapeCode.B / 2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, b, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode14 shapeCode, double diameter, double bendRadius)
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

            double angle = Math.Asin(shapeCode.D / shapeCode.A);

            Point cEnd = new Point() { X = shapeCode.C - bendRadius };
            Point arcCentre = cEnd.Translate(new Vector() { Y = bendRadius });

            Line radius = new Line() { Start = arcCentre, End = cEnd }.Rotate(arcCentre, Vector.ZAxis, Math.PI - angle);
            Point aStart = radius.End;

            Line c = new Line() { Start = new Point(), End = cEnd };
            Arc arc = Geometry.Create.ArcByCentre(arcCentre, cEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector() { X = -shapeCode.A + bendRadius }) }.Rotate(aStart, Vector.ZAxis, -angle);

            return new PolyCurve() { Curves = new List<ICurve>() { c, arc, a } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode15 shapeCode, double diameter, double bendRadius)
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

            double angle = Math.Asin(shapeCode.D / shapeCode.A);

            Point cEnd = new Point() { X = -shapeCode.C + bendRadius };
            Point arcCentre = cEnd.Translate(new Vector() { Y = bendRadius });

            Line radius = new Line() { Start = arcCentre, End = cEnd }.Rotate(arcCentre, Vector.ZAxis, - Math.PI/2 + angle);
            Point aStart = radius.End;

            Line c = new Line() { Start = new Point(), End = cEnd };
            Arc arc = Geometry.Create.ArcByCentre(arcCentre, cEnd, aStart);
            Line a = new Line() { Start = aStart, End = aStart.Translate(new Vector { X = -shapeCode.A + bendRadius }) }.Rotate(aStart, Vector.ZAxis, -angle);

            return new PolyCurve() { Curves = new List<ICurve>() { c, arc, a } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode21 shapeCode, double diameter, double bendRadius)
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

            Point aEnd = new Point() { Y = -shapeCode.A + bendRadius };
            Point abArcCentre = aEnd.Translate(new Vector() { X = bendRadius });
            Point bStart = abArcCentre.Translate(new Vector() { Y = -bendRadius });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius });
            Point bcArcCentre = bEnd.Translate(new Vector() { Y = bendRadius });
            Point cStart = bcArcCentre.Translate(new Vector() { X = bendRadius });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Geometry.Create.ArcByCentre(bcArcCentre, bEnd, cStart);
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { Y = shapeCode.C }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode22 shapeCode, double diameter, double bendRadius)
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

            Point aEnd = new Point() { Y = shapeCode.A - bendRadius };
            Point abArcCentre = aEnd.Translate(new Vector() { X = bendRadius });
            Point bStart = abArcCentre.Translate(new Vector() { Y = bendRadius });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius });
            Point bdArcCentre = bEnd.Translate(new Vector() { Y = -shapeCode.C / 2 });
            Point dStart = bdArcCentre.Translate(new Vector() { Y = -shapeCode.C / 2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Circle circle = new Circle() { Centre = bdArcCentre, Radius = bendRadius, Normal = Vector.ZAxis };
            ICurve bdArc = circle.SplitAtPoints(new List<Point>() { bEnd, dStart })[0];
            Line d = new Line() { Start = dStart, End = dStart.Translate(new Vector() { X = -shapeCode.D + shapeCode.C / 2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bdArc, d } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode23 shapeCode, double diameter, double bendRadius)
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

            Point aEnd = new Point() { Y = -shapeCode.A + bendRadius };
            Point abCentre = aEnd.Translate(new Vector() { X = bendRadius });
            Point bStart = abCentre.Translate(new Vector() { Y = -bendRadius });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - 2 * bendRadius });
            Point bcCentre = bEnd.Translate(new Vector() { Y = -bendRadius });
            Point cStart = bcCentre.Translate(new Vector() { X = bendRadius });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Arc bcArc = Geometry.Create.ArcByCentre(bcCentre, bEnd, cStart);
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
        public static ICurve Centreline(ShapeCode24 shapeCode, double diameter, double bendRadius)
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

            double angle = Math.Asin(shapeCode.D / shapeCode.B);

            Point aEnd = new Point() { X = shapeCode.A - bendRadius };
            Point abCentre = aEnd.Translate(new Vector() { Y = bendRadius });
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, Math.PI / 2 - angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector() { X = shapeCode.B }) }.Rotate(bStart, Vector.ZAxis, angle);
            Line bcRadius = new Line() { Start = b.End, End = b.End.Translate(new Vector() { X = -bendRadius })}.Rotate(b.End, Vector.ZAxis,-Math.PI + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre.Translate(new Vector(){ Y = bendRadius});

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Arc bcArc = Geometry.Create.ArcByCentre(bcCentre, b.End, cStart);
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector() { Y = shapeCode.C }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bcArc, c} };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve ICentreline(ShapeCode25 shapeCode, double diameter, double bendRadius)
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

            double aeAngle = Math.Acos(shapeCode.C / shapeCode.A);
            double ebAngle = Math.Asin(shapeCode.D / shapeCode.B);

            Line a = new Line() { Start = new Point(), End = new Point() { Y = -shapeCode.A + bendRadius } }.Rotate(new Point(), Vector.ZAxis, aeAngle);
            Line aeRadius = new Line() { Start = a.End, End = a.End.Translate(new Vector() { X = bendRadius }) }.Rotate(a.End, Vector.ZAxis, aeAngle);
            Point eStart = aeRadius.End.Translate(new Vector() { Y = -bendRadius });
            Point eEnd = eStart.Translate(new Vector() { X = shapeCode.E - 2 * bendRadius });
            Point ebCentre = eEnd.Translate(new Vector() { Y = bendRadius });
            Line ebRadius = new Line() { Start = ebCentre, End = eEnd }.Rotate(ebCentre, Vector.ZAxis, Math.PI - ebAngle);
            Point bStart = ebRadius.End;

            Arc aeArc = Geometry.Create.ArcByCentre(aeRadius. End, a.End, eStart);
            Line e = new Line() { Start = eStart, End = eEnd };
            Arc ebArc = Geometry.Create.ArcByCentre(ebCentre, eEnd, bStart);
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector{ X = shapeCode.B - bendRadius })}.Rotate(bStart,Vector.ZAxis,ebAngle);

            return new PolyCurve() { Curves = new List<ICurve>() { a, aeArc, e, ebArc, b } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode26 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode27 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode28 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode29 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode31 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode32 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode33 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode34 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode35 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode36 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode41 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode44 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode46 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode47 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode48 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode51 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode52 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode56 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode63 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode64 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode67 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode75 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode77 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode98 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(ShapeCode99 shapeCode, double diameter, double bendRadius)
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
        public static ICurve Centreline(IShapeCode shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/


    }
}

