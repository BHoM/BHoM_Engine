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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Reinforcement;
using BH.oM.Structure.Elements;
using BH.oM.Spatial.Layouts;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.Engine.Spatial;
using BH.Engine.Geometry;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - 2D layout                ****/
        /***************************************************/

        [Description("Gets the LongitudinalReinforcement positions in the ConcreteSection as a list of Points.")]
        [Input("section", "The concrete section to return all points from.")]
        [Input("position", "Position along the section to extract reinforcement. A negative value will return all reinforcement.")]
        [Output("points", "The positions of the LongitudinalReinforcement.")]
        public static List<Point> LongitudinalReinforcementLayout(this ConcreteSection section, double position = -1)
        {
            if (section.IsNull())
                return null;

            List<Point> rebarPoints = new List<Point>();

            List<ICurve> outerProfileEdges;
            List<ICurve> innerProfileEdges;
            List<LongitudinalReinforcement> longReif;
            List<TransverseReinforcement> tranReif;
            double longCover, tranCover;

            if (section.CheckSectionAndExtractParameters(out outerProfileEdges, out innerProfileEdges, out longReif, out tranReif, out longCover, out tranCover))
            {
                foreach (LongitudinalReinforcement reif in longReif)
                    if (position < 0 || (reif.StartLocation <= position && reif.EndLocation >= position))
                        rebarPoints.AddRange(ReinforcementLayout(reif, longCover, outerProfileEdges, innerProfileEdges));
            }
            return rebarPoints;
        }

        /***************************************************/

        [Description("Gets the TransverseReinforcement positions in the ConcreteSection as a list of Curves (centerlines).")]
        [Input("section", "The concrete section to return all points from.")]
        [Input("position", "Position along the section to extract reinforcement. A negative value will return all reinforcement.")]
        [Output("curves", "The positions of the LongitudinalReinforcement.")]
        public static List<ICurve> TransverseReinforcementLayout(this ConcreteSection section, double position = -1)
        {
            if (section.IsNull())
                return null;

            List<ICurve> rebarCurves = new List<ICurve>();

            List<ICurve> outerProfileEdges;
            List<ICurve> innerProfileEdges;
            List<LongitudinalReinforcement> longReif;
            List<TransverseReinforcement> tranReif;
            double longCover, tranCover;

            if (section.CheckSectionAndExtractParameters(out outerProfileEdges, out innerProfileEdges, out longReif, out tranReif, out longCover, out tranCover))
            {
                foreach (TransverseReinforcement reif in tranReif)
                    if (position < 0 || (reif.StartLocation <= position && reif.EndLocation >= position))
                        rebarCurves.AddRange(ReinforcementLayout(reif, tranCover, outerProfileEdges, innerProfileEdges));
            }
            return rebarCurves;
        }

        /***************************************************/

        [Description("Gets the IBarReinforcement positions in the ConcreteSection as a list of points (centers) and curves (centerlines).")]
        [Input("section", "The concrete section to return all points from.")]
        [Input("position", "Position along the section to extract reinforcement. A negative value will return all reinforcement.")]
        [Output("geometry", "The positions of the IBarReinforcement.")]
        public static List<IGeometry> ReinforcementLayout(this ConcreteSection section, double position = -1)
        {
            if (section.IsNull())
                return null;

            List<IGeometry> rebarLayout = new List<IGeometry>();
            List<ICurve> rebarCurves = new List<ICurve>();
            List<Point> rebarPoints = new List<Point>();

            List<ICurve> outerProfileEdges;
            List<ICurve> innerProfileEdges;
            List<LongitudinalReinforcement> longReif;
            List<TransverseReinforcement> tranReif;
            double longCover, tranCover;

            if (section.CheckSectionAndExtractParameters(out outerProfileEdges, out innerProfileEdges, out longReif, out tranReif, out longCover, out tranCover))
            {
                foreach (LongitudinalReinforcement reif in longReif)
                    if (position < 0 || (reif.StartLocation <= position && reif.EndLocation >= position))
                        rebarLayout.AddRange(ReinforcementLayout(reif, longCover, outerProfileEdges, innerProfileEdges));

                foreach (TransverseReinforcement reif in tranReif)
                    if (position < 0 || (reif.StartLocation <= position && reif.EndLocation >= position))
                        rebarLayout.AddRange(ReinforcementLayout(reif, tranCover, outerProfileEdges, innerProfileEdges));
            }
            return rebarLayout;
        }

        /***************************************************/

        [Description("Gets the LongitudinalReinforcement positions as a list of points, based on inner and outer profile edges.")]
        [Input("reinforcement", "The LongitudinalReinforcement to extract the points from.")]
        [Input("cover", "Any additional spacing to be added in relation to the edge curves, such as minimum cover and stirups. This will be added on top of half the rebar Diameter.", typeof(Length))]
        [Input("outerProfileEdges", "The outer profile edges of the ConcreteSection to be populated.")]
        [Input("innerProfileEdges", "The inner profile edges, or openings, of the ConcreteSection to be populated.")]
        [Output("points", "The positions of the LongitudinalReinforcement.")]
        public static List<Point> ReinforcementLayout(this LongitudinalReinforcement reinforcement, double cover, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges = null)
        {
            if (reinforcement.IsNull() || outerProfileEdges.Any(x => x.IsNull()))
                return null;

            innerProfileEdges = innerProfileEdges ?? new List<ICurve>();
            double offset = cover + reinforcement.Diameter / 2;

            IEnumerable<ICurve> outerCurves = outerProfileEdges.Select(x => x.IOffset(offset, -x.INormal())).Where(x => x != null).ToList();
            IEnumerable<ICurve> innerCurves = innerProfileEdges.Select(x => x.IOffset(offset, x.INormal())).Where(x => x != null).ToList();

            if (outerCurves.Count() == 0)
            {
                Base.Compute.RecordError("Cover is to large for the section curve. Could not generate layout.");
                return null;
            }
            return reinforcement.RebarLayout.IPointLayout(outerCurves, innerCurves);
        }

        /***************************************************/

        [Description("Gets the TransverseReinforcement positions as a list of curves, based on inner and outer profile edges.")]
        [Input("reinforcement", "The TransverseReinforcement to extract the points from.")]
        [Input("cover", "Rebar cover.", typeof(Length))]
        [Input("outerProfileEdges", "The outer profile edges of the ConcreteSection to be populated.")]
        [Input("innerProfileEdges", "The inner profile edges, or openings, of the ConcreteSection to be populated.")]
        [Output("points", "The centerlines of the TransverseReinforcement.")]
        public static List<ICurve> ReinforcementLayout(this TransverseReinforcement reinforcement, double cover, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges = null)
        {
            if (reinforcement.IsNull() || outerProfileEdges.Any(x => x.IsNull()))
                return null;

            return reinforcement.CenterlineLayout.ICurveLayout(outerProfileEdges, innerProfileEdges, cover);
        }


        /***************************************************/
        /**** Public Methods - 3D layout                ****/
        /***************************************************/

        [Description("Gets all the reinforcement centrelines in the Bar as a list of Curves.")]
        [Input("bar", "The Bar to extract all longitudinal reinforcement from. If the Bar does not contain a ConcreteSection an empty list will be returned.")]
        [Output("lines", "The centrelines of the IBarReinforcement.")]
        public static List<ICurve> ReinforcementLayout(this Bar bar)
        {
            if (bar.IsNull())
                return null;

            List<ICurve> barLocations = new List<ICurve>();
            ConcreteSection section = bar.SectionProperty as ConcreteSection;

            List<ICurve> outerProfileEdges;
            List<ICurve> innerProfileEdges;
            List<LongitudinalReinforcement> longReif;
            List<TransverseReinforcement> tranReif;
            double longCover, tranCover;

            if (section.CheckSectionAndExtractParameters(out outerProfileEdges, out innerProfileEdges, out longReif, out tranReif, out longCover, out tranCover))
            {
                TransformMatrix transformation = bar.BarSectionTranformation();
                double length = bar.Length();

                foreach (LongitudinalReinforcement reif in longReif)
                    barLocations.AddRange(ReinforcementLayout(reif, longCover, outerProfileEdges, innerProfileEdges, length, transformation));

                foreach (TransverseReinforcement reif in tranReif)
                    barLocations.AddRange(ReinforcementLayout(reif, tranCover, outerProfileEdges, innerProfileEdges, length, transformation));
            }

            return barLocations;
        }

        /***************************************************/

        [Description("Gets the LongitudinalReinforcement centrelines as a list of lines, based on inner and outer profile edges and Bar parameters.")]
        [Input("reinforcement", "The LongitudinalReinforcement to extract the centrelines from.")]
        [Input("cover", "Any additional spacing to be added in relation to the edge curves, such as minimum cover and stirups. This will be added on top of half the rebar Diameter.", typeof(Length))]
        [Input("outerProfileEdges", "The outer profile edges of the ConcreteSection to be populated.")]
        [Input("innerProfileEdges", "The inner profile edges, or openings, of the ConcreteSection to be populated.")]
        [Input("length", "Length of the host Bar used to generate the lines.", typeof(Length))]
        [Input("transformation", "Transformation needed to move the lines from the position of the host element.")]
        [Output("points", "The centrelines of the LongitudinalReinforcement.")]
        public static List<Line> ReinforcementLayout(this LongitudinalReinforcement reinforcement, double cover, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges, double length, TransformMatrix transformation)
        {
            if (reinforcement.IsNull() || outerProfileEdges.Any(x => x.IsNull()) || transformation.IsNull())
                return null;
            else if (length < Tolerance.MicroDistance)
            {
                Base.Compute.RecordError("The length provided is less than or equal to the tolerance.");
                return null;
            }

            List<Point> planLayout = ReinforcementLayout(reinforcement, cover, outerProfileEdges, innerProfileEdges);

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

        [Description("Gets the TransverseReinforcement centrelines as a list of curves, based on inner and outer profile edges and Bar parameters.")]
        [Input("reinforcement", "The TransverseReinforcement to extract the centrelines from.")]
        [Input("cover", "Rebar cover.", typeof(Length))]
        [Input("outerProfileEdges", "The outer profile edges of the ConcreteSection to be populated.")]
        [Input("innerProfileEdges", "The inner profile edges, or openings, of the ConcreteSection to be populated.")]
        [Input("length", "Length of the host Bar used to generate the lines.", typeof(Length))]
        [Input("transformation", "Transformation needed to move the lines from the position of the host element.")]
        [Output("points", "The centrelines of the LongitudinalReinforcement.")]
        public static List<ICurve> ReinforcementLayout(this TransverseReinforcement reinforcement, double cover, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges, double length, TransformMatrix transformation)
        {
            if (reinforcement.IsNull() || outerProfileEdges.Any(x => x.IsNull()) || transformation.IsNull())
                return null;
            else if (length <= Tolerance.MicroDistance)
            {
                Base.Compute.RecordError("The length provided is less than or equal to the tolerance.");
                return null;
            }

            List<ICurve> rebarLines = new List<ICurve>();
            List<ICurve> stirrupOutline = reinforcement.ReinforcementLayout(cover, outerProfileEdges, innerProfileEdges);

            Vector dir = Vector.ZAxis.Transform(transformation);
            double stirrupRange = (reinforcement.EndLocation - reinforcement.StartLocation) * length - (2 * cover + reinforcement.Diameter);
            int count;
            double spacing = reinforcement.Spacing;

            if (reinforcement.AdjustSpacingToFit)
            {
                count = (int)Math.Ceiling(stirrupRange / reinforcement.Spacing);
                spacing = stirrupRange / count;
            }
            else
                count = (int)Math.Floor(stirrupRange / reinforcement.Spacing);

            if (stirrupOutline.Count != 0)
            {
                for (int k = 0; k < stirrupOutline.Count; k++)
                {
                    rebarLines.Add(stirrupOutline[k].ITransform(transformation).ITranslate(dir * (reinforcement.StartLocation * length + cover + reinforcement.Diameter / 2)));
                    for (int i = 0; i < count; i++)
                        rebarLines.Add(rebarLines.Last().ITranslate(dir * spacing));
                }
            }
            return rebarLines;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets the IBarReinforcement centrelines as a list of curves, based on inner and outer profile edges and Bar parameters.")]
        [Input("reinforcement", "The TransverseReinforcement to extract the centrelines from.")]
        [Input("cover", "Rebar cover.", typeof(Length))]
        [Input("outerProfileEdges", "The outer profile edges of the ConcreteSection to be populated.")]
        [Input("innerProfileEdges", "The inner profile edges, or openings, of the ConcreteSection to be populated.")]
        [Input("length", "Length of the host Bar used to generate the lines.", typeof(Length))]
        [Input("transformation", "Transformation needed to move the lines from the position of the host element.")]
        [Output("points", "The centrelines of the LongitudinalReinforcement.")]
        public static List<ICurve> IReinforcementLayout(this IBarReinforcement reinforcement, double cover, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges, double length, TransformMatrix transformation)
        {
            if (reinforcement.IsNull() || outerProfileEdges.Any(x => x.IsNull()) || transformation.IsNull())
                return null;
            else if (length <= Tolerance.MicroDistance)
            {
                Base.Compute.RecordError("The length provided is less than or equal to the tolerance.");
                return null;
            }

            return new List<ICurve>(ReinforcementLayout(reinforcement as dynamic, cover, outerProfileEdges, innerProfileEdges, length, transformation));
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool CheckSectionAndExtractParameters(this ConcreteSection section, out List<ICurve> outerProfileEdges, out List<ICurve> innerProfileEdges, out List<LongitudinalReinforcement> longReif, out List<TransverseReinforcement> tranReif, out double longCover, out double tranCover)
        {
            //Assigning out parameters
            outerProfileEdges = new List<ICurve>();
            innerProfileEdges = new List<ICurve>();
            longReif = new List<LongitudinalReinforcement>();
            tranReif = new List<TransverseReinforcement>();

            if (section == null || section.RebarIntent == null)
            {
                longCover = 0;
                tranCover = 0;
                return false;
            }

            //Starting default value to the minimum cover of the section.
            longCover = section.RebarIntent.MinimumCover;
            tranCover = section.RebarIntent.MinimumCover;



            longReif = section.LongitudinalReinforcement();
            tranReif = section.TransverseReinforcement();

            //Check if any offsetlayout, if so, check if larger than minimum cover
            if (tranReif.Any(x => x.CenterlineLayout is OffsetCurveLayout))
            {
                double maxCover = Math.Max(section.RebarIntent.MinimumCover, tranReif.Select(x => x.CenterlineLayout).OfType<OffsetCurveLayout>().Max(x => x.Offset));
                longCover = maxCover;
                tranCover = maxCover;
            }

            //No  reinforcement available
            if (longReif.Count == 0 && tranReif.Count == 0)
                return false;

            ExtractInnerAndOuterEdges(section, out outerProfileEdges, out innerProfileEdges);

            //Need at least one external edge curve
            if (outerProfileEdges.Count == 0)
                return false;

            //Add additional offset to include transverse rebar's diameters
            if (tranReif.Count > 0)
            {
                double maxTranDiam = tranReif.Select(r => r.Diameter).Max();
                longCover += maxTranDiam;
                tranCover += maxTranDiam / 2;
            }

            return true;
        }

        /***************************************************/

        private static void ExtractInnerAndOuterEdges(ConcreteSection section, out List<ICurve> outerProfileEdges, out List<ICurve> innerProfileEdges)
        {
            outerProfileEdges = new List<ICurve>();
            innerProfileEdges = new List<ICurve>();

            if (section == null || section.SectionProfile == null || section.SectionProfile.Edges.IsNullOrEmpty())
                return;

            List<List<ICurve>> distCurves = Engine.Geometry.Compute.DistributeOutlines(Engine.Geometry.Compute.IJoin(section.SectionProfile.Edges.ToList()).Cast<ICurve>().ToList());

            foreach (List<ICurve> curves in distCurves)
            {
                outerProfileEdges.Add(curves.First());
                innerProfileEdges.AddRange(curves.Skip(1));
            }
        }

        /***************************************************/

        private static List<LongitudinalReinforcement> LongitudinalReinforcement(this ConcreteSection section)
        {
            if (section == null || section.RebarIntent == null || section.RebarIntent.BarReinforcement.IsNullOrEmpty())
                return new List<LongitudinalReinforcement>();

            return section.RebarIntent.BarReinforcement.Where(x => x is LongitudinalReinforcement).Cast<LongitudinalReinforcement>().ToList();
        }

        /***************************************************/

        private static List<TransverseReinforcement> TransverseReinforcement(this ConcreteSection section)
        {
            if (section == null || section.RebarIntent == null || section.RebarIntent.BarReinforcement.IsNullOrEmpty())
                return new List<TransverseReinforcement>();

            return section.RebarIntent.BarReinforcement.Where(x => x is TransverseReinforcement).Cast<TransverseReinforcement>().ToList();
        }

        /***************************************************/
    }
}





