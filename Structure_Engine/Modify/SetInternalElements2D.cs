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
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static Opening SetInternalElements2D(this Opening opening, List<IElement2D> internalElements2D)
        {
            if (internalElements2D.Count != 0)
                Reflection.Compute.RecordError("Cannot set internal 2D elements to an opening.");

            return opening.GetShallowClone() as Opening;
        }

        /***************************************************/

        public static Panel SetInternalElements2D(this Panel Panel, List<IElement2D> internalElements2D)
        {
            Panel pp = Panel.GetShallowClone() as Panel;
            pp.Openings = new List<Opening>(internalElements2D.Cast<Opening>().ToList());
            return pp;
        }

        /***************************************************/
    }
}

