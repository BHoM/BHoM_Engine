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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.Constraints;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("Returns true if any of the degrees of freedom is of a value based type or the values are non-zero.")]
        public static bool IsNumericallyDependent(this Constraint3DOF constraint)
        {
            if (constraint.IsNull())
                return false;

            if (constraint.UX.IsNumericallyDependent())
                return true;

            if (constraint.UY.IsNumericallyDependent())
                return true;

            if (constraint.Normal.IsNumericallyDependent())
                return true;

            if (constraint.KX != 0)
                return true;

            if (constraint.KY != 0)
                return true;

            if (constraint.KNorm != 0)
                return true;

            return false;
        }

        /***************************************************/

        [Description("Returns true if any of the degrees of freedom is of a value based type or the values are non-zero.")]
        public static bool IsNumericallyDependent(this Constraint4DOF constraint)
        {
            if (constraint.IsNull())
                return false;

            if (constraint.TranslationX.IsNumericallyDependent())
                return true;

            if (constraint.TranslationY.IsNumericallyDependent())
                return true;

            if (constraint.TranslationZ.IsNumericallyDependent())
                return true;

            if (constraint.RotationX.IsNumericallyDependent())
                return true;

            if (constraint.TranslationalStiffnessX != 0)
                return true;

            if (constraint.TranslationalStiffnessY != 0)
                return true;

            if (constraint.TranslationalStiffnessZ != 0)
                return true;

            if (constraint.RotationalStiffnessX != 0)
                return true;

            return false;
        }

        /***************************************************/

        [Description("Returns true if any of the degrees of freedom is of a value based type or the values are non-zero.")]
        public static bool IsNumericallyDependent(this Constraint6DOF constraint)
        {
            if (constraint.IsNull())
                return false;

            if (constraint.TranslationX.IsNumericallyDependent())
                return true;

            if (constraint.TranslationY.IsNumericallyDependent())
                return true;

            if (constraint.TranslationZ.IsNumericallyDependent())
                return true;

            if (constraint.RotationX.IsNumericallyDependent())
                return true;

            if (constraint.RotationY.IsNumericallyDependent())
                return true;

            if (constraint.RotationZ.IsNumericallyDependent())
                return true;

            if (constraint.TranslationalStiffnessX != 0)
                return true;

            if (constraint.TranslationalStiffnessY != 0)
                return true;

            if (constraint.TranslationalStiffnessZ != 0)
                return true;

            if (constraint.RotationalStiffnessX != 0)
                return true;

            if (constraint.RotationalStiffnessY != 0)
                return true;

            if (constraint.RotationalStiffnessZ != 0)
                return true;

            return false;
        }

        /***************************************************/

        [Description("Returns true if the DOF is of a type that depends on a stiffness value.")]
        public static bool IsNumericallyDependent(this DOFType dof)
        {
            switch (dof)
            {
                case DOFType.Free:
                case DOFType.Fixed:
                case DOFType.FixedNegative:
                case DOFType.FixedPositive:
                    return false;
                case DOFType.SpringNegative:
                case DOFType.SpringPositive:
                case DOFType.SpringRelative:
                case DOFType.SpringRelativeNegative:
                case DOFType.SpringRelativePositive:
                case DOFType.NonLinear:
                case DOFType.Friction:
                case DOFType.Damped:
                case DOFType.Gap:
                default:
                    return true;
            }
        }

        /***************************************************/
    }
}



