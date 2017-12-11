//using BH.oM.Geometry;
//using BH.oM.Structural.Elements;
//using BH.oM.Structural.Properties;
//using BH.Engine.Geometry;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BH.Engine.Structure
//{
//    public static partial class Query
//    {


//        /***************************************************/
//        /**** Public Methods - ConcreteSEction          ****/
//        /***************************************************/

//        public static CompositeGeometry GetReinforcementLayout(this ConcreteSection property, double xStart = 0, double xEnd = 1)
//        {
//            CompositeGeometry geometry = new CompositeGeometry();
//            foreach (Reinforcement reo in property.Reinforcement)
//            {
//                CompositeGeometry layout = reo.IGetLayout(property);

//                foreach (IBHoMGeometry obj in layout.Elements)
//                {
//                    if (obj is Point)
//                    {

//                        geometry.Elements.Add(new Circle(obj as Point, Vector.ZAxis, reo.Diameter / 2));
//                    }
//                    else
//                    {
//                        geometry.Elements.Add(obj);
//                    }
//                }
//            }

            
//            return geometry;
//        }


//        /***************************************************/
//        /**** Public Methods - Reinforcement            ****/
//        /***************************************************/

//        public static CompositeGeometry GetLayout(this LayerReinforcement reinforcement, ConcreteSection property, bool extrude = false)
//        {
//            BoundingBox bounds = new BoundingBox();

//            foreach (ICurve curve in property.Edges)
//            {
//                bounds += curve.IGetBounds();
//            }

//            double relativeDepth = reinforcement.IsVertical ? bounds.Max.X - reinforcement.Depth : bounds.Max.Y - reinforcement.Depth;
//            double[] range = null;
//            double tieDiameter = property.GetTieDiameter();
//            if (property.Shape == ShapeType.Rectangle && tieDiameter > 0)
//            {
//                //TODO: Check this part
//                tieDiameter = tieDiameter + Math.Cos(Math.PI / 4) * (2 * tieDiameter * (Math.Sqrt(2) - 1) + reinforcement.Diameter / 2) - reinforcement.Diameter / 2;
//            }
//            double width =  reinforcement.IsVertical ? property.DepthAt(relativeDepth, ref range) : property.WidthAt(relativeDepth, ref range);

//            double spacing = (width - 2 * property.MinimumCover - reinforcement.Diameter - 2 * tieDiameter) / (reinforcement.BarCount - 1.0);
//            double start = range != null && range.Length > 0 ? range[0] : 0;

//            List<Point> location = new List<Point>();

//            for (int i = 0; i < reinforcement.BarCount; i++)
//            {
//                double x = reinforcement.IsVertical ? relativeDepth : property.MinimumCover + reinforcement.Diameter / 2 + tieDiameter + spacing * i + start;
//                double y = reinforcement.IsVertical ? property.MinimumCover + reinforcement.Diameter / 2 + spacing * i + tieDiameter + start : relativeDepth;

//                location.Add(new Point(x, y, 0));
//            }

//            return new CompositeGeometry(location);


//            //GeometryGroup<Point> location = new GeometryGroup<Point>();
//            //for (int i = 0; i < BarCount; i++)
//            //{
//            //    double x = IsVertical ? relativeDepth : property.MinimumCover + Diameter / 2 + tieDiameter + spacing * i + start;
//            //    double y = IsVertical ? property.MinimumCover + Diameter / 2 + spacing * i + tieDiameter + start : relativeDepth;

//            //    location.Add(new Point(x, y, 0));
//            //}
//            //return location;
//        }

//        /***************************************************/

//        public static CompositeGeometry GetLayout(this PerimeterReinforcement reinforcement, ConcreteSection property, bool extrude = false)
//        {
//            double d = property.TotalDepth;
//            double w = property.TotalWidth;
//            double tieDiameter = property.GetTieDiameter();
//            List<Point> location = new List<Point>();
//            if (property.Shape == ShapeType.Rectangle) //Rectangle
//            {
//                int topCount = 0;
//                int sideCount = 0;
//                double tieOffset = tieDiameter + Math.Cos(Math.PI / 4) * (2 * tieDiameter * (Math.Sqrt(2) - 1) + reinforcement.Diameter / 2) - reinforcement.Diameter / 2;
//                switch (reinforcement.Pattern)
//                {
//                    case ReoPattern.Equispaced:
//                        topCount = (int)(reinforcement.BarCount * w / (2 * w + 2 * d) + 1);
//                        sideCount = (reinforcement.BarCount - 2 * topCount) / 2 + 2;
//                        break;
//                    case ReoPattern.Horizontal:
//                        topCount = reinforcement.BarCount / 2;
//                        sideCount = 2;
//                        break;
//                    case ReoPattern.Vertical:
//                        topCount = 2;
//                        sideCount = reinforcement.BarCount / 2;
//                        break;
//                }
//                double verticalSpacing = (d - 2 * property.MinimumCover - reinforcement.Diameter - 2 * tieOffset) / (sideCount - 1);
//                double depth = property.MinimumCover + reinforcement.Diameter / 2 + tieOffset;
//                for (int i = 0; i < sideCount; i++)
//                {
//                    int count = topCount;
//                    double currentDepth = depth + i * verticalSpacing;
//                    if (i > 0 && i < sideCount - 1)
//                    {
//                        count = 2;
//                    }
//                    List<IBHoMGeometry> layout = ((CompositeGeometry)new LayerReinforcement(reinforcement.Diameter, currentDepth, count).GetLayout(property)).Elements;

