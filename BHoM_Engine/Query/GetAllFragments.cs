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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns all Fragments of an object that inherit from the given parentType, or all of them if no parentType is provided.")]
        [Input("iBHoMObject", "Any IBHoMObject object.")]
        [Input("parentType", "All fragments of a type that inherits from this parent type will be returned. If not specified, all fragments are returned.")]
        [Output("fragmentList", "A deep copy of the fragments is returned for immutability.")]
        public static List<IFragment> GetAllFragments(this IBHoMObject iBHoMObject, Type parentType = null)
        {
            if (iBHoMObject == null)
                return new List<IFragment>();

            List<IFragment> fragments = new List<IFragment>();

            if (parentType == null)
                return iBHoMObject.Fragments.Select(fr => fr.DeepClone()).ToList();

            if (!typeof(IFragment).IsAssignableFrom(parentType))
            {
                Base.Compute.RecordError("Provided input in parentType is not a Fragment type (does not implement IFragment interface).");
                return null;
            }

            return iBHoMObject.Fragments.Where(fr => parentType.IsAssignableFrom(fr.GetType())).Select(fr => fr.DeepClone()).ToList();
        }

    }
}





