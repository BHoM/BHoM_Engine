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

using BH.oM.Materials;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using BH.oM.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHP = BH.oM.Structural.Properties;

namespace BH.oM.Structural.Properties
{
    public static class Create
    {
        /// <summary>
        /// Create a section property from standard input values
        /// </summary>
        /// <param name="sType">Shape type</param>
        /// <param name="mType">Material type</param>
        /// <param name="height">Total Height</param>
        /// <param name="width">Total width</param>
        /// <param name="t1">Flange Thickness</param>
        /// <param name="t2">Web Thickness</param>
        /// <param name="r1">Radius 1</param>
        /// <param name="r2">Radius 2</param>
        /// <param name="mass">Mass per metre</param>
        public static SteelSection SteelSection(ShapeType sType, double height, double width, double t1, double t2, double r1, double r2, double mass = 0, double b1 = 0, double b2 = 0, double t3 = 0, double b3 = 0)
        {
            SteelSection section = new BH.oM.Structural.Properties.SteelSection();
            section.SetSectionData(height, width, t1, t2, r1, r2, mass, b1, b2, t3, b3);
            section.SetGeometry(sType, height, width, t1, t2, r1, r2, b1, b2, t3, b3);
            section.Shape = sType;
            return section;
        }

        public static SectionProperty NewCustomSection(MaterialType matType, BH.oM.Geometry.Group<Curve> edges)
        {
            SectionProperty section = CreateSection(matType);
            //section.SectionData = CreateSectionData(totalDepth, totalwidth, webThickness, flangeThickness, r1, r2);
            section.Edges = edges;
            if (matType == MaterialType.Steel) (section as SteelSection).SetSectionData();
            return section;
        }

        public static SectionProperty NewTeeSection(MaterialType matType, double totalDepth, double totalwidth, double flangeThickness, double webThickness, double r1 = 0, double r2 = 0)
        {
            SectionProperty section = CreateSection(matType);
            if (matType == MaterialType.Steel)
            {
                (section as SteelSection).SetSectionData(totalDepth, totalwidth, webThickness, flangeThickness, r1, r2);
            }
            section.SetGeometry(ShapeType.Tee, totalDepth, totalwidth, webThickness, flangeThickness, r1, r2);
            section.Shape = ShapeType.Tee;
            return section;
        }

        /// <summary>
        /// Create an I Shaped section property
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="widthTopFlange"></param>
        /// <param name="widthBotFlange"></param>
        /// <param name="totalDepth"></param>
        /// <param name="flangeThicknessTop"></param>
        /// <param name="flangeThicknessBot"></param>
        /// <param name="webThickness"></param>
        /// <param name="webRadius"></param>
        /// <param name="toeRadius"></param>
        /// <returns></returns>
        public static SectionProperty NewISection(MaterialType matType, double widthTopFlange, double widthBotFlange, double totalDepth, double flangeThicknessTop, double flangeThicknessBot, double webThickness, double webRadius, double toeRadius)
        {
            SectionProperty section = CreateSection(matType);
            if (matType == MaterialType.Steel)
            {
                (section as SteelSection).SetSectionData(totalDepth, System.Math.Max(widthTopFlange, widthBotFlange), webThickness, flangeThicknessTop, webRadius, toeRadius, 0, widthTopFlange, widthBotFlange, flangeThicknessBot);
            }
                section.SetGeometry(ShapeType.ISection, totalDepth, System.Math.Max(widthTopFlange, widthBotFlange), webThickness, flangeThicknessTop, webRadius, toeRadius, widthTopFlange, widthBotFlange, flangeThicknessBot);
            section.Shape = ShapeType.ISection;
            return section;
        }

