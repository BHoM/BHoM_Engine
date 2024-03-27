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

using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using BH.oM.Architecture.BuildersWork;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Spatial.ShapeProfiles;

namespace BH.Engine.Architecture
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Assign new geometry to an Architecture Room. If either geometry is null then original geometry is used")]
        [Input("room", "An Architecture Room to set the geometry of")]
        [Input("locationPoint", "A BHoM Geometry Point defining the location of the room, default null")]
        [Input("perimeterCurve", "A BHoM Geometry ICurve defining the perimeter of the room, default null")]
        [Output("room", "An Architecture Room with an updated geometry")]
        public static Room SetGeometry(this Room room, Point locationPoint = null, ICurve perimeterCurve = null)
        {
            if (room == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the geometry of a null room.");
                return room;
            }

            Room clone = room.DeepClone();
            clone.Location = locationPoint == null ? room.Location.DeepClone() : locationPoint;
            clone.Perimeter = perimeterCurve == null ? room.Perimeter.DeepClone() : perimeterCurve;
            return clone;
        }

        /***************************************************/

        [Description("Assign new geometry (location) to a BuildersWork Opening. Dimensions need to be set by updating the properties of Opening's profile.")]
        [Input("opening", "A BuildersWork Opening to set the geometry (location) of.")]
        [Input("locationPoint", "A BHoM geometry Point defining the new location of the Opening.")]
        [Output("opening", "A BuildersWork Opening with updated geometry (location).")]
        public static Opening SetGeometry(this Opening opening, Point locationPoint)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the geometry of a null opening.");
                return null;
            }

            if (locationPoint == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the geometry of an opening to a null point.");
                return null;
            }

            Opening clone = opening.ShallowClone();
            clone.CoordinateSystem = new Cartesian(locationPoint, opening.CoordinateSystem.X, opening.CoordinateSystem.Y, opening.CoordinateSystem.Z);

            BH.Engine.Base.Compute.RecordWarning("Only the location point of the opening has been updated - to update its dimension, please modify relevant properties of the Profile.");
            return clone;
        }

        /***************************************************/

        [Description("Assign new geometry (location) to a BuildersWork Opening. Dimensions need to be set by updating the properties of Opening's profile.")]
        [Input("opening", "A BuildersWork Opening to set the geometry (location) of.")]
        [Input("coordinateSystem", "A BHoM geometry Cartesian coordinate system defining the new location (incl. orientation) of the Opening.")]
        [Output("opening", "A BuildersWork Opening with updated geometry (location).")]
        public static Opening SetGeometry(this Opening opening, Cartesian coordinateSystem)
        {
            if (opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the geometry of a null opening.");
                return null;
            }
            
            Opening clone = opening.ShallowClone();
            clone.CoordinateSystem = coordinateSystem;
            
            BH.Engine.Base.Compute.RecordWarning("Only the coordinate system of the opening has been updated - to update its dimension, please modify relevant properties of the Profile.");
            return clone;
        }

        /***************************************************/
    }
}





