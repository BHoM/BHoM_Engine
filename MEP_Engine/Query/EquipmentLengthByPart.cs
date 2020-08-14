/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.MEP;
using BH.oM.MEP.Equipment;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.Geometry.SettingOut;

using BH.Engine.Base;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Returns the length of the equipment based on the input of a list of parts")]
        [Input("airsidePartsByLength", "Collection of airside parts from a dataset that contains parts by length")]
        [Output("length", "The total length of the equipment based on the length of the parts")]
        public static double EquipmentLengthByPart (this List<CustomObject> airsidePartsByLength)
        {
            double length = 0;
            foreach (CustomObject o in airsidePartsByLength)
            {
                if (o.CustomData.ContainsKey("length"))
                    length += System.Convert.ToDouble(o.CustomData["length"]);
            }
            return length;
        }
        /***************************************************/
    }
}
