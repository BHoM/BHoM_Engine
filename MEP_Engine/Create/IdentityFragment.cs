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

using System.Collections.Generic;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.MEP.Fragments;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Returns an MEP Identity Fragment which can be applied to an MEP object to provide information about an objects origins.")]
        [Input("manufacturer", "The manufacturer of the equipment, default empty string.")]
        [Input("modelNumber", "The model number of the equipment, default empty string.")]
        [Input("location", "The location of the equipment within the model/building, default empty string.")]
        [Input("service", "The service of the equipment, default empty string.")]
        [Input("remarks", "A collection of remarks about the identity of the equipment, default null.")]
        [Output("identityFragment", "An MEP Identity Fragment.")]
        public static IdentityFragment IdentityFragment(string manufacturer = "", string modelNumber = "", string location = "", string service = "", List<string> remarks = null)
        {
            remarks = remarks ?? new List<string>();

            return new IdentityFragment
            {
                Manufacturer = manufacturer,
                ModelNumber = modelNumber,
                Location = location,
                Service = service,
                Remarks = remarks,
            };
        }
        /***************************************************/
    }
}

