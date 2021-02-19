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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Structure;
using BH.Engine.Geometry;
using BH.Engine.Spatial;

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
        public static IGeometry Geometry3D(this Bar bar, bool onlyOuterExtrusion = true)
        {
            // . If the profile is made of two curves (e.g. I section), selects only the outermost.
            IEnumerable<IGeometry> extrusions = bar.Extrude(false);
            Extrusion barOutermostExtrusion = extrusions.OfType<Extrusion>().OrderBy(extr => Engine.Geometry.Query.IArea(extr.Curve)).First();

            if (onlyOuterExtrusion)
                return barOutermostExtrusion;
            else
                return new CompositeGeometry() { Elements = extrusions.ToList() };
        }

        public static IGeometry Geometry3D(this Panel panel, bool onlyCentralSurface = false)
        {
            PlanarSurface centralPlanarSurface = Engine.Geometry.Create.PlanarSurface(
                    Engine.Geometry.Compute.IJoin(panel.ExternalEdges.Select(x => x.Curve).ToList()).FirstOrDefault(),
                    panel.Openings.SelectMany(x => Engine.Geometry.Compute.IJoin(x.Edges.Select(y => y.Curve).ToList())).Cast<ICurve>().ToList());

            if (onlyCentralSurface)
                return centralPlanarSurface;
            else
            {
                CompositeGeometry compositeGeometry = new CompositeGeometry();

                double thickness = panel.Property.IAverageThickness();
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
    }
}


