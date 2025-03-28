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

using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.Engine.Reflection;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a I-shaped profile with a void through the web based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("openingHeight")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [Output("I", "The created ISectionProfile.")]
        public static VoidedISectionProfile VoidedISectionProfile(double height, double openingHeight, double width, double webThickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            if (height < flangeThickness * 2 + rootRadius * 2 || height <= flangeThickness * 2)
            {
                InvalidRatioError("height", "flangeThickness and rootRadius");
                return null;
            }

            if (width < webThickness + rootRadius * 2 + toeRadius * 2)
            {
                InvalidRatioError("width", "webthickness, rootRadius and toeRadius");
                return null;
            }

            if (toeRadius > flangeThickness)
            {
                InvalidRatioError("toeRadius", "flangeThickness");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            if (openingHeight > height - 2 * (flangeThickness + rootRadius))
            {
                InvalidRatioError(nameof(openingHeight), $"{nameof(height)}, {nameof(flangeThickness)} and {nameof(rootRadius)}");
                return null;
            }

            List<ICurve> curves = TeeProfileCurves(flangeThickness, width, webThickness, (height - openingHeight) / 2 - flangeThickness, rootRadius, toeRadius);

            BoundingBox boundsOpeningProfile = Geometry.Query.Bounds(curves.Select(x => Geometry.Query.IBounds(x)).ToList());

            double move = height / 2 - boundsOpeningProfile.Max.Y;
            Vector translationVector = new Vector { Y = move };
            curves = curves.Select(x => Geometry.Modify.ITranslate(x, translationVector)).ToList();
            Plane mirrorPlane = Plane.XZ;
            curves = curves.Concat(curves.Select(x => x.IMirror(mirrorPlane))).ToList();


            return new VoidedISectionProfile(height, openingHeight, (height - openingHeight) / 2, width, webThickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/

    }
}





