using System;
using System.Collections.Generic;
using BHB = BHoM.Base;
using BHG = BHoM.Geometry;
using BHE = BHoM.Structural.Elements;
using BHP = BHoM.Structural.Properties;

namespace ModelLaundry_Engine
{
    public class Structure
    {
        public static List<BHE.Panel> BarsToPanels(List<BHE.Bar> bars, string key = "", BHB.FilterOption option = BHB.FilterOption.Guid)
        {
            List<BHE.Panel> panels = new List<BHoM.Structural.Elements.Panel>();
            BHB.ObjectManager<BHP.PanelProperty> thicknessManager = new BHB.ObjectManager<BHP.PanelProperty>();

            foreach (BHE.Bar bar in bars)
            {
                if (bar.SectionProperty != null && bar.Line.Direction.IsParallel(BHG.Vector.ZAxis(), Math.PI / 24))
                {
                    double length = 0;
                    double thickness = 0;
                    double angleIncrement = 0;
                    if (bar.SectionProperty.TotalDepth > bar.SectionProperty.TotalWidth)
                    {
                        length = bar.SectionProperty.TotalDepth;
                        thickness = bar.SectionProperty.TotalWidth;
                    }
                    else
                    {
                        length = bar.SectionProperty.TotalWidth;
                        thickness = bar.SectionProperty.TotalDepth;
                        angleIncrement = Math.PI / 2;
                    }
                    double rad = bar.OrientationAngle /* Math.PI / 180*/ + angleIncrement;

                    BHG.Vector normal = new BHG.Vector(Math.Cos(rad), Math.Sin(rad), 0);
                    BHG.Vector up = new BHG.Vector(0, 0, 1);
                    BHG.Vector direction = BHG.Vector.CrossProduct(up, normal);

                    direction.Unitize();
                    direction = direction * length / 2;

                    BHG.Line line1 = bar.Line.DuplicateCurve() as BHG.Line;
                    BHG.Line line2 = bar.Line.DuplicateCurve() as BHG.Line;

                    line1.Translate(direction);
                    line2.Translate(BHG.Vector.Zero - direction);

                    BHG.Line line3 = new BHG.Line(line1.StartPoint, line2.StartPoint);
                    BHG.Line line4 = new BHG.Line(line1.EndPoint, line2.EndPoint);

                    BHG.Group<BHG.Curve> perimeter = new BHG.Group<BHG.Curve>(new List<BHG.Curve>() { line1, line2, line3, line4 });

                    string propertyName = "Wall " + Math.Round(thickness * 1000, 2) + " mm";

                    if (thicknessManager[propertyName] == null)
                    {
                        thicknessManager.Add(propertyName, new BHP.ConstantThickness(propertyName, thickness, BHoM.Structural.Properties.PanelType.Wall));
                    }

                    BHE.Panel panel = new BHE.Panel(perimeter);

                    BHP.PanelProperty property = thicknessManager[propertyName];
                    property.Material = bar.Material;
                    panel.PanelProperty = property;
                    panels.Add(panel);

                    BHoM.Global.Project.ActiveProject.RemoveObject(bar.BHoM_Guid);
                }
            }
            return panels;
        }
    }
}
