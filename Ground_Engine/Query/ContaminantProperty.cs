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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Data;
using BH.oM.Geometry;
using BH.oM.Ground;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;
using BH.Engine.Data;
using BH.Engine.Geometry;
using BH.oM.Data.Requests;


namespace BH.Engine.Ground
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the IContaminantProperty matching the type provided..")]
        [Input("sample", "The ContaminantSample to retrieve the property from.")]
        [Input("type", "The type that inherits IContaminantProperty to search the ContaminantSample for.")]
        [Output("property", "The IContaminantProperty found on the ContaminantSample.")]
        public static IContaminantProperty ContaminantProperty(this ContaminantSample sample, Type type)
        {
            if (sample.IsValid())
            {
                List<IContaminantProperty> contaminantProperties = sample.ContaminantProperties;

                if (contaminantProperties.Select(x => x.GetType()).Contains(type))
                    return (IContaminantProperty)Base.Query.FilterByType(contaminantProperties, type).First();
                else
                {
                    Base.Compute.RecordError($"The ContaminantSample does not contain {type}.");
                    return null;
                }
            }
            else
                return null;
        }

        /***************************************************/

    }
}
