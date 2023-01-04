/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
using System.ComponentModel;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        //public static double TorsionalConstantThinWalled(ShapeType shape, double totalDepth, double totalWidth, double b1, double b2, double tf1, double tf2, double tw)
        //{
        //    switch (shape)
        //    {
        //        case ShapeType.ISection:
        //        case ShapeType.Channel:
        //        case ShapeType.Zed:
        //            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (totalDepth - tf1) * Math.Pow(tw, 3)) / 3;
        //        case ShapeType.Tee:
        //        case ShapeType.Angle:
        //            return totalWidth * Math.Pow(tf1, 3) + totalDepth * Math.Pow(tw, 3);
        //        case ShapeType.Circle:
        //            return Math.PI * Math.Pow(totalDepth, 4) / 2;
        //        case ShapeType.Box:
        //            return 2 * tf1 * tw * Math.Pow(totalWidth - tw, 2) * Math.Pow(totalDepth - tf1, 2) /
        //                (totalWidth * tw + totalDepth * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        //        case ShapeType.Tube:
        //            return Math.PI * (Math.Pow(totalDepth, 4) - Math.Pow(totalDepth - tw, 4)) / 2;
        //        case ShapeType.Rectangle:
        //            if (Math.Abs(totalDepth - totalWidth) < Tolerance.Distance)
        //                return 2.25 * Math.Pow(totalDepth, 4);
        //            else
        //            {
        //                double a = Math.Max(totalDepth, totalWidth);
        //                double b = Math.Min(totalDepth, totalWidth);
        //                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
        //            }
        //        default:
        //            return 0;
        //    }
        //}


        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this CircleProfile profile)
        {
            return profile.IsNull() ? 0 : Math.PI * Math.Pow(profile.Diameter, 4) / 32;
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this TubeProfile profile)
        {
            return profile.IsNull() ? 0 : Math.PI * (Math.Pow(profile.Diameter, 4) - Math.Pow(profile.Diameter - 2 * profile.Thickness, 4)) / 32;
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this FabricatedBoxProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double tf1 = profile.TopFlangeThickness; //TODO: Allow for varying plate thickness
            double tw = profile.WebThickness;
            double width = profile.Width;
            double height = profile.Height;


            return 2 * tf1 * tw * Math.Pow(width - tw, 2) * Math.Pow(height - tf1, 2) /
                        (width * tw + height * tf1 - Math.Pow(tw, 2) - Math.Pow(tf1, 2));
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile in accordance with section A.3 from BS EN 10210-2:2019. Note that this is not the polar moment of intertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this BoxProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double t = profile.Thickness;
            double width = profile.Width;
            double height = profile.Height;

            double rc = (profile.OuterRadius + profile.InnerRadius) / 2;
            double h = 2 * ((width - t) + (height - t)) - 2 * rc * (4 - Math.PI);
            double ah = (width - t) * (height - t) - Math.Pow(rc, 2) * (4 - Math.PI);
            double k = 2 * ah * t / h;
            return Math.Pow(t, 3) * h / 3 + 2 * k * ah;
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this FabricatedISectionProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double b1 = profile.TopFlangeWidth;
            double b2 = profile.BotFlangeWidth;
            double height = profile.Height;
            double tf1 = profile.TopFlangeThickness;
            double tf2 = profile.BotFlangeThickness;
            double tw = profile.WebThickness;

            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (height - (tf1 + tf2) / 2) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.\n" +
                     "Formulae taken from https://orangebook.arcelormittal.com/explanatory-notes/long-products/section-properties/.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this ISectionProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double b = profile.Width;
            double h = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;
            double r = profile.RootRadius;

            double alpha = AlphaTJunction(tw, tf, r);
            double D = InscribedDiameterTJunction(tw, tf, r);

            return (2 * b * Math.Pow(tf, 3) + (h - 2 * tf) * Math.Pow(tw, 3)) / 3 + 2 * alpha * Math.Pow(D, 4) - 0.420 * Math.Pow(tf, 4);
        }


        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.\n" +
                     "Formulae taken from https://orangebook.arcelormittal.com/explanatory-notes/long-products/section-properties/.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this ChannelProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double b = profile.FlangeWidth;
            double h = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;
            double r = profile.RootRadius;

            double alpha = AlphaLJunction(tw, tf, r);
            double D = InscribedDiameterLJunction(tw, tf, r);

            //Note that 'P385 Design of steel beams in torsion' states that the reduction in the end should only be  `- 0.210 * Math.Pow(tf, 4);`
            //As orange and blue book is using `- 0.420 * Math.Pow(tf, 4);`, and this is more conservative, using the latter until clarified. Johnston & El Darwish agrees that it should be 0.420, i.e. 4 times the end/L correction 
            return (2 * b * Math.Pow(tf, 3) + (h - 2 * tf) * Math.Pow(tw, 3)) / 3 + 2 * alpha * Math.Pow(D, 4) - 0.420 * Math.Pow(tf, 4);
        }/***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.\n" +
                     "Formulae taken from Johnston & El Darwish, 1965.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this TaperFlangeISectionProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double b = profile.Width;
            double d = profile.Height;
            double tw = profile.WebThickness;
            double r = profile.RootRadius;
            double s = profile.FlangeSlope;
            double tf = profile.FlangeThickness;
            double t1 = profile.FlangeThickness - s * b / 4;
            double t2 = t1 + s * (b - tw) / 2;
            double t3 = tf + s * (b / 4);
            double vs = EndLossCorrectionVs(s);

            double alpha = AlphaTaperTJunction(tw, tf, r, t2, s);
            
            double D = InscribedDiameterTaperTJunction(tw, t3, r, s);

            //Equation 35
            return (b - tw) / 6 * (t1 + t2) * (Math.Pow(t1, 2) + Math.Pow(t2, 2)) 
                + (2.0 / 3) * tw * Math.Pow(t2, 3) + (1.0 / 3)*(d - 2*t2) * Math.Pow(tw, 3) + 2 * alpha * Math.Pow(D, 4) 
                - 4 * vs * Math.Pow(t1, 4);
        }


        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.\n" +
                     "Formulae taken from Johnston & El Darwish, 1964.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this TaperFlangeChannelProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double b = profile.FlangeWidth;
            double h = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;
            double r = profile.RootRadius;
            double s = profile.FlangeSlope;
            double t1 = profile.FlangeThickness - s * b / 2;
            double t2 = t1 + s * (b - tw);
            double vs = EndLossCorrectionVs(s);

            double alpha = AlphaTaperLJunction(tw, tf, r, t2, s);

            double D = InscribedDiameterTaperLJunction(tw, t2, r, s);

            //Equation 37
            return (b - tw) / 6 * (t1 + t2) * (Math.Pow(t1, 2) + Math.Pow(t2, 2)) + (2.0 / 3) * tw * Math.Pow(t2, 3) 
                + (1.0 / 3) * (h - 2 * t2) * Math.Pow(tw, 3) + 2 * alpha * Math.Pow(D, 4) - 2 * vs * Math.Pow(t1, 4) 
                - 0.210 * Math.Pow(t2, 4);
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this ZSectionProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double b1 = profile.FlangeWidth;
            double b2 = profile.FlangeWidth;
            double height = profile.Height;
            double tf1 = profile.FlangeThickness;
            double tf2 = profile.FlangeThickness;
            double tw = profile.WebThickness;

            return (b1 * Math.Pow(tf1, 3) + b2 * Math.Pow(tf2, 3) + (height - tf1) * Math.Pow(tw, 3)) / 3;
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.\n" +
                     "Formulae taken from 'P385 Design of steel beams in torsion'.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this TSectionProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double b = profile.Width;
            double h = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;
            double r = profile.RootRadius;

            double alpha = AlphaTJunction(tw, tf, r);
            double D = InscribedDiameterTJunction(tw, tf, r);

            return (b * Math.Pow(tf, 3) + (h - tf) * Math.Pow(tw, 3)) / 3 + alpha * Math.Pow(D, 4) - 0.21 * Math.Pow(tf, 4) - 0.105 * Math.Pow(tw, 4);
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this GeneralisedTSectionProfile profile)
        {
            if (profile.IsNull())
                return 0;

            bool leftOutstand = profile.LeftOutstandWidth > 0 && profile.LeftOutstandThickness > 0;
            bool rightOutstand = profile.RightOutstandWidth > 0 && profile.RightOutstandThickness > 0;

            if (!leftOutstand && !rightOutstand)
            {
                //No outstands => Rectangle

                double a = Math.Max(profile.Height, profile.WebThickness) / 2;
                double b = Math.Min(profile.Height, profile.WebThickness) / 2;
                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
            }

            if (Math.Abs(profile.RightOutstandThickness - profile.LeftOutstandThickness) < Tolerance.Distance
                && Math.Abs(profile.RightOutstandWidth - profile.LeftOutstandWidth) < Tolerance.Distance)
            {
                //Symmetric T
                double totalWidth = profile.RightOutstandWidth * 2 + profile.WebThickness;
                double totalDepth = profile.Height;
                double tf = profile.RightOutstandThickness;
                double tw = profile.WebThickness;

                return (totalWidth * Math.Pow(tf, 3) + (totalDepth - tf / 2) * Math.Pow(tw, 3)) / 3;
            }


            if (leftOutstand && !rightOutstand || !leftOutstand && rightOutstand)
            {
                //One outstand => angle
                double totalWidth = (leftOutstand ? profile.LeftOutstandWidth : profile.RightOutstandWidth) + profile.WebThickness;
                double totalDepth = profile.Height;
                double tf = leftOutstand ? profile.LeftOutstandThickness : profile.RightOutstandThickness;
                double tw = profile.WebThickness;

                return ((totalWidth - tw / 2) * Math.Pow(tf, 3) + (totalDepth - tf / 2) * Math.Pow(tw, 3)) / 3;
            }


            Base.Compute.RecordWarning("Can only calculate torsional constant of symmetric T sections or angles");
            return 0;

        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.\n" +
                     "Formulae taken from 'P385 Design of steel beams in torsion'.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this AngleProfile profile)
        {
            if (profile.IsNull())
                return 0;

            double b = profile.Width;
            double h = profile.Height;
            double tf = profile.FlangeThickness;
            double tw = profile.WebThickness;
            double r = profile.RootRadius;

            double alpha = AlphaLJunction(tw, tf, r);
            double D = InscribedDiameterLJunction(tw, tf, r);

            return (b * Math.Pow(tf, 3) + (h - tf) * Math.Pow(tw, 3)) / 3 + alpha * Math.Pow(D, 4) - 0.105 * Math.Pow(tf, 4) - 0.105 * Math.Pow(tw, 4);
        }

        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double TorsionalConstant(this RectangleProfile profile)
        {
            if (profile.IsNull())
                return 0;

            if (Math.Abs(profile.Height - profile.Width) < Tolerance.Distance)
                return 2.25 * Math.Pow(profile.Height / 2, 4);
            else
            {
                double a = Math.Max(profile.Height, profile.Width) / 2;
                double b = Math.Min(profile.Height, profile.Width) / 2;
                return a * Math.Pow(b, 3) * (16 / 3 - 3.36 * b / a * (1 - Math.Pow(b, 4) / (12 * Math.Pow(a, 4))));
            }
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Calculates the torsional constant for the profile. Note that this is not the polar moment of inertia.")]
        [Input("profile", "The ShapeProfile to calculate the torsional constant for.")]
        [Output("J", "Torsional constant of the profile. Note that this is not the polar moment of inertia.", typeof(TorsionConstant))]
        public static double ITorsionalConstant(this IProfile profile)
        {
            return profile.IsNull() ? 0 : TorsionalConstant(profile as dynamic);
        }

        /***************************************************/
        /**** Private Methods - fall back               ****/
        /***************************************************/

        private static double TorsionalConstant(this IProfile profile)
        {
            Base.Compute.RecordWarning("Cannot calculate Torsional constants for profiles of type " + profile.GetType().Name + ". Returned value will be 0.");
            return 0; //Return 0 for not specifically implemented ones
        }

        /***************************************************/
        /**** Private Methods - helper methods          ****/
        /***************************************************/

        [Description("Diameter of an circles inscribed in a T-junction connection such as in a parallel flange I-Section. Taken from 'P385 Design of steel beams in torsion', Appendix B.")]
        [Input("tw", "Web thickness, assumed to be the stem of the T.", typeof(Length))]
        [Input("tf", "Flange thickness, assumed to be the top of the T.", typeof(Length))]
        [Input("r", "Root radius, assumed to be the same on both sides of the T.", typeof(Length))]
        private static double InscribedDiameterTJunction(double tw, double tf, double r)
        {
            //Equation 23
            return (Math.Pow(tf + r, 2) + (r + 0.25 * tw) * tw) / (2 * r + tf);
        }

        /***************************************************/

        [Description("Diameter of an circles inscribed in an L-junction connection such as in an L-Section. Taken from 'P385 Design of steel beams in torsion', Appendix B.")]
        [Input("tw", "Web thickness.", typeof(Length))]
        [Input("tf", "Flange thickness.", typeof(Length))]
        [Input("r", "Root radius.", typeof(Length))]
        private static double InscribedDiameterLJunction(double tw, double tf, double r)
        {
            //Equation 25
            return 2 * ((3 * r + tw + tf) - Math.Sqrt(2 * (2 * r + tw) * (2 * r + tf)));
        }

        /***************************************************/

        [Description("Diameter of a circle inscribed in a tapered flange T-junction connection such as a taper flange I-section. Taken from Johnston & El Darwish, 1964, Figure 6 Case 2")]
        [Input("tw", "Web thickness, assumed to be the stem of the T.", typeof(Length))]
        [Input("t3", "Flange thickness at theoretical intersection of flange and web centerline.", typeof(Length))]
        [Input("r", "Root radius, assumed to be the same on both sides of the T.", typeof(Length))]
        [Input("s", "Flange taper slope.", typeof(Ratio))]
        private static double InscribedDiameterTaperTJunction(double tw, double t3, double r, double s)
        {
            if (s < Tolerance.Angle) return InscribedDiameterTJunction(tw, t3, r);

            //Equation 24
            double f = r * s * (Math.Sqrt((1 / Math.Pow(s, 2)) + 1) - 1 - tw / (2 * r));
            return (Math.Pow(f + t3, 2) + tw * (r + tw / 4)) / (f + r + t3);
        }

        /***************************************************/

        [Description("Diameter of a circle inscribed in a tapered L-junction connection such as a taper flange channel. Taken from Johnston & El Darwish, 1964, Figure 6 Case 4")]
        [Input("tw", "Web thickness.", typeof(Length))]
        [Input("t2", "Flange thickness at theoretical intersection of flange and near face of web.", typeof(Length))]
        [Input("r", "Root radius.", typeof(Length))]
        [Input("s", "Flange taper slope.", typeof(Ratio))]
        private static double InscribedDiameterTaperLJunction(double tw, double t2, double r, double s)
        {
            //Equation 26
            double h = t2 - r * (s + 1 - Math.Sqrt(1 + Math.Pow(s, 2)));
            return 2 * ((3 * r + tw + h) - Math.Sqrt(2 * (2 * r + tw) * (2 * r + h)));
        }

        /***************************************************/

        [Description("Empirical formula used to correct the torsional constant with enhancement from a T-junction. Taken from 'P385 Design of steel beams in torsion', Appendix B.")]
        [Input("tw", "Web thickness, assumed to be the stem of the T.", typeof(Length))]
        [Input("tf", "Flange thickness, assumed to be the top of the T.", typeof(Length))]
        [Input("r", "Root radius, assumed to be the same on both sides of the T.", typeof(Length))]
        private static double AlphaTJunction(double tw, double tf, double r)
        {
            //Equation 27
            return -0.042 + 0.2204 * tw / tf + 0.1355 * r / tf - 0.0865 * (r * tw) / Math.Pow(tf, 2) - 0.0725 * Math.Pow(tw / tf, 2);
        }

        /***************************************************/

        [Description("Empirical formula used to correct the torsional constant with enhancement from a L-junction. Taken from 'P385 Design of steel beams in torsion', Appendix B.")]
        [Input("tw", "Web thickness.", typeof(Length))]
        [Input("tf", "Flange thickness.", typeof(Length))]
        [Input("r", "Root radius.", typeof(Length))]
        private static double AlphaLJunction(double tw, double tf, double r)
        {
            //Equation 29
            return -0.0908 + 0.2621 * tw / tf + 0.1231 * r / tf - 0.0752 * (tw * r) / Math.Pow(tf, 2) - 0.0945 * Math.Pow(tw / tf, 2);
        }

        /***************************************************/

        [Description("Empirical formula used to correct the torsional constant with enhancement from a tapered T-junction such as a taper flange I-Section. Taken from Johnston & El Darwish, 1964")]
        [Input("tw", "Web thickness.", typeof(Length))]
        [Input("tf", "Mean flange thickness.", typeof(Length))]
        [Input("r", "Root radius.", typeof(Length))]
        [Input("t2", "Thickness of flange at intersection with near face of web.", typeof(Length))]
        [Input("s", "Flange taper slope.", typeof(Ratio))]
        private static double AlphaTaperTJunction(double tw, double tf, double r, double t2, double s)
        {
            if (0.2 > r / t2 || r / t2 > 1.0)
                Base.Compute.RecordWarning("Calculation of alpha term of torsional constant may not be accurate because ratio of root radius to flange thickness is out of the applicable range.");

            if (0.2 > tw / t2 || tw / t2 > 1.0)
                Base.Compute.RecordWarning("Calculation of alpha term of torsional constant may not be accurate because ratio of web thickness to flange thickness is out of the applicable range.");

            double alpha0 = AlphaTJunction(tw, tf, r);

            //Equation 28
            double alpha16 = -0.0836 + 0.2536 * tw / t2 + 0.1268 * r / t2 - 0.0806 * tw * r / Math.Pow(t2, 2) - 0.0858 * Math.Pow(tw / t2, 2); 
            //Interpolate alpha between parallel flange and slope of 1 to 6:
            return alpha0 + (alpha16 - alpha0) * s / (1.0/6);
        }

        /***************************************************/

        [Description("Empirical formula used to correct the torsional constant with enhancement from a tapered L-junction such as a taper flange channel. Taken from Johnston & El Darwish, 1964")]
        [Input("tw", "Web thickness.", typeof(Length))]
        [Input("tf", "Mean flange thickness.", typeof(Length))]
        [Input("r", "Root radius.", typeof(Length))]
        [Input("t2", "Thickness of flange at intersection with near face of web.", typeof(Length))]
        [Input("s", "Flange taper slope.", typeof(Ratio))]
        private static double AlphaTaperLJunction(double tw, double tf, double r, double t2, double s)
        {
            if (0.2 > r / t2 || r / t2 > 1.0)
                Base.Compute.RecordWarning("Calculation of alpha term of torsional constant may not be accurate because ratio of root radius to flange thickness is out of the applicable range.");

            if (0.2 > tw / t2 || tw / t2 > 1.0)
                Base.Compute.RecordWarning("Calculation of alpha term of torsional constant may not be accurate because ratio of web thickness to flange thickness is out of the applicable range.");

            double alpha0 = AlphaLJunction(tw, tf, r);
            //Equation 30
            double alpha16 = -0.1325 + 0.3015 * tw / t2 + 0.1400 * r / t2 - 0.1070 * tw * r / Math.Pow(t2, 2) - 0.0956 * Math.Pow(tw / t2, 2); 
            //Interpolate alpha between parallel flange and slope of 1 to 6:
            return alpha0 + (alpha16 - alpha0) * s / (1.0/6);
        }

        /***************************************************/

        private static double EndLossCorrectionVl(double s)
        {
            return 0.10504 - 0.10000 * s + 0.08480 * Math.Pow(s, 2) - 0.06746 * Math.Pow(s, 3) + 0.05153 * Math.Pow(s, 4);
        }

        /***************************************************/

        private static double EndLossCorrectionVs(double s)
        {
            return 0.10504 + 0.10000 * s + 0.08480 * Math.Pow(s, 2) + 0.06746 * Math.Pow(s, 3) + 0.05153 * Math.Pow(s, 4);
        }

        /***************************************************/
    }
}




