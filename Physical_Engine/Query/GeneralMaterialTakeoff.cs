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
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the general material takeoff from the ISurface object. The takeoff will contain materials and volumes, mass and areas from the surface itself, as well as takeoffs from any openings, such as doors and windows.")]
        [Input("surface", "The physical surface object to extract the general material takeoff from.")]
        [Output("genTakeoff", "The general material takeoff based on buildup of the surface object as well as any of its inner objects, such as doors and windows.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this ISurface surface)
        {
            if (surface == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a null {nameof(ISurface)}.");
                return null;
            }

            if (surface.Location == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a {nameof(ISurface)} with null geometry.");
                return null;
            }

            if (surface.Construction == null)
            {
                Engine.Base.Compute.RecordError($"The {nameof(GeneralMaterialTakeoff)} could not be queried as no {nameof(IConstruction)} has been assigned to the {nameof(ISurface)}.");
                return null;
            }

            double openingAreas = 0;

            List<GeneralMaterialTakeoff> takeoffs = new List<GeneralMaterialTakeoff>();

            foreach (IOpening opening in surface.Openings)
            {
                GeneralMaterialTakeoff opeingTakeoff = opening.IGeneralMaterialTakeoff();
                if (opeingTakeoff == null)
                    return null;

                takeoffs.Add(opeingTakeoff);
                openingAreas += opening.Area();

            }
            takeoffs.Add(IGeneralMaterialTakeoff(surface.Construction, surface.Location.IArea() - openingAreas));

            return Matter.Compute.AggregateGeneralMaterialTakeoff(takeoffs);
        }

        /***************************************************/

        [Description("Gets the volumetric material takeoff from the Void object. This will always return an empty takeoff as a void represent an area of no materiality.")]
        [Input("opening", "The physical opening object to extract the volumetric material takeoff from.")]
        [Output("genTakeoff", "The volumetric material takeoff for the opening. For a void this will alwys be an empty Material Takeoff.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this BH.oM.Physical.Elements.Void opening)
        {
            //Voids represent empty space, hence returning a completely empty composition
            return new GeneralMaterialTakeoff();
        }

        /***************************************************/

        [Description("Gets the volumetric material takeoff from the Window.")]
        [Input("opening", "The physical opening object to extract the volumetric material takeoff from.")]
        [Output("genTakeoff", "The volumetric material takeoff for the opening, made of up the volume and materiality of the construction and surface area.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Window opening)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a null {nameof(Door)}.");
                return null;
            }

            if (opening.Location == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a {nameof(Door)} with null geometry.");
                return null;
            }

            if (opening.Construction as Construction == null)
            {
                Engine.Base.Compute.RecordError($"The {nameof(GeneralMaterialTakeoff)} could not be queried as no {nameof(IConstruction)} has been assigned to the {nameof(Door)}.");
                return null;
            }

            return IGeneralMaterialTakeoff(opening.Construction, opening.Location.IArea());
        }

        /***************************************************/

        [Description("Gets the volumetric material takeoff from the Door.")]
        [Input("opening", "The physical opening object to extract the volumetric material takeoff from.")]
        [Output("genTakeoff", "The volumetric material takeoff for the opening, made of up the volume and materiality of the construction and surface area.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Door opening)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a null {nameof(Door)}.");
                return null;
            }

            if (opening.Location == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot query the {nameof(GeneralMaterialTakeoff)} of a {nameof(Door)} with null geometry.");
                return null;
            }

            if (opening.Construction as Construction == null)
            {
                Engine.Base.Compute.RecordError($"The {nameof(GeneralMaterialTakeoff)} could not be queried as no {nameof(IConstruction)} has been assigned to the {nameof(Door)}.");
                return null;
            }

            return IGeneralMaterialTakeoff(opening.Construction, opening.Location.IArea());
        }

        /***************************************************/


        [Description("Gets the GeneralMaterialTakeoff takeoff from the IConstruction given an area.")]
        [Input("construction", "The physical IConstruction object to extract the general material takeoff from.")]
        [Input("area", "The area on where the construction is applied.", typeof(Area))]
        [Output("genTakeoff", "The GeneralMaterialTakeoff for the IConstruction, made of up the volume, area and mass and materiality of the construction.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Construction construction, double area)
        {
            if (construction == null)
            {
                return null;
            }

            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff();

            foreach (Layer layer in construction.Layers)
            {
                TakeoffItem item = new TakeoffItem
                {
                    Material = layer.Material,
                    Volume = area * layer.Thickness,
                    Area = area,
                    Mass = layer.Material.Density * area * layer.Thickness,
                    NumberItem = 1,
                };
                takeoff.MaterialTakeoffItems.Add(item);
            }

            return takeoff;
        }


        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets the volumetric material takeoff from the IOpening.")]
        [Input("opening", "The physical opening object to extract the volumetric material takeoff from.")]
        [Output("genTakeoff", "The volumetric material takeoff for the opening, made of up the volume and materiality of the construction and surface area.")]
        public static GeneralMaterialTakeoff IGeneralMaterialTakeoff(this IOpening opening)
        {
            return GeneralMaterialTakeoff(opening as dynamic);
        }

        /***************************************************/

        [Description("Gets the GeneralMaterialTakeoff takeoff from the IConstruction given an area.")]
        [Input("construction", "The physical IConstruction object to extract the general material takeoff from.")]
        [Input("area", "The area on where the construction is applied.", typeof(Area))]
        [Output("genTakeoff", "The GeneralMaterialTakeoff for the IConstruction, made of up the volume, area and mass and materiality of the construction.")]
        public static GeneralMaterialTakeoff IGeneralMaterialTakeoff(this IConstruction construction, double area)
        {
            return GeneralMaterialTakeoff(construction as dynamic, area);
        }

        /***************************************************/
    }
}
