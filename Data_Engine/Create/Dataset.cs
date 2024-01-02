/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

namespace BH.Engine.Data
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Dataset. A Dataset contains a list of BHoMObjects as well as metadata such as source information and time of creation. \n The datasets are used together with the serialised datasets accessed with the Library_Engine")]
        [Input("data", "The list of BHoMObjects to store in the Dataset")]
        [Input("source", "Citation for the source of the data")]
        [Input("name", "Name of the dataset")]
        [Input("timeOfCreation", "The time the Dataset was generated. If no time is provided, the current UTC time will be used.")]
        [Output("Dataset", "The created Dataset")]
        public static Dataset Dataset(List<IBHoMObject> data, Source source, string name, DateTime? timeOfCreation = null)
        {
            DateTime time = timeOfCreation == null ? DateTime.UtcNow : (DateTime)timeOfCreation;

            return new Dataset
            {
                SourceInformation = source,
                Data = data,
                Name = name,
                TimeOfCreation = time
            };

        }

        /***************************************************/
    }
}