        /// <summary>
        /// Create a rectangular shaped section
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="outerRadius"></param>
        /// <returns></returns>
        public static SectionProperty NewRectangularSection(MaterialType matType, double height, double width, double outerRadius = 0)
        {
            SectionProperty section = CreateSection(matType);
            if (matType == MaterialType.Steel) (section as SteelSection).SetSectionData(height, width, 0, 0, outerRadius, 0);
            section.SetGeometry(ShapeType.Rectangle, height, width, 0, 0, outerRadius, 0);
            section.Shape = ShapeType.Rectangle;
            return section;
        }

        /// <summary>
        /// Create a rectangular shaped section
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="outerRadius"></param>
        /// <returns></returns>
        public static SectionProperty NewBoxSection(MaterialType matType, double height, double width, double tf, double tw, double outerRadius = 0, double innerRadius = 0)
        {
            SectionProperty section = CreateSection(matType);
            if (matType == MaterialType.Steel) (section as SteelSection).SetSectionData(height, width, tw, tf, outerRadius, innerRadius);
            section.SetGeometry(ShapeType.Box, height, width, tw, tf, outerRadius, innerRadius);
            section.Shape = ShapeType.Box;
            return section;// new SteelSection(ShapeType.Box, height, width, tw, tf, outerRadius, innerRadius);
        }

        /// <summary>
        /// Create an angle section
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="flangeThickness"></param>
        /// <param name="webThickness"></param>
        /// <returns></returns>
        public static SectionProperty NewAngleSection(MaterialType matType, double height, double width, double flangeThickness, double webThickness, double webRadius, double toeRadius)
        {
            SectionProperty section = CreateSection(matType);
            if (matType == MaterialType.Steel) (section as SteelSection).SetSectionData(height, width, webThickness, flangeThickness, webRadius, toeRadius);
            section.SetGeometry(ShapeType.Angle, height, width, webThickness, flangeThickness, webRadius, toeRadius);
            section.Shape = ShapeType.Angle;
            return section;
        }

        /// <summary>
        /// create a circular section
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="diameter"></param>
        /// <returns></returns>
        public static SectionProperty NewCircularSection(MaterialType matType, double diameter)
        {
            SectionProperty section = CreateSection(matType);
            if (matType == MaterialType.Steel) (section as SteelSection).SetSectionData(diameter, diameter, 0, 0, 0, 0);
            section.SetGeometry(ShapeType.Circle, diameter, diameter, 0, 0, 0, 0);
            section.Shape = ShapeType.Circle;
            return section;// new SteelSection(ShapeType.Box, height, width, tw, tf, outerRadius, innerRadius);
        }

        /// <summary>
        /// create a circular section
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="diameter"></param>
        /// <returns></returns>
        public static SectionProperty NewTubeSection(MaterialType matType, double diameter, double thickness)
        {
            SectionProperty section = CreateSection(matType);
            if (matType == MaterialType.Steel) (section as SteelSection).SetSectionData(diameter, diameter, thickness, thickness, 0, 0);
            section.SetGeometry(ShapeType.Tube, diameter, diameter, thickness, thickness, 0, 0);
            section.Shape = ShapeType.Tube;
            return section;
        }

        public static SectionProperty NewSection(Group<Curve> edges, ShapeType type, MaterialType matType)
        {
            SectionProperty property = null;

            switch (matType)
            {
                case BH.oM.Materials.MaterialType.Steel:
                    return new SteelSection(edges, type);
                case BH.oM.Materials.MaterialType.Concrete:
                    return new ConcreteSection(edges, type);
                default:
                    property = new SteelSection(edges, type);
                    property.Material = Materials.Create.Default(matType);
                    return property;
            }
        }

        public static SectionProperty CreateSection(MaterialType matType)
        {
            SectionProperty property = null;
            switch (matType)
            {
                case BH.oM.Materials.MaterialType.Steel:
                    return new SteelSection();
                case BH.oM.Materials.MaterialType.Concrete:
                    return new ConcreteSection();
                default:
                    property = new SteelSection();
                    property.Material = Materials.Create.Default(matType);
                    return property;
            }
        }

