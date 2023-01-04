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


using BH.oM.Structure.Constraints;
using System.Collections.Generic;
using System;

namespace BH.Engine.Structure
{
    public class Constraint6DOFComparer : IEqualityComparer<Constraint6DOF>
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public bool Equals(Constraint6DOF support1, Constraint6DOF support2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(support1, support2))
                return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(support1, null) || Object.ReferenceEquals(support2, null))
                return false;


            if (support1.Name == support2.Name &&
                support1.TranslationX == support2.TranslationX &&
                support1.TranslationY == support2.TranslationY &&
                support1.TranslationZ == support2.TranslationZ &&
                support1.RotationX == support2.RotationX &&
                support1.RotationY == support2.RotationY &&
                support1.RotationZ == support2.RotationZ &&
                support1.TranslationalStiffnessX == support2.TranslationalStiffnessX &&
                support1.TranslationalStiffnessY == support2.TranslationalStiffnessY &&
                support1.TranslationalStiffnessZ == support2.TranslationalStiffnessZ &&
                support1.RotationalStiffnessX == support2.RotationalStiffnessX &&
                support1.RotationalStiffnessY == support2.RotationalStiffnessY &&
                support1.RotationalStiffnessZ == support2.RotationalStiffnessZ)
                return true;
            return false;
        }

        /***************************************************/

        public int GetHashCode(Constraint6DOF obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            return obj.Name == null ? 0 : obj.Name.GetHashCode();
        }

        /***************************************************/
    }
}




