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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Physical.Materials;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Common;

namespace BH.Engine.Physical
{
    public static partial class Modify
    {

        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Attempts to set the internal 2D elements of a IOpening, returns a error if any internal elements are provided as IOpening does not have internal elements.")]
        [Input("opening", "The IOpening to set the internal elements of.")]
        [Input("internalElements2D", "New internal elements, returns a error if any are provided as IOpening does not have internal elements")]
        [Output("opening", "The IOpening with new internal elements.")]
        public static IOpening SetInternalElements2D(this IOpening opening, List<IElement2D> internalElements2D)
        {
            if (internalElements2D.Count != 0)
                Reflection.Compute.RecordError("Cannot set internal 2D elements to an opening.");

            return opening.GetShallowClone() as IOpening;
        }

        /***************************************************/

        [Description("Set the internal 2D elements of a ISurface.")]
        [Input("surface", "The ISurface to set the internal elements of.")]
        [Input("internalElements2D", "New internal elements, will replace any existing ones.")]
        [Output("surface", "The ISurface with new internal elements.")]
        public static oM.Physical.Elements.ISurface SetInternalElements2D(this oM.Physical.Elements.ISurface surface, List<IElement2D> internalElements2D)
        {
            oM.Physical.Elements.ISurface pp = surface.GetShallowClone() as oM.Physical.Elements.ISurface;
            pp.Openings = new List<IOpening>(internalElements2D.Cast<IOpening>().ToList());
            return pp;
        }

        /***************************************************/

    }
}

