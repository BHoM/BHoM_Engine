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

        public static TSectionProfile TSectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalY = false)
        {
            if (height < flangeThickness + rootRadius)
            {
                InvalidRatioError("height", "flangeThickness and rootRadius");
                return null;
            }

            if (width < webthickness + 2 * rootRadius + 2 * toeRadius)
            {
                InvalidRatioError("width", "webThickess, rootRadius and toeRadius");
                return null;
            }

            if (toeRadius > flangeThickness)
            {
                InvalidRatioError("toeTadius", "flangeThickness");
                return null;
            }

            if (height <= 0 || width <= 0 || webthickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = TeeProfileCurves(flangeThickness, width, webthickness, height - flangeThickness, rootRadius, toeRadius);

            if (mirrorAboutLocalY)
                curves = curves.MirrorAboutLocalY();

            return new TSectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalY, curves);
        }

        /***************************************************/
        
    }
}
