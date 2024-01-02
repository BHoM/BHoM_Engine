/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Solids                   ****/
        /***************************************************/

        [Description("Gets the enclosed volume created by the BoundaryRepresentation Surfaces. This value is retrieved from the immutable property stored on the object itself.")]
        [Input("solid","The solid BoundaryRepresentaion to query the volume from.")]
        [Output("volume", "", typeof(Volume))]
        public static double Volume(this BoundaryRepresentation solid)
        {
            return solid.Volume;
        }

        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("cone", "The solid cone to query the volume from.")]
        [Output("volume", "", typeof(Volume))]
        public static double Volume(this Cone cone)
        {
            return (1.0 / 3.0) * Math.PI * Math.Pow(cone.Radius, 2) * cone.Height;
        }

        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("cuboid", "The cuboid to query the volume from.")]
        [Output("volume", "", typeof(Volume))]
        public static double Volume(this Cuboid cuboid)
        {
            return cuboid.Length * cuboid.Depth * cuboid.Height;
        }

        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("cylinder", "The cylinder to query the volume from.")]
        [Output("volume", "", typeof(Volume))]
        public static double Volume(this Cylinder cylinder)
        {
            return Math.PI * Math.Pow(cylinder.Radius, 2) * cylinder.Height;
        }

        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("sphere", "The sphere to query the volume from.")]
        [Output("volume", "", typeof(Volume))]
        public static double Volume(this Sphere sphere)
        {
            return (4.0 / 3.0) * Math.PI * Math.Pow(sphere.Radius, 3);
        }


        /***************************************************/

        [Description("Calculates the analytical solid volume.")]
        [Input("torus", "The torus to query the volume from.")]
        [Output("volume", "", typeof(Volume))]
        public static double Volume(this Torus torus)
        {
            return 2.0 * Math.Pow(Math.PI, 2) * Math.Pow(torus.RadiusMinor, 2) * torus.RadiusMajor;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the enclosed volume of a solid.")]
        [Input("solid", "The solid to query the volume from.")]
        [Output("volume", "", typeof(Volume))]
        public static double IVolume(this ISolid solid)
        {
            return Volume(solid as dynamic);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static double Volume(this ISolid solid)
        {
            Base.Compute.RecordError($"Volume is not implemented for ISolids of type: {solid.GetType().Name}.");
            return double.NaN;
        }

        /***************************************************/
    }
}



