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

using System.Collections.Generic;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using System.Linq;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> IProfileCurves(double tft, double tfw, double bft, double bfw, double wt, double wd, double r1, double r2, double weldSize)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = bfw / 2, Y = 0, Z = 0 };

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;
            double weldLength = weldSize * 2 / Math.Sqrt(2);

            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (bft - r2) });
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - xAxis * r2, p, p = p + new Vector { X = -r2, Y = r2, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - xAxis * (bfw / 2 - wt / 2 - r1 - r2 - weldLength) });
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * r1, p, p = p + new Vector { X = -r1, Y = r1, Z = 0 }));
            if (weldSize > 0) perimeter.Add(new Line { Start = p, End = p = p + new Vector { X = -weldLength, Y = weldLength, Z = 0 } });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (wd - 2 * r1 - 2 * weldLength) });
            if (weldSize > 0) perimeter.Add(new Line { Start = p, End = p = p + new Vector { X = weldLength, Y = weldLength, Z = 0 } });
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + xAxis * r1, p, p = p + new Vector { X = r1, Y = r1, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (tfw / 2 - wt / 2 - r1 - r2 - weldLength) });
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * r2, p, p = p + new Vector { X = r2, Y = r2, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (tft - r2) });

            int count = perimeter.Count;
            for (int i = 0; i < count; i++)
            {
                perimeter.Add(perimeter[i].IMirror(new Plane { Origin = origin, Normal = xAxis }));
            }
            perimeter.Add(new Line { Start = p, End = p - xAxis * (tfw) });
            perimeter.Add(new Line { Start = origin + xAxis * (-bfw / 2), End = origin + xAxis * (bfw / 2) });

            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> TeeProfileCurves(double tft, double tfw, double wt, double wd, double r1, double r2)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = wt / 2, Y = 0, Z = 0 };

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (wd - r1) });
            if (r1 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + xAxis * (r1), p, p = p + new Vector { X = r1, Y = r1, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (tfw / 2 - wt / 2 - r1 - r2) });
            if (r2 > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * (r2), p, p = p + new Vector { X = r2, Y = r2, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (tft - r2) });

            int count = perimeter.Count;
            for (int i = 0; i < count; i++)
            {
                perimeter.Add(perimeter[i].IMirror(new Plane { Origin = origin, Normal = xAxis }));
            }

            perimeter.Add(new Line { Start = p, End = p - xAxis * (tfw) });
            perimeter.Add(new Line { Start = origin + xAxis * (-wt / 2), End = origin + xAxis * (wt / 2) });

            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> GeneralisedTeeProfileCurves(double height, double webThickness, double leftOutstandWidth, double leftOutstandThickness, double rightOutstandWidth, double rightOutstandThickness)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = -webThickness / 2, Y = 0, Z = 0 };

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (height - leftOutstandThickness) });
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (-leftOutstandWidth) });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (leftOutstandThickness) });
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (leftOutstandWidth + webThickness + rightOutstandWidth) });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (-rightOutstandThickness) });
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (-rightOutstandWidth) });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (-height + rightOutstandThickness) });
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (-webThickness) });

            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> AngleProfileCurves(double width, double depth, double flangeThickness, double webThickness, double innerRadius, double toeRadius)
        {
            List<ICurve> perimeter = new List<ICurve>();

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            Point p = new Point { X = 0, Y = 0, Z = 0 };
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (width) });
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (flangeThickness - toeRadius) });
            if (toeRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - xAxis * (toeRadius), p, p = p + new Vector { X = -toeRadius, Y = toeRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - xAxis * (width - webThickness - innerRadius - toeRadius) });
            if (innerRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * (innerRadius), p, p = p + new Vector { X = -innerRadius, Y = innerRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (depth - flangeThickness - innerRadius - toeRadius) });
            if (toeRadius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - xAxis * (toeRadius), p, p = p + new Vector { X = -toeRadius, Y = toeRadius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - xAxis * (webThickness - toeRadius) });
            perimeter.Add(new Line { Start = p, End = p = p - yAxis * (depth) });
            List<ICurve> translatedCurves = new List<ICurve>();

            foreach (ICurve crv in perimeter)
                translatedCurves.Add(crv.ITranslate(new Vector { X = -width / 2, Y = -depth / 2, Z = 0 }));

            return translatedCurves;
        }

        /***************************************************/

        public static List<ICurve> RectangleProfileCurves(double width, double height, double radius)
        {

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = -width / 2, Y = height / 2 - radius, Z = 0 };
            perimeter.Add(new Line { Start = p, End = p = p - yAxis * (height - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + xAxis * radius, p, p = p + new Vector { X = radius, Y = -radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + xAxis * (width - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p + yAxis * radius, p, p = p + new Vector { X = radius, Y = radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p + yAxis * (height - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - xAxis * radius, p, p = p + new Vector { X = -radius, Y = radius, Z = 0 }));
            perimeter.Add(new Line { Start = p, End = p = p - xAxis * (width - 2 * radius) });
            if (radius > 0) perimeter.Add(BH.Engine.Geometry.Create.ArcByCentre(p - yAxis * radius, p, p = p + new Vector { X = -radius, Y = -radius, Z = 0 }));
            return perimeter;
        }

        /***************************************************/

        public static List<ICurve> BoxProfileCurves(double width, double height, double webThickness, double flangeThickness, double innerRadius, double outerRadius)
        {
            List<ICurve> box = RectangleProfileCurves(width, height, outerRadius);
            box.AddRange(RectangleProfileCurves(width - 2 * webThickness, height - 2 * flangeThickness, innerRadius));
            return box;
        }

        /***************************************************/

        public static List<ICurve> FabricatedBoxProfileCurves(double width, double height, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            List<ICurve> box = RectangleProfileCurves(width, height, 0);

            List<ICurve> welds = new List<ICurve>();
            double weldLength = weldSize * 2 / Math.Sqrt(2);
            Point q1 = new Point { X = (width / 2) - webThickness, Y = (height / 2) - topFlangeThickness, Z = 0 };
            Point q2 = new Point { X = -(width / 2) + webThickness, Y = (height / 2) - topFlangeThickness, Z = 0 };
            Point q3 = new Point { X = -(width / 2) + webThickness, Y = -(height / 2) + botFlangeThickness, Z = 0 };
            Point q4 = new Point { X = (width / 2) - webThickness, Y = -(height / 2) + botFlangeThickness, Z = 0 };
            Vector wx = new Vector { X = weldLength, Y = 0, Z = 0 };
            Vector wy = new Vector { X = 0, Y = weldLength, Z = 0 };

            if (weldSize > 0)
            {
                welds.Add(new Line { Start = q1 - wx, End = q1 - wy });
                welds.Add(new Line { Start = q2 + wx, End = q2 - wy });
                welds.Add(new Line { Start = q3 + wx, End = q3 + wy });
                welds.Add(new Line { Start = q4 - wx, End = q4 + wy });
                box.AddRange(welds);
            }

            List<ICurve> innerBox = new List<ICurve>();
            innerBox.Add(new Line { Start = q1 - wy, End = q4 + wy });
            innerBox.Add(new Line { Start = q4 - wx, End = q3 + wx });
            innerBox.Add(new Line { Start = q3 + wy, End = q2 - wy });
            innerBox.Add(new Line { Start = q2 + wx, End = q1 - wx });

            box.AddRange(innerBox);
            
            return box;
        }

        /***************************************************/

        public static List<ICurve> GeneralisedFabricatedBoxProfileCurves(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double topLeftCorbelWidth, double topRightCorbelWidth, double botLeftCorbelWidth, double botRightCorbelWidth)
        {

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            List<ICurve> externalEdges = new List<ICurve>();
            List<ICurve> internalEdges = new List<ICurve>();
            List<ICurve> group = new List<ICurve>();
            Point p1 = new Point { X = 0, Y = 0, Z = 0 };
            Point p2 = new Point { X = 0, Y = botFlangeThickness, Z = 0 };

            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + xAxis * ((width / 2) + botRightCorbelWidth) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + yAxis * botFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - xAxis * botRightCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + yAxis * (height - botFlangeThickness - topFlangeThickness) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + xAxis * topRightCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + yAxis * topFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - xAxis * (width + topRightCorbelWidth + topLeftCorbelWidth) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - yAxis * topFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + xAxis * topLeftCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - yAxis * (height - botFlangeThickness - topFlangeThickness) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - xAxis * botLeftCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - yAxis * botFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + xAxis * ((width / 2) + botLeftCorbelWidth) });

            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + xAxis * ((width / 2) - webThickness) });
            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + yAxis * (height - botFlangeThickness - topFlangeThickness) });
            internalEdges.Add(new Line { Start = p2, End = p2 = p2 - xAxis * ((width / 2) - webThickness) });

            int intCount = internalEdges.Count;
            for (int i = 0; i < intCount; i++)
            {
                internalEdges.Add(internalEdges[i].IMirror(new Plane { Origin = origin, Normal = xAxis }));
            }

            group.AddRange(externalEdges);
            group.AddRange(internalEdges);

            return group;
        }

        /***************************************************/

        public static List<ICurve> KiteProfileCurves(double width1, double angle1, double thickness)
        {

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Vector zAxis = oM.Geometry.Vector.ZAxis;
            Point origin = oM.Geometry.Point.Origin;

            List<ICurve> externalEdges = new List<ICurve>();
            List<ICurve> internalEdges = new List<ICurve>();
            List<ICurve> group = new List<ICurve>();

            double width2 = width1 * Math.Tan(angle1 / 2);
            double angle2 = Math.PI - angle1;

            double tolerance = 1e-3;

            if (angle2 < tolerance || angle2 > Math.PI - tolerance)
            {
                throw new NotImplementedException("Angles must be well between 0 and Pi");
            }

            Point p1 = new Point { X = 0, Y = 0, Z = 0 };
            Point p2 = p1 + xAxis * Math.Abs(thickness / Math.Sin(angle1 / 2));

            Vector dirVec1 = xAxis.Rotate(angle1 / 2, zAxis);
            Vector dirVec2 = dirVec1.Rotate(-(Math.PI / 2), zAxis);

            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + dirVec1 * width1 });
            externalEdges.Add(new Line { Start = p1, End = p1 + dirVec2 * (width1 * Math.Tan(angle1 / 2)) });

            int extCount = externalEdges.Count;
            for (int i = 0; i < extCount; i++)
            {
                externalEdges.Add(externalEdges[i].IMirror(new Plane { Origin = origin, Normal = yAxis }));
            }

            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + dirVec1 * (width1 - thickness - (thickness * Math.Cos(angle1 / 2)) / Math.Sin(angle1 / 2)) });
            internalEdges.Add(new Line { Start = p2, End = p2 + dirVec2 * (width2 - thickness - (thickness * Math.Cos(angle2 / 2)) / Math.Sin(angle2 / 2)) });

            int intCount = internalEdges.Count;
            for (int i = 0; i < intCount; i++)
            {
                internalEdges.Add(internalEdges[i].IMirror(new Plane { Origin = origin, Normal = yAxis }));
            }

            group.AddRange(externalEdges);
            group.AddRange(internalEdges);

            return group;
        }

        /***************************************************/

        public static List<ICurve> CircleProfileCurves(double radius)
        {
            return new List<ICurve> { new Circle { Centre = BH.oM.Geometry.Point.Origin, Radius = radius } };
        }

        /***************************************************/

        public static List<ICurve> TubeProfileCurves(double outerRadius, double thickness)
        {
            List<ICurve> group = new List<ICurve>();
            group.AddRange(CircleProfileCurves(outerRadius));
            group.AddRange(CircleProfileCurves(outerRadius - thickness));
            return group;
        }

        /***************************************************/

        public static List<ICurve> ChannelProfileCurves(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            List<ICurve> edges = new List<ICurve>();

            Point p = new Point() { X = 0, Y = -height / 2 };
            edges.Add(new Line() { Start = new Point() { X = 0, Y = 0 }, End = new Point() { X = 0, Y = height / 2 } });
            edges.Add(new Line() { Start = new Point() { X = 0, Y = height / 2 }, End = new Point() { X = width, Y = height / 2 } });
            edges.Add(new Line() { Start = new Point() { X = width, Y = height / 2 }, End = new Point() { X = width, Y = height / 2 - flangeThickness + toeRadius } });
            if (toeRadius > 0) edges.Add(Geometry.Create.ArcByCentre(new Point() { X = width - toeRadius, Y = height / 2 - flangeThickness + toeRadius }, new Point() { X = width, Y = height / 2 - flangeThickness + toeRadius }, new Point() { X = width - toeRadius, Y = height / 2 - flangeThickness }));
            edges.Add(new Line() { Start = new Point() { X = width - toeRadius, Y = height / 2 - flangeThickness }, End = new Point() { X = webthickness + rootRadius, Y = height / 2 - flangeThickness } });
            if (rootRadius > 0) edges.Add(Geometry.Create.ArcByCentre(new Point() { X = webthickness + rootRadius, Y = height / 2 - flangeThickness - rootRadius }, new Point() { X = webthickness + rootRadius, Y = height / 2 - flangeThickness }, new Point() { X = webthickness, Y = height / 2 - flangeThickness - rootRadius }));
            edges.Add(new Line() { Start = new Point() { X = webthickness, Y = height / 2 - flangeThickness - rootRadius }, End = new Point() { X = webthickness, Y = 0 } });

            int count = edges.Count;
            for (int i = 0; i < count; i++)
            {
                edges.Add(edges[i].IMirror(new Plane { Origin = oM.Geometry.Point.Origin, Normal = oM.Geometry.Vector.YAxis }));
            }

            return edges;
        }

        /***************************************************/
    }
}
