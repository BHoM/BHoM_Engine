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

        [Description("Set the external outline elements of an ISurface.")]
        [Input("surface", "The ISurface to set the external outline elements of.")]
        [Input("outlineElements1D", "New outline elements, will replace any existing ones. They must be closed and planar.")]
        [Output("surface", "The ISurface with new external outline elements.")]
        public static oM.Physical.Elements.ISurface SetOutlineElements1D(this oM.Physical.Elements.ISurface surface, List<IElement1D> outlineElements1D)
        {
            oM.Physical.Elements.ISurface clone = (oM.Physical.Elements.ISurface)surface.GetShallowClone();
            ICurve outline = outlineElements1D.Select(x => x.IGeometry()).Single();
            clone.Location = Engine.Geometry.Create.PlanarSurface(outline);
            return clone;
        }

        /***************************************************/

        [Description("Set the external outline elements of an IOpening.")]
        [Input("surface", "The IOpening to set the external outline elements of.")]
        [Input("outlineElements1D", "New outline elements, will replace any existing ones. They must be closed and planar.")]
        [Output("opening", "The IOpening with new external outline elements.")]
        public static IOpening SetOutlineElements1D(this IOpening surface, List<IElement1D> outlineElements1D)
        {
            IOpening clone = (IOpening)surface.GetShallowClone();
            ICurve outline = outlineElements1D.Select(x => x.IGeometry()).Single();
            clone.Location = Engine.Geometry.Create.PlanarSurface(outline);
            return clone;
        }

        /***************************************************/

    }
}

