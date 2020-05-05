/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Elements;
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
        public static List<Point> ReinforcementLayout(ConcreteSection section, double position = -1)
        {
            //Extract Longitudinal reinforcement
            List<LongitudinalReinforcement> longReif = section.LongitudinalReinforcement();

            //No longitudinal reinforcement available
            if (longReif.Count == 0)
                return new List<Point>();

            List<ICurve> outerProfileEdges;
            List<ICurve> innerProfileEdges;

            ExtractInnerAndOuterEdges(section, out outerProfileEdges, out innerProfileEdges);

            //Need at least one external edge curve
            if (outerProfileEdges.Count == 0)
                return new List<Point>();

            //TODO: include stirups for offset distance
            double stirupOffset = 0;

            List<Point> rebarPoints = new List<Point>();

            foreach (LongitudinalReinforcement reif in longReif)
            {
                if (position < 0 || (reif.StartLocation <= position && reif.EndLocation >= position))
                {
                    rebarPoints.AddRange(ReinforcementLayout(reif, stirupOffset, outerProfileEdges, innerProfileEdges));
                }
            }

            return rebarPoints;
        }

        /***************************************************/

        public static List<Point> ReinforcementLayout(this LongitudinalReinforcement reinforcement, double extraOffset, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges)
        {
            double offset = extraOffset + reinforcement.MinimumCover + reinforcement.Diameter / 2;
            IEnumerable<ICurve> outerCurves = outerProfileEdges.Select(x => x.IOffset(offset, -Vector.ZAxis));
            IEnumerable<ICurve> innerCurves = innerProfileEdges.Select(x => x.IOffset(offset, Vector.ZAxis));
            return reinforcement.RebarLayout.IPointLayout(outerCurves, innerCurves);
        }

        /***************************************************/

        public static List<ICurve> ReinforcementLayout(Bar bar)
        {
            ConcreteSection section = bar.SectionProperty as ConcreteSection;
            if (section == null)
                return new List<ICurve>();

            //Extract Longitudinal reinforcement
            List<LongitudinalReinforcement> longReif = section.LongitudinalReinforcement();

            //No longitudinal reinforcement available
            if (longReif.Count == 0)
                return new List<ICurve>();

            List<ICurve> outerProfileEdges;
            List<ICurve> innerProfileEdges;

            ExtractInnerAndOuterEdges(section, out outerProfileEdges, out innerProfileEdges);

            //Need at least one external edge curve
            if (outerProfileEdges.Count == 0)
                return new List<ICurve>();


            TransformMatrix transformation = bar.BarSectionTranformation();
            double length = bar.Length();

            //TODO: include stirups for offset distance
            double stirupOffset = 0;

            List<ICurve> barLocations = new List<ICurve>();

            foreach (LongitudinalReinforcement reif in longReif)
            {
                barLocations.AddRange(ReinforcementLayout(reif, stirupOffset, outerProfileEdges, innerProfileEdges, length, transformation));
            }

            return barLocations;
        }

        /***************************************************/

        public static List<Line> ReinforcementLayout(this LongitudinalReinforcement reinforcement, double extraOffset, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges, double length, TransformMatrix transformation)
        {
            List<Point> planLayout = ReinforcementLayout(reinforcement, extraOffset, outerProfileEdges, innerProfileEdges);

            double start = reinforcement.StartLocation * length;
            double end = reinforcement.EndLocation * length;

            List<Line> rebarLines = new List<Line>();
            foreach (Point pt in planLayout)
            {
                Point startPoint = new Point { X = pt.X, Y = pt.Y, Z = start };
                Point endPoint = new Point { X = pt.X, Y = pt.Y, Z = end };
                rebarLines.Add((new Line { Start = startPoint, End = endPoint }).Transform(transformation));
            }
            return rebarLines;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractInnerAndOuterEdges(ConcreteSection section, out List<ICurve> outerProfileEdges, out List<ICurve> innerProfileEdges)
        {
            outerProfileEdges = new List<ICurve>();
            innerProfileEdges = new List<ICurve>();

            if (section == null || section.SectionProfile == null || section.SectionProfile.Edges == null || section.SectionProfile.Edges.Count == 0)
                return;

            List <List<ICurve>> distCurves = Engine.Geometry.Compute.DistributeOutlines(Engine.Geometry.Compute.IJoin(section.SectionProfile.Edges.ToList()).Cast<ICurve>().ToList());

            foreach (List<ICurve> curves in distCurves)
            {
                outerProfileEdges.Add(curves.First());
                innerProfileEdges.AddRange(curves.Skip(1));
            }
        }

        /***************************************************/

        private static List<LongitudinalReinforcement> LongitudinalReinforcement(this ConcreteSection section)
        {
            if (section == null || section.Reinforcement == null || section.Reinforcement.Count == 0)
                return new List<LongitudinalReinforcement>();

            return section.Reinforcement.Where(x => x is LongitudinalReinforcement).Cast<LongitudinalReinforcement>().ToList();
        }

        /***************************************************/
    }
}
