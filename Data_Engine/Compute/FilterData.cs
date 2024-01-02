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

using BH.oM.Base;
using BH.oM.Data.Requests;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IEnumerable<IBHoMObject> FilterData(this FilterRequest request, IEnumerable<IBHoMObject> objects)
        {
            if(request == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot filter data with a null request.");
                return new List<IBHoMObject>();
            }

            IEnumerable<IBHoMObject> result = objects;

            if (request.Tag != "")
                result = objects.Where(x => x.Tags.Contains(request.Tag));

            if (request.Type != null)
                result = result.Where(x => request.Type.IsAssignableFrom(x.GetType()));

            foreach (KeyValuePair<string, object> kvp in request.Equalities)
            {
                //TODO: Need to check the equalities as well
            }

            return result;
        }

        /***************************************************/
    }
}





