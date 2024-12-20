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
using System.Reflection;
using BH.oM.Base.Attributes;
using BH.oM.Data.Library;
using BH.oM.Base;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extracts the source information, if available, from a library and returns a compiled version of the source information and a general disclaimer the dataset.")]
        [Input("libraryName", "The name of the Dataset(s) to extract source information from")]
        [Output("source", "Text representation of the source and general disclaimer of the data.")]
        public static string SourceAndDisclaimer(string libraryName)
        {
            List<Source> sources = Source(libraryName).Where(x => x != null).ToList();

            string desc;
            if (sources.Count == 1)
            {
                desc = "Dataset based on the following source:" + Environment.NewLine;
                desc += sources[0].ToText();
            }
            else if (sources.Count > 1)
            {
                desc = "Multiple sources found for the library. Datasets based on the following sources:" + Environment.NewLine;

                for (int i = 0; i < sources.Count; i++)
                {
                    desc += Environment.NewLine;
                    int sourceNumber = i + 1;
                    desc += "Source " + sourceNumber + ":" + Environment.NewLine;
                    desc += sources[i].ToText();
                }
            }
            else
            {
                desc = "No source information available for this dataset." + Environment.NewLine;
            }
            desc += Environment.NewLine;
            desc += GeneralDisclaimer();

            return desc;
        }

        /***************************************************/
    }
}





