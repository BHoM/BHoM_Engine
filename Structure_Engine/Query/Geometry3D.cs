/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the BH.oM.Geometry.Extrusion out of the Bar as its Geometry3D.")]
        [Input("bar", "The input Bar to get the Geometry3D out of, i.e.its extrusion with its cross section along its centreline.")]
        [Input("onlyOuterExtrusion", "If true, and if the cross-section of the Bar is composed by multiple edges (e.g. a Circular Hollow Section), only return the extrusion of the outermost edge.")]
        [Output("3d", "Three-dimensional geometry of the Bar.")]
        public static IGeometry Geometry3D(this Bar bar, bool onlyOuterExtrusion = true)
        {
            if (bar.IsNull())
                return null;

            // . If the profile is made of two curves (e.g. I section), selects only the outermost.
            IEnumerable<IGeometry> extrusions = bar.Extrude(false);
            Extrusion barOutermostExtrusion = extrusions.OfType<Extrusion>().OrderBy(extr => Engine.Geometry.Query.IArea(extr.Curve)).First();

            if (onlyOuterExtrusion)
                return barOutermostExtrusion;
            else
                return new CompositeGeometry() { Elements = extrusions.ToList() };
        }

        /***************************************************/

        [Description("Gets a CompositeGeometry made of the boundary surfaces of the Panel, or only its central Surface.")]
        [Input("panel", "The input panel to get the Geometry3D out of.")]
        [Input("onlyCentralSurface", "If true, the returned geometry is only the central (middle) surface of the panel. Otherwise, the whole external solid is returned as a CompositeGeometry of many surfaces.")]
        [Output("3d", "Three-dimensional geometry of the Panel.")]
        public static IGeometry Geometry3D(this Panel panel, bool onlyCentralSurface = false)
        {
            if (panel.IsNull())
                return null;

            PlanarSurface centralPlanarSurface = Engine.Geometry.Create.PlanarSurface(
                    Engine.Geometry.Compute.IJoin(panel.ExternalEdges.Select(x => x.Curve).ToList()).FirstOrDefault(),
                    panel.Openings.SelectMany(x => Engine.Geometry.Compute.IJoin(x.Edges.Select(y => y.Curve).ToList())).Cast<ICurve>().ToList());

            if (onlyCentralSurface)
                return centralPlanarSurface;
            else
            {
                CompositeGeometry compositeGeometry = new CompositeGeometry();

                double thickness = panel.Property.IVolumePerArea();
                Vector translateVect = new Vector() { Z = -thickness / 2 };
                Vector extrudeVect = new Vector() { Z = thickness };

                Vector upHalf = new Vector() { X = 0, Y = 0, Z = thickness / 2 };
                Vector downHalf = new Vector() { X = 0, Y = 0, Z = -thickness / 2 };

                PlanarSurface topSrf = centralPlanarSurface.ITranslate(upHalf) as PlanarSurface;
                PlanarSurface botSrf = centralPlanarSurface.ITranslate(downHalf) as PlanarSurface;

                IEnumerable<ICurve> internalEdgesBot = panel.InternalElementCurves().Select(c => c.ITranslate(translateVect));
                IEnumerable<Extrusion> internalEdgesExtrusions = internalEdgesBot.Select(c => BH.Engine.Geometry.Create.Extrusion(c, extrudeVect));

                IEnumerable<ICurve> externalEdgesBot = panel.ExternalEdges.Select(c => c.Curve.ITranslate(translateVect));
                IEnumerable<Extrusion> externalEdgesExtrusions = externalEdgesBot.Select(c => BH.Engine.Geometry.Create.Extrusion(c, extrudeVect));

                compositeGeometry.Elements.Add(topSrf);
                compositeGeometry.Elements.Add(botSrf);
                compositeGeometry.Elements.AddRange(internalEdgesExtrusions);
                compositeGeometry.Elements.AddRange(externalEdgesExtrusions);

                return compositeGeometry;

            }
        }

        /***************************************************/

        [Description("Gets the BH.oM.Geometry.Extrusion out of the Pile as its Geometry3D.")]
        [Input("pile", "The input Pile to get the Geometry3D out of, i.e.its extrusion with its cross section along its centreline.")]
        [Output("3d", "Three-dimensional geometry of the Pile.")]
        public static IGeometry Geometry3D(this Pile pile)
        {
            return Create.Bar((Line)pile.Geometry(), pile.Section, pile.OrientationAngle).Geometry3D();
        }

        /***************************************************/

        [Description("Gets a CompositeGeometry made of the boundary surfaces of the PadFoundation, or only its central Surface.")]
        [Input("pad", "The input Panel to get the Geometry3D out of.")]
        [Output("3d", "Three-dimensional geometry of the PadFoundation.")]
        public static IGeometry Geometry3D(this PadFoundation pad)
        {
            if (pad.IsNull() || !pad.IsPlanar())
                return null;

            PlanarSurface top = Engine.Geometry.Create.PlanarSurface(pad.TopOutline);

            CompositeGeometry compositeGeometry = new CompositeGeometry();

            double thickness = pad.Property.IVolumePerArea();
            Vector translateVect = -top.Normal() * pad.Property.ITotalThickness(); // negative because the pad contains a top outline

            PlanarSurface bot = top.ITranslate(translateVect) as PlanarSurface;

            IEnumerable<Extrusion> externalEdgesExtrusions = pad.TopOutline.SubParts().Select(c => Engine.Geometry.Create.Extrusion(c, translateVect));

            compositeGeometry.Elements.Add(top);
            compositeGeometry.Elements.Add(bot);
            compositeGeometry.Elements.AddRange(externalEdgesExtrusions);

            return compositeGeometry;
        }

        [Description("Gets a CompositeGeometry made of the PileCap and Piles of a PileFoundation.")]
        [Input("pileFoundation", "The input PileFoundation to get the Geometry3D out of.")]
        [Output("3d", "Three-dimensional geometry of the PileFoundation.")]
        public static IGeometry Geometry3D(this PileFoundation pileFoundation)
        {
            if (pileFoundation.IsNull())
                return null;

            CompositeGeometry compositeGeometry = new CompositeGeometry();
            compositeGeometry.Elements.Add(pileFoundation.PileCap.Geometry3D());
            compositeGeometry.Elements.AddRange(pileFoundation.Piles.Select(x => x.Geometry3D()));

            return compositeGeometry;
        }

        /***************************************************/

        [Description("Gets a CompositeGeometry made of the PileCap and Piles of a PileFoundation.")]
        [Input("pileFoundation", "The input PileFoundation to get the Geometry3D out of.")]
        [Output("3d", "Three-dimensional geometry of the PileFoundation.")]
        public static IGeometry Geometry3D(this RTBase rTBase)
        {
            if (rTBase.IsNull())
                return null;

            CompositeGeometry compositeGeometry = new CompositeGeometry();

            PlanarSurface botSrf = Engine.Geometry.Create.PlanarSurface(rTBase.Outline);


            double thickness = rTBase.Property.ITotalThickness();
            Vector extrudeVect = new Vector() { Z = thickness };

            PlanarSurface topSrf = botSrf.ITranslate(extrudeVect) as PlanarSurface;

            Extrusion externalEdgesExtrusion = Engine.Geometry.Create.Extrusion(rTBase.Outline, extrudeVect);

            compositeGeometry.Elements.Add(topSrf);
            compositeGeometry.Elements.Add(botSrf);
            compositeGeometry.Elements.Add(externalEdgesExtrusion);

            return compositeGeometry;
        }

        /***************************************************/

        [Description("Gets a CompositeGeometry made of the PileCap and Piles of a PileFoundation.")]
        [Input("pileFoundation", "The input PileFoundation to get the Geometry3D out of.")]
        [Output("3d", "Three-dimensional geometry of the PileFoundation.")]
        public static IGeometry Geometry3D(this Stem stem)
        {
            if (stem.IsNull())
                return null;

            CompositeGeometry compositeGeometry = new CompositeGeometry();

            PlanarSurface centralPlanarSrf = Engine.Geometry.Create.PlanarSurface(stem.Outline);

            PlanarSurface retainingSrf = centralPlanarSrf.ITranslate(stem.Orientation.Normalise() * -stem.ThicknessBottom / 2) as PlanarSurface;

            PlanarSurface frontsrf = centralPlanarSrf.ITranslate(stem.Orientation.Normalise() * stem.ThicknessBottom / 2) as PlanarSurface;

            //Figure out angle to rotate frontface by. Need a value of stem bound in Z direction. think there is something within BHoM to handle that.
            double a = stem.ThicknessBottom - stem.ThicknessTop;
            double b = 5; // THis is a palceholder value and will be changed.
            double rad = Math.Atan(a / b);

            frontsrf = frontsrf.IRotate(frontsrf.ICentroid(), stem.Orientation.CrossProduct(Vector.ZAxis), rad).ITranslate(stem.Orientation * -Math.Tan(rad) * b/2) as PlanarSurface;

            Extrusion externalEdgesExtrusion = Engine.Geometry.Create.Extrusion(stem.Outline.ITranslate(stem.Orientation.Normalise() * -stem.ThicknessBottom / 2), stem.Orientation.Normalise() * stem.ThicknessBottom);


            compositeGeometry.Elements.Add(retainingSrf);
            compositeGeometry.Elements.Add(frontsrf);
            compositeGeometry.Elements.Add(externalEdgesExtrusion);

            return compositeGeometry;

        }

        /***************************************************/

        [Description("Gets a CompositeGeometry made of the PileCap and Piles of a PileFoundation.")]
        [Input("pileFoundation", "The input PileFoundation to get the Geometry3D out of.")]
        [Output("3d", "Three-dimensional geometry of the PileFoundation.")]
        public static IGeometry Geometry3D(this RetainingWall retainingWall)
        {
            if (retainingWall.IsNull())
                return null;

            CompositeGeometry compositeGeometry = new CompositeGeometry();
            compositeGeometry.Elements.Add(retainingWall.Stem.Geometry3D());
            compositeGeometry.Elements.Add(retainingWall.RTBase.Geometry3D());

            return compositeGeometry;
        }
    }
}





