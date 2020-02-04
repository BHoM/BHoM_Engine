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

using BH.Engine.Common;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Panel Panel(ICurve outline, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();

            return Panel(externalEdges, openings, property, name);
        }

        /***************************************************/

        public static Panel Panel(ICurve outline, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();
            return Panel(externalEdges, pOpenings, property, name);
        }

        /***************************************************/

        public static Panel Panel(List<Edge> externalEdges, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            return Panel(externalEdges, pOpenings, property, name);
        }

        /***************************************************/

        public static Panel Panel(List<Edge> externalEdges, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            return new Panel
            {
                ExternalEdges = externalEdges,
                Openings = openings ?? new List<Opening>(),
                Property = property,
                Name = name
            };
        }

        /***************************************************/

        [DeprecatedAttribute("Generic method for ICurve in place")]
        public static List<Panel> PanelPlanar(List<Polyline> outlines, ISurfaceProperty property = null, string name = "")
        {
            return Panel(new List<ICurve>(outlines), property, name);
        }

        /***************************************************/

        public static List<Panel> Panel(List<ICurve> outlines, ISurfaceProperty property = null, string name = "")
        {
            List<Panel> result = new List<Panel>();
            List<List<IElement1D>> outlineEdges = outlines.Select(x => x.ISubParts().Select(y => new Edge { Curve = y } as IElement1D).ToList()).ToList();
            
            List<List<List<IElement1D>>> sortedOutlines = outlineEdges.DistributeOutlines(true);
            foreach (List<List<IElement1D>> panelOutlines in sortedOutlines)
            {
                Panel panel = new Panel();
                panel = panel.SetOutlineElements1D(panelOutlines[0]);
                List<Opening> openings = new List<Opening>();
                foreach (List<IElement1D> p in panelOutlines.Skip(1))
                {
                    Opening opening = (new Opening()).SetOutlineElements1D(p);
                    openings.Add(opening);
                }
                panel.Openings = openings;
                panel.Property = property;
                panel.Name = name;
                result.Add(panel);
            }
            return result;
        }

        /***************************************************/

        [Deprecated("2.3", "Name of class and method changed from PanelPlanar to Panel", null, "Panel")]
        public static Panel PanelPlanar(ICurve outline, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            return Panel(outline, openings, property, name);
        }

        /***************************************************/

        [Deprecated("2.3", "Name of class and method changed from PanelPlanar to Panel", null, "Panel")]
        public static Panel PanelPlanar(ICurve outline, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            return Panel(outline, openings, property, name);
        }

        /***************************************************/

        [Deprecated("2.3", "Name of class and method changed from PanelPlanar to Panel", null, "Panel")]
        public static Panel PanelPlanar(List<Edge> externalEdges, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            return Panel(externalEdges, openings, property, name);
        }

        /***************************************************/

        [Deprecated("2.3", "Name of class and method changed from PanelPlanar to Panel", null, "Panel")]
        public static Panel PanelPlanar(List<Edge> externalEdges, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            return Panel(externalEdges, openings, property, name);
        }

        /***************************************************/

        [Deprecated("2.3", "Name of class and method changed from PanelPlanar to Panel", null, "Panel")]
        public static List<Panel> PanelPlanar(List<ICurve> outlines, ISurfaceProperty property = null, string name = "")
        {
            return Panel(outlines, property, name);
        }

        /***************************************************/
    }
}


