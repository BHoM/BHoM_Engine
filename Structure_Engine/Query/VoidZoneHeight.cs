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

        [Description("Gets the height of the hollow core zone of the hollow core property.")]
        [Input("hollowCore", "The hollow core property to get the height of the voided zone from. The height is extracted from the opening profile of the property.")]
        [Output("voidHeight", "The height of the voided zone.", typeof(Length))]
        public static double VoidZoneHeight(this HollowCore hollowCore)
        {
            if (hollowCore.IsNull())
                return double.NaN;

            if (hollowCore.Openings == null)
            {
                Base.Compute.RecordError($"Opening profile for {nameof(HollowCore)} is null. Unable to extract void zone height.");
                return double.NaN;
            }

            return hollowCore.Openings.IProfileHeight();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double IProfileHeight(this IHollowCoreOpeningProfiles openingProfile)
        {
            return ProfileHeight(openingProfile as dynamic);
        }

        /***************************************************/

        private static double ProfileHeight(this CircularHollowCoreOpeningProfiles openingProfile)
        {
            return openingProfile.Diameter;
        }

        /***************************************************/

        private static double ProfileHeight(this ElongatedCircularHollowCoreOpeningProfiles openingProfile)
        {
            return openingProfile.Height;
        }

        /***************************************************/

        private static double ProfileHeight(this IHollowCoreOpeningProfiles openingProfile)
        {
            BH.Engine.Base.Compute.RecordError($"{nameof(ProfileHeight)} not implemented for {nameof(IHollowCoreOpeningProfiles)} of type {openingProfile.GetType()}.");
            return double.NaN;
        }

        /***************************************************/
    }
}


