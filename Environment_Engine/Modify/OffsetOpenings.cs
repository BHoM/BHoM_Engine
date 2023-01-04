/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Reduces the area of the opening(s) if the total area of the opening(s) is equal to the area of the panel itself. Returns an Environment Panel object.")]
        [Input("panel", "An Environment Panel object")]
        [Output("panel", "An Environment Panel object")]
        public static Panel OffsetOpenings(Panel panel)
        {
            if(panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot offset the openings of a null panel.");
                return panel;
            }

            Panel returnPanel = new Panel();
            //Checking if there are openings            
            if (panel.Openings.Count == 0)
                return panel;
            else
            {
                //OpeningArea                
                List<Opening> openings = panel.Openings;
                List<double> openingAreas = new List<double>();
                List<PolyCurve> openingPolyCurves = new List<PolyCurve>();

                foreach (Opening opening in panel.Openings)
                {
                    List<ICurve> openingCrvs = opening.Edges.Select(x => x.Curve).ToList();
                    PolyCurve openingOutline = BH.Engine.Geometry.Create.PolyCurve(openingCrvs);
                    double openingArea = openingOutline.Area();
                    openingAreas.Add(openingArea);
                    openingPolyCurves.Add(openingOutline);
                }

                double totalOpeningArea = openingAreas.Sum();

                //PanelArea
                List<ICurve> panelCrvs = panel.ExternalEdges.Select(x => x.Curve).ToList();
                PolyCurve panelOutline = BH.Engine.Geometry.Create.PolyCurve(panelCrvs);
                double panelArea = panelOutline.Area();

                //Comparing the total opening area to the panel area, if equal: reduce the area of the opening(s).
                if (totalOpeningArea != panelArea)
                    return panel;
                else
                {
                    List<BH.oM.Geometry.Polyline> openingPolylines = new List<BH.oM.Geometry.Polyline>();
                    List<List<BH.oM.Geometry.Polyline>> offsetPolylines = new List<List<BH.oM.Geometry.Polyline>>();
                    List<Opening> newOpenings = new List<Opening>();

                    double distance = -0.01;
                    panel.Openings.Clear();

                    foreach (BH.oM.Geometry.PolyCurve openingPolyCurve in openingPolyCurves)
                    {
                        List<BH.oM.Geometry.Point> polyPoints = openingPolyCurve.IDiscontinuityPoints();
                        BH.oM.Geometry.Polyline openingPolyLine = BH.Engine.Geometry.Create.Polyline(polyPoints);
                        Polyline offsetPolyline = Geometry.Modify.Offset(openingPolyLine, distance);
                        List<Edge> edges = offsetPolyline.ToEdges().ToList();
                        Opening newOpening = new Opening { Edges = edges };
                        panel.Openings.Add(newOpening);
                    }

                    return panel;
                }
            }
        }
    }
}



