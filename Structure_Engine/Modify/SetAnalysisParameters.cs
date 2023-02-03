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

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static void ISetAnalysisParameters(this ITimber timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            SetAnalysisParameters(timber as dynamic, stiffnessType, orientationType);
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static void SetAnalysisParameters(this SawnTimber timber, TimberAnalysisStiffness stiffnessType)
        {
            if (stiffnessType == TimberAnalysisStiffness.Mean)
            {
                timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelMean, Y = timber.YoungsModulusPerpendicularMean, Z = timber.YoungsModulusPerpendicularMean };
                timber.ShearModulus = new Vector { X = timber.ShearModulusMean, Y = timber.ShearModulusMean, Z = timber.ShearModulusMean };
            }
            else
            {
                timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelCharacteristic, Y = timber.YoungsModulusPerpendicularMean, Z = timber.YoungsModulusPerpendicularMean };
                timber.ShearModulus = new Vector { X = timber.ShearModulusMean, Y = timber.ShearModulusMean, Z = timber.ShearModulusMean };
            }
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static void SetAnalysisParameters(this Glulam timber, TimberAnalysisStiffness stiffnessType)
        {
            if (stiffnessType == TimberAnalysisStiffness.Mean)
            {
                timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelMean, Y = timber.YoungsModulusPerpendicularMean, Z = timber.YoungsModulusPerpendicularMean };
                timber.ShearModulus = new Vector { X = timber.ShearModulusMean, Y = timber.ShearModulusMean, Z = timber.RollingShearModulusMean };
            }
            else
            {
                timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelCharacteristic, Y = timber.YoungsModulusPerpendicularCharacteristic, Z = timber.YoungsModulusPerpendicularCharacteristic };
                timber.ShearModulus = new Vector { X = timber.ShearModulusCharacteristic, Y = timber.ShearModulusCharacteristic, Z = timber.RollingShearModulusCharacteristic };
            }
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static void SetAnalysisParameters(this LVLC timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType)
        {
            if (orientationType == TimberAnalysisOrientation.Unknown)
                BH.Engine.Base.Compute.RecordError($"Requires set {orientationType} to be able to set analysis parameters for materials of type {nameof(LVLC)}.");
            else if (orientationType == TimberAnalysisOrientation.Flatwise)
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelMean, Y = timber.YoungsModulusPerpendicularFlatMean, Z = timber.YoungsModulusPerpendicularFlatMean };
                    timber.ShearModulus = new Vector { X = timber.ShearModulusParallelFlatMean, Y = timber.ShearModulusParallelFlatMean, Z = timber.ShearModulusPerpendicularFlatMean };
                }
                else
                {
                    timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelCharacteristic, Y = timber.YoungsModulusPerpendicularFlatCharacteristic, Z = timber.YoungsModulusPerpendicularFlatCharacteristic };
                    timber.ShearModulus = new Vector { X = timber.ShearModulusParallelFlatCharacteristic, Y = timber.ShearModulusParallelFlatCharacteristic, Z = timber.ShearModulusPerpendicularFlatCharacteristic };
                }
            }
            else
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelMean, Y = timber.YoungsModulusPerpendicularEdgeMean, Z = timber.YoungsModulusPerpendicularEdgeMean };
                    timber.ShearModulus = new Vector { X = timber.ShearModulusEdgeMean, Y = timber.ShearModulusEdgeMean, Z = timber.ShearModulusPerpendicularFlatMean };
                }
                else
                {
                    timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelCharacteristic, Y = timber.YoungsModulusPerpendicularFlatCharacteristic, Z = timber.YoungsModulusPerpendicularFlatCharacteristic };
                    timber.ShearModulus = new Vector { X = timber.ShearModulusParallelFlatCharacteristic, Y = timber.ShearModulusParallelFlatCharacteristic, Z = timber.ShearModulusPerpendicularFlatCharacteristic };
                }
            }
        }

        /***************************************************/
        /**** Private Methods - fallback                ****/
        /***************************************************/

        public static void SetAnalysisParameters(this ITimber timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            SetAnalysisParameters(timber as dynamic, stiffnessType);
        }

        /***************************************************/

        public static void SetAnalysisParameters(this ITimber timber, TimberAnalysisStiffness stiffnessType)
        {
            BH.Engine.Base.Compute.RecordError($"{nameof(SetAnalysisParameters)} is not implemented for ITimber of type {timber.GetType()}");
        }

        /***************************************************/



    }
}
