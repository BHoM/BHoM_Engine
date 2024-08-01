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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using System.Drawing.Drawing2D;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural RetainingWall from a stem, a footing and properties.")]
        [Input("stem", "The stem of the retaining wall.")]
        [Input("footing", "The footing of the retaining wall.")]
        [Input("retainedHeight", "The retained height of soil measured from the bottom of the wall Footing.")]
        [Input("coverDepth", "The distance from top of Footing to finished floor level on the exposed face.")]
        [Input("groundWaterDepth", "The distance from the base of the Footing to ground water level.")]
        [Input("retentionAngle", "A property of the material being retained measured from the horizontal plane.")]
        [Output("retainingWall", "The created RetainingWall containing the stem and footing.")]
        public static RetainingWall RetainingWall(Stem stem, PadFoundation footing, double retainedHeight = 0.0, double coverDepth = 0.0, double groundWaterDepth = 0.0, double retentionAngle = 0.0)
        {
            if (stem.IsNull())
                return null;

            if (footing.IsNull())
                return null;

            return new RetainingWall() { Stem = stem, Footing = footing, RetainedHeight = retainedHeight, CoverDepth = coverDepth, GroundWaterDepth = groundWaterDepth, RetentionAngle = retentionAngle };
        }

        /***************************************************/

        [Description("Creates a structural RetainingWall from a stem and a footing.")]
        [Input("stem", "The stem of the retaining wall.")]
        [Input("footing", "The footing of the retaining wall.")]
        [Output("retainingWall", "The created RetainingWall containing the stem and footing.")]
        public static RetainingWall RetainingWall(Stem stem, PadFoundation footing)
        {
            return new RetainingWall()
            {
                Stem = stem,
                Footing = footing
            };
        }

        /***************************************************/
    }

}