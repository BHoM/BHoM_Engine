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

        [Description("Creates a physical floor element. For elements for structral analytical applications look at BH.oM.Structure.Elements.Panel. For elements for environmental analytical applications look at BH.oM.Environments.Elements.Panel.")]
        [Input("location", "Location surface which represents the outer geometry of the floor. Should not contain any openings.")]
        [Input("construction", "Construction representing the thickness and materiality of the floor.")]
        [Input("openings", "Openings of the floor. Could be simple voids or more detailed objects.")]
        [Input("offset", "Represents the positioning of the construction in relation to the location surface of the floor.")]
        [Input("name", "The name of the floor, default empty string.")]
        [Output("Floor", "The created physical floor.")]
        public static Floor Floor(oM.Geometry.ISurface location, IConstruction construction, List<IOpening> openings = null, Offset offset = Offset.Undefined, string name = "")
        {
            openings = openings ?? new List<IOpening>();
            return new Floor
            {
                Location = location,
                Construction = construction,
                Openings = openings,
                Offset = offset,
                Name = name
            };
        }

        /***************************************************/

        [Description("Creates physical floor based on given construction and external edges.")]
        [Input("construction", "Construction of the floor.")]
        [Input("edges", "External edges of the floor (Profile - planar closed curve).")]
        [Output("floor", "A physical floor.")]
        public static Floor Floor(Construction construction, ICurve edges)
        {
            return Floor(construction, edges, null);
        }

        /***************************************************/

        [Description("Creates physical floor based on given construction, external and internal edges.")]
        [Input("construction", "Construction of the floor.")]
        [Input("edges", "External edges of the floor (Profile - planar closed curve).")]
        [Input("internalEdges", "Internal edges of openings.")]
        [Output("floor", "A physical floor.")]
        public static Floor Floor(Construction construction, ICurve edges, IEnumerable<ICurve> internalEdges = null)
        {
            if (construction == null || edges == null)
            {
                Base.Compute.RecordError("Physical Floor could not be created because some input data are null.");
                return null;
            }

            //Create the location for the floor
            PlanarSurface location = Geometry.Create.PlanarSurface(edges);
            if (location == null)
            {
                Base.Compute.RecordError("Physical Floor could not be created because of invalid geometry of edges.");
                return null;
            }

            //Create the openings
            List<IOpening> openings = new List<IOpening>();

            if (internalEdges != null && internalEdges.Count() > 0)
            {
                foreach (ICurve openingCurve in internalEdges)
                    if (openingCurve != null)
                        openings.Add(new Void { Location = Geometry.Create.PlanarSurface(openingCurve) });
            }

            return Floor(location, construction, openings);
        }
        
        /***************************************************/
    }
}
