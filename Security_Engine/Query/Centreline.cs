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

using System.ComponentModel;

using BH.oM.Geometry;
using BH.oM.Security.Elements;
using BH.oM.Base.Attributes;

namespace BH.Engine.Security
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Returns the centreline of a CameraDevice object.")]
        [Input("cameraDevice", "The CameraDevice object to get the centreline from.")]
        [Output("centreline", "The centreline of the CameraDevice object.")]
        public static Line Centreline(this CameraDevice cameraDevice)
        {
            if(cameraDevice == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the centreline of a null camera device.");
                return null;
            }

            return new Line { Start = cameraDevice.EyePosition, End = cameraDevice.TargetPosition };
        }

        /***************************************************/
    }
}



