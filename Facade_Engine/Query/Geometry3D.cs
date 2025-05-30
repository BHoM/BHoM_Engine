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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base.Attributes;
using BH.oM.Facade.Elements;
using BH.Engine.Physical;
using BH.oM.Physical.Constructions;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets a CompositeGeometry made of the boundary surfaces of the Panel envelope, or only its central Surface.")]
        [Input("panel", "The input panel to get the Geometry3D out of.")]
        [Input("separateLayerGeometry", "If true, the returned geometry includes separate surface per construction layer.")]
        [Output("3d", "Three-dimensional geometry of the Panel envelope per layer/center surface.")]
        public static IGeometry Geometry3D(this Panel panel, bool separateLayerGeometry = false)
        {
            if (panel == null)
            {
                Base.Compute.RecordError("Cannot evaluate geometry because the Panel is null.");
                return null;
            }
            else if (panel.ExternalEdges == null || panel.ExternalEdges.Count == 0)
            {
                Base.Compute.RecordError("Cannot evaluate geometry because the panel does not have valid external edge geometry.");
                return null;
            }

            PlanarSurface centralPlanarSurface = Engine.Geometry.Create.PlanarSurface(
                    Engine.Geometry.Compute.IJoin(panel.ExternalEdges.Select(x => x.Curve).ToList()).FirstOrDefault(),
                    panel.Openings.SelectMany(x => Engine.Geometry.Compute.IJoin(x.Edges.Select(y => y.Curve).ToList())).Cast<ICurve>().ToList());

            // Check if the panel.Construction is a Construction
            if (!(panel.Construction is Construction))
                return null;
            Construction construction = panel.Construction as Construction;

            CompositeGeometry compositeGeometry = new CompositeGeometry();
            Vector localZ = centralPlanarSurface.Normal().Normalise();
            double thickness = panel.Construction.IThickness();

            Vector translateVect = localZ * -thickness / 2;

            Vector upHalf = localZ * thickness / 2;
            Vector downHalf = localZ * -thickness / 2;

            if (!separateLayerGeometry)
            {
                Vector extrudeVect = localZ * thickness;

                PlanarSurface topSrf = centralPlanarSurface.ITranslate(upHalf) as PlanarSurface;
                PlanarSurface botSrf = centralPlanarSurface.ITranslate(downHalf) as PlanarSurface;

                IEnumerable<ICurve> internalEdgesBot = panel.InternalElementCurves().Select(c => c.ITranslate(translateVect));
                IEnumerable<Extrusion> internalEdgesExtrusions = internalEdgesBot.Select(c => BH.Engine.Geometry.Create.Extrusion(c, extrudeVect, false));

                IEnumerable<ICurve> externalEdgesBot = panel.ExternalEdges.Select(c => c.Curve.ITranslate(translateVect));
                IEnumerable<Extrusion> externalEdgesExtrusions = externalEdgesBot.Select(c => BH.Engine.Geometry.Create.Extrusion(c, extrudeVect, false));

                compositeGeometry.Elements.Add(topSrf);
                compositeGeometry.Elements.Add(botSrf);
                compositeGeometry.Elements.AddRange(internalEdgesExtrusions);
                compositeGeometry.Elements.AddRange(externalEdgesExtrusions);

                return compositeGeometry;

            }
            else
            {
                Vector curLayerTranslation = new Vector();

                PlanarSurface botSrf = centralPlanarSurface.ITranslate(downHalf) as PlanarSurface;
                IEnumerable<ICurve> internalEdgesBot = panel.InternalElementCurves().Select(c => c.ITranslate(downHalf));
                IEnumerable<ICurve> externalEdgesBot = panel.ExternalEdges.Select(c => c.Curve.ITranslate(downHalf));

                foreach (Layer layer in construction.Layers)
                {
                    Vector extrudeVect = localZ * layer.Thickness;
                    PlanarSurface topSrf = botSrf.ITranslate(extrudeVect) as PlanarSurface;

                    IEnumerable<Extrusion> internalEdgesExtrusions = botSrf.InternalEdges().Select(c => BH.Engine.Geometry.Create.Extrusion(c, extrudeVect, false));
                    IEnumerable<Extrusion> externalEdgesExtrusions = botSrf.IExternalEdges().Select(c => BH.Engine.Geometry.Create.Extrusion(c, extrudeVect, false));
                    compositeGeometry.Elements.Add(topSrf);
                    compositeGeometry.Elements.Add(botSrf);
                    compositeGeometry.Elements.AddRange(internalEdgesExtrusions);
                    compositeGeometry.Elements.AddRange(externalEdgesExtrusions);

                    botSrf = botSrf.Translate(extrudeVect);
                }

                return compositeGeometry;
            }
        }
    }
}

