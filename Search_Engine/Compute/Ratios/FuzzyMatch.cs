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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FuzzySharp;
using FuzzySharp.PreProcess;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio.Scorer.Composite;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace BH.Engine.Search
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Carries out a fuzzy match of the two strings provided using the scorer specified.")]
        [Input("text", "The string to carry out the fuzzy matching on.")]
        [Input("compare", "The string to compare against.")]
        [Input("scorer", "The method to use to score the strings when compared.")]
        [Output("r", "The ratio of similarity between the two strings.")]
        public static int MatchScore(string text, string compare, Scorer scorer = Scorer.DefaultRatio)
        {
            switch (scorer)
            {
                case Scorer.DefaultRatio:
                default:
                    return Fuzz.Ratio(text, compare);
                case Scorer.PartialRatio:
                    return Fuzz.PartialRatio(text, compare);
                case Scorer.TokenSet:
                    return Fuzz.TokenSetRatio(text, compare);
                case Scorer.PartialTokenSet:
                    return Fuzz.PartialTokenSetRatio(text, compare);
                case Scorer.TokenSort:
                    return Fuzz.TokenSortRatio(text, compare);
                case Scorer.PartialTokenSort:
                    return Fuzz.PartialTokenSortRatio(text, compare);
                case Scorer.TokenAbbreviation:
                    return Fuzz.TokenAbbreviationRatio(text, compare, PreprocessMode.Full);
                case Scorer.PartialTokenAbbreviation:
                    return Fuzz.PartialTokenAbbreviationRatio(text, compare, PreprocessMode.Full);
                case Scorer.TokenInitialism:
                    return Fuzz.TokenInitialismRatio(text, compare);
                case Scorer.PartialTokenInitialism:
                    return Fuzz.PartialTokenInitialismRatio(text, compare);
                case Scorer.WeightedRatio:
                    return Fuzz.WeightedRatio(text, compare);
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Returns the scorer method to be used in FuzzyMatching methods.")]
        [Input("scorer", "The scorer input type")]
        [Output("o", "The scorer method.")]
        private static IRatioScorer GetScorer(Scorer scorer)
        {
            switch (scorer)
            {
                case Scorer.DefaultRatio:
                default:
                    return ScorerCache.Get<DefaultRatioScorer>();
                case Scorer.PartialRatio:
                    return ScorerCache.Get<PartialRatioScorer>();
                case Scorer.TokenSet:
                    return ScorerCache.Get<TokenSetScorer>();
                case Scorer.PartialTokenSet:
                    return ScorerCache.Get<PartialTokenSetScorer>();
                case Scorer.TokenSort:
                    return ScorerCache.Get<TokenSortScorer>();
                case Scorer.TokenAbbreviation:
                    return ScorerCache.Get<TokenSortScorer>();
                case Scorer.PartialTokenAbbreviation:
                    return ScorerCache.Get<PartialTokenAbbreviationScorer>();
                case Scorer.WeightedRatio:
                    return ScorerCache.Get<WeightedRatioScorer>();
            }
        }

    }
}


