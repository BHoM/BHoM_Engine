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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material based on the product specific properties.")]
        [Input("timber", "Timber material to update the Analysis proeprties of.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner. Not used for all timber products.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void ISetAnalysisParameters(this ITimber timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            SetAnalysisParameters(timber as dynamic, stiffnessType, orientationType);
        }

        /***************************************************/


        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material based on the product specific properties.")]
        [Input("timber", "Timber material to update the Analysis proeprties of.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void SetAnalysisParameters(this SawnTimber timber, TimberAnalysisStiffness stiffnessType)
        {
            string message = "";
            if (stiffnessType == TimberAnalysisStiffness.Mean)
            {
                timber.YoungsModulus = new Vector { X = timber.E_0_Mean, Y = timber.E_90_Mean, Z = timber.E_90_Mean };
                timber.ShearModulus = new Vector { X = timber.G_Mean, Y = timber.G_Mean, Z = timber.G_Mean };

                message = $"{nameof(SawnTimber)} only contaiains mean value for shear modulus, no rolling shear stiffness. Mean shear stiffness ({nameof(timber.G_Mean)}) will be applied in all directions.";
            }
            else
            {
                timber.YoungsModulus = new Vector { X = timber.E_0_k, Y = timber.E_90_Mean, Z = timber.E_90_Mean };
                timber.ShearModulus = new Vector { X = timber.G_Mean, Y = timber.G_Mean, Z = timber.G_Mean };

                message = $"{nameof(SawnTimber)} materials does not contain characteristic values for youngs modulus perpendicular. Mean value of perpendicular stiffness used." + Environment.NewLine;
                message += $"{nameof(SawnTimber)} only contaiains mean value for shear modulus, no characteristic values or rolling shear stiffness. Mean shear stiffness ({nameof(timber.G_Mean)}) will be applied in all directions.";
            }
            Base.Compute.RecordNote(message);
        }

        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material based on the product specific properties.")]
        [Input("timber", "Timber material to update the Analysis proeprties of.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void SetAnalysisParameters(this Glulam timber, TimberAnalysisStiffness stiffnessType)
        {
            if (stiffnessType == TimberAnalysisStiffness.Mean)
            {
                timber.YoungsModulus = new Vector { X = timber.E_0_Mean, Y = timber.E_90_Mean, Z = timber.E_90_Mean };
                timber.ShearModulus = new Vector { X = timber.G_Mean, Y = timber.Gr_Mean, Z = timber.G_Mean };
            }
            else
            {
                timber.YoungsModulus = new Vector { X = timber.E_0_k, Y = timber.E_90_k, Z = timber.E_90_k };
                timber.ShearModulus = new Vector { X = timber.G_k, Y = timber.Gr_k, Z = timber.G_k };
            }
        }

        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material based on the product specific properties.")]
        [Input("timber", "Timber material to update the Analysis proeprties of.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void SetAnalysisParameters(this LVLC timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType)
        {
            if (orientationType == TimberAnalysisOrientation.Unknown)
            {
                Base.Compute.RecordError($"Requires set {orientationType} to be able to set analysis parameters for materials of type {nameof(LVLC)}.");
                return;
            }
            
            if (orientationType == TimberAnalysisOrientation.Flatwise)
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    timber.YoungsModulus = new Vector { X = timber.E_0_Mean, Y = timber.E_90_Edge_Mean, Z = timber.E_90_Flat_Mean };
                    timber.ShearModulus = new Vector { X = timber.G_0_Flat_Mean, Y = timber.G_90_Flat_Mean, Z = timber.G_0_Edge_Mean };
                }
                else
                {
                    timber.YoungsModulus = new Vector { X = timber.E_0_k, Y = timber.E_90_Edge_k, Z = timber.E_90_Flat_k };
                    timber.ShearModulus = new Vector { X = timber.G_0_Flat_k, Y = timber.G_90_Flat_k, Z = timber.G_0_Edge_k };
                }
            }
            else
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    timber.YoungsModulus = new Vector { X = timber.E_0_Mean, Y = timber.E_90_Flat_Mean, Z = timber.E_90_Edge_Mean };
                    timber.ShearModulus = new Vector { X = timber.G_0_Edge_Mean, Y = timber.G_90_Flat_Mean, Z = timber.G_0_Flat_Mean };
                }
                else
                {
                    timber.YoungsModulus = new Vector { X = timber.E_0_k, Y = timber.E_90_Flat_k, Z = timber.E_90_Edge_k };
                    timber.ShearModulus = new Vector { X = timber.G_0_Edge_k, Y = timber.G_90_Flat_k, Z = timber.G_0_Flat_k };
                }
            }
        }

        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material based on the product specific properties.")]
        [Input("timber", "Timber material to update the Analysis proeprties of.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void SetAnalysisParameters(this LVLP timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType)
        {
            if (orientationType == TimberAnalysisOrientation.Unknown)
            {
                Base.Compute.RecordError($"Requires set {orientationType} to be able to set analysis parameters for materials of type {nameof(LVLC)}.");
                return;
            }

            string message = $"{nameof(LVLP)} does not contain a value for flatwise perpendicular youngs modulus. {stiffnessType} value for edgewise perpendicular youngs modulus will be assigned in its place.";
            message += $"{nameof(LVLP)} does not contain a value for perpendicular shear modulus. {stiffnessType} value for flat shear modulus has been assigned to the Y component of the shear modulus Vector.";
            Base.Compute.RecordNote(message);

            if (orientationType == TimberAnalysisOrientation.Flatwise)
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    timber.YoungsModulus = new Vector { X = timber.E_0_Mean, Y = timber.E_90_Edge_Mean, Z = timber.E_90_Edge_Mean };
                    timber.ShearModulus = new Vector { X = timber.G_0_Flat_Mean, Y = timber.G_0_Flat_Mean, Z = timber.G_0_Edge_Mean };
                }
                else
                {
                    timber.YoungsModulus = new Vector { X = timber.E_0_k, Y = timber.E_90_Edge_k, Z = timber.E_90_Edge_k };
                    timber.ShearModulus = new Vector { X = timber.G_0_Flat_k, Y = timber.G_0_Flat_k, Z = timber.G_0_Edge_k };
                }
            }
            else
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    timber.YoungsModulus = new Vector { X = timber.E_0_Mean, Y = timber.E_90_Edge_Mean, Z = timber.E_90_Edge_Mean };
                    timber.ShearModulus = new Vector { X = timber.G_0_Edge_Mean, Y = timber.G_0_Flat_Mean, Z = timber.G_0_Flat_Mean };
                }
                else
                {
                    timber.YoungsModulus = new Vector { X = timber.E_0_k, Y = timber.E_90_Edge_k, Z = timber.E_90_Edge_k };
                    timber.ShearModulus = new Vector { X = timber.G_0_Edge_k, Y = timber.G_0_Flat_k, Z = timber.G_0_Flat_k };
                }
            }
        }

        /***************************************************/
        /**** Private Methods - fallback                ****/
        /***************************************************/

        [Description("Method allowing the dynamic casting to find method that do not require orientationType for setting analytical properties.")]
        private static void SetAnalysisParameters(this ITimber timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            SetAnalysisParameters(timber as dynamic, stiffnessType);
        }

        /***************************************************/

        [Description("Fallback for materials not yet implemented.")]
        private static void SetAnalysisParameters(this ITimber timber, TimberAnalysisStiffness stiffnessType)
        {
            BH.Engine.Base.Compute.RecordError($"{nameof(SetAnalysisParameters)} is not implemented for ITimber of type {timber.GetType()}");
        }

        /***************************************************/

    }
}
