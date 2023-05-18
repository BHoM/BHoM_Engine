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

using BH.oM.Base.Attributes;
using BH.oM.Base.Attributes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return a dictionary indicating the level of UI Exposure an input should have.")]
        [Output("Dictionary where the keys are the names of the inputs, and the values their exposure levels.")]
        public static Dictionary<string, UIExposure> InputExposure(this MethodBase method)
        {
            if (method == null)
            {
                Base.Compute.RecordWarning("Cannot query the input exposure of a null method. Returning an empty dictionary instead.");
                return new Dictionary<string, UIExposure>();
            }

            List<InputAttribute> inputAttributes = method.GetCustomAttributes<InputAttribute>().ToList();
            Dictionary<string, UIExposure> exposures = new Dictionary<string, UIExposure>();

            foreach (ParameterInfo info in method.GetParameters())
            {
                var inputAttribute = inputAttributes.Where(x => x.Name == info.Name).FirstOrDefault();
                if(inputAttribute != null)
                    exposures[info.Name] = inputAttribute.Exposure;
                else
                    exposures[info.Name] = UIExposure.Display;
            }

            return exposures;
        }

        /***************************************************/
    }
}
