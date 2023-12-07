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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Ground;
using BH.Engine.Base;

namespace BH.Engine.Ground
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Borehole object containing the start, end, geology and a number of optional parameters. The coordinate system the points are provided in is included.")]
        [Input("id", "Location identifier for the borehole unique to the project (LOCA_ID).")]
        [Input("top", "The top of the borehole within the coordinate system provided (LOCA_NATE, LOCA_NATEN, LOCA_GL).")]
        [Input("bottom", "The bottom of the borehole within the coordinate system provided (LOCA_ETRV, LOCA_NTRV, LOCA_FDEP).")]
        [Input("strata", "A list of strata objects containing geology units and descriptions of the ground.")]
        [Input("properties", "A list of properties related to the borehole.")]
        [Input("coordinateSystem", "The coordinate system referenced by the top and bottom point. (LOCA_GREF, LOCA_NATD).")]
        [Output("borehole", "The created Borehole defined by a coordinate system, start point and end point based on the AGS schema.")]
        public static Borehole Borehole(string id, Point top = null, Point bottom = null, Cartesian coordinateSystem = null, List<IBoreholeProperty> properties = null, List<Stratum> strata = null, List<ContaminantSample> contaminants = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Compute.RecordError("The id is null or whitespace.");
                return null;
            }

            return new Borehole() { Id = id, Top = top, Bottom = bottom, Strata = strata, BoreholeProperties = properties,ContaminantSamples = contaminants, CoordinateSystem = coordinateSystem };

        }

        /***************************************************/
    }
}




