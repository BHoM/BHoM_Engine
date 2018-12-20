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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static class ShapeBuilder
    {
        /// <summary>
        /// Create an I Section shape
        /// </summary>
        /// <param name="tft">Thickness of top Flange</param>
        /// <param name="tfw">Width of Top flange</param>
        /// <param name="bft">Thicknees of bottom flange</param>
        /// <param name="bfw">With of bottom flange</param>
        /// <param name="wt">thickness of web</param>
        /// <param name="wd">depth of web</param>
        /// <param name="r1">web radius</param>
        /// <param name="r2">toe radius</param>
        /// <returns></returns>
        public static Group<Curve> CreateISecction(double tft, double tfw, double bft, double bfw, double wt, double wd, double r1, double r2)
        {
            Group<Curve> perimeter = new Group<Curve>();
            Point p = new Point(bfw / 2, 0, 0);

            perimeter.Add(new Line(p, p = p + Vector.YAxis(bft - r2)));
            if (r2 > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(-r2, r2, 0), new Plane(p - Vector.YAxis(r2), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p - Vector.XAxis(bfw / 2 - wt / 2 - r1 - r2)));
            if (r1 > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(-r1, r1, 0), new Plane(p + Vector.XAxis(r1), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p + Vector.YAxis(wd - 2 * r1)));
            if (r1 > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(r1, r1, 0), new Plane(p - Vector.YAxis(r1), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p + Vector.XAxis(tfw / 2 - wt / 2 - r1 - r2)));
            if (r2 > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(r2, r2, 0), new Plane(p - Vector.XAxis(r2), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p + Vector.YAxis(tft - r2)));
            Group<Curve> oppositeSide = perimeter.DuplicateGroup();
            oppositeSide.Mirror(new Plane(Point.Origin, Vector.XAxis(1)));
            perimeter.AddRange(oppositeSide);
            perimeter.Add(new Line(p, p - Vector.XAxis(tfw)));
            perimeter.Add(new Line(Point.Origin + Vector.XAxis(-bfw / 2), Point.Origin + Vector.XAxis(bfw / 2)));

            //double xVector = -(perimeter.Max(0) + perimeter.Min(0)) / 2;
            //double yVector = -(perimeter.Max(1) + perimeter.Min(1)) / 2;

            perimeter.Update();

            return perimeter;//.Move(new XYZ(xVector, yVector, 0)) as Curve;
        }

        /// <summary>
        /// Create an T Section shape
        /// </summary>
        /// <param name="tft">Thickness of Flange</param>
        /// <param name="tfw">Width of flange</param>
        /// <param name="wt">thickness of web</param>
        /// <param name="wd">depth of web</param>
        /// <param name="r1">web radius</param>
        /// <param name="r2">toe radius</param>
        /// <returns></returns>
        public static Group<Curve> CreateTee(double tft, double tfw, double wt, double wd, double r1, double r2)
        {
            Group<Curve> perimeter = new Group<Curve>();
            Point p = new Point(wt / 2, 0, 0);

            perimeter.Add(new Line(p, p = p + Vector.YAxis(wd - r1)));
            if (r1 > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(r1, r1, 0), new Plane(p - Vector.YAxis(r1), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p + Vector.XAxis(tfw / 2 - wt / 2 - r1 - r2)));
            if (r2 > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(r2, r2, 0), new Plane(p - Vector.XAxis(r2), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p + Vector.YAxis(tft - r2)));
            Group<Curve> oppositeSide = perimeter.DuplicateGroup();
            oppositeSide.Mirror(new Plane(Point.Origin, Vector.XAxis(1)));
            perimeter.AddRange(oppositeSide);
            perimeter.Add(new Line(p, p - Vector.XAxis(tfw)));
            perimeter.Add(new Line(Point.Origin + Vector.XAxis(-wd / 2), Point.Origin + Vector.XAxis(wd / 2)));

            perimeter.Update();

            return perimeter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="depth"></param>
        /// <param name="flangeThickness"></param>
        /// <param name="webThickness"></param>
        /// <param name="innerRadius"></param>
        /// <param name="toeRadius"></param>
        /// <returns></returns>
        public static Group<Curve> CreateAngle(double width, double depth, double flangeThickness, double webThickness, double innerRadius, double toeRadius)
        {
            Group<Curve> perimeter = new Group<Curve>();
            Point p = new Point(0, 0, 0);
            perimeter.Add(new Line(p, p = p + Vector.XAxis(width)));
            perimeter.Add(new Line(p, p = p + Vector.YAxis(flangeThickness - toeRadius)));
            if (toeRadius > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(-toeRadius, toeRadius, 0), new Plane(p - Vector.YAxis(toeRadius), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p - Vector.XAxis(width - webThickness - innerRadius - toeRadius)));
            if (innerRadius > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(-innerRadius, innerRadius, 0), new Plane(p + Vector.XAxis(innerRadius), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p + Vector.YAxis(depth - flangeThickness - innerRadius - toeRadius)));
            if (toeRadius > 0) perimeter.Add(Create.Arc(p, p = p + new Vector(-toeRadius, toeRadius, 0), new Plane(p - Vector.YAxis(toeRadius), Vector.ZAxis())));
            perimeter.Add(new Line(p, p = p - Vector.XAxis(webThickness - toeRadius)));
            perimeter.Add(new Line(p, p = p - Vector.YAxis(depth)));

            perimeter.Translate(new Vector(-width / 2, -depth / 2, 0));

            return perimeter;
        }

        /// <summary>
        /// Create a rectange in the XY plane with it's centre at the origin
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="radius">Radius at sharp edge</param>
        /// <returns></returns>
        public static Group<Curve> CreateRectangle(double width, double height, double radius)
        {
            Group<Curve> perimeter = new Group<Curve>();
            perimeter.Add(new Line(new double[] { -width / 2, radius - height / 2, 0 }, new double[] { -width / 2, height / 2 - radius, 0 }));
            perimeter.Add(new Line(new double[] { radius - width / 2, height / 2, 0 }, new double[] { -radius + width / 2, height / 2, 0 }));
            perimeter.Add(new Line(new double[] { width / 2, height / 2 - radius, 0 }, new double[] { width / 2, radius - height / 2, 0 }));
            perimeter.Add(new Line(new double[] { width / 2 - radius, -height / 2, 0 }, new double[] { -width / 2 + radius, -height / 2, 0 }));

            if (radius > 0)
            {
                perimeter.Add(Create.Arc(-Math.PI / 2, -Math.PI, radius, new Plane(new Point(radius + -width / 2, radius - height / 2, 0), Vector.ZAxis())));
                perimeter.Add(Create.Arc(Math.PI, Math.PI / 2, radius, new Plane(new Point(radius + -width / 2, height / 2 - radius, 0), Vector.ZAxis())));
                perimeter.Add(Create.Arc(Math.PI / 2, 0, radius, new Plane(new Point(-radius + width / 2, height / 2 - radius, 0), Vector.ZAxis())));
                perimeter.Add(Create.Arc(0, -Math.PI / 2, radius, new Plane(new Point(-radius + width / 2, radius - height / 2, 0), Vector.ZAxis())));
            }

            return perimeter;
        }

        /// <summary>
        /// Create a Box in the XY plane with it's centre at the origin
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="thickness">plate thickness</param>
        /// <param name="innerRadius">inner radius</param>
        /// <param name="outerRadius">outer radius</param>
        /// <returns></returns>
        public static Group<Curve> CreateBox(double width, double height, double tw, double tf, double innerRadius, double outerRadius)
        {
            Group<Curve> box = CreateRectangle(width, height, outerRadius);
            box.AddRange(CreateRectangle(width - 2 * tw, height - 2 * tf, innerRadius));

            return box;
        }

        /// <summary>
        /// Create a Circle in the XY plane with it's centre at the origin
        /// </summary>
        /// <param name="radius">Radius</param>
        /// <returns></returns>
        public static Group<Curve> CreateCircle(double radius)
        {
            return new Group<Curve>() { new Circle(radius, new Plane(Point.Origin, Vector.ZAxis())) };
        }

        /// <summary>
        /// Create a hollow tube in the XY plane with it's centre at the origin
        /// </summary>
        /// <param name="outerRadius"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static Group<Curve> CreateTube(double outerRadius, double thickness)
        {
            Group<Curve> group = new Group<Curve>();
            group.AddRange(CreateCircle(outerRadius));
            group.AddRange(CreateCircle(outerRadius - thickness));
            return group;
        }
    }
}
