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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Facade.Enums;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        public static bool DeflectionCheck(Bar bar, double windLoad, double tributaryWidth, BuildingCode buildingCode)
        {
            if (bar == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot check a null bar.");
                return false;
            }

            double allowableDeflection = Query.AllowableDeflection(bar.Length(), buildingCode);
            if (double.IsNaN(allowableDeflection))
            {
                BH.Engine.Base.Compute.RecordError("Check could not be executed.");
                return false;
            }

            SupportType supportType = bar.SupportType();
            if (supportType == oM.Facade.Enums.SupportType.Undefined)
            {
                // undefined support error
                return false;
            }

            double momentOfInertia = bar.MomentOfInertia();
            if (double.IsNaN(momentOfInertia) || momentOfInertia == 0)
            {
                // no moment of inertia error
                return false;
            }

            double youngsModulus = bar.YoungsModulus();
            if (double.IsNaN(youngsModulus) || youngsModulus == 0)
            {
                // no youngs modulus error
                return false;
            }

            double actualDeflection = Query.ActualDeflection(supportType, windLoad, tributaryWidth, bar.Length(), momentOfInertia, youngsModulus);
            if (double.IsNaN(actualDeflection))
            {
                // error
                return false;
            }

            return actualDeflection <= allowableDeflection;
        }


        /***************************************************/

        private static double MomentOfInertia(this Bar bar)
        {
            ISectionProperty property = bar?.SectionProperty;
            if (property == null)
                return double.NaN;
            else
                return property.Iy;
        }

        private static double YoungsModulus(this Bar bar)
        {
            IIsotropic material = bar?.SectionProperty?.Material as IIsotropic;
            if (material == null)
                return double.NaN;
            else
                return material.YoungsModulus;
        }

        private static SupportType SupportType(this BH.oM.Structure.Elements.Bar bar)
        {
            bool isStartFix = IsFix(bar?.Release?.StartRelease);
            bool isStartPin = IsPin(bar?.Release?.StartRelease);
            bool isEndFix = IsFix(bar?.Release?.EndRelease);
            bool isEndPin = IsPin(bar?.Release?.EndRelease);

            if (isStartFix && isEndFix)
                return oM.Facade.Enums.SupportType.FixFix;
            else if (isStartPin && isEndPin)
                return oM.Facade.Enums.SupportType.PinPin;
            else if ((isStartFix && isEndPin) || (isStartPin && isEndFix))
                return oM.Facade.Enums.SupportType.FixPin;
            else
                return oM.Facade.Enums.SupportType.Undefined;
        }

        private static bool IsFix(BH.oM.Structure.Constraints.Constraint6DOF support)
        {
            if (support == null)
            {
                // Fill in
                BH.Engine.Base.Compute.RecordError("");
                return false;
            }

            if (!IsFullStiffness(support))
            {
                // Fill in - weird stiffness error
                BH.Engine.Base.Compute.RecordError("");
                return false;
            }

            return support.TranslationX == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationY == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationZ == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationX == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationY == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationZ == oM.Structure.Constraints.DOFType.Fixed;
        }

        private static bool IsPin(BH.oM.Structure.Constraints.Constraint6DOF support)
        {
            if (support == null)
            {
                // Fill in
                BH.Engine.Base.Compute.RecordError("");
                return false;
            }

            if (!IsFullStiffness(support))
            {
                // Fill in - weird stiffness error
                BH.Engine.Base.Compute.RecordError("");
                return false;
            }

            return support.TranslationX == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationY == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationZ == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationX == oM.Structure.Constraints.DOFType.Free
                && support.RotationY == oM.Structure.Constraints.DOFType.Free
                && support.RotationZ == oM.Structure.Constraints.DOFType.Free;
        }

        private static bool IsFullStiffness(BH.oM.Structure.Constraints.Constraint6DOF support)
        {
            return support.RotationalStiffnessX == 0
                && support.RotationalStiffnessY == 0
                && support.RotationalStiffnessZ == 0
                && support.TranslationalStiffnessX == 0
                && support.TranslationalStiffnessY == 0
                && support.TranslationalStiffnessZ == 0;
        }
    }
}




