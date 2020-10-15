/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.Engine.Reflection;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a C-shaped profile based on input dimensions. Method generates edgecurves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("flangeWidth")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [InputFromProperty("mirrorAboutLocalZ")]
        [Output("channel", "The created ChannelProfile.")]
        public static ChannelProfile ChannelProfile(double height, double flangeWidth, double webThickness, double flangeThickness, double rootRadius = 0, double toeRadius = 0, bool mirrorAboutLocalZ = false)
        {
            if (height < flangeThickness * 2 + rootRadius * 2 || height <= flangeThickness * 2)
            {
                InvalidRatioError("height", "flangeThickness and rootRadius");
                return null;
            }

            if (flangeWidth < webThickness + rootRadius + toeRadius)
            {
                InvalidRatioError("width", "webthickness, toeRadius and rootRadius");
                return null;
            }

            if (flangeThickness < toeRadius)
            {
                InvalidRatioError("flangeThickness", "toeRadius");
                return null;
            }

            if (height <= 0 || flangeWidth <= 0 || webThickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = ChannelProfileCurves(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius);

            if (mirrorAboutLocalZ)
                curves = curves.MirrorAboutLocalZ();

            return new ChannelProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, curves);
        }

        /***************************************************/
        
    }
}
