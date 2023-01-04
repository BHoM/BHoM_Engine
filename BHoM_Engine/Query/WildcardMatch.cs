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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        [Description("Checks a wildcardPattern (e.g. *.someExtension) matches an input text.")]
        [Input("text", "Text to match against the wildcard pattern.")]
        [Input("wildcardPattern", "WildcardPattern (e.g. *.someExtension) to be used to see if a match is found against the input text.")]
        public static bool WildcardMatch(this string text, string wildcardPattern)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(wildcardPattern))
                return false;

            try
            {
                bool isLike = true;
                byte matchCase = 0;
                char[] filter;
                char[] reversedFilter;
                char[] reversedWord;
                char[] word;
                int currentPatternStartIndex = 0;
                int lastCheckedHeadIndex = 0;
                int lastCheckedTailIndex = 0;
                int reversedWordIndex = 0;
                List<char[]> reversedPatterns = new List<char[]>();

                if (text == null || wildcardPattern == null)
                    return false;

                word = text.ToCharArray();
                filter = wildcardPattern.ToCharArray();

                //Set which case will be used (0 = no wildcards, 1 = only ?, 2 = only *, 3 = both ? and *
                for (int i = 0; i < filter.Length; i++)
                {
                    if (filter[i] == '?')
                    {
                        matchCase += 1;
                        break;
                    }
                }

                for (int i = 0; i < filter.Length; i++)
                {
                    if (filter[i] == '*')
                    {
                        matchCase += 2;
                        break;
                    }
                }

                if ((matchCase == 0 || matchCase == 1) && word.Length != filter.Length)
                    return false;

                switch (matchCase)
                {
                    case 0:
                        isLike = (text == wildcardPattern);
                        break;

                    case 1:
                        for (int i = 0; i < text.Length; i++)
                        {
                            if ((word[i] != filter[i]) && filter[i] != '?')
                            {
                                isLike = false;
                            }
                        }
                        break;

                    case 2:
                        //Search for matches until first *
                        for (int i = 0; i < filter.Length; i++)
                        {
                            if (filter[i] != '*')
                            {
                                if (filter[i] != word[i])
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                lastCheckedHeadIndex = i;
                                break;
                            }
                        }
                        //Search Tail for matches until first *
                        for (int i = 0; i < filter.Length; i++)
                        {
                            if (filter[filter.Length - 1 - i] != '*')
                            {
                                if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i])
                                    return false;

                            }
                            else
                            {
                                lastCheckedTailIndex = i;
                                break;
                            }
                        }


                        //Create a reverse word and filter for searching in reverse. The reversed word and filter do not include already checked chars
                        reversedWord = new char[word.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
                        reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];

                        for (int i = 0; i < reversedWord.Length; i++)
                            reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];

                        for (int i = 0; i < reversedFilter.Length; i++)
                            reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];

                        //Cut up the filter into seperate patterns, exclude * as they are not longer needed
                        for (int i = 0; i < reversedFilter.Length; i++)
                        {
                            if (reversedFilter[i] == '*')
                            {
                                if (i - currentPatternStartIndex > 0)
                                {
                                    char[] pattern = new char[i - currentPatternStartIndex];
                                    for (int j = 0; j < pattern.Length; j++)
                                        pattern[j] = reversedFilter[currentPatternStartIndex + j];

                                    reversedPatterns.Add(pattern);
                                }
                                currentPatternStartIndex = i + 1;
                            }
                        }

                        //Search for the patterns
                        for (int i = 0; i < reversedPatterns.Count; i++)
                        {
                            for (int j = 0; j < reversedPatterns[i].Length; j++)
                            {

                                if ((reversedPatterns[i].Length - 1 - j) > (reversedWord.Length - 1 - reversedWordIndex))
                                    return false;

                                if (reversedPatterns[i][j] != reversedWord[reversedWordIndex + j])
                                {
                                    reversedWordIndex += 1;
                                    j = -1;
                                }
                                else
                                {
                                    if (j == reversedPatterns[i].Length - 1)
                                        reversedWordIndex = reversedWordIndex + reversedPatterns[i].Length;
                                }
                            }
                        }
                        break;

                    case 3:
                        //Same as Case 2 except ? is considered a match
                        //Search Head for matches util first *
                        for (int i = 0; i < filter.Length; i++)
                        {
                            if (filter[i] != '*')
                            {
                                if (filter[i] != word[i] && filter[i] != '?')
                                    return false;
                            }
                            else
                            {
                                lastCheckedHeadIndex = i;
                                break;
                            }
                        }
                        //Search Tail for matches until first *
                        for (int i = 0; i < filter.Length; i++)
                        {
                            if (filter[filter.Length - 1 - i] != '*')
                            {
                                if (filter[filter.Length - 1 - i] != word[word.Length - 1 - i] && filter[filter.Length - 1 - i] != '?')
                                    return false;
                            }
                            else
                            {
                                lastCheckedTailIndex = i;
                                break;
                            }
                        }
                        // Reverse and trim word and filter
                        reversedWord = new char[word.Length - lastCheckedHeadIndex - lastCheckedTailIndex];
                        reversedFilter = new char[filter.Length - lastCheckedHeadIndex - lastCheckedTailIndex];

                        for (int i = 0; i < reversedWord.Length; i++)
                            reversedWord[i] = word[word.Length - (i + 1) - lastCheckedTailIndex];

                        for (int i = 0; i < reversedFilter.Length; i++)
                            reversedFilter[i] = filter[filter.Length - (i + 1) - lastCheckedTailIndex];

                        for (int i = 0; i < reversedFilter.Length; i++)
                        {
                            if (reversedFilter[i] == '*')
                            {
                                if (i - currentPatternStartIndex > 0)
                                {
                                    char[] pattern = new char[i - currentPatternStartIndex];
                                    for (int j = 0; j < pattern.Length; j++)
                                        pattern[j] = reversedFilter[currentPatternStartIndex + j];
                                    reversedPatterns.Add(pattern);
                                }

                                currentPatternStartIndex = i + 1;
                            }
                        }
                        //Search for the patterns
                        for (int i = 0; i < reversedPatterns.Count; i++)
                        {
                            for (int j = 0; j < reversedPatterns[i].Length; j++)
                            {
                                if ((reversedPatterns[i].Length - 1 - j) > (reversedWord.Length - 1 - reversedWordIndex))
                                    return false;

                                if (reversedPatterns[i][j] != '?' && reversedPatterns[i][j] != reversedWord[reversedWordIndex + j])
                                {
                                    reversedWordIndex += 1;
                                    j = -1;
                                }
                                else
                                {
                                    if (j == reversedPatterns[i].Length - 1)
                                        reversedWordIndex = reversedWordIndex + reversedPatterns[i].Length;
                                }
                            }
                        }
                        break;
                }

                return isLike;
            }
            catch
            {
                return false;
            }
        }

        /***************************************************/

        [Description("Checks a wildcardPattern (e.g. *.someExtension) matches an input text.")]
        [Input("text", "Text to match against the wildcard pattern.")]
        [Input("wildcardPattern", "WildcardPattern (e.g. *.someExtension) to be used to see if a match is found against the input text.")]
        [Input("ignoreCase", "If we want to ignore the text case (lowercase/uppercase).")]
        public static bool WildcardMatch(this string text, string wildcardPattern, bool ignoreCase)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(wildcardPattern))
                return false;

            if (ignoreCase == true)
                return text.ToLower().WildcardMatch(wildcardPattern.ToLower());
            else
                return text.WildcardMatch(wildcardPattern);
        }
    }
}

