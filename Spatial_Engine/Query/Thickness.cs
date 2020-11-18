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

using BH.oM.Reflection.Attributes;
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

        [Description("Returns the thickness of an angleProfile.")]
        [Input("angleProfile", "The angleProfile to query.")]
        [Output("thickness", "Thickness of the angleProfile.", typeof(Length))]
        public static double Thickness(this AngleProfile angleProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {angleProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of a boxProfile.")]
        [Input("boxProfile", "The boxProfile to query.")]
        [Output("thickness", "Thickness of the boxProfile.", typeof(Length))]
        public static double Thickness(this BoxProfile boxProfile)
        {
            return boxProfile.Thickness;
        }

        /******************************************/

        [Description("Returns the thickness of an channelProfile.")]
        [Input("channelProfile", "The channelProfile to query.")]
        [Output("thickness", "Thickness of the channelProfile.", typeof(Length))]
        public static double Thickness(this ChannelProfile channelProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {channelProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of an circleProfile.")]
        [Input("circleProfile", "The circleProfile to query.")]
        [Output("thickness", "Thickness of the circleProfile.", typeof(Length))]
        public static double Thickness(this CircleProfile circleProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {circleProfile.GetType().Name} does not have a Thickness property as it is a solid profile.");
            return double.NaN;
        }


        /******************************************/

        [Description("Returns the thickness of an fabricatedBoxProfile.")]
        [Input("fabricatedBoxProfile", "The fabricatedBoxProfile to query.")]
        [Output("thickness", "Thickness of the fabricatedBoxProfile.", typeof(Length))]
        public static double Thickness(this FabricatedBoxProfile fabricatedBoxProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {fabricatedBoxProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of an fabricatedISectionProfile.")]
        [Input("fabricatedISectionProfile", "The fabricatedISectionProfile to query.")]
        [Output("thickness", "Thickness of the fabricatedISectionProfile.", typeof(Length))]
        public static double Thickness(this FabricatedISectionProfile fabricatedISectionProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {fabricatedISectionProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of an freeFormProfile.")]
        [Input("freeFormProfile", "The freeFormProfile to query.")]
        [Output("thickness", "Thickness of the freeFormProfile.", typeof(Length))]
        public static double Thickness(this FreeFormProfile freeFormProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {freeFormProfile.GetType().Name} is unsupported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of an generalisedFabricatedBoxProfile.")]
        [Input("generalisedFabricatedBoxProfile", "The generalisedFabricatedBoxProfile to query.")]
        [Output("thickness", "Thickness of the generalisedFabricatedBoxProfile.", typeof(Length))]
        public static double Thickness(this GeneralisedFabricatedBoxProfile generalisedFabricatedBoxProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {generalisedFabricatedBoxProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of an generalisedTSectionProfile.")]
        [Input("generalisedTSectionProfile", "The generalisedTSectionProfile to query.")]
        [Output("thickness", "Thickness of the generalisedTSectionProfile.", typeof(Length))]
        public static double Thickness(this GeneralisedTSectionProfile generalisedTSectionProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {generalisedTSectionProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of an iSectionProfile.")]
        [Input("iSectionProfile", "The iSectionProfile to query.")]
        [Output("thickness", "Thickness of the iSectionProfile.", typeof(Length))]
        public static double Thickness(this ISectionProfile iSectionProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {iSectionProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of a kiteProfile.")]
        [Input("kiteProfile", "The kiteProfile to query.")]
        [Output("thickness", "Thickness of the kiteProfile.", typeof(Length))]
        public static double Thickness(this KiteProfile kiteProfile)
        {
            return kiteProfile.Thickness;
        }

        /******************************************/

        [Description("Returns the thickness of an rectangleProfile.")]
        [Input("rectangleProfile", "The rectangleProfile to query.")]
        [Output("thickness", "Thickness of the rectangleProfile.", typeof(Length))]
        public static double Thickness(this RectangleProfile rectangleProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {rectangleProfile.GetType().Name} does not have a Thickness property as it is a solid profile.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of an taperedProfile.")]
        [Input("taperedProfile", "The taperedProfile to query.")]
        [Output("thickness", "Thickness of the taperedProfile.", typeof(Length))]
        public static double Thickness(this TaperedProfile taperedProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {taperedProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of an tSectionProfile.")]
        [Input("tSectionProfile", "The tSectionProfile to query.")]
        [Output("thickness", "Thickness of the tSectionProfile.", typeof(Length))]
        public static double Thickness(this TSectionProfile tSectionProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {tSectionProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
        }

        /******************************************/

        [Description("Returns the thickness of a tubeProfile.")]
        [Input("tubeProfile", "The tubeProfile to query.")]
        [Output("thickness", "Thickness of the tubeProfile.", typeof(Length))]
        public static double Thickness(this TubeProfile tubeProfile)
        {
            return tubeProfile.Thickness;
        }

        /******************************************/

        [Description("Returns the thickness of an zSectionProfile.")]
        [Input("zSectionProfile", "The zSectionProfile to query.")]
        [Output("thickness", "Thickness of the zSectionProfile.", typeof(Length))]
        public static double Thickness(this ZSectionProfile zSectionProfile)
        {
            Reflection.Compute.RecordError($"IProfile type: {zSectionProfile.GetType().Name} has multiple Thickness parameters and is not supported. Consider using a different method of accessing these values.");
            return double.NaN;
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
            Reflection.Compute.RecordError($"Thickness is not implemented for IProfile of type: {profile.GetType().Name}.");
            return double.NaN;
        }

        /***************************************************/
    }
}

