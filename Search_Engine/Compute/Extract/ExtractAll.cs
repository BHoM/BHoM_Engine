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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Search;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FuzzySharp;
using FuzzySharp.Extractor;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace BH.Engine.Search
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extracts all values unsorted with a score greater than the cutoff when comparing the query to the choices.")]
        [Input("query", "The string to carry out the fuzzy matching on.")]
        [Input("choices", "A list of strings to compare the query against.")]
        [Input("scorer", "The method to use to score the strings when compared.")]
        [Input("cutOff", "The cutoff score (i.e. lower bound) for results to be returned.")]
        [Output("result", "A SearchResult containing the strings, scores and indexes resulting from the fuzzy matching algorithm.")]
        public static List<SearchResult<string>> ExtractAll(string query, IEnumerable<string> choices, Scorer scorer = Scorer.DefaultRatio, int cutOff = 0)
        {
            IRatioScorer scorerMethod = ScorerCache.Get<DefaultRatioScorer>();
            if (scorer != Scorer.DefaultRatio)
                scorerMethod = GetScorer(scorer);

            return Process.ExtractAll(query, choices.ToArray(), s => s, scorerMethod, cutOff)
                               .Select(x => new SearchResult<string>(x.Value, x.Score, x.Index)).ToList();
        }

        /***************************************************/

        [Description("Extracts all BHoMObjects unsorted with a score greater than the cutoff when comparing the query to the choices.")]
        [Input("query", "The string to carry out the fuzzy matching on.")]
        [Input("objects", "A list of BHoMObjects to compare the query against.")]
        [Input("propertyName", "The propertyName to compare the query against - the property must be a string and an exact match.")]
        [Input("scorer", "The method to use to score the strings when compared.")]
        [Input("cutOff", "The cutoff score (i.e. lower bound) for results to be returned.")]
        [Output("result", "A SearchResult containing the objects, scores and indexes resulting from the fuzzy matching algorithm.")]
        public static List<SearchResult<BHoMObject>> ExtractAll(string query, List<BHoMObject> objects, string propertyName, Scorer scorer = Scorer.DefaultRatio, int cutOff = 0)
        {
            IRatioScorer scorerMethod = ScorerCache.Get<DefaultRatioScorer>();
            if (scorer != Scorer.DefaultRatio)
                scorerMethod = GetScorer(scorer);

            IEnumerable<string> choices = objects.Select(x => x.PropertyValue(propertyName).ToString());

            return Process.ExtractAll(query, choices, s => s, scorerMethod, cutOff)
                        .Select(x => new SearchResult<BHoMObject>(objects[x.Index], x.Score, x.Index)).ToList();
        }

        /***************************************************/

    }
}