        public static SectionProperty FromString(string str)
        {
            string[] arr = System.Text.RegularExpressions.Regex.Split(str, @"\s+");

            //Assuming [mm]
            double scalefactor = 0.001;

            if (arr[0] == "RHS")
            {
                double h, w, tw, tf;
                string[] props = arr[1].Split('x');

                if (props.Length < 3)
                    return null;

                if (!(double.TryParse(props[0], out h) && double.TryParse(props[1], out w) && double.TryParse(props[2], out tw)))
                    return null;

                if (props.Length == 3)
                    tf = tw;
                else
                {
                    if (!double.TryParse(props[3], out tf))
                        return null;
                }

                return NewBoxSection(MaterialType.Steel, h * scalefactor, w * scalefactor, tf * scalefactor, tw * scalefactor);

            }
            else if (arr[0] == "CHS")
            {
                double d, t;
                string[] props = arr[1].Split('x');

                if (props.Length < 2)
                    return null;

                if (!(double.TryParse(props[0], out d) && double.TryParse(props[1], out t)))
                    return null;


                return NewTubeSection(MaterialType.Steel, d * scalefactor, t * scalefactor);
            }
            else if (arr[0] == "C")
            {
                double d;
                if (!double.TryParse(arr[1], out d))
                    return null;

                return NewCircularSection(MaterialType.Steel, d * scalefactor);
            }
            else if (arr[0] == "R")
            {
                double w, h;
                string[] props = arr[1].Split('x');

                if (props.Length < 2)
                    return null;

                if (!(double.TryParse(props[0], out h) && double.TryParse(props[1], out w)))
                    return null;

                return NewRectangularSection(MaterialType.Steel, h * scalefactor, w * scalefactor);
            }


            return null;
        }


        private static void SetSectionData(this SteelSection section, double height, double width, double tw, double tf1, double r1, double r2, double mass = 0, double b1 = 0, double b2 = 0, double tf2 = 0, double b3 = 0, double spacing = 0)
        {
            section.Mass = mass;
            section.TotalWidth = width;
            section.TotalDepth = height;
            section.Tw = tw;
            section.Tf1 = tf1;
            section.Tf2 = tf2 == 0 ? tf1 : tf2;
            section.r1 = r1;
            section.r2 = r2;
            section.B1 = b1 == 0 ? width : b1;
            section.B2 = b2 == 0 ? b1 : b2;
            section.B3 = b3;
            section.Spacing = spacing;
        }

        private static void SetGeometry(this SectionProperty section, ShapeType shapeType, double height, double width, double tw, double tf1, double r1, double r2, double mass = 0, double b1 = 0, double b2 = 0, double tf2 = 0, double b3 = 0, double spacing = 0)
        {
            BH.oM.Geometry.Group<BH.oM.Geometry.Curve> edges = null;

            switch (shapeType)
            {
                case ShapeType.ISection:
                    edges = ShapeBuilder.CreateISecction(tf1, b1 == 0 ? width : b1, tf2 == 0 ? tf1 : tf2, b2 == 0 ? width : b2, tw, height - 2 * tf1, r1, r2);
                    break;
                case ShapeType.Tee:
                    edges = ShapeBuilder.CreateTee(tf1, b1 == 0 ? width : b1, tw, height - tf1, r1, r2);
                    break;
                case ShapeType.Box:
                    edges = ShapeBuilder.CreateBox(width, height, tw, tf1, r1, r2);
                    break;
                case ShapeType.Angle:
                    edges = ShapeBuilder.CreateAngle(width, height, tf1, tw, r1, r2);
                    break;
                case ShapeType.Circle:
                    edges = ShapeBuilder.CreateCircle(width / 2);
                    break;
                case ShapeType.Rectangle:
                    edges = ShapeBuilder.CreateRectangle(width, height, r1);
                    break;
                case ShapeType.Tube:
                    edges = ShapeBuilder.CreateTube(width / 2, tw);
                    break;
            }
            section.Edges = edges;
        }

