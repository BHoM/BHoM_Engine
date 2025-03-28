/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Constraints;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Constraint6DOF from a list of fixities and spring values.")]
        [Input("name", "Name to be used for the Constraint. This is required for various structural packages to create the object.")]
        [Input("fixity", "A list of 6 booleans, in the order TransX, TranxY, TransZ, RotX, RotY, RotZ. If true, the fixity is set to fixed. If false, the fixity is set to free or spring, depending if the double values provided are 0 or not.")]
        [Input("values", "A list of 6 doubles with positive values (>= 0), in the orderTransX, TranxY, TransZ, RotX, RotY, RotZ. If zero, and corresponding fixity is false, the DOF will be free. If non-zero, these values will be set as the spring values. \n" +
                         "First three should have the quantity force per unit length, last three should be moment per unit angle.")]
        [Output("cons", "The created custom Constraint6DOF.")]
        public static Constraint6DOF Constraint6DOF(string name, List<bool> fixity, List<double> values)
        {
            if (fixity.IsNullOrEmpty() || values.IsNullOrEmpty())
                return null;

            if(fixity.Count != 6 || values.Count != 6)
            {
                Base.Compute.RecordError("The list of fixities or the list of values are not equal to 6 and therefore the Constraint6DOF cannot be created.");
                return null;
            }

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

        [Description("Creates a Constraint6DOF from a set of booleans. True denotes fixed, false denotes free.")]
        [Input("x", "Translational fixity in x-direction. True denotes fixed, false denotes free.")]
        [Input("y", "Translational fixity in y-direction. True denotes fixed, false denotes free.")]
        [Input("z", "Translational fixity in z-direction. True denotes fixed, false denotes free.")]
        [Input("xx", "Rotational fixity about the x-axis. True denotes fixed, false denotes free.")]
        [Input("yy", "Rotational fixity about the y-axis. True denotes fixed, false denotes free.")]
        [Input("zz", "Rotational fixity about the z-axis. True denotes fixed, false denotes free.")]
        [Input("name", "Name of the Constraint6DOF. This is required for various structural packages to create the object.")]
        [Output("cons", "The created custom Constraint6DOF.")]
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






