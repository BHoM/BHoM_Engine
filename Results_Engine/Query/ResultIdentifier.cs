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
using BH.oM.Base.Attributes;
using BH.oM.Analytical.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets a function taking an IResult and returning a string used to identify the result.")]
        [Input("result", "The object to get an identifier from.")]
        [Input("identifier", "Property of the result used to identify the result.")]
        [Output("func", "The function used to identify the result.")]
        public static Func<T, string> ResultIdentifier<T>(this T result, string identifier) where T : IResult
        {
            //If result type and identifier match for ObjectIdResult or MeshResult, create custom lamdas to speed things up
            string idLower = identifier.ToLower();
            if (idLower == nameof(IObjectIdResult.ObjectId).ToLower() && result is IObjectIdResult)
                return x => ((IObjectIdResult)x).ObjectId.ToString();

            if (idLower == nameof(IMeshElementResult.NodeId).ToLower() && result is IMeshElementResult)
                return x => ((IMeshElementResult)x).NodeId.ToString();

            if (idLower == nameof(IMeshElementResult.MeshFaceId).ToLower() && result is IMeshElementResult)
                return x => ((IMeshElementResult)x).MeshFaceId.ToString();

            if (idLower == nameof(ICasedResult.ResultCase).ToLower() && result is ICasedResult)
                return x => ((ICasedResult)x).ResultCase.ToString();

            if (idLower == nameof(ITimeStepResult.TimeStep).ToLower() && result is ITimeStepResult)
                return x => ((ITimeStepResult)x).TimeStep.ToString();

            //Fall back on PropertyValue
            return x => Base.Query.PropertyValue(x, identifier).ToString();
        }

        /***************************************************/
    }
}


