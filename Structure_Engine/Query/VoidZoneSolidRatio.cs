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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.SurfaceProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the ratio of solid material to void in the opening sone of the HollowCore, where 1 means no voids and 0 means complete void. Please note that this only relates to the thickness zone with voids, not including solid areas at top and bottom.")]
        [Input("hollowCore", "The hollow core property to get the solid ratio of the voided zone from. The ratio is extracted from the opening profile of the property.")]
        [Output("solidRatio", "The solid ratio of the voided zone.", typeof(Ratio))]
        public static double VoidZoneSolidRatio(this HollowCore hollowCore)
        {
            if (hollowCore.IsNull())
                return double.NaN;

            if (hollowCore.Openings == null)
            {
                Base.Compute.RecordError($"Opening profile for {nameof(HollowCore)} is null. Unable to extract void zone solid ratio.");
                return double.NaN;
            }

            return hollowCore.Openings.ISolidRatio();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double ISolidRatio(this IHollowCoreOpeningProfiles openingProfile)
        {
            return SolidRatio(openingProfile as dynamic);
        }

        /***************************************************/

        private static double SolidRatio(this CircularHollowCoreOpeningProfiles openingProfile)
        {
            if (openingProfile.Diameter > openingProfile.Spacing)
            {
                Base.Compute.RecordError($"{nameof(openingProfile.Diameter)} is larger than the {nameof(openingProfile.Spacing)}. The {nameof(CircularHollowCoreOpeningProfiles)} is invalid, and solid ratio cannot be computed.");
                return double.NaN;
            }
            double r = openingProfile.Diameter / 2;
            double openingArea = r * r * Math.PI;
            double fullArea = openingProfile.Diameter * openingProfile.Spacing;
            //Ratio calcualted as area of rectangle formed by spacing x diameter - area of the circular opening divided by the area of the full rectangle
            return (fullArea - openingArea) / fullArea;
        }

        /***************************************************/

        private static double SolidRatio(this ElongatedCircularHollowCoreOpeningProfiles openingProfile)
        {
            if (openingProfile.Width > openingProfile.Spacing)
            {
                Base.Compute.RecordError($"{nameof(openingProfile.Width)} is larger than the {nameof(openingProfile.Spacing)}. The {nameof(ElongatedCircularHollowCoreOpeningProfiles)} is invalid, and solid ratio cannot be computed.");
                return double.NaN;
            }

            if (openingProfile.Width > openingProfile.Height)
            {
                Base.Compute.RecordError($"{nameof(openingProfile.Width)} is larger than the {nameof(openingProfile.Height)}. The {nameof(ElongatedCircularHollowCoreOpeningProfiles)} is invalid, and solid ratio cannot be computed.");
                return double.NaN;
            }

            double r = openingProfile.Width / 2;
            double openingArea = r * r * Math.PI + (openingProfile.Height - openingProfile.Width) * openingProfile.Width;
            double fullArea = openingProfile.Height * openingProfile.Spacing;
            //Ratio calcualted as area of rectangle formed by spacing x height - area of the circular opening divided by the area of the full rectangle
            return (fullArea - openingArea) / fullArea;
        }

        /***************************************************/

        private static double SolidRatio(this IHollowCoreOpeningProfiles openingProfile)
        {
            BH.Engine.Base.Compute.RecordError($"{nameof(SolidRatio)} not implemented for {nameof(IHollowCoreOpeningProfiles)} of type {openingProfile.GetType()}.");
            return double.NaN;
        }

        /***************************************************/
    }
}

