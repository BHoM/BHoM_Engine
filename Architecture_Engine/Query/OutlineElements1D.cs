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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Architecture.Elements;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Architecture
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the outline 1D elements of an Architecture Room")]
        [Input("room", "An Architecture Room")]
        [Output("outlineElements", "A collection of outline 1D elements")]
        public static List<IElement1D> OutlineElements1D(this Room room)
        {
            if(room == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the outline 1D elements of a null room.");
                return new List<IElement1D>();
            }

            return room.Perimeter.ISubParts().Cast<IElement1D>().ToList();
        }

        /***************************************************/

        [Description("Returns the outline 1D elements of an Architecture Ceiling")]
        [Input("ceiling", "An Architecture Ceiling")]
        [Output("outlineElements", "A collection of outline 1D elements")]
        public static List<IElement1D> OutlineElements1D(this Ceiling ceiling)
        {
            if(ceiling == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the outline 1D elements of a null ceiling.");
                return new List<IElement1D>();
            }

            return ceiling.Surface.ISubParts().Cast<IElement1D>().ToList();
        }
    }
}



