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

using System.ComponentModel;

using BH.oM.Physical.Constructions;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates physical roof based on given construction and edges")]
        [Input("construction", "Construction of the roof")]
        [Input("edges", "External edges of the roof (Profile - planar closed curve)")]
        [Output("roof", "A physical roof")]
        public static Roof Roof(IConstruction construction, ICurve edges)
        {
            return Roof(construction, edges, null);
        }

        /***************************************************/

        [Description("Creates a physical Roof element. For elements for structral analytical applications look at BH.oM.Structure.Elements.Panel. For elements for environmental analytical applications look at BH.oM.Environments.Elements.Panel.")]
        [Input("construction", "Construction representing the thickness and materiality of the Roof.")]
        [Input("location", "Location surface which represents the outer geometry of the Roof. Should not contain any openings.")]
        [Input("openings", "Openings of the Roof. Could be simple voids or more detailed objects.")]
        [Input("offset", "Represents the positioning of the construction in relation to the location surface of the Roof.")]
        [Input("name", "The name of the roof, default empty string.")]
        [Output("Roof", "The created physical Roof.")]
        public static Roof Roof(IConstruction construction, oM.Geometry.ISurface location, List<IOpening> openings = null, Offset offset = Offset.Undefined, string name = "")
        {
            openings = openings ?? new List<IOpening>();

            return new Roof
            {
                Location = location,
                Construction = construction,
                Openings = openings,
                Offset = offset,
                Name = name
            };
        }

        /***************************************************/

        [Description("Creates physical roof based on given construction, external and internal edges.")]
        [Input("construction", "Construction of the roof.")]
        [Input("edges", "External edges of the roof (Profile - planar closed curve).")]
        [Input("internalEdges", "Internal edges of openings etc.")]
        [Output("roof", "A physical roof.")]
        public static Roof Roof(IConstruction construction, ICurve edges, IEnumerable<ICurve> internalEdges)
        {
            if (construction == null || edges == null)
            {
                Base.Compute.RecordError("Physical Roof could not be created because some input data are null.");
                return null;
            }

            List<ICurve> aInternalCurveList = null;
            if (internalEdges != null && internalEdges.Count() > 0)
                aInternalCurveList = internalEdges.ToList().ConvertAll(x => x as ICurve);

            PlanarSurface aPlanarSurface = Geometry.Create.PlanarSurface(edges, aInternalCurveList);
            if (aPlanarSurface == null)
            {
                Base.Compute.RecordError("Physical Roof could not be created because invalid geometry of edges.");
                return null;
            }

            return new Roof()
            {
                Construction = construction,
                Location = aPlanarSurface
            };
        }

        /***************************************************/
    }
}






