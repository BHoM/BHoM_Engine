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

        [Description("Returns the scorer method to be used in FuzzyMatching methods.")]
        [Input("scorer", "The scorer input type")]
        [Output("o", "The scorer method.")]
        private static IRatioScorer GetScorer(Scorer scorer)
        {
            switch (scorer)
            {
                case Scorer.DefaultRatioScorer:
                default:
                    return ScorerCache.Get<DefaultRatioScorer>();
                case Scorer.PartialRatioScorer:
                    return ScorerCache.Get<PartialRatioScorer>();
                case Scorer.TokenSetScorer:
                    return ScorerCache.Get<TokenSetScorer>();
                case Scorer.PartialTokenSetScorer:
                    return ScorerCache.Get<PartialTokenSetScorer>();
                case Scorer.TokenSortScorer:
                    return ScorerCache.Get<TokenSortScorer>();
                case Scorer.TokenAbbreviationScorer:
                    return ScorerCache.Get<TokenSortScorer>();
                case Scorer.PartialTokenAbbreviationScorer:
                    return ScorerCache.Get<PartialTokenAbbreviationScorer>();
                case Scorer.WeightedRatioScorer:
                    return ScorerCache.Get<WeightedRatioScorer>();
            }
        }

        /***************************************************/

    }
}


