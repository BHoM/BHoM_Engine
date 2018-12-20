/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Properties.Constraint;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Constraint6DOF Constraint6DOF(string name = "")
        {
            return new Constraint6DOF { Name = name };
        }

        /***************************************************/

        public static Constraint6DOF Constraint6DOF(string name, List<bool> fixity, List<double> values)
        {
            return new Constraint6DOF
            {
                Name = name,
                TranslationX = (fixity[0]) ? DOFType.Fixed : (values[0] == 0) ? DOFType.Free : DOFType.Spring,
                TranslationY = (fixity[1]) ? DOFType.Fixed : (values[1] == 0) ? DOFType.Free : DOFType.Spring,
                TranslationZ = (fixity[2]) ? DOFType.Fixed : (values[2] == 0) ? DOFType.Free : DOFType.Spring,
                RotationX = (fixity[3]) ? DOFType.Fixed : (values[3] == 0) ? DOFType.Free : DOFType.Spring,
                RotationY = (fixity[4]) ? DOFType.Fixed : (values[4] == 0) ? DOFType.Free : DOFType.Spring,
                RotationZ = (fixity[5]) ? DOFType.Fixed : (values[5] == 0) ? DOFType.Free : DOFType.Spring,

                TranslationalStiffnessX = values[0],
                TranslationalStiffnessY = values[1],
                TranslationalStiffnessZ = values[2],
                RotationalStiffnessX = values[3],
                RotationalStiffnessY = values[4],
                RotationalStiffnessZ = values[5]
            };
        }

        /***************************************************/

        public static Constraint6DOF PinConstraint6DOF(string name = "Pin")
        {
            return new Constraint6DOF
            {
                Name = name,
                TranslationX = DOFType.Fixed,
                TranslationY = DOFType.Fixed,
                TranslationZ = DOFType.Fixed
            };
        }

        /***************************************************/

        public static Constraint6DOF FixConstraint6DOF(string name = "Fix")
        {
            return new Constraint6DOF
            {
                Name = name,
                TranslationX = DOFType.Fixed,
                TranslationY = DOFType.Fixed,
                TranslationZ = DOFType.Fixed,
                RotationX = DOFType.Fixed,
                RotationY = DOFType.Fixed,
                RotationZ = DOFType.Fixed
            };
        }

        /***************************************************/

        public static Constraint6DOF FullReleaseConstraint6DOF(string name = "Release")
        {
            return new Constraint6DOF
            {
                Name = name,
                TranslationX = DOFType.Free,
                TranslationY = DOFType.Free,
                TranslationZ = DOFType.Free,
                RotationX = DOFType.Free,
                RotationY = DOFType.Free,
                RotationZ = DOFType.Free
            };
        }

        /***************************************************/

        public static Constraint6DOF Constraint6DOF(bool x, bool y, bool z, bool xx, bool yy, bool zz, string name = "")
        {
            return new Constraint6DOF
            {
                Name = name,
                TranslationX = x ? DOFType.Fixed : DOFType.Free,
                TranslationY = y ? DOFType.Fixed : DOFType.Free,
                TranslationZ = z ? DOFType.Fixed : DOFType.Free,
                RotationX = xx ? DOFType.Fixed : DOFType.Free,
                RotationY = yy ? DOFType.Fixed : DOFType.Free,
                RotationZ = zz ? DOFType.Fixed : DOFType.Free,
            };
        }

        /***************************************************/
    }
}
