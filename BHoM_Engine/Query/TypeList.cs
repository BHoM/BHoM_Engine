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
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        [Description("Returns all BHoM interface types loaded in the current domain.")]
        [Output("types", "List of BHoM interface types loaded in the current domain.")]
        public static List<Type> BHoMInterfaceTypeList()
        {
            return Global.InterfaceList.ToList();
        }

        /***************************************************/

        [Description("Returns all BHoM types loaded in the current domain.")]
        [Output("types", "List of BHoM types loaded in the current domain.")]
        public static List<Type> BHoMTypeList()
        {
            return Global.BHoMTypeList.ToList();
        }

        /***************************************************/

        [Description("Returns all BHoM adapter types loaded in the current domain.")]
        [Output("types", "List of BHoM adapter types loaded in the current domain.")]
        public static List<Type> AdapterTypeList()
        {
            return Global.AdapterTypeList.ToList();
        }

        /***************************************************/

        [Description("Returns all types loaded in the current domain.")]
        [Output("types", "List of all types loaded in the current domain.")]
        public static List<Type> AllTypeList()
        {
            return Global.AllTypeList.ToList();
        }

        /***************************************************/

        [Description("Returns all BHoM engine types loaded in the current domain.")]
        [Output("types", "List of BHoM engine types loaded in the current domain.")]
        public static List<Type> EngineTypeList()
        {
            return Global.EngineTypeList.ToList();
        }

        /***************************************************/
    }
}




