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
using BH.oM.Base.Attributes;
using BH.oM.Ground;


namespace BH.Engine.Ground
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the IStratumProperty matching the type provided..")]
        [Input("sample", "The IStratumProperty to retrieve the property from.")]
        [Input("type", "The type that inherits IStratumProperty to search the Stratum for.")]
        [Output("property", "The IStratumProperty found on the Stratum.")]
        public static IStratumProperty StratumProperty(this Stratum stratum, Type type)
        {
            if (stratum.IsValid())
            {
                List<IStratumProperty> stratumProperties = stratum.Properties;

                if (stratumProperties.Select(x => x.GetType()).Contains(type))
                    return (IStratumProperty)Base.Query.FilterByType(stratumProperties, type).First();
                else
                {
                    Base.Compute.RecordError($"The Stratum does not contain {type}.");
                    return null;
                }
            }
            else
                return null;
        }

        /***************************************************/

    }
}
