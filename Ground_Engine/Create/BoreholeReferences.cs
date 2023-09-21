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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Ground;
using BH.Engine.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Ground
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a BoreholeReference object with properties that can be added to a Borehole object. ")]
        [Input("file", "Associated file reference including instructions and photographs (FILE_FSET).")]
        [Input("url", "Link to storage of borehole data.")]
        [Input("originalId", "Original hole id (LOCA_ORID).")]
        [Input("originalReference", "Original job reference (LOCA_ORJO).")]
        [Input("originalCompany", "Originating company (LOCA_ORCO).")]
        [Output("boreholeReference", "Associated file reference including instructions and photographs (FILE_FSET).")]
        public static BoreholeReference BoreholeReference(string file = null, string url = "", string originalId = "", string originalReference = "", string originalCompany = "")
        {
            return new BoreholeReference()
            {
                File = file,
                URL = url,
                OriginalId = originalId,
                OriginalReference = originalReference,
                OriginalCompany = originalCompany
            };
        }

        /***************************************************/
    }
}




