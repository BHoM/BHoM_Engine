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
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Structure.Results;
using BH.oM.Structure.Elements;
using BH.Engine.Structure;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Creates a polyline graph in the bars coordinate system displaying the provided forces.")]
        [Input("bars", "The bars the results will be dispalyed on.")]
        [Input("barForces", "A collection of BarForces. Matching takes place inside the component.")]
        [Input("loadcase", "Loadcase to display.")]
        [Input("identifier", "The type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Input("settings", "1.0 is 1 unit per kN and displaying all forces.")]
        [Output("results", "Results as a List of List of Polyline graphs where each inner list corresponds to one Bar Object based on the input order.")]
        public static List<List<CompositeGeometry>> DisplayForce(this IEnumerable<Bar> bars, IEnumerable<BarForce> barForces, string loadcase, Type identifier = null, /*DisplayForceSettings*/Object settings = null)
        {
            /*if (settings == null)
                settings = new DisplayForceSettings();*/

            double scalefactor = 0.000001;//settings.ScaleFactor * 0.001;

            List<List<CompositeGeometry>> results = new List<List<CompositeGeometry>>();

            List<List<BarForce>> mappedBarForces = MapResults(bars, barForces, /*MapResultsBy.ObjectId*/"ObjectId", identifier, new List<string>() { loadcase });

            List<Bar> barsL = bars.ToList();
            for (int i = 0; i < barsL.Count; i++)
            {
                // sort by position
                mappedBarForces[i].Sort();


                Polyline[] plys = { new Polyline(), new Polyline(), new Polyline(),
                                    new Polyline(), new Polyline(), new Polyline() };

                CompositeGeometry[] lines = { new CompositeGeometry(), new CompositeGeometry(), new CompositeGeometry(),
                                              new CompositeGeometry(), new CompositeGeometry(), new CompositeGeometry() };

                BH.oM.Geometry.CoordinateSystem.Cartesian coordinateSystem = barsL[i].CoordinateSystem();
                Vector tangent = barsL[i].Tangent();
                Point end;
                for (int j = 0; j < mappedBarForces[i].Count; j++)
                {
                    Point pos = coordinateSystem.Origin + mappedBarForces[i][j].Position * tangent;

                    // Forces
                    if (true)//settings.FX)
                    {
                        end = pos + mappedBarForces[i][j].FX * scalefactor * coordinateSystem.Y;
                        plys[0].ControlPoints.Add(end);                                     // Building the polyline
                        lines[0].Elements.Add(new Line() { Start = pos, End = end });       // Connecting the ploylines vertecies to the bar
                    }
                    if (true)//settings.FY)
                    {
                        end = pos + mappedBarForces[i][j].FY * scalefactor * coordinateSystem.Y;
                        plys[1].ControlPoints.Add(end);
                        lines[1].Elements.Add(new Line() { Start = pos, End = end });
                    }
                    if (true)//settings.FZ)
                    {
                        end = pos + mappedBarForces[i][j].FZ * scalefactor * coordinateSystem.Z;
                        plys[2].ControlPoints.Add(end);
                        lines[2].Elements.Add(new Line() { Start = pos, End = end });
                    }

                    // Moments
                    if (true)//settings.MX)
                    {
                        end = pos + mappedBarForces[i][j].MX * scalefactor * coordinateSystem.Y;
                        plys[3].ControlPoints.Add(end);
                        lines[3].Elements.Add(new Line() { Start = pos, End = end });
                    }
                    if (true)//settings.MY)
                    {
                        end = pos + mappedBarForces[i][j].MY * scalefactor * coordinateSystem.Z;
                        plys[4].ControlPoints.Add(end);
                        lines[4].Elements.Add(new Line() { Start = pos, End = end });
                    }
                    if (true)//settings.MZ)
                    {
                        end = pos + mappedBarForces[i][j].MZ * scalefactor * coordinateSystem.Y;
                        plys[5].ControlPoints.Add(end);
                        lines[5].Elements.Add(new Line() { Start = pos, End = end });
                    }
                }

                // Placing the Polylines within the line objects
                for (int j = 0; j < 6; j++)
                    lines[j].Elements.Add(plys[j]);

                results.Add(lines.ToList());
            }

            return results;
        }

        /***************************************************/

    }
}
