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

        [Description("Creates a T-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("leftOutstandWidth")]
        [InputFromProperty("leftOutstandThickness")]
        [InputFromProperty("rightOutstandWidth")]
        [InputFromProperty("rightOutstandThickness")]
        [InputFromProperty("mirrorAboutLocalY")]
        [Output("genT", "The created GeneralisedTSectionProfile.")]
        public static GeneralisedTSectionProfile GeneralisedTSectionProfile(double height, double webThickness, double leftOutstandWidth, double leftOutstandThickness, double rightOutstandWidth, double rightOutstandThickness, bool mirrorAboutLocalY = false)
        {
            if (!AreFinite(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness))
                return null;

            if (!ArePositive(height, webThickness) || !AreNonNegative(leftOutstandThickness, leftOutstandWidth, rightOutstandThickness, rightOutstandWidth))
                return null;

            if (height < leftOutstandThickness)
            {
                InvalidRatioError(nameof(height), nameof(leftOutstandThickness));
                return null;
            }

            if (height < rightOutstandThickness)
            {
                InvalidRatioError(nameof(height), nameof(rightOutstandThickness));
                return null;
            }

            if (leftOutstandThickness <= 0 && leftOutstandWidth > 0 || leftOutstandWidth <= 0 && leftOutstandThickness > 0)
            {
                InvalidRatioError(nameof(leftOutstandThickness), nameof(leftOutstandWidth));
                return null;
            }

            if (rightOutstandThickness <= 0 && rightOutstandWidth > 0 || rightOutstandWidth <= 0 && rightOutstandThickness > 0)
            {
                InvalidRatioError(nameof(rightOutstandThickness), nameof(rightOutstandWidth));
                return null;
            }

            List<ICurve> curves = GeneralisedTeeProfileCurves(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness, mirrorAboutLocalY);
            return new GeneralisedTSectionProfile(height, webThickness, leftOutstandWidth, leftOutstandThickness, rightOutstandWidth, rightOutstandThickness, mirrorAboutLocalY, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> GeneralisedTeeProfileCurves(double height, double webThickness, double leftOutstandWidth, double leftOutstandThickness, double rightOutstandWidth, double rightOutstandThickness, bool mirrorAboutLocalY)
        {
            //Web centred on the y-axis with its bottom at the origin and outstands at the top
            double wx = webThickness / 2;                       //Right face of the web
            double rx = wx + rightOutstandWidth;                //Tip of the right outstand
            double ry = height - rightOutstandThickness;        //Underside of the right outstand
            double lx = -wx - leftOutstandWidth;                //Tip of the left outstand
            double ly = height - leftOutstandThickness;         //Underside of the left outstand

            //Only corners that exist are added, so that outstands of zero size, or running the full height, give no coinciding corners or corners in the middle of a straight edge
            List<ProfileCorner> corners = new List<ProfileCorner>(8);

            //Right side, from bottom to top
            if (rightOutstandWidth <= 0)
            {
                corners.Add(new ProfileCorner(wx, 0));
                corners.Add(new ProfileCorner(wx, height));
            }
            else if (rightOutstandThickness >= height)
            {
                corners.Add(new ProfileCorner(rx, 0));
                corners.Add(new ProfileCorner(rx, height));
            }
            else
            {
                corners.Add(new ProfileCorner(wx, 0));
                corners.Add(new ProfileCorner(wx, ry));
                corners.Add(new ProfileCorner(rx, ry));
                corners.Add(new ProfileCorner(rx, height));
            }

            //Left side, from top to bottom
            if (leftOutstandWidth <= 0)
            {
                corners.Add(new ProfileCorner(-wx, height));
                corners.Add(new ProfileCorner(-wx, 0));
            }
            else if (leftOutstandThickness >= height)
            {
                corners.Add(new ProfileCorner(lx, height));
                corners.Add(new ProfileCorner(lx, 0));
            }
            else
            {
                corners.Add(new ProfileCorner(lx, height));
                corners.Add(new ProfileCorner(lx, ly));
                corners.Add(new ProfileCorner(-wx, ly));
                corners.Add(new ProfileCorner(-wx, 0));
            }

            return ProfileCurves(true, corners.MirrorCorners(false, mirrorAboutLocalY));
        }

        /***************************************************/

    }
}