//                    foreach (IBHoMGeometry geom in layout)
//                    {
//                        location.Add(geom as Point);
//                    }
//                }
//            }
//            else if (property.Shape == ShapeType.Circle) //Circular
//            {
//                double angle = Math.PI * 2 / reinforcement.BarCount;
//                double startAngle = 0;
//                double radius = d / 2 - property.MinimumCover - reinforcement.Diameter / 2;
//                switch (reinforcement.Pattern)
//                {
//                    case ReoPattern.Horizontal:
//                        startAngle = angle / 2;
//                        break;
//                }
//                for (int i = 0; i < reinforcement.BarCount; i++)
//                {
//                    double x = Math.Cos(startAngle + angle * i) * radius;
//                    double y = Math.Sin(startAngle + angle * i) * radius;
//                    location.Add(new Point(x, y, 0));
//                }

//            }
//            return new CompositeGeometry(location);
//        }

//        /***************************************************/

//        public static CompositeGeometry GetLayout(this TieReinforcement reinforcement, ConcreteSection property, bool extrude = false)
//        {
//            double tieDiameter = property.GetTieDiameter();
//            switch (property.Shape)
//            {
//                case ShapeType.Rectangle:
//                    double X = property.TotalWidth / 2 - property.MinimumCover - tieDiameter * 3;
//                    double Y = property.TotalDepth / 2 - property.MinimumCover - tieDiameter * 3;
//                    double yIn = property.TotalDepth / 2 - property.MinimumCover - tieDiameter / 2;
//                    double xIn = property.TotalWidth / 2 - property.MinimumCover - tieDiameter / 2;

//                    //TODO: Implement. Below copied from BHoM 1.0
                    
                    
//                    /*TEMP****************
//                    Group<Curve> curves = new Group<Curve>();
//                    curves.Add(new Line(new Point(-X, yIn, 0), new Point(X, yIn, 0)));
//                    curves.Add(new Line(new Point(-X, -yIn, 0), new Point(X, -yIn, 0)));
//                    curves.Add(new Line(new Point(xIn, -Y, 0), new Point(xIn, Y, -tieDiameter)));
//                    curves.Add(new Line(new Point(-xIn, -Y, 0), new Point(-xIn, Y, 0)));
//                    Plane p = new Plane(new Point(-X, -Y, 0), Vector.ZAxis());
//                    curves.Add(new Arc(Math.PI * 3 / 2, Math.PI, tieDiameter * 2.5, p));
//                    p = new Plane(new Point(-X, Y, 0), Vector.ZAxis());
//                    curves.Add(new Arc(Math.PI, Math.PI / 2, tieDiameter * 2.5, p));
//                    p = new Plane(new Point(X, Y, 0), Vector.ZAxis());
//                    Vector lap = new Vector(-tieDiameter * 3.5, -tieDiameter * 3.5, 0);
//                    Arc a1 = new Arc(Math.PI / 2, -Math.PI / 4, tieDiameter * 2.5, p);
//                    curves.Add(a1);
//                    curves.Add(new Line(a1.EndPoint, a1.EndPoint + lap));
//                    p = new Plane(new Point(X, Y, -tieDiameter), Vector.ZAxis());
//                    Arc a2 = new Arc(0, 3 * Math.PI / 4, tieDiameter * 2.5, p);
//                    curves.Add(a2);
//                    curves.Add(new Line(a2.EndPoint, a2.EndPoint + lap));
//                    p = new Plane(new Point(X, -Y, 0), Vector.ZAxis());
//                    curves.Add(new Arc(0, -Math.PI / 2, tieDiameter * 2.5, p));

//                    Curve c = Curve.Join(curves)[0];

