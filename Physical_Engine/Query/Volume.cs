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

using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Bulk Material            ****/
        /***************************************************/

        [Description("Return the total volume of BulkMaterial")]
        [Input("bulkMaterial", "Solid geometric elements that have a material composition")]
        [Output("volume","The combined volume of BulkMaterial", typeof(Volume))]
        public static double Volume(this BulkMaterial bulkMaterial)
        {
            return bulkMaterial.Geometry.Select(x => BH.Engine.Geometry.Query.IVolume((ISolid)x)).Sum();
        }

        /***************************************************/
    }
}