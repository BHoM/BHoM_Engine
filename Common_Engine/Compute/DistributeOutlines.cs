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

using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Common
{
    public static partial class Compute
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [Deprecated("3.1", "Migrated to the Spatial_Engine")]
        public static List<List<List<IElement1D>>> DistributeOutlines(this List<List<IElement1D>> outlines, bool canCutOpenings = true, double tolerance = Tolerance.Distance)
        {
            return Spatial.Compute.DistributeOutlines(outlines, canCutOpenings, tolerance);
        }

        /***************************************************/

        [DeprecatedAttribute("2.3", "Replaced with the same method taking an extra canCutOpenings argument.", null, "DistributeOutlines")]
        public static List<List<List<IElement1D>>> DistributeOutlines(this List<List<IElement1D>> outlines, double tolerance = Tolerance.Distance)
        {
            return outlines.DistributeOutlines(true, tolerance);
        }
        
        /***************************************************/
    }
}

