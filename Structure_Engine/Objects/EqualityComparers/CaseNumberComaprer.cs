/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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


using BH.oM.Structure.Loads;
using System.Collections.Generic;
using System;

namespace BH.Engine.Structure
{
    public class CaseNumberComaprer : IEqualityComparer<ICase>
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public bool Equals(ICase case1, ICase case2)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(case1, case2))
                return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(case1, null) || Object.ReferenceEquals(case2, null))
                return false;

            return case1.Number == case2.Number;
        }

        /***************************************************/

        public int GetHashCode(ICase obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            return obj.Number.GetHashCode();
        }

        /***************************************************/

    }
}


