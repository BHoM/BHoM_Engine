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

using BH.oM.Acoustic;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Acoustic
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static Ray Ray(Polyline path, int source, int target, List<int> bouncingPattern = null)
        {
            return new Ray()
            {
                Path = path,
                SpeakerID = source,
                ReceiverID = target,
                PanelsID = bouncingPattern
            };
        }

        /***************************************************/

        public static Ray Ray(Speaker speaker, Receiver receiver)
        {
            if(speaker == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a Ray from a null Speaker.");
                return null;
            }

            return Create.Ray(new Polyline { ControlPoints = new List<Point> { speaker.Location, receiver.Location } }, speaker.SpeakerID, receiver.ReceiverID);
        }

        /***************************************************/
    }
}






