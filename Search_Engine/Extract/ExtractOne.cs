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
using BH.oM.Search;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Search
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extracts the value with the highest score when comparing the query to the choices.")]
        [Input("query", "The string to carry out the fuzzy matching on.")]
        [Input("choices", "A list of strings to compare the query against.")]
        [Input("scorer", "The method to use to score the strings when compared.")]
        [Output("result", "A FuzzyStringResult containing the string, score and index resulting from the fuzzy matching algorithm.")]
        public static FuzzyResult<string> ExtractOne(string query, IEnumerable<string> choices, Scorer scorer = Scorer.DefaultRatioScorer)
        {
            return ExtractTop(query, choices, scorer).First();
        }

        /***************************************************/

        [Description("Extracts the BHoMObject with the highest score when comparing the query to the choices.")]
        [Input("query", "The string to carry out the fuzzy matching on.")]
        [Input("objects", "A list of BHoMObjects to compare the query against.")]
        [Input("propertyName", "The propertyName to compare the query against - the property must be a string.")]
        [Input("scorer", "The method to use to score the strings when compared.")]
        [Output("result", "A FuzzyObjectResult containing the object, score and index resulting from the fuzzy matching algorithm.")]
        public static FuzzyResult<BHoMObject> ExtractOne(string query, List<BHoMObject> objects, string propertyName, Scorer scorer = Scorer.DefaultRatioScorer)
        {
            return ExtractTop(query, objects, propertyName, scorer).First();
        }

        /***************************************************/

    }
}


