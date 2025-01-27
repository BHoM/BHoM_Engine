/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
using BH.oM.Facade;
using BH.oM.Facade.SectionProperties;
using BH.oM.Physical.Constructions;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns total areas of each unique Construction contained in the provided elements (as per Construction name), as well as the total Frame area (Used for a panel containing openings with FameEdgeProperties).")]
        [Input("elems", "elements to find total areas for.")]
        [MultiOutput(0, "areas", "Total area per each unique Construction. These areas account for decreased areas for openings where frame edges occur.")]
        [MultiOutput(1, "constructions", "Construction corresponding to each area value in areas.")]
        [MultiOutput(2, "frameArea", "Total frame area.")]
        public static Output<List<double>, List <string>, double> FacadeAreasByConstruction(this IEnumerable<IFacadeObject> elems)
        {
            if(elems == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot calculate facade areas if the input elements are null.");
                return new Output<List<double>, List<string>, double>();
            }

            List<IFacadeObject> elemList = elems.ToList();
            if (elems.Any(x => !(x is Panel) && !(x is Opening) && !(x is CurtainWall)))
                Base.Compute.RecordWarning("Some of the provided elements are not valid Facade Element Types. These elements have been ignored.");

            Dictionary<string, double> areasDict = new Dictionary<string, double>();
            double frameArea = 0;

            IEnumerable<CurtainWall> cws = elems.OfType<CurtainWall>();
            foreach (CurtainWall cw in cws.ToList())
            {
                List<Opening> cwOpenings = cw.Openings;
                elemList.AddRange(cwOpenings);
            }
            
            IEnumerable<Panel> panels = elems.OfType<Panel>();
            foreach (Panel panel in panels.ToList())
            {
                List<Opening> panelOpenings = panel.Openings;
                elemList.AddRange(panelOpenings);
            }

            List<IFacadeObject> uniqueElems = elemList.Distinct().ToList();

            foreach (IFacadeObject elem in uniqueElems)
            {
                if (elem is Panel)
                {
                    Panel panel = elem as Panel;
                    double area = panel.Area();
                    if (areasDict.Keys.Contains(panel.IPrimaryPropertyName()))
                    {
                        double prevArea = areasDict[panel.IPrimaryPropertyName()];
                        areasDict[panel.IPrimaryPropertyName()] = prevArea + area;
                    }
                    else
                    {
                        areasDict.Add(panel.IPrimaryPropertyName(), area);
                    }
                }
                else if (elem is Opening)
                {
                    Opening opening = elem as Opening;
                    frameArea += opening.ComponentAreas().Item2;
                    double openArea = opening.ComponentAreas().Item1;
                    if (areasDict.Keys.Contains(opening.IPrimaryPropertyName()))
                    {
                        double prevArea = areasDict[opening.IPrimaryPropertyName()];
                        areasDict[opening.IPrimaryPropertyName()] = prevArea + openArea;
                    }
                    else
                    {
                        areasDict.Add(opening.IPrimaryPropertyName(), openArea);
                    }
                }
            }
            
            List<string> constructions = areasDict.Keys.ToList();
            List<double> areas = areasDict.Values.ToList();

            return new Output<List<double>, List<string>, double>
            {
                Item1 = areas,
                Item2 = constructions,
                Item3 = frameArea
            };
        }

        /***************************************************/

    }
}






