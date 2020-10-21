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

using System;
using BH.oM.Spatial.ShapeProfiles;

using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this CircleProfile profile)
        {
            return 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this TubeProfile profile)
        {
            return 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this BoxProfile profile)
        {
            return 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this FabricatedBoxProfile profile)
        {
            return 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this KiteProfile profile)
        {
            return 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this RectangleProfile profile)
        {
            return 0;
        }

        /***************************************************/

        //TODO: Add warping constant calculation for Angle, T sections, generalised fabricated box, generalised T section and Z.

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this ISectionProfile profile)
        {
            double width = profile.Width;
            double height = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;


            return tf * Math.Pow(height - tf, 2) * Math.Pow(width, 3) / 24;

        }

        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this FabricatedISectionProfile profile)
        {
            double b1 = profile.TopFlangeWidth;
            double b2 = profile.BotFlangeWidth;
            double height = profile.Height;
            double tf1 = profile.TopFlangeThickness;
            double tf2 = profile.BotFlangeThickness;
            double tw = profile.WebThickness;


            if (tf1 == tf2 && b1 == b2)
            {
                return tf1 * Math.Pow(height - tf1, 2) * Math.Pow(b1, 3) / 24;
            }
            else
            {
                return tf1 * Math.Pow(height - (tf1 + tf2) / 2, 2) / 12 * (Math.Pow(b1, 3) * Math.Pow(b2, 3) / (Math.Pow(b1, 3) + Math.Pow(b2, 3)));
            }
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this ChannelProfile profile)
        {
            double width = profile.FlangeWidth;
            double height = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;


            return tf * Math.Pow(height, 2) / 12 * (3 * width * tf + 2 * height * tw / (6 * width * tf + height * tw));

        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double IWarpingConstant(this IProfile profile)
        {
            return WarpingConstant(profile as dynamic);
        }

        /***************************************************/
        /**** Private Methods - fall back               ****/
        /***************************************************/

        private static double WarpingConstant(this IProfile profile)
        {
            Reflection.Compute.RecordWarning("Can not calculate Warping constants for profiles of type " + profile.GetType().Name + ". Returned value will be 0.");
            return 0; // Return 0 for not specifically implemented ones
        }

        /***************************************************/
    }
}

