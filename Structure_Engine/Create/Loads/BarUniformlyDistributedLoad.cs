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
using BH.oM.Structure.Loads;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BarUniformlyDistributedLoad BarUniformlyDistributedLoad(Loadcase loadcase, BHoMGroup<Bar> group, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            if (force == null && moment == null)
                throw new ArgumentException("Bar uniform load requires either the force or the moment vector to be defined");

            return new BarUniformlyDistributedLoad
            {
                Loadcase = loadcase,
                Objects = group,
                Force = force == null ? new Vector() : force,
                Moment = moment == null ? new Vector() : moment,
                Axis = axis,
                Name = name,
                Projected = projected

            };
        }

        /***************************************************/

        public static BarUniformlyDistributedLoad BarUniformlyDistributedLoad(Loadcase loadcase, IEnumerable<Bar> objects, Vector force = null, Vector moment = null, LoadAxis axis = LoadAxis.Global, bool projected = false, string name = "")
        {
            return BarUniformlyDistributedLoad(loadcase, new BHoMGroup<Bar>() { Elements = objects.ToList() }, force, moment, axis, projected, name);
        }

        /***************************************************/

    }
}

