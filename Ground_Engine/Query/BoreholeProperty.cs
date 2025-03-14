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
using BH.Engine.Base;


namespace BH.Engine.Ground
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the IBoreholeProperty matching the type provided..")]
        [Input("borehole", "The IBoreholeProperty to retrieve the property from.")]
        [Input("type", "The type that inherits IBoreholeProperty to search the Stratum for.")]
        [Output("property", "The IBoreholeProperty found on the Stratum.")]
        public static IBoreholeProperty BoreholeProperty(this Borehole borehole, Type type)
        {
            if (borehole.IsValid())
            {
                List<IBoreholeProperty> props = borehole.BoreholeProperties.Where(x => x.GetType() == type).ToList();

                if (props.IsNullOrEmpty($"The Borehole does not contain a property of {type}."))
                    return null;
                else
                {
                    if(props.Count > 1)
                        Base.Compute.RecordWarning($"Ambigous match as Borehole contains more than one property of type {type}. First one is returned.");

                    return props.First();
                }
            }
            else
                return null;
        }

        /***************************************************/

    }
}
