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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using BH.Engine.Reflection;
using BH.oM.Analytical.Results;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the name of all properties of the result that are of ObjectIdentifier types. This is all properties tagged with the ObjectIdentifierAttribute.")]
        [Input("result", "The result to get properties from.")]
        [Output("objectIdentifiers", "All object identifier properties of the provided result.")]
        public static List<string> ObjectIdentifierProperties(this IResult result)
        {
            if (result == null)
            {
                Engine.Base.Compute.RecordError("Cannot get object identifiers from a null result.");
                return new List<string>();
            }

            return result.GetType().ObjectIdentifierProperties();
        }

        /***************************************************/

        [Description("Gets the name of all properties of the result that are of ObjectIdentifier types. This is all properties tagged with the ObjectIdentifierAttribute.")]
        [Input("resultType", "The result type to get properties from.")]
        [Output("objectIdentifiers", "All object identifier properties of the provided result type.")]
        public static List<string> ObjectIdentifierProperties(this Type resultType)
        {
            if (resultType == null)
            {
                Engine.Base.Compute.RecordError("Cannot get object identifiers from a null type.");
                return new List<string>();
            }
            return resultType.PropertiesWithAttribute(typeof(ObjectIdentifierAttribute), true, true).Select(x => x.Name).ToList();
        }

        /***************************************************/

        [Description("Gets the name of all properties of the result that are of Scenario types. This is all properties tagged with the ScenarioIdentifierAttribute.")]
        [Input("result", "The result to get properties from.")]
        [Output("scenarioIdentifiers", "All scenario identifier properties of the provided result.")]
        public static List<string> ScenarioIdentifierProperties(this IResult result)
        {
            if (result == null)
            {
                Engine.Base.Compute.RecordError("Cannot get scenario identifiers from a null result.");
                return new List<string>();
            }

            return result.GetType().ScenarioIdentifierProperties();
        }

        /***************************************************/

        [Description("Gets the name of all properties of the result that are of Scenario types. This is all properties tagged with the ScenarioIdentifierAttribute.")]
        [Input("resultType", "The result type to get properties from.")]
        [Output("scenarioIdentifiers", "All scenario identifier properties of the provided result type.")]
        public static List<string> ScenarioIdentifierProperties(this Type resultType)
        {
            if (resultType == null)
            {
                Engine.Base.Compute.RecordError("Cannot get scenario identifiers from a null type.");
                return new List<string>();
            }
            return resultType.PropertiesWithAttribute(typeof(ScenarioIdentifierAttribute), true, true).Select(x => x.Name).ToList();
        }

        /***************************************************/

        [Description("Gets the name of all properties of the result that are of identifier types. This is all properties tagged with any IdentifierAttribute.")]
        [Input("result", "The result to get properties from.")]
        [Output("allIdentifiers", "All identifier properties of the provided result.")]
        public static List<string> AllIdentifierProperties(this IResult result)
        {
            if (result == null)
            {
                Engine.Base.Compute.RecordError("Cannot get scenario identifiers from a null result.");
                return new List<string>();
            }

            return result.GetType().AllIdentifierProperties();
        }

        /***************************************************/

        [Description("Gets the name of all properties of the result that are of identifier types. This is all properties tagged with any IdentifierAttribute.")]
        [Input("resultType", "The result type to get properties from.")]
        [Output("allIdentifiers", "All identifier properties of the provided result type.")]
        public static List<string> AllIdentifierProperties(this Type resultType)
        {
            if (resultType == null)
            {
                Engine.Base.Compute.RecordError("Cannot get scenario identifiers from a null type.");
                return new List<string>();
            }
            return resultType.PropertiesWithAttribute(typeof(IdentifierAttribute), true, true).Select(x => x.Name).ToList();
        }

        /***************************************************/

    }
}

