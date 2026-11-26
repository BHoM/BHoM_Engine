/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a circular hollow profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("diameter")]
        [InputFromProperty("thickness")]
        [Output("tube", "The created TubeProfile.")]
        public static TubeProfile TubeProfile(double diameter, double thickness)
        {
            if (!AreFinite(diameter, thickness))
                return null;

            if (!ArePositive(diameter, thickness))
                return null;

            if (thickness >= diameter / 2)
            {
                InvalidRatioError(nameof(diameter), nameof(thickness));
                return null;
            }

            List<ICurve> curves = TubeProfileCurves(diameter / 2, thickness);
            return new TubeProfile(diameter, thickness, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> TubeProfileCurves(double outerRadius, double thickness)
        {
            //Inner circle with normal in negative z, i.e. clockwise, to mark it as a void
            return new List<ICurve>
            {
                new Circle { Centre = new Point(), Radius = outerRadius },
                new Circle { Centre = new Point(), Normal = new Vector { Z = -1 }, Radius = outerRadius - thickness }
            };
        }

        /***************************************************/
    }
}






