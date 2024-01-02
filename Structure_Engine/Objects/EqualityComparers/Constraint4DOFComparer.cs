/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
    public class Constraint4DOFComparer : IEqualityComparer<Constraint4DOF>
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public bool Equals(Constraint4DOF linearRelease1, Constraint4DOF linearRelease2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(linearRelease1, linearRelease2))
                return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(linearRelease1, null) || Object.ReferenceEquals(linearRelease2, null))
                return false;


            if (linearRelease1.Name == linearRelease2.Name &&
                linearRelease1.TranslationX == linearRelease2.TranslationX &&
                linearRelease1.TranslationY == linearRelease2.TranslationY &&
                linearRelease1.TranslationZ == linearRelease2.TranslationZ &&
                linearRelease1.RotationX == linearRelease2.RotationX &&
                linearRelease1.TranslationalStiffnessX == linearRelease2.TranslationalStiffnessX &&
                linearRelease1.TranslationalStiffnessY == linearRelease2.TranslationalStiffnessY &&
                linearRelease1.TranslationalStiffnessZ == linearRelease2.TranslationalStiffnessZ &&
                linearRelease1.RotationalStiffnessX == linearRelease2.RotationalStiffnessX)
                return true;

            return false;
        }

        /***************************************************/

        public int GetHashCode(Constraint4DOF obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            return obj.Name == null ? 0 : obj.Name.GetHashCode();
        }

        /***************************************************/
    }
}





