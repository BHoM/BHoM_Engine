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

using BH.oM.Base.Attributes;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****          ShapeProfiles           ****/
        /******************************************/

        [Description("Returns the thickness of a BoxProfile.")]
        [Input("boxProfile", "The BoxProfile to query.")]
        [Output("thickness", "Thickness of the BoxProfile.", typeof(Length))]
        public static double Thickness(this BoxProfile boxProfile)
        {
            return boxProfile.Thickness;
        }

        /******************************************/

        [Description("Returns the thickness of a KiteProfile.")]
        [Input("kiteProfile", "The KiteProfile to query.")]
        [Output("thickness", "Thickness of the KiteProfile.", typeof(Length))]
        public static double Thickness(this KiteProfile kiteProfile)
        {
            return kiteProfile.Thickness;
        }

        /******************************************/

        [Description("Returns the thickness of a TubeProfile.")]
        [Input("tubeProfile", "The TubeProfile to query.")]
        [Output("thickness", "Thickness of the TubeProfile.", typeof(Length))]
        public static double Thickness(this TubeProfile tubeProfile)
        {
            return tubeProfile.Thickness;
        }

        /******************************************/
        /****    Public Methods - Interfaces   ****/
        /******************************************/

        [Description("Returns the thickness of a ShapeProfile.")]
        [Input("profile", "The ShapeProfile to query.")]
        [Output("thickness", "Thickness of the ShapeProfile.", typeof(Length))]
        public static double IThickness(this IProfile profile)
        {
            return Thickness(profile as dynamic);
        }

        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static double Thickness(this IProfile profile)
        {
            Base.Compute.RecordError($"Thickness is not implemented for IProfile of type: {profile.GetType().Name}.");
            return double.NaN;
        }

        /***************************************************/
    }
}




