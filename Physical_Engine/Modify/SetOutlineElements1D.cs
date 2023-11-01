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

using System.Collections.Generic;
using System.Linq;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Dimensional;
using BH.Engine.Spatial;
using BH.Engine.Base;

namespace BH.Engine.Physical
{
    public static partial class Modify
    {

        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Set the location surface of an ISurface as a PlanarSurface by providing a list of IElements1D.")]
        [Input("surface", "The ISurface to set the location surface of.")]
        [Input("outlineElements1D", "One dimensional elements defining the external boundery geometry, ISurface has no edge properties and all IElements1D will be treated as their geometry. They must be closed and planar.")]
        [Output("surface", "The ISurface with new location surface.")]
        public static oM.Physical.Elements.ISurface SetOutlineElements1D(this oM.Physical.Elements.ISurface surface, List<IElement1D> outlineElements1D)
        {
            if(surface == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the outline 1D elements of a null surface.");
                return null;
            }

            oM.Physical.Elements.ISurface clone = surface.ShallowClone();
            ICurve outline = Engine.Geometry.Compute.IJoin(outlineElements1D.Select(x => x.IGeometry()).ToList()).Single();
            clone.Location = Engine.Geometry.Create.PlanarSurface(outline);
            return clone;
        }

        /***************************************************/

        [Description("Set the location surface of an IOpening as a PlanarSurface by providing a list of IElements1D.")]
        [Input("opening", "The IOpening to set the location surface of.")]
        [Input("outlineElements1D", "One dimensional elements defining the external boundery geometry, IOpening has no edge properties and all IElements1D will be treated as their geometry. They must be closed and planar.")]
        [Output("opening", "The IOpening with new location surface.")]
        public static IOpening SetOutlineElements1D(this IOpening opening, List<IElement1D> outlineElements1D)
        {
            if(opening == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot set the outline 1D elements of a null opening.");
                return null;
            }

            IOpening clone = opening.ShallowClone();
            ICurve outline = Engine.Geometry.Compute.IJoin(outlineElements1D.Select(x => x.IGeometry()).ToList()).Single();
            clone.Location = Engine.Geometry.Create.PlanarSurface(outline);
            return clone;
        }

        /***************************************************/

        [Description("Set the location surface of a PadFoundation as a PlanarSurface by providing a list of IElements1D.")]
        [Input("padFoundation", "The PadFoundation to set the location surface of.")]
        [Input("outlineElements1D", "One dimensional elements defining the external boundery geometry, PadFoundation has no edge properties and all IElements1D will be treated as their geometry. They must be closed and planar.")]
        [Output("padFoundation", "The PadFoundation with new location surface.")]
        public static PadFoundation SetOutlineElements1D(this PadFoundation padFoundation, List<IElement1D> outlineElements1D)
        {
            if (padFoundation.IsNull())
                return null;

            PadFoundation clone = padFoundation.ShallowClone();
            ICurve outline = Geometry.Compute.IJoin(outlineElements1D.Select(x => x.IGeometry()).ToList()).Single();
            clone.Location = Geometry.Create.PlanarSurface(outline);
            return clone;
        }

        /***************************************************/
    }
}




