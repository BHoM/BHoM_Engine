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

        [Description("Checks if the table contains a specific axis")]
        [Input("table", "The table to check")]
        [Input("axis", "The name of the axis to check for")]
        [Output("exists", "Returns true if the axis is in the table")]
        public static bool AxisExists(this Table table, string axis)
        {
            if(table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if an axis exists on a null table.");
                return false;
            }

            return table.Axes().Contains(axis);
        }

        /***************************************************/

        [Description("Checks if the table contains a specific axis")]
        [Input("table", "The table to check")]
        [Input("axes", "The name of the axes to check for")]
        [Output("exists", "Returns true if the axis is in the table")]
        public static bool AxisExists(this Table table, List<string> axes)
        {
            if(table == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query if axes exist on a null table.");
                return false;
            }

            bool success = true;

            List<string> tableAxes = table.Axes();

            foreach (string axis in axes)
            {
                bool exists = tableAxes.Contains(axis);
                success &= exists;

                if (!exists)
                    Base.Compute.RecordWarning("Table does not contain any axis with the name " + axis);
            }

            return success;
        }

        /***************************************************/
    }
}