//                    double width = property.TotalWidth - 2 * property.MinimumCover - tieDiameter;
//                    double spacing = width / (BarCount - 1);
//                    Curve singleTie = null;
//                    if (BarCount > 2)
//                    {
//                        List<Curve> crvs = new List<Curve>();
//                        double startAngle = 0;
//                        double endAngle = Math.PI * 3 / 4;                       
//                        Vector lap2 = lap.DuplicateVector();
//                        p = new Plane(new Point(0, property.TotalDepth / 2 - property.MinimumCover - 3 * tieDiameter, -tieDiameter), Vector.ZAxis());
//                        a1 = new Arc(startAngle, endAngle, 2.5 * tieDiameter, p);
//                        a2 = a1.DuplicateCurve() as Arc;
//                        a2.Mirror(Plane.XZ());
//                        lap2.Mirror(Plane.XZ());
//                        crvs.Add(new Line(a1.StartPoint, a2.StartPoint));
//                        crvs.Add(new Line(a1.EndPoint, a1.EndPoint + lap));
//                        crvs.Add(new Line(a2.EndPoint, a2.EndPoint + lap2));
//                        crvs.Add(a1);
//                        crvs.Add(a2);
//                        singleTie = Curve.Join(crvs)[0];
//                    }

//                    Group<Pipe> bars = new Group<Pipe>();
//                    bars.Add(new Pipe(c, tieDiameter / 2));
//                    for (int i = 0; i < BarCount - 2; i++)
//                    {
//                        c = singleTie.DuplicateCurve();
//                        double location = -width / 2 + (i + 1) * spacing;
//                        //if (location < 0)
//                        //{
//                        //    c.Mirror(Plane.YZ());
//                        //}
//                        //TEMP UNDO c.Translate(Vector.XAxis(location));
//                        bars.Add(new Pipe(c, tieDiameter / 2));
//                    }
//                    */
//                    return null;//temp bars;

//                    //double X = property.TotalWidth / 2 - property.MinimumCover - tieDiameter * 3;
//                    //double Y = property.TotalDepth / 2 - property.MinimumCover - tieDiameter * 3;
//                    //double yIn = property.TotalDepth / 2 - property.MinimumCover - tieDiameter;
//                    //double yOut = property.TotalDepth / 2 - property.MinimumCover;
//                    //double xIn = property.TotalWidth / 2 - property.MinimumCover - tieDiameter;
//                    //double xOut = property.TotalWidth / 2 - property.MinimumCover;

//                    //Group<Curve> curves = new Group<Curve>();
//                    //curves.Add(new Line(new Point(-X, yIn, 0), new Point(X, yIn, 0)));
//                    //curves.Add(new Line(new Point(-X, -yIn, 0), new Point(X, -yIn, 0)));
//                    //curves.Add(new Line(new Point(-X, yOut, 0), new Point(X, yOut, 0)));
//                    //curves.Add(new Line(new Point(-X, -yOut, 0), new Point(X, -yOut, 0)));
//                    //curves.Add(new Line(new Point(xIn, -Y, 0), new Point(xIn, Y, 0)));
//                    //curves.Add(new Line(new Point(-xIn, -Y, 0), new Point(-xIn, Y, 0)));
//                    //curves.Add(new Line(new Point(xOut, -Y, 0), new Point(xOut, Y, 0)));
//                    //curves.Add(new Line(new Point(-xOut, -Y, 0), new Point(-xOut, Y, 0)));
//                    //Plane p = new Plane(new Point(-X,-Y,0), Vector.ZAxis());
//                    //curves.Add(new Arc(Math.PI * 3 / 2, Math.PI, tieDiameter * 2, p));
//                    //curves.Add(new Arc(Math.PI * 3 / 2, Math.PI, tieDiameter * 3, p));
//                    //p = new Plane(new Point(-X, Y, 0), Vector.ZAxis());
//                    //curves.Add(new Arc(Math.PI, Math.PI / 2, tieDiameter * 2, p));
//                    //curves.Add(new Arc(Math.PI, Math.PI / 2, tieDiameter * 3, p));
//                    //p = new Plane(new Point(X, Y, 0), Vector.ZAxis());
//                    //curves.Add(new Arc(Math.PI / 2, 0, tieDiameter * 2, p));
//                    //curves.Add(new Arc(Math.PI / 2, 0, tieDiameter * 3, p));
//                    //p = new Plane(new Point(X, -Y, 0), Vector.ZAxis());
//                    //curves.Add(new Arc(0, -Math.PI / 2, tieDiameter * 2, p));
//                    //curves.Add(new Arc(0, -Math.PI / 2, tieDiameter * 3, p));
//                    //return new Group<Curve>(Curve.Join(curves));
//            }

//            return null;
//        }


//        /***************************************************/
//        /**** Public Methods - Interfaces               ****/
//        /***************************************************/

//        public static CompositeGeometry IGetLayout(this Reinforcement reinforcement, ConcreteSection property, bool extrude = false)
//        {
//            return GetLayout(reinforcement as dynamic, property, extrude);
//        }

//    }
//}
