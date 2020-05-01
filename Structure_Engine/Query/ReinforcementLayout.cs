/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Spatial.Layouts;
using BH.oM.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Geometry;
using BH.oM.Base;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static List<Point> ReinforcementLayout(ConcreteSection section)
        {
            //Check geometry and reinforcement exists
            if (section == null || section.SectionProfile == null || section.SectionProfile.Edges == null || section.SectionProfile.Edges.Count == 0 || section.Reinforcement == null || section.Reinforcement.Count == 0)
                return new List<Point>();

            List<LongitudinalReinforcement> longReif = section.Reinforcement.Where(x => x is LongitudinalReinforcement).Cast<LongitudinalReinforcement>().ToList();

            //No longitudinal reinforcement available
            if (longReif.Count == 0)
                return new List<Point>();

            List<ICurve> outerEdges = new List<ICurve>();
            List<ICurve> innerEdges = new List<ICurve>();

            List<List<ICurve>> distCurves = Engine.Geometry.Compute.DistributeOutlines(Engine.Geometry.Compute.IJoin(section.SectionProfile.Edges.ToList()).Cast<ICurve>().ToList());

            foreach (List<ICurve> curves in distCurves)
            {
                outerEdges.Add(curves.First());
                innerEdges.AddRange(curves.Skip(1));
            }

            //TODO: include stirups for offset distance
            double cover = section.MinimumCover;

            List<Point> rebarPoints = new List<Point>();

            foreach (LongitudinalReinforcement reif in longReif)
            {
                double offset = cover + reif.Diameter / 2;
                IEnumerable<ICurve> outerCurves = outerEdges.Select(x => x.IOffset(offset, -Vector.ZAxis));
                IEnumerable<ICurve> innerCurves = innerEdges.Select(x => x.IOffset(offset, Vector.ZAxis));

                rebarPoints.AddRange(reif.RebarLayout.IPointLayout(outerCurves, innerCurves));
            }

            return rebarPoints;
        }

        /***************************************************/
    }
}
