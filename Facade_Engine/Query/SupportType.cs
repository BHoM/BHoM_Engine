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

using BH.oM.Facade.Enums;
using BH.oM.Structure.Elements;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        public static SupportType SupportType(this Bar bar)
        {
            bool isStartFix = IsFix(bar?.Release?.StartRelease);
            bool isStartPin = IsPin(bar?.Release?.StartRelease);
            bool isStartSlide = IsSlide(bar?.Release?.StartRelease);
            bool isEndFix = IsFix(bar?.Release?.EndRelease);
            bool isEndPin = IsPin(bar?.Release?.EndRelease);
            bool isEndSlide = IsSlide(bar?.Release?.EndRelease);

            if (isStartFix && isEndFix)
                return oM.Facade.Enums.SupportType.FixFix;
            else if (isStartPin && isEndPin)
                return oM.Facade.Enums.SupportType.PinPin;
            else if ((isStartSlide && isEndPin) || (isStartPin && isEndSlide))
                return oM.Facade.Enums.SupportType.PinSlide;
            else
                return oM.Facade.Enums.SupportType.Undefined;
        }


        /***************************************************/
        /****          Private Methods                  ****/
        /***************************************************/

        private static bool IsFix(BH.oM.Structure.Constraints.Constraint6DOF support)
        {
            if (support == null)
            {
                BH.Engine.Base.Compute.RecordError("Support condition information is missing.");
                return false;
            }

            if (!IsFullStiffness(support))
                return false;

            return support.TranslationX == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationY == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationZ == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationX == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationY == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationZ == oM.Structure.Constraints.DOFType.Fixed;
        }

        /***************************************************/

        private static bool IsPin(BH.oM.Structure.Constraints.Constraint6DOF support)
        {
            if (support == null)
            {
                BH.Engine.Base.Compute.RecordError("Support condition information is missing.");
                return false;
            }

            if (!IsFullStiffness(support))
                return false;

            // Rotation around own self (x) not relevant for this exercise
            return support.TranslationX == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationY == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationZ == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationY == oM.Structure.Constraints.DOFType.Free
                && support.RotationZ == oM.Structure.Constraints.DOFType.Free;
        }

        /***************************************************/

        private static bool IsSlide(BH.oM.Structure.Constraints.Constraint6DOF support)
        {
            if (support == null)
            {
                BH.Engine.Base.Compute.RecordError("Support condition information is missing.");
                return false;
            }

            if (!IsFullStiffness(support))
                return false;

            // Rotation around own self (x) not relevant for this exercise
            return support.TranslationX == oM.Structure.Constraints.DOFType.Free
                && support.TranslationY == oM.Structure.Constraints.DOFType.Fixed
                && support.TranslationZ == oM.Structure.Constraints.DOFType.Fixed
                && support.RotationY == oM.Structure.Constraints.DOFType.Free
                && support.RotationZ == oM.Structure.Constraints.DOFType.Free;
        }

        /***************************************************/

        private static bool IsFullStiffness(BH.oM.Structure.Constraints.Constraint6DOF support)
        {
            return support.RotationalStiffnessX == 0
                && support.RotationalStiffnessY == 0
                && support.RotationalStiffnessZ == 0
                && support.TranslationalStiffnessX == 0
                && support.TranslationalStiffnessY == 0
                && support.TranslationalStiffnessZ == 0;
        }

        /***************************************************/
    }
}
