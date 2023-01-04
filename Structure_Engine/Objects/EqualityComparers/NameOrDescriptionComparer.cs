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
using System.Collections.Generic;
using BH.oM.Structure;

namespace BH.Engine.Structure
{
    public class NameOrDescriptionComparer : IEqualityComparer<IProperty>
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public NameOrDescriptionComparer()
        {

        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Equals(IProperty prop1, IProperty prop2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(prop1, prop2)) return true;

            //Check whether any of the compared objects is null.
            if (prop1 == null || prop2 == null)
                return false;

            string desc1 = prop1.DescriptionOrName();
            string desc2 = prop2.DescriptionOrName();

            if (desc1 == null || desc2 == null)
                return false;

            //Return true if name or description string for both objects are the same
            return desc1 == desc2;
        }

        /***************************************************/

        public int GetHashCode(IProperty prop)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(prop, null)) return 0;

            return prop.DescriptionOrName().GetHashCode();

        }

        /***************************************************/
    }
}




