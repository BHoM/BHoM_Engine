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
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Data.Collections;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the first table row matching the expression string sorted by a specified axis. Values returned as CustomObjects")]
        [Input("table", "The table to extract values from")]
        [Input("expression", "Expression string for extracting the values from the table.")]
        [Input("sortOrder", "The axis the values should be sorted by.")]
        [Output("Data", "The data matching the provided axes and values as CustomObjects.")]
        public static CustomObject FirstValueAt(this Table table, string expression, string sortOrder)
        {
            if (table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot get the first value at from a null table.");
                return null;
            }

            return AsCustomObject(table.Data.Select(expression, sortOrder).First(), table.Data.Columns);
        }
    }
}



