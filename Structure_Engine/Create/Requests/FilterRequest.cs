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

using BH.oM.Data.Requests;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Results;
using BH.oM.Geometry.CoordinateSystem;

namespace BH.Engine.Structure.Results
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods - ToBeRemoved               ****/
        /***************************************************/

        [ToBeRemoved("3.1", "ToBeRemoved as functionality has been replaced by MeshResultRequest.")]
        [Description("Specific filter request to retrieve structural mesh results.")]
        public static FilterRequest MeshResult(   MeshResultSmoothingType smoothing, 
                                                MeshResultLayer layer,
                                                double layerPosition,
                                                MeshResultType resultType, 
                                                Cartesian coordinateSystem = null,                                           
                                                IEnumerable<object> cases = null, 
                                                IEnumerable<object> objectIds = null)
        {
            FilterRequest request = new FilterRequest();
            request.Type = typeof(MeshResult);

            request.Equalities["Smoothing"] = smoothing;
            request.Equalities["Layer"] = layer;
            request.Equalities["LayerPosition"] = layerPosition;
            request.Equalities["ResultType"] = resultType;
            request.Equalities["CoordinateSystem"] = coordinateSystem;
            if (cases != null)
                request.Equalities["Cases"] = cases.ToList();
            if (objectIds != null)
                request.Equalities["ObjectIds"] = objectIds.ToList();

            return request;
        }

        /***************************************************/
    }
}




