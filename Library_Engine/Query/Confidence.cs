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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Data.Library;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extract the level of confidence of the library. If the provided library name returns multiple levels of confidence, the lowest level is returned.")]
        [Input("libraryName", "Name of the library to extract confidence from.")]
        [Output("confidence", "The lowest level of confidence found in any opf the libraries matching the provided library name.")]
        public static Confidence Confidence(this string libraryName)
        {
            if (string.IsNullOrWhiteSpace(libraryName))
                return oM.Data.Library.Confidence.Undefined;

            List<Source> sources = Source(libraryName);

            if (sources == null || sources.Count == 0 || sources.Any(x => x == null))
                return oM.Data.Library.Confidence.Undefined;

            return sources.Select(x => x.Confidence).OrderBy(x => x).First();
        }

        /***************************************************/
    }
}




