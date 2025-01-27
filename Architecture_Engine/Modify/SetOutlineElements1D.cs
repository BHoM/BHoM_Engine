/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Architecture.Elements;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;

namespace BH.Engine.Architecture
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Assign a new collection of external 1D boundaries to an Architecture Room")]
        [Input("room", "An Architecture Room to update")]
        [Input("outlineElements1D", "A collection of outline 1D elements to assign to the Room")]
        [Output("room", "The updated Architecture Room")]
        public static Room SetOutlineElements1D(this Room room, List<IElement1D> outlineElements1D)
        {
            if(room == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the outline elements of a null room.");
                return room;
            }

            Room r = room.ShallowClone() as Room;
            r.Perimeter = BH.Engine.Geometry.Compute.IJoin(outlineElements1D.Cast<ICurve>().ToList())[0];
            return r;
        }

        /***************************************************/

        [Description("Assign a new collection of external 1D boundaries to an Architecture Ceiling")]
        [Input("ceiling", "An Architecture ceiling to update")]
        [Input("outlineElements1D", "A collection of outline 1D elements to assign to the Ceiling")]
        [Output("ceiling", "The updated Architecture Ceiling")]
        public static Ceiling SetOutlineElements1D(this Ceiling ceiling, List<IElement1D> outlineElements1D)
        {
            if(ceiling == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the outline elements of a null ceiling.");
                return ceiling;
            }

            ceiling.Surface = BH.Engine.Geometry.Create.PlanarSurface(BH.Engine.Geometry.Compute.IJoin(outlineElements1D.Cast<ICurve>().ToList())[0]);
            return ceiling;
        }
    }
}






