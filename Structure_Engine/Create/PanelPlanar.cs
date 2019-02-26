/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Structure.Properties.Surface;
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

        public static PanelPlanar PanelPlanar(ICurve outline, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();

            return PanelPlanar(externalEdges, openings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();
            return PanelPlanar(externalEdges, pOpenings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(List<Edge> externalEdges, List<ICurve> openings = null, ISurfaceProperty property = null, string name = "")
        {
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            return PanelPlanar(externalEdges, pOpenings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(List<Edge> externalEdges, List<Opening> openings = null, ISurfaceProperty property = null, string name = "")
        {
            return new PanelPlanar
            {
                ExternalEdges = externalEdges,
                Openings = openings ?? new List<Opening>(),
                Property = property,
                Name = name
            };
        }

        /***************************************************/

        [DeprecatedAttribute("Generic method for ICurve in place")]
        public static List<PanelPlanar> PanelPlanar(List<Polyline> outlines, ISurfaceProperty property = null, string name = "")
        {
            return PanelPlanar(new List<ICurve>(outlines), property, name);
        }

        /***************************************************/

        public static List<PanelPlanar> PanelPlanar(List<ICurve> outlines, ISurfaceProperty property = null, string name = "")
        {
            List<PanelPlanar> result = new List<PanelPlanar>();
            List<List<IElement1D>> outlineEdges = outlines.Select(x => x.ISubParts().Select(y => new Edge { Curve = y } as IElement1D).ToList()).ToList();
            
            List<List<List<IElement1D>>> sortedOutlines = outlineEdges.DistributeOutlines();
            foreach (List<List<IElement1D>> panelOutlines in sortedOutlines)
            {
                PanelPlanar panel = new PanelPlanar();
                panel = panel.SetOutlineElements1D(panelOutlines[0]);
                List<IElement2D> openings = new List<IElement2D>();
                foreach (List<IElement1D> p in panelOutlines.Skip(1))
                {
                    IElement2D opening = (panel.NewInternalElement2D() as Opening).SetOutlineElements1D(p);
                    openings.Add(opening);
                }
                panel = panel.SetInternalElements2D(openings);
                panel.Property = property;
                panel.Name = name;
                result.Add(panel);
            }
            return result;
        }

        /***************************************************/
    }
}

