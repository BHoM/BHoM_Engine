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
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.Engine.Geometry;

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
            return profile.IsNull() ? 0 : 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this TubeProfile profile)
        {
            return profile.IsNull() ? 0 : 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this BoxProfile profile)
        {
            return profile.IsNull() ? 0 : 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this FabricatedBoxProfile profile)
        {
            return profile.IsNull() ? 0 : 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this KiteProfile profile)
        {
            return profile.IsNull() ? 0 : 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for closed sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this RectangleProfile profile)
        {
            return profile.IsNull() ? 0 : 0;
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile. This will always return 0 for angle sections.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this AngleProfile profile)
        {
            return profile.IsNull() ? 0 : 0;
        }

        /***************************************************/

        //TODO: Add warping constant calculation for generalised fabricated box, generalised T section and Z.

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this ISectionProfile profile)
        {
            if (profile.IsNull())
                return 0;


            //Calculated according to https://www.steelforlifebluebook.co.uk/explanatory-notes/ec3-ukna/properties/

            List<PolyCurve> curvesZ = Engine.Geometry.Compute.IJoin(profile.Edges.ToList());

            double iz = curvesZ.Sum(x => x.IIntegrateRegion(2));

            double tf = profile.FlangeThickness;
            double hs = profile.Height - tf;

            return iz * hs * hs / 4;

        }

        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this VoidedISectionProfile profile)
        {
            if (profile.IsNull())
                return 0;


            //Calculated according to https://www.steelforlifebluebook.co.uk/explanatory-notes/ec3-ukna/properties/

            List<PolyCurve> curvesZ = Engine.Geometry.Compute.IJoin(profile.Edges.ToList());

            double iz = curvesZ.Sum(x => x.IIntegrateRegion(2));

            double tf = profile.FlangeThickness;
            double hs = profile.Height - tf;

            return iz * hs * hs / 4;

        }

        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this FabricatedISectionProfile profile)
        {
            if (profile.IsNull())
                return 0;

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
            if (profile.IsNull())
                return 0;

            //Calculated according to https://www.steelforlifebluebook.co.uk/explanatory-notes/ec3-ukna/properties/

            List<PolyCurve> curvesZ = Engine.Geometry.Compute.IJoin(profile.Edges.ToList());
            List<PolyCurve> curvesY = curvesZ.Select(x => x.Rotate(Point.Origin, Vector.ZAxis, -Math.PI / 2)).ToList();

            double a = curvesZ.Sum(x => x.IIntegrateRegion(0));
            double iy = curvesY.Sum(x => x.IIntegrateRegion(2));
            double iz = curvesZ.Sum(x => x.IIntegrateRegion(2));

            double h = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            double cz = Math.Abs(curvesZ.Bounds().Min.X);

            double hSubTf2 = (h - tf) * (h - tf);

            return hSubTf2 / 4 * (iz - a * Math.Pow(cz - tw / 2, 2) * (hSubTf2 * a / (4 * iy) - 1));
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this TaperFlangeISectionProfile profile)
        {
            if (profile.IsNull())
                return 0;


            //Calculated according to Anngex G of ENV 1993-1-1:1992

            List<PolyCurve> curvesZ = Engine.Geometry.Compute.IJoin(profile.Edges.ToList());

            double iz = curvesZ.Sum(x => x.IIntegrateRegion(2));

            double tf = profile.FlangeThickness;
            double hs = profile.Height - tf;

            return iz * hs * hs / 4;

        }

        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this TaperFlangeChannelProfile profile)
        {
            if (profile.IsNull())
                return 0;

            //Calculated according to Anngex G of ENV 1993-1-1:1992

            List<PolyCurve> curvesZ = Engine.Geometry.Compute.IJoin(profile.Edges.ToList());
            List<PolyCurve> curvesY = curvesZ.Select(x => x.Rotate(Point.Origin, Vector.ZAxis, -Math.PI / 2)).ToList();

            double a = curvesZ.Sum(x => x.IIntegrateRegion(0));
            double iy = curvesY.Sum(x => x.IIntegrateRegion(2));
            double iz = curvesZ.Sum(x => x.IIntegrateRegion(2));

            double h = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;

            double y0 = Math.Abs(curvesZ.Bounds().Min.X);
            double ew = y0 - 0.5 * tw;

            double hf2 = (h - tf) * (h - tf);

            return 0.25*hf2 * (iz - a * Math.Pow(ew, 2) * (hf2 * a / (4 * iy) - 1));
        }

        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double WarpingConstant(this TSectionProfile profile)
        {
            if (profile.IsNull())
                return 0;

            //Calculated according to https://www.steelforlifebluebook.co.uk/explanatory-notes/ec3-ukna/properties/

            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;
            double b = profile.Width;
            double hSubTf = profile.Height - tf / 2;

            return tf * tf * tf * b * b * b / 144 + hSubTf * hSubTf * hSubTf * tw * tw * tw / 36;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the warping constant for the profile.")]
        [Input("profile", "The ShapeProfile to calculate the warping constant for.")]
        [Output("Iw", "The warping constant of the profile.", typeof(WarpingConstant))]
        public static double IWarpingConstant(this IProfile profile)
        {
            return profile.IsNull() ? 0 : WarpingConstant(profile as dynamic);
        }

        /***************************************************/
        /**** Private Methods - fall back               ****/
        /***************************************************/

        private static double WarpingConstant(this IProfile profile)
        {
            Base.Compute.RecordWarning("Can not calculate Warping constants for profiles of type " + profile.GetType().Name + ". Returned value will be 0.");
            return 0; // Return 0 for not specifically implemented ones
        }

        /***************************************************/
    }
}





