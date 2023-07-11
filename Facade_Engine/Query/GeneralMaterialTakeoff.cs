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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Constructions;
using BH.oM.Physical.Elements;
using BH.oM.Physical.Materials;
using BH.oM.Facade.Elements;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Facade.SectionProperties;
using BH.oM.Geometry;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the general material takeoff from the panel object. The takeoff will contain materials and volumes, mass and areas from the panel itself, as well as takeoffs from any openings.")]
        [Input("panel", "The facade panel object to extract the general material takeoff from.")]
        [Output("genTakeoff", "The general material takeoff based on buildup of the panel object as well as any of its openings.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Panel panel)
        {
            if (panel == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a null {nameof(Panel)}.");
                return null;
            }

            //Add null check for opening geo
            if (panel.ExternalEdges == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a {nameof(Panel)} with null geometry.");
                return null;
            }

            if (panel.Construction == null)
            {
                Engine.Base.Compute.RecordError($"The {nameof(GeneralMaterialTakeoff)} could not be queried as no {nameof(IConstruction)} has been assigned to the {nameof(Panel)}.");
                return null;
            }

            double openingAreas = 0;

            List<GeneralMaterialTakeoff> takeoffs = new List<GeneralMaterialTakeoff>();

            foreach (Opening opening in panel.Openings)
            {
                GeneralMaterialTakeoff openingTakeoff = opening.GeneralMaterialTakeoff();
                if (openingTakeoff == null)
                    continue;

                takeoffs.Add(openingTakeoff);
                openingAreas += opening.Area();
            }

            foreach (FrameEdge edge in panel.ExternalEdges)
            {
                takeoffs.Add(GeneralMaterialTakeoff(edge));
            }
            takeoffs.Add(Physical.Query.IGeneralMaterialTakeoff(panel.Construction, panel.Area()));

            return Matter.Compute.AggregateGeneralMaterialTakeoff(takeoffs);
        }

        /***************************************************/

        [Description("Gets the volumetric material takeoff from the Opening.")]
        [Input("opening", "The facade opening object to extract the general material takeoff from.")]
        [Output("genTakeoff", "The general material takeoff for the opening.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Opening opening)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a null {nameof(Opening)}.");
                return null;
            }

            if (opening.Edges == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a {nameof(Opening)} with null geometry.");
                return null;
            }

            if (opening.OpeningConstruction as Construction == null)
            {
                Engine.Base.Compute.RecordError($"The {nameof(GeneralMaterialTakeoff)} could not be queried as no {nameof(IConstruction)} has been assigned to the {nameof(Opening)}.");
                return null;
            }

            List<GeneralMaterialTakeoff> takeoffs = new List<GeneralMaterialTakeoff>();

            foreach (FrameEdge edge in opening.Edges)
            {
                takeoffs.Add(GeneralMaterialTakeoff(edge));
            }
            takeoffs.Add(Physical.Query.IGeneralMaterialTakeoff(opening.OpeningConstruction, opening.ComponentAreas().Item1));

            return Matter.Compute.AggregateGeneralMaterialTakeoff(takeoffs);
        }

        /***************************************************/

        [Description("Gets the general material takeoff from the FrameEdge.")]
        [Input("edge", "The frame edge to extract the general material takeoff from.")]
        [Output("genTakeoff", "The general material takeoff for the FrameEdge.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this FrameEdge edge)
        {
            if (edge == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a null {nameof(FrameEdge)}.");
                return null;
            }

            if (edge.Curve == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a {nameof(FrameEdge)} with null geometry.");
                return null;
            }

            if (edge.FrameEdgeProperty == null)
            {
                Engine.Base.Compute.RecordError($"The {nameof(GeneralMaterialTakeoff)} could not be queried as no {nameof(FrameEdgeProperty)} has been assigned to the {nameof(FrameEdge)}.");
                return null;
            }

            VolumetricMaterialTakeoff volTakeoff = Matter.Create.VolumetricMaterialTakeoff(edge.MaterialComposition(), edge.SolidVolume());
            
            return Matter.Create.GeneralMaterialTakeoff(volTakeoff);
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets the volumetric material takeoff from the IOpening.")]
        [Input("opening", "The physical opening object to extract the general material takeoff from.")]
        [Output("genTakeoff", "The general material takeoff for the opening, made of up the volume and materiality of the construction and surface area.")]
        public static GeneralMaterialTakeoff IGeneralMaterialTakeoff(this IOpening opening)
        {
            return GeneralMaterialTakeoff(opening as dynamic);
        }


        /***************************************************/
        /**** Private Methods - Fallbacks               ****/
        /***************************************************/

        [Description("Gets the volumetric material takeoff from the object.")]
        [Input("obj", "The object to extract the material takeoff from.")]
        [Output("genTakeoff", "The volumetric material takeoff for the opening, made of up the volume and materiality of the construction and surface area.")]
        public static GeneralMaterialTakeoff IGeneralMaterialTakeoff(this object obj)
        {
            Base.Compute.RecordWarning("General Material Takeoff for object of type " + obj.GetType().Name + " is not implemented.");
            return null;
        }

        /***************************************************/
    }
}
