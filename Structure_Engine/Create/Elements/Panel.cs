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

using BH.Engine.Spatial;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Geometry;
using BH.Engine.Base;
using System.ComponentModel;
using BH.Engine.Analytical;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural Panel from its fundamental parts and an local orientation vector.")]
        [InputFromProperty("externalEdges")]
        [InputFromProperty("openings")]
        [InputFromProperty("property")]
        [Input("localX", "Vector to set as local x of the Panel. Default value of null gives default orientation. If this vector is not in the plane of the Panel it will get projected. If the vector is parallel to the normal of the Panel the operation will fail and the Panel local orientation will be set to default.")]
        [Input("name", "The name of the created Panel.")]
        [Output("panel", "The created Panel.")]
        public static Panel Panel(List<Edge> externalEdges, List<Opening> openings = null, ISurfaceProperty property = null, Vector localX = null, string name = "")
        {
            if (externalEdges.IsNullOrEmpty() || externalEdges.Any(x => x.IsNull()))
                return null;

            Panel panel = new Panel
            {
                ExternalEdges = externalEdges,
                Openings = openings ?? new List<Opening>(),
                Property = property,
                Name = name
            };

            if (localX != null)
                return panel.SetLocalOrientation(localX);
            else
                return panel;
        }

        /***************************************************/

        [PreviousVersion("6.1", "BH.Engine.Structure.Create.Panel(BH.oM.Geometry.ICurve, System.Collections.Generic.List<BH.oM.Geometry.ICurve>, BH.oM.Structure.SurfaceProperties.ISurfaceProperty, BH.oM.Geometry.Vector, System.String)")]
        [Description("Creates a structural Panel from a closed curve defining the outline, and any number of closed curves defining openings.")]
        [Input("outline", "A closed Curve defining the outline of the Panel. The ExternalEdges of the Panel will be the subparts of this curve, where each edge will corespond to one curve segment.")]
        [Input("openings", "A collection of closed curves representing the openings of the Panel.")]
        [InputFromProperty("property")]
        [Input("localX", "Vector to set as local x of the Panel. Default value of null gives default orientation. If this vector is not in the plane of the Panel it will get projected. If the vector is parallel to the normal of the Panel the operation will fail and the Panel local orientation will be set to default.")]
        [Input("name", "The name of the created Panel.")]
        [Input("tolerance", "Distance tolerance, used as a margin of error for checking that the outline is closed.", typeof(Length))]
        [Output("panel", "The created Panel.")]
        public static Panel Panel(ICurve outline, List<ICurve> openings = null, ISurfaceProperty property = null, Vector localX = null, string name = "", double tolerance = Tolerance.Distance)
        {
            if (outline.IsNull())
                return null;
            else if (!outline.IIsClosed(tolerance))
            {
                Base.Compute.RecordError("Outline is not closed. Could not create Panel.");
                return null;
            }
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).Where(x => x != null).ToList() : new List<Opening>();
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();

            return Create.Panel(externalEdges, pOpenings, property, localX, name);
        }

        /***************************************************/

        [PreviousVersion("6.0", "BH.Engine.Structure.Create.Panel(System.Collections.Generic.List<BH.oM.Geometry.ICurve>, BH.oM.Structure.SurfaceProperties.ISurfaceProperty, BH.oM.Geometry.Vector, System.String)")]
        [Description("Creates a list of Panels based on a collection of outline curves. \n" +
                     "Method will distribute the outlines such that the outermost curve will be assumed to be an external outline of a panel, and any curve contained in this outline will be assumed as an opening of this Panel. \n" +
                     "Any outline curve inside an opening will again be assumed to be the outline of a new Panel.")]
        [Input("outlines", "A collection of outline curves representing outlines of external edges and opening used to create the Panel(s).")]
        [InputFromProperty("property")]
        [Input("localX", "Vector to set as local x of the Panel. Default value of null gives default orientation. If this vector is not in the plane of the Panel it will get projected. If the vector is parallel to the normal of the Panel the operation will fail and the Panel local orientation will be set to default.")]
        [Input("name", "The name of the created Panel(s).")]
        [Input("tolerance", "Distance tolerance, used as a margin of error for comparing the outlines.", typeof(Length))]
        [Output("panel", "The created Panel(s).")]
        public static List<Panel> Panel(List<ICurve> outlines, ISurfaceProperty property = null, Vector localX = null, string name = "", double tolerance = Tolerance.Distance)
        {
            if (outlines.IsNullOrEmpty() || outlines.Any(x => x.IsNull()))
                return null;

            List<Panel> result = new List<Panel>();
            List<List<IElement1D>> outlineEdges = outlines.Select(x => x.ISubParts().Select(y => new Edge { Curve = y } as IElement1D).ToList()).ToList();

            List<List<List<IElement1D>>> sortedOutlines = outlineEdges.DistributeOutlines(true, tolerance);
            foreach (List<List<IElement1D>> panelOutlines in sortedOutlines)
            {
                Panel panel = new Panel();
                panel = panel.SetOutlineElements1D(panelOutlines[0]) as Panel;
                List<Opening> openings = new List<Opening>();
                foreach (List<IElement1D> p in panelOutlines.Skip(1))
                {
                    Opening opening = (new Opening()).SetOutlineElements1D(p) as Opening;
                    openings.Add(opening);
                }
                panel.Openings = openings;
                panel.Property = property;
                panel.Name = name;

                if (localX != null)
                    result.Add(panel.SetLocalOrientation(localX));
                else
                    result.Add(panel);
            }
            return result;
        }

        /***************************************************/

        [PreviousVersion("BH.Engine.Structure.Create.Panel(BH.oM.Geometry.PlanarSurface, BH.oM.Structure.SurfaceProperties.ISurfaceProperty, BH.oM.Geometry.Vector, System.String)")]
        [Description("Creates a structural Panel from a PlanarSurface, creating external edges from the ExternalBoundary and openings from the InternalBoundaries of the PlanarSurface.")]
        [Input("surface", "A planar surface used to define the geometry of the panel, i.e. the external edges and the openings.")]
        [InputFromProperty("property")]
        [Input("localX", "Vector to set as local x of the Panel. Default value of null gives default orientation. If this vector is not in the plane of the Panel it will get projected. If the vector is parallel to the normal of the Panel the operation will fail and the Panel local orientation will be set to default.")]
        [Input("name", "The name of the created Panel.")]
        [Input("tolerance", "Distance tolerance, used as a margin of error for checking that the outline is closed.", typeof(Length))]
        [Output("panel", "The created Panel.")]
        public static Panel Panel(PlanarSurface surface, ISurfaceProperty property = null, Vector localX = null, string name = "", double tolerance = Tolerance.Distance)
        {
            return surface.IsNull() ? null : Panel(surface.ExternalBoundary, surface.InternalBoundaries.ToList(), property, localX, name, tolerance);
        }

        /***************************************************/
        /**** Public Methods - ToBeRemoved               ****/
        /***************************************************/

        [ToBeRemoved("3.1", "Method that use a mixture of geometry and objects between edges and openings removed.")]
        public static Panel Panel(List<Edge> externalEdges, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            return Panel(externalEdges, pOpenings, property, null, name);
        }

        /***************************************************/

        [ToBeRemoved("3.1", "Method that use a mixture of geometry and objects between edges and openings removed.")]
        public static Panel Panel(ICurve outline, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();

            return Panel(externalEdges, openings, property, null, name);
        }

        /***************************************************/

    }
}





