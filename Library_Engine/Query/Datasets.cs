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
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base;
using System.IO;
using System.Linq;
using BH.oM.Data.Collections;
using BH.Engine.Data;
using BH.oM.Data.Library;
using BH.oM.Base.Attributes;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the full Dataset(s) from the library, containing source information and Data")]
        [Input("libraryName", "The name of the Dataset(s) to extract")]
        [Output("dataset", "The datasets extracted from the library")]
        public static List<Dataset> Datasets(string libraryName)
        {
            HashSet<string> keys;
            if (!LibraryPaths().TryGetValue(libraryName, out keys))
            {
                BH.Engine.Base.Compute.RecordWarning(String.Format("No file or subfolder named {0} could be found in the Datasets folder.", libraryName));
                return new List<Dataset>();
            }

            return keys.Select(x => ParseLibrary(x)).ToList();

        }
    }
}






