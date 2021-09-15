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

            return Centreline(shapeCode as dynamic, diameter, bendRadius);
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode00 shapeCode, double diameter, double bendRadius = 0)
        {
            if (shapeCode.IsNull())
                return null;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return null;
            }
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

            return new Line() { Start = new Point() { X = -shapeCode.A / 2 }, End = new Point() { X = shapeCode.A / 2 } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode11 shapeCode, double diameter, double bendRadius = 0)
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
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

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
        public static ICurve Centreline(this ShapeCode12 shapeCode, double diameter, double bendRadius)
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
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

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
        public static ICurve Centreline(this ShapeCode13 shapeCode, double diameter, double bendRadius)
        {
            if (shapeCode.IsNull())
                return null;
            else if (diameter <= 0)
            {
                Reflection.Compute.RecordError("The diameter must be greater than zero.");
                return null;
            }
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

            Point aEnd = new Point() { X = shapeCode.A - shapeCode.B / 2 - diameter/2 };
            Point arcCentre = aEnd.Translate(new Vector() { Y = shapeCode.B / 2 + diameter/2 });
            Point cStart = aEnd.Translate(new Vector() { Y = shapeCode.B + diameter });
            Line a = new Line() { Start = new Point(), End = aEnd };
            Circle circle = new Circle() { Centre = arcCentre, Radius = shapeCode.B/2 + diameter/2, Normal = Vector.ZAxis };
            ICurve b = circle.SplitAtPoints(new List<Point>() { aEnd, cStart })[1];
            Line c = new Line() { Start = cStart, End = cStart.Translate(new Vector { X = -shapeCode.C + shapeCode.B / 2 + diameter/2 }) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, b, c } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode14 shapeCode, double diameter, double bendRadius)
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
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

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
        public static ICurve Centreline(this ShapeCode15 shapeCode, double diameter, double bendRadius)
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
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

            double angle = Math.Asin(shapeCode.D / shapeCode.A);

            Point cEnd = new Point() { X = -shapeCode.C + bendRadius };
            Point arcCentre = cEnd.Translate(new Vector() { Y = bendRadius });

            Line radius = new Line() { Start = arcCentre, End = cEnd }.Rotate(arcCentre, Vector.ZAxis, -Math.PI / 2 + angle);
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
        public static ICurve Centreline(this ShapeCode21 shapeCode, double diameter, double bendRadius)
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
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

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
        public static ICurve Centreline(this ShapeCode22 shapeCode, double diameter, double bendRadius)
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
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

            Point aEnd = new Point() { Y = shapeCode.A - bendRadius - diameter };
            Point abArcCentre = aEnd.Translate(new Vector() { X = bendRadius + diameter / 2 });
            Point bStart = abArcCentre.Translate(new Vector() { Y = bendRadius + diameter / 2 });
            Point bEnd = bStart.Translate(new Vector() { X = shapeCode.B - bendRadius - shapeCode.C / 2 - 2*diameter });
            Point bdArcCentre = bEnd.Translate(new Vector() { Y = -shapeCode.C / 2 - diameter/2});
            Point dStart = bdArcCentre.Translate(new Vector() { Y = -shapeCode.C / 2 - diameter/2});

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Geometry.Create.ArcByCentre(abArcCentre, aEnd, bStart);
            Line b = new Line() { Start = bStart, End = bEnd };
            Circle circle = new Circle() { Centre = bdArcCentre, Radius = shapeCode.C/2 + diameter/2, Normal = Vector.ZAxis };
            ICurve bdArc = circle.SplitAtPoints(new List<Point>() { bEnd, dStart })[1].IFlip();
            Line d = new Line() { Start = dStart, End = dStart.Translate(new Vector() { X = -shapeCode.D + shapeCode.C / 2  + diameter}) };

            return new PolyCurve() { Curves = new List<ICurve>() { a, abArc, b, bdArc, d } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode23 shapeCode, double diameter, double bendRadius)
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
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

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
        public static ICurve Centreline(this ShapeCode24 shapeCode, double diameter, double bendRadius)
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
            else if (!shapeCode.ICompliantShapeCode(diameter))
                return null;

            double angle = Math.Asin(shapeCode.D / shapeCode.B);

            Point aEnd = new Point() { X = shapeCode.A - bendRadius };
            Point abCentre = aEnd.Translate(new Vector() { Y = bendRadius });
            Line abRadius = new Line() { Start = abCentre, End = aEnd }.Rotate(abCentre, Vector.ZAxis, Math.PI / 2 - angle);
            Point bStart = abRadius.End;
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector() { X = shapeCode.B }) }.Rotate(bStart, Vector.ZAxis, angle);
            Line bcRadius = new Line() { Start = b.End, End = b.End.Translate(new Vector() { X = -bendRadius - diameter/2 }) }.Rotate(b.End, Vector.ZAxis, -Math.PI/2 + angle);
            Point bcCentre = bcRadius.End;
            Point cStart = bcCentre.Translate(new Vector() { X = bendRadius + diameter/2 });

            Line a = new Line() { Start = new Point(), End = aEnd };
            Arc abArc = Geometry.Create.ArcByCentre(abCentre, aEnd, bStart);
            Circle circle = new Circle() { Centre = bcCentre, Radius = bendRadius + diameter/2, Normal = Vector.ZAxis };
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
        public static ICurve Centreline(this ShapeCode25 shapeCode, double diameter, double bendRadius)
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

            Line a = new Line() { Start = new Point(), End = new Point() { Y = -shapeCode.A + bendRadius + diameter/2 } }.Rotate(new Point(), Vector.ZAxis, aeAngle);
            Line aeRadius = new Line() { Start = a.End, End = a.End.Translate(new Vector() { X = bendRadius + diameter/2 }) }.Rotate(a.End, Vector.ZAxis, Math.PI/2 - aeAngle);
            Point eStart = aeRadius.End.Translate(new Vector() { Y = -bendRadius - diameter/2 });
            Point eEnd = eStart.Translate(new Vector() { X = shapeCode.E - 2 * bendRadius - diameter});
            Point ebCentre = eEnd.Translate(new Vector() { Y = bendRadius + diameter/2});
            Line ebRadius = new Line() { Start = ebCentre, End = eEnd }.Rotate(ebCentre, Vector.ZAxis, Math.PI/2 - ebAngle);
            Point bStart = ebRadius.End;

            Arc aeArc = Geometry.Create.ArcByCentre(aeRadius.End, a.End, eStart);
            Line e = new Line() { Start = eStart, End = eEnd };
            Arc ebArc = Geometry.Create.ArcByCentre(ebCentre, eEnd, bStart);
            Line b = new Line() { Start = bStart, End = bStart.Translate(new Vector { X = shapeCode.B - bendRadius }) }.Rotate(bStart, Vector.ZAxis, ebAngle);

            return new PolyCurve() { Curves = new List<ICurve>() { a, aeArc, e, ebArc, b } };
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode26 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode27 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode28 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode29 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode31 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode32 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode33 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode34 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode35 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode36 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode41 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode44 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode46 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode47 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode48 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode51 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode52 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode56 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode63 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode64 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode67 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode75 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode77 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode98 shapeCode, double diameter, double bendRadius)
        {
            return null;
        }

        /***************************************************/

        [Description("Computes the centreline for the ShapeCode to BS 8666:2020.")]
        [Input("shapeCode", "The ShapeCode to determine the curve parameters.")]
        [Input("diameter", "The diameter of the reinforcement bar used to caclulate minimum scheduling radii, hook diameters and end projection lengths.")]
        [Input("bendRadius", "The bending radius provided, if zero then the minimum bend radius will be used.")]
        [Output("curve", "The centreline curve of the shape code provided.")]
        public static ICurve Centreline(this ShapeCode99 shapeCode, double diameter, double bendRadius)
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
            Engine.Reflection.Compute.RecordError("ShapeCode not recognised or supported.");
            return null;
        }

        /***************************************************/


    }
}

