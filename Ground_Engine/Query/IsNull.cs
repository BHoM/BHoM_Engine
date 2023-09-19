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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BH.oM.Base.Attributes;
using BH.oM.Ground;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;


namespace BH.Engine.Ground
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a Geology or its defining properties are null and outputs relevant error message.")]
        [Input("geology", "The Geology to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Geology or its defining properties are null.")]
        public static bool IsNull(this Geology geology, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            // Check the strata lists 
            if(geology.StrataTop.IsNullOrEmpty() || geology.StrataBottom.IsNullOrEmpty())
            {
                ErrorMessage(methodName, geology.GetType().ToString(), "The StrataTop and/or the StataBottom lists are empty or null. " + msg);
                return true;
            }
            else if(geology.StrataBottom.Count != geology.StrataTop.Count)
            {
                ErrorMessage(methodName, geology.GetType().ToString(), "The StrataTop and StrataBotom do not have equal list lengths." + msg);
                return true;
            }
            
            if(geology.LogDescription.IsNullOrEmpty() || geology.Legend.IsNullOrEmpty() || geology.ObservedGeology.IsNullOrEmpty())
            {
                ErrorMessage(methodName, geology.GetType().ToString(), "The LogDescription, Legend and ObservedGeology are empty or null." + msg);
                return true;
            }
            else if (geology.LogDescription.Count != geology.Legend.Count || geology.LogDescription.Count != geology.InterpretedGeology.Count)
            {
                ErrorMessage(methodName, geology.GetType().ToString(), "The LogDescription, Legend and InterpretedGeology do not have equal list lengths." + msg);
                return true;
            }
            else if(geology.StrataTop.Count != geology.LogDescription.Count)
            {
                ErrorMessage(methodName, geology.GetType().ToString(), "The strata depths and log parameters do not have equal list lengths." + msg);
                return true;
            }

            return false;
        }

        [Description("Checks if a IGeologyProperty is null and outputs relevant error message.")]
        [Input("property", "The IGeologyProperty to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the Geology or its defining properties are null.")]
        public static bool IsNull(this IGeologicalProperties property, string msg = "", [CallerMemberName] string methodName = "Method")
        {
            if(property == null)
            {
                ErrorMessage(methodName, property.GetType().ToString(), msg);
                return true;
            }

            return false;
        }

            /***************************************************/
            /**** Private Methods                           ****/
            /***************************************************/

            private static void ErrorMessage(string methodName = "Method", string type = "type", string msg = "")
        {
            Base.Compute.RecordError($"Cannot evaluate {methodName} because the {type} is null. {msg}");
        }

        /***************************************************/

    }
}


