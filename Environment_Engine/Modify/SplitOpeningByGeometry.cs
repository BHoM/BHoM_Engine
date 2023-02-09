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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;

using BH.oM.Environment.Fragments;
using System;

using BH.oM.Geometry;

using BH.Engine.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        [Description("Split an Environment Opening by assigning new geometry with the original core data. Returns one opening per geometry provided")]
        [Input("opening", "An Environment Opening to split")]
        [Input("polylines", "Geometry polylines to split the opening by - one opening per polyline will be returned")]
        [Output("openings", "A collection of Environment Openings split into the geometry parts provided")]
        public static List<Opening> SplitOpeningByGeometry(this Opening opening, List<Polyline> polylines)
        {
            Polyline outline = opening.Polyline();
            if (outline == null)
            {
                BH.Engine.Base.Compute.RecordError("The outline of the opening needs to be polylinear.");
                return null;
            }

            List<Line> cutting = polylines.SelectMany(x => x.SubParts()).ToList();

            List<Opening> openings = new List<Opening>();
            foreach (Polyline p in outline.Split(cutting))
            {
                Opening pan = opening.ShallowClone();
                pan.Edges = p.ToEdges();
                openings.Add(pan);
            }

            return openings;
        }
    }
}



