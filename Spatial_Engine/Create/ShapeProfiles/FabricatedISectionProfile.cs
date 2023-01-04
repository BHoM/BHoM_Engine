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

        [Description("Creates a I-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("topFlangeWidth")]
        [InputFromProperty("botFlangeWidth")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("topFlangeThickness")]
        [InputFromProperty("botFlangeThickness")]
        [InputFromProperty("weldSize")]
        [Output("fabI", "The created FabricatedISectionProfile.")]
        public static FabricatedISectionProfile FabricatedISectionProfile(double height, double topFlangeWidth, double botFlangeWidth, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize = 0)
        {
            if (height < topFlangeThickness + botFlangeThickness + 2 * Math.Sqrt(2) * weldSize || height <= topFlangeThickness + botFlangeThickness)
            {
                InvalidRatioError("height", "topFlangeThickness, botFlangeThickness and weldSize");
                return null;
            }

            if (botFlangeWidth < webThickness + 2 * Math.Sqrt(2) * weldSize)
            {
                InvalidRatioError("botFlangeWidth", "webThickness and weldSize");
                return null;
            }

            if (topFlangeWidth < webThickness + 2 * Math.Sqrt(2) * weldSize)
            {
                InvalidRatioError("topFlangeWidth", "webThickness and weldSize");
                return null;
            }

            if (height <= 0 || topFlangeWidth <= 0 || botFlangeWidth <= 0 || webThickness <= 0 || topFlangeThickness <= 0 || botFlangeThickness <= 0 || weldSize < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = IProfileCurves(topFlangeThickness, topFlangeWidth, botFlangeThickness, botFlangeWidth, webThickness, height - botFlangeThickness - topFlangeThickness, 0, 0, weldSize);

            Point centroid = curves.IJoin().Centroid();
            Vector translation = Point.Origin - centroid;
            curves = curves.Select(x => x.ITranslate(translation)).ToList();

            return new FabricatedISectionProfile(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        /***************************************************/

    }
}



