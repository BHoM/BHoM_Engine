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

using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether if the Dataset is a prototype Dataset.")]
        public static bool IsPrototype(this string libraryName)
        {
            if (m_CoreDatasetNames == null)
                ExtractCoreDatasetNames();

            return m_CoreDatasetNames.Count > 0 && !m_CoreDatasetNames.Contains(libraryName);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ExtractCoreDatasetNames()
        {
            string refFile = @"C:\ProgramData\BHoM\Settings\IncludedDatasets.txt";

            if (File.Exists(refFile))
                m_CoreDatasetNames = new HashSet<string>(File.ReadAllLines(refFile));
            else
                m_CoreDatasetNames = new HashSet<string>();
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static HashSet<string> m_CoreDatasetNames = null;

        /***************************************************/
    }
}



