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

using BH.Engine.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns all loaded BHoM types that inherit from the specified parent type.")]
        [Input("parentType", "Parent type of the subtypes to be returned.")]
        [Output("subtypes", "Collection of BHoM subtypes of the input parent type.")]
        public static List<Type> Subtypes(this Type parentType)
        {
            if (parentType == null)
            {
                Base.Compute.RecordWarning("Cannot query the subtypes of a null type.");
                return new List<Type>();
            }

            List<Type> implemented = new List<Type>();
            foreach (Type t in Base.Query.BHoMTypeList())
            {
                if (t == parentType)
                    continue;

                if (parentType.IsAssignableFromIncludeGenerics(t))
                    implemented.Add(t);
            }

            return implemented;
        }

        /***************************************************/
    }
}
