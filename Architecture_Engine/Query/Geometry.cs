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

using BH.Engine.Geometry;
using BH.oM.Architecture.BuildersWork;
using BH.oM.Architecture.Elements;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Architecture
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extracts geometry of the given Grid.")]
        [Input("grid", "Grid to be queried for its geometry.")]
        [Output("Geometry of the input Grid.")]
        public static ICurve Geometry(this Grid grid)
        {
            return grid?.Curve;
        }

        /***************************************************/

        [Description("Extracts geometry of the given Theatron.")]
        [Input("theatron", "Theatron to be queried for its geometry.")]
        [Output("Geometry of the input Theatron.")]
        public static CompositeGeometry Geometry(this oM.Architecture.Theatron.TheatronGeometry theatron)
        {
            return Engine.Geometry.Create.CompositeGeometry(theatron?.Tiers3d?.SelectMany(x => x?.TierBlocks).Select(x => x?.Floor));
        }

        /***************************************************/

        [Description("Extracts geometry of the given TheatronFullProfile.")]
        [Input("theatronFullProfile", "TheatronFullProfile to be queried for its geometry.")]
        [Output("Geometry of the input TheatronFullProfile.")]
        public static CompositeGeometry Geometry(this oM.Architecture.Theatron.TheatronFullProfile theatronFullProfile)
        {
            return Engine.Geometry.Create.CompositeGeometry(theatronFullProfile?.BaseTierProfiles?.Select(x => x?.Profile));
        }

        /***************************************************/

        [Description("Extracts geometry of the given BuildersWork Opening.")]
        [Input("opening", "BuildersWork Opening to be queried for its geometry.")]
        [Output("Geometry of the input BuildersWork Opening.")]
        public static Point Geometry(this Opening opening)
        {
            if (opening == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot extract the geometry from a null opening.");
                return null;
            }

            if (opening.CoordinateSystem == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot extract the geometry from an opening without coordinate system.");
                return null;
            }

            return opening.CoordinateSystem.Origin;
        }

        /***************************************************/
    }
}