        private static void SetSectionData(this SteelSection section)
        {
            if (section.Edges != null)
            {
                section.Calculate();
                BoundingBox box = section.Edges.Bounds();
                double area = section.Area;
                double width = section.TotalWidth;
                double depth = section.TotalDepth;
                double b1 = Numerics.Integration.GetSliceAt(section.Edges, box.Max.Y - 0.001, 1, Plane.XZ()).Length;
                double b2 = Numerics.Integration.GetSliceAt(section.Edges, box.Min.Y + 0.001, 1, Plane.XZ()).Length;
                double d1 = Numerics.Integration.GetSliceAt(section.Edges, box.Min.X + 0.001, 1, Plane.YZ()).Length;
                double d2 = Numerics.Integration.GetSliceAt(section.Edges, box.Max.X - 0.001, 1, Plane.YZ()).Length;
                double tw = Numerics.Integration.GetSliceAt(section.Edges, box.Centre.Y, 1, Plane.XZ()).Length;
                Slice midHeight = Numerics.Integration.GetSliceAt(section.Edges, box.Centre.X, 1, Plane.YZ());
                double tf1 = 0;
                double tf2 = 0;

                if (area > width * depth * 0.95)
                {
                    //Rectangle
                    section.Shape = ShapeType.Rectangle;
                }
                else if (ArrayUtils.NearEqual(area, width * depth * System.Math.PI / 4, 0.001))
                {
                    section.Shape = ShapeType.Circle;
                }
                else if (ArrayUtils.NearEqual(midHeight.Length, depth, 0.001))
                {
                    //ISection, TSection, ZSection
                    Slice right = Numerics.Integration.GetSliceAt(section.Edges, (box.Max.X + box.Centre.X) / 2, 1, Plane.YZ());
                    Slice left = Numerics.Integration.GetSliceAt(section.Edges, (box.Min.X + box.Centre.X) / 2, 1, Plane.YZ());
                    if (right.Placement.Length == 4 && left.Placement.Length == 4)
                    {
                        tf2 = right.Placement[1] - right.Placement[0];
                        tf1 = right.Placement[3] - right.Placement[2];
                        section.Shape = ShapeType.ISection;
                    }
                    else if (b1 > tw && b2 > tw) //not a tee
                    {
                        //if symetrical could be a tee
                        //else
                        tf1 = left.Placement[0] > right.Placement[0] ? right.Length : left.Length;
                        tf2 = left.Placement[0] > right.Placement[0] ? left.Length : right.Length;
                        section.Shape = ShapeType.Zed;
                    }
                    else
                    {
                        tf1 = left.Length;
                        section.Shape = ShapeType.Tee;
                    }
                }
                else if (midHeight.Placement.Length == 4 && b1 > width * 0.8 && b2 > width * 0.8 && d1 > depth * 0.8 && d2 > depth * 0.8)
                {
                    tw = tw / 2;
                    tf1 = midHeight.Placement[3] - midHeight.Placement[2];
                    tf2 = midHeight.Placement[1] - midHeight.Placement[0];
                    section.Shape = ShapeType.Box;
                }
                else if ((ArrayUtils.NearEqual(b1, width, 0.001) || ArrayUtils.NearEqual(b2, width, 0.001)) &&
                    (ArrayUtils.NearEqual(d1, width, 0.001) || ArrayUtils.NearEqual(d2, width, 0.001)))
                {
                    section.Shape = ShapeType.Angle;
                    tf1 = midHeight.Length;
                }
                else if (ArrayUtils.NearEqual(tw, midHeight.Length, 0.001))
                {
                    section.Shape = ShapeType.Tube;
                    tf1 = tw / 2;
                    tw = tf1;
                }
                else
                {
                    section.Shape = ShapeType.Polygon;
                }
                double mass = section.Area * section.Material.Density / 9.8;
                section.SetSectionData(depth, width, tw, tf1, 0, 0, mass, b1, b2, tf2);
            }
        }
    }
}
