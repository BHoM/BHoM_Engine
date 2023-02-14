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



        ///***************************************************/

        //[Description("")]
        //[Input("", "")]
        //[Output("", "")]
        //public static void SetAnalysisParameters(this SawnTimber timber, TimberAnalysisStiffness stiffnessType)
        //{
        //    if (stiffnessType == TimberAnalysisStiffness.Mean)
        //    {
        //        timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelMean, Y = timber.YoungsModulusPerpendicularMean, Z = timber.YoungsModulusPerpendicularMean };
        //        timber.ShearModulus = new Vector { X = timber.ShearModulusMean, Y = timber.ShearModulusMean, Z = timber.ShearModulusMean };
        //    }
        //    else
        //    {
        //        timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelCharacteristic, Y = timber.YoungsModulusPerpendicularMean, Z = timber.YoungsModulusPerpendicularMean };
        //        timber.ShearModulus = new Vector { X = timber.ShearModulusMean, Y = timber.ShearModulusMean, Z = timber.ShearModulusMean };
        //    }
        //}

        ///***************************************************/

        //[Description("")]
        //[Input("", "")]
        //[Output("", "")]
        //public static void SetAnalysisParameters(this Glulam timber, TimberAnalysisStiffness stiffnessType)
        //{
        //    if (stiffnessType == TimberAnalysisStiffness.Mean)
        //    {
        //        timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelMean, Y = timber.YoungsModulusPerpendicularMean, Z = timber.YoungsModulusPerpendicularMean };
        //        timber.ShearModulus = new Vector { X = timber.ShearModulusMean, Y = timber.ShearModulusMean, Z = timber.RollingShearModulusMean };
        //    }
        //    else
        //    {
        //        timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelCharacteristic, Y = timber.YoungsModulusPerpendicularCharacteristic, Z = timber.YoungsModulusPerpendicularCharacteristic };
        //        timber.ShearModulus = new Vector { X = timber.ShearModulusCharacteristic, Y = timber.ShearModulusCharacteristic, Z = timber.RollingShearModulusCharacteristic };
        //    }
        //}

        ///***************************************************/

        //[Description("")]
        //[Input("", "")]
        //[Output("", "")]
        //public static void SetAnalysisParameters(this LVLC timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType)
        //{
        //    if (orientationType == TimberAnalysisOrientation.Unknown)
        //        BH.Engine.Base.Compute.RecordError($"Requires set {orientationType} to be able to set analysis parameters for materials of type {nameof(LVLC)}.");
        //    else if (orientationType == TimberAnalysisOrientation.Flatwise)
        //    {
        //        if (stiffnessType == TimberAnalysisStiffness.Mean)
        //        {
        //            timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelMean, Y = timber.YoungsModulusPerpendicularFlatMean, Z = timber.YoungsModulusPerpendicularFlatMean };
        //            timber.ShearModulus = new Vector { X = timber.ShearModulusParallelFlatMean, Y = timber.ShearModulusParallelFlatMean, Z = timber.ShearModulusPerpendicularFlatMean };
        //        }
        //        else
        //        {
        //            timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelCharacteristic, Y = timber.YoungsModulusPerpendicularFlatCharacteristic, Z = timber.YoungsModulusPerpendicularFlatCharacteristic };
        //            timber.ShearModulus = new Vector { X = timber.ShearModulusParallelFlatCharacteristic, Y = timber.ShearModulusParallelFlatCharacteristic, Z = timber.ShearModulusPerpendicularFlatCharacteristic };
        //        }
        //    }
        //    else
        //    {
        //        if (stiffnessType == TimberAnalysisStiffness.Mean)
        //        {
        //            timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelMean, Y = timber.YoungsModulusPerpendicularEdgeMean, Z = timber.YoungsModulusPerpendicularEdgeMean };
        //            timber.ShearModulus = new Vector { X = timber.ShearModulusEdgeMean, Y = timber.ShearModulusEdgeMean, Z = timber.ShearModulusPerpendicularFlatMean };
        //        }
        //        else
        //        {
        //            timber.YoungsModulus = new Vector { X = timber.YoungsModulusParallelCharacteristic, Y = timber.YoungsModulusPerpendicularFlatCharacteristic, Z = timber.YoungsModulusPerpendicularFlatCharacteristic };
        //            timber.ShearModulus = new Vector { X = timber.ShearModulusParallelFlatCharacteristic, Y = timber.ShearModulusParallelFlatCharacteristic, Z = timber.ShearModulusPerpendicularFlatCharacteristic };
        //        }
        //    }
        //}

        private static void SetAnalysisParameters(this ITimber<IYoungsModulusTimber, IShearModulusTimber, IStrengthTimber> timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        { 
            timber.YoungsModulus = timber.YoungsModulusProperties.IYoungsModulusVector(stiffnessType, orientationType);
            timber.ShearModulus = timber.ShearModulusProperties.IShearModulusVector(stiffnessType, orientationType);
        }

        /***************************************************/
        /**** Private Methods - fallback                ****/
        /***************************************************/

        private static void SetAnalysisParameters(this ITimber timber, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            SetAnalysisParameters(timber as dynamic, stiffnessType);
        }

        /***************************************************/

        private static void SetAnalysisParameters(this ITimber timber, TimberAnalysisStiffness stiffnessType)
        {
            BH.Engine.Base.Compute.RecordError($"{nameof(SetAnalysisParameters)} is not implemented for ITimber of type {timber.GetType()}");
        }

        /***************************************************/

        private static Vector IYoungsModulusVector(this IYoungsModulusTimber youngsModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            return YoungsModulusVector(youngsModulusProprties as dynamic, stiffnessType, orientationType);
        }

        /***************************************************/

        private static Vector YoungsModulusVector(this YoungsModulusSawnTimber youngsModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            if (stiffnessType == TimberAnalysisStiffness.Mean)
            {
                return new Vector { X = youngsModulusProprties.ParallelMean, Y = youngsModulusProprties.PerpendicularMean, Z = youngsModulusProprties.PerpendicularMean };
            }
            else
            {
                BH.Engine.Base.Compute.RecordNote("Sawn timber materials does not contain characteristic values for youngs modulus perpendicular. Mean value of perpendicular stiffness used.");
               return new Vector { X = youngsModulusProprties.ParallelCharacteristic, Y = youngsModulusProprties.PerpendicularMean, Z = youngsModulusProprties.PerpendicularMean };
            }
        }

        /***************************************************/

        private static Vector YoungsModulusVector(this YoungsModulusGlulam youngsModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            if (stiffnessType == TimberAnalysisStiffness.Mean)
            {
                return new Vector { X = youngsModulusProprties.ParallelMean, Y = youngsModulusProprties.PerpendicularMean, Z = youngsModulusProprties.PerpendicularMean };
            }
            else
            {
                return new Vector { X = youngsModulusProprties.ParallelCharacteristic, Y = youngsModulusProprties.PerpendicularCharacteristic, Z = youngsModulusProprties.PerpendicularCharacteristic };
            }
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static Vector YoungsModulusVector(this YoungsModulusLVLC youngsModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType)
        {
            if (orientationType == TimberAnalysisOrientation.Unknown)
            {
                Base.Compute.RecordError($"Requires set {orientationType} to be able to set analysis parameters for materials of type {nameof(LVLC)}.");
                return null;
            }
            else if (orientationType == TimberAnalysisOrientation.Flatwise)
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    return new Vector { X = youngsModulusProprties.ParallelMean, Y = youngsModulusProprties.PerpendicularEdgeMean, Z = youngsModulusProprties.PerpendicularFlatMean };
                }
                else
                {
                    return new Vector { X = youngsModulusProprties.ParallelCharacteristic, Y = youngsModulusProprties.PerpendicularEdgeCharacteristic, Z = youngsModulusProprties.PerpendicularFlatCharacteristic };
                }
            }
            else
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    return new Vector { X = youngsModulusProprties.ParallelMean, Y = youngsModulusProprties.PerpendicularFlatMean, Z = youngsModulusProprties.PerpendicularEdgeMean };
                }
                else
                {
                    return new Vector { X = youngsModulusProprties.ParallelCharacteristic, Y = youngsModulusProprties.PerpendicularFlatCharacteristic, Z = youngsModulusProprties.PerpendicularEdgeCharacteristic };
                }
            }
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static Vector YoungsModulusVector(this YoungsModulusLVLP youngsModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType)
        {
            if (stiffnessType == TimberAnalysisStiffness.Mean)
            {
                return new Vector { X = youngsModulusProprties.ParallelMean, Y = youngsModulusProprties.PerpendicularEdgeMean, Z = youngsModulusProprties.PerpendicularEdgeMean };
            }
            else
            {
                return new Vector { X = youngsModulusProprties.ParallelCharacteristic, Y = youngsModulusProprties.PerpendicularEdgeCharacteristic, Z = youngsModulusProprties.PerpendicularEdgeCharacteristic };
            }
        }

        /***************************************************/

        private static Vector IShearModulusVector(this IShearModulusTimber shearModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            return ShearModulusVector(shearModulusProprties as dynamic, stiffnessType, orientationType);
        }

        /***************************************************/

        private static Vector ShearModulusVector(this ShearModulusSawnTimber shearModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            string message = "";

            if (stiffnessType == TimberAnalysisStiffness.Characteristic)
                message = $"{nameof(SawnTimber)} only contaiains mean value for shear modulus (G0,mean), no characteristic values or rolling shear stiffness. Mean shear strength will be applied in all directions.";
            else
                message = $"{nameof(SawnTimber)} only contaiains mean value for shear modulus (G0,mean), no rolling shear stiffness. Mean shear strength will be applied in all directions.";

            Base.Compute.RecordNote(message);

            return new Vector { X = shearModulusProprties.Mean, Y = shearModulusProprties.Mean, Z = shearModulusProprties.Mean };
        }

        /***************************************************/

        private static Vector ShearModulusVector(this ShearModulusGlulam shearModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            if (stiffnessType == TimberAnalysisStiffness.Mean)
            {
                return new Vector { X = shearModulusProprties.Mean, Y = shearModulusProprties.RollingMean, Z = shearModulusProprties.Mean };
            }
            else
            {
                return new Vector { X = shearModulusProprties.Characteristic, Y = shearModulusProprties.RollingCharacteristic, Z = shearModulusProprties.Mean };
            }
        }

        /***************************************************/

        private static Vector ShearModulusVector(this ShearModulusLVLC shearModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            if (orientationType == TimberAnalysisOrientation.Unknown)
            {
                Base.Compute.RecordError($"Requires set {orientationType} to be able to set analysis parameters for materials of type {nameof(LVLC)}.");
                return null;
            }
            else if (orientationType == TimberAnalysisOrientation.Flatwise)
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    return new Vector { X = shearModulusProprties.ParallelFlatMean, Y = shearModulusProprties.PerpendicularFlatMean, Z = shearModulusProprties.EdgeMean };
                }
                else
                {
                    return new Vector { X = shearModulusProprties.ParallelFlatCharacteristic, Y = shearModulusProprties.PerpendicularFlatCharacteristic, Z = shearModulusProprties.EdgeCharacteristic };
                }
            }
            else
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    return new Vector { X = shearModulusProprties.EdgeMean, Y = shearModulusProprties.PerpendicularFlatMean, Z = shearModulusProprties.ParallelFlatMean };
                }
                else
                {
                    return new Vector { X = shearModulusProprties.EdgeCharacteristic, Y = shearModulusProprties.PerpendicularFlatCharacteristic, Z = shearModulusProprties.ParallelFlatCharacteristic };
                }
            }
        }

        /***************************************************/

        private static Vector ShearModulusVector(this ShearModulusLVLP shearModulusProprties, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            if (orientationType == TimberAnalysisOrientation.Unknown)
            {
                Base.Compute.RecordError($"Requires set {orientationType} to be able to set analysis parameters for materials of type {nameof(LVLC)}.");
                return null;
            }

            Base.Compute.RecordNote($"{nameof(LVLP)} does not contain a value for perpendicular shear modulus. {stiffnessType} value for flat shear modulus has been assigned to the Y component of the shear modulus Vector.");
            if (orientationType == TimberAnalysisOrientation.Flatwise)
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    return new Vector { X = shearModulusProprties.FlatMean, Y = shearModulusProprties.FlatMean, Z = shearModulusProprties.EdgeMean };
                }
                else
                {
                    return new Vector {X = shearModulusProprties.FlatCharacteristic, Y = shearModulusProprties.FlatCharacteristic, Z = shearModulusProprties.EdgeCharacteristic };
                }
            }
            else
            {
                if (stiffnessType == TimberAnalysisStiffness.Mean)
                {
                    return new Vector { X = shearModulusProprties.EdgeMean, Y = shearModulusProprties.FlatMean, Z = shearModulusProprties.FlatMean };
                }
                else
                {
                    return new Vector { X = shearModulusProprties.EdgeCharacteristic, Y = shearModulusProprties.FlatCharacteristic, Z = shearModulusProprties.FlatCharacteristic };
                }
            }
        }

        /***************************************************/

    }
}
