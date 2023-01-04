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

using BH.oM.Data.Requests;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static FilterRequest FilterRequest(Type type = null, string tag = "")
        {
            return new FilterRequest { Type = type, Tag = tag };
        }

        /***************************************************/
        public static FilterRequest FilterRequest(Type type, Dictionary<string, object> equalities, string tag = "")
        {
            return new FilterRequest { Type = type, Tag = tag, Equalities = equalities };
        }

        /***************************************************/

        public static FilterRequest FilterRequest(Type type, IEnumerable<object> cases = null, IEnumerable<object> objectIds = null, string tag = "")
        {
            Dictionary<string, object> equalities = new Dictionary<string, object>();

            if (cases != null)
                equalities["Cases"] = cases.ToList();
            if (objectIds != null)
                equalities["ObjectIds"] = objectIds.ToList();

            return FilterRequest(type, equalities, tag);
        }

        /***************************************************/
    }
}




