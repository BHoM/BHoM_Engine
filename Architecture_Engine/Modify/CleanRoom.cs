/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

namespace BH.Engine.Architecture
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a room that has been cleaned of short segments and insignificant vertices in its perimeter curve")]
        [Input("room", "An architecture room that will be cleaned")]
        [Input("angleTolerance", "The tolerance of the angle that defines a straight line. Default is set to the value defined by BH.oM.Geometry.Tolerance.Angle")]
        [Input("minimumSegmentLength", "The length of the smallest allowed segment. Segments smaller than this will be removed. Default is set to the value defined by BH.oM.Geometry.Tolerance.Distance")]
        [Output("cleanedRoom", "A room that has been cleaned")]
        public static Room CleanRoom(this Room room, double angleTolerance = Tolerance.Angle, double minimumSegmentLength = Tolerance.Distance)
        {
            if(room == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot clean the geometry of a null room.");
                return room;
            }

            Room clonedRoom = room.DeepClone();
            clonedRoom.Perimeter = clonedRoom.Perimeter.ICollapseToPolyline(Tolerance.Angle).CleanPolyline(angleTolerance, minimumSegmentLength);
            return clonedRoom;
        }
    }
}



