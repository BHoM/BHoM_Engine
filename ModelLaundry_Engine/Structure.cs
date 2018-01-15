﻿//using System;
//using System.Collections.Generic;
//using BHB = BH.oM.Base;
//using BHG = BH.oM.Geometry;
//using BH.Engine.Geometry;
//using BHE = BH.oM.Structural.Elements;
//using BH.Engine.Structure;
//using BHP = BH.oM.Structural.Properties;

//namespace ModelLaundry_Engine
//{
//    public class Structure
//    {
//        public static List<BHE.Panel> BarsToPanels(List<BHE.Bar> bars, string key = "")
//        {
//            List<BHE.Panel> panels = new List<BH.oM.Structural.Elements.Panel>();

//            foreach (BHE.Bar bar in bars)
//            {
//                if (bar.SectionProperty != null && BHG.Vector.XAxis.IsParallel(bar.GetCentreline().GetDirection(), BHG.Vector.ZAxis, Math.PI / 24))
//                {
//                    double length = 0;
//                    double thickness = 0;
//                    double angleIncrement = 0;
//                    if (bar.SectionProperty.TotalDepth > bar.SectionProperty.TotalWidth)
//                    {
//                        length = bar.SectionProperty.TotalDepth;
//                        thickness = bar.SectionProperty.TotalWidth;
//                    }
//                    else
//                    {
//                        length = bar.SectionProperty.TotalWidth;
//                        thickness = bar.SectionProperty.TotalDepth;
//                        angleIncrement = Math.PI / 2;
//                    }
//                    double rad = bar.OrientationAngle /* Math.PI / 180*/ + angleIncrement;

//                    BHG.Vector normal = new BHG.Vector(Math.Cos(rad), Math.Sin(rad), 0);
//                    BHG.Vector up = new BHG.Vector(0, 0, 1);
//                    BHG.Vector direction = BHG.Vector.CrossProduct(up, normal);

//                    direction.Normalise();
//                    direction = direction * length / 2;

//                    BHG.Line line1 = bar.Line.DuplicateCurve() as BHG.Line;
//                    BHG.Line line2 = bar.Line.DuplicateCurve() as BHG.Line;

//                    BHG.XCurve.Translate(line1, direction);
//                    BHG.XCurve.Translate(line2, BHG.Vector.Zero - direction);

//                    BHG.Line line3 = new BHG.Line(line1.StartPoint, line2.StartPoint);
//                    BHG.Line line4 = new BHG.Line(line1.EndPoint, line2.EndPoint);

//                    BHG.Group<BHG.Curve> perimeter = new BHG.Group<BHG.Curve>(new List<BHG.Curve>() { line1, line2, line3, line4 });

//                    string propertyName = "Wall " + Math.Round(thickness * 1000, 2) + " mm";

//                    if (thicknessManager[propertyName] == null)
//                    {
//                        thicknessManager.Add(propertyName, new BHP.ConstantThickness(propertyName, thickness, BHoM.Structural.Properties.PanelType.Wall));
//                    }

//                    BHE.Panel panel = new BHE.Panel(BHG.Create.SurfaceFromBoundaryCurves(perimeter));

//                    BHP.PanelProperty property = thicknessManager[propertyName];
//                    property.Material = bar.Material;
//                    panel.PanelProperty = property;
//                    panels.Add(panel);

//                    BHoM.Project.Instance.Active.RemoveObject(bar.BHoM_Guid);
//                }
//            }
//            return panels;
//        }
//    }
//}
