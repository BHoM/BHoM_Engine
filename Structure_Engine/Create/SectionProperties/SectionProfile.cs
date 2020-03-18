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
using BH.oM.Geometry.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "ISectionProfile")]
        public static ISectionProfile ISectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            return Geometry.Create.ISectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "BoxProfile")]
        public static BoxProfile BoxProfile(double height, double width, double thickness, double outerRadius, double innerRadius)
        {
            return Geometry.Create.BoxProfile(height, width, thickness, outerRadius, innerRadius);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "AngleProfile")]
        public static AngleProfile AngleProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalZ = false, bool mirrorAboutLocalY = false)
        {
            return Geometry.Create.AngleProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ, mirrorAboutLocalY);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "ChannelProfile")]
        public static ChannelProfile ChannelProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalZ = false)
        {
            return Geometry.Create.ChannelProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalZ);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "CircleProfile")]
        public static CircleProfile CircleProfile(double diameter)
        {
            return Geometry.Create.CircleProfile(diameter);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "FabricatedBoxProfile")]
        public static FabricatedBoxProfile FabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            return Geometry.Create.FabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, weldSize);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "GeneralisedFabricatedBoxProfile")]
        public static GeneralisedFabricatedBoxProfile GeneralisedFabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness = 0.0, double botFlangeThickness = 0.0, double topCorbelWidth = 0.0, double botCorbelWidth = 0.0)
        {
            return Geometry.Create.GeneralisedFabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, botCorbelWidth);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "KiteProfile")]
        public static KiteProfile KiteProfile(double width1, double angle1, double thickness)
        {
            return Geometry.Create.KiteProfile(width1, angle1, thickness);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "FabricatedISectionProfile")]
        public static FabricatedISectionProfile FabricatedISectionProfile(double height, double topFlangeWidth, double botFlangeWidth, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            return Geometry.Create.FabricatedISectionProfile(height, topFlangeThickness, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "FreeFormProfile")]
        public static FreeFormProfile FreeFormProfile(IEnumerable<ICurve> edges)
        {
            return Geometry.Create.FreeFormProfile(edges);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "RectangleProfile")]
        public static RectangleProfile RectangleProfile(double height, double width, double cornerRadius)
        {
            return Geometry.Create.RectangleProfile(height, width, cornerRadius);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "TSectionProfile")]
        public static TSectionProfile TSectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius, bool mirrorAboutLocalY = false)
        {
            return Geometry.Create.TSectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, mirrorAboutLocalY);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "GeneralisedTSectionProfile")]
        public static GeneralisedTSectionProfile GeneralisedTSectionProfile(double height, double webThickness, double leftOutstandWidth, double leftOutstandThickness, double rightOutstandWidth, double rightOutstandThickness, bool mirrorAboutLocalY = false)
        {
            return Geometry.Create.GeneralisedTSectionProfile(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness, mirrorAboutLocalY);
        }

        /***************************************************/

        [Deprecated("2.3", "Class and method moved to Geometry_oM and Geometry_Engine", typeof(BH.Engine.Geometry.Create), "TubeProfile")]
        public static TubeProfile TubeProfile(double diameter, double thickness)
        {
            return Geometry.Create.TubeProfile(diameter, thickness);
        }

        /***************************************************/

    }
}

