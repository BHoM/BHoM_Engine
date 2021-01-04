/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Query
    {
        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [PreviousInputNames("element2D", "panel,opening")]
        [PreviousVersion("4.0", "BH.Engine.Structure.Query.Outline(BH.oM.Structure.Elements.Panel)")]
        [PreviousVersion("4.0", "BH.Engine.Structure.Query.Outline(BH.oM.Structure.Elements.Opening)")]
        [PreviousVersion("3.2", "BH.Engine.Common.Query.IOutlineCurve(BH.oM.Dimensional.IElement2D)")]
        [Description("Returns a single polycurve outline created from the external elements.")]
        [Input("element2D", "The IElement2D to get the outline curve from.")]
        [Output("curve", "A single outline curve for the IElement2D.")]
        public static PolyCurve OutlineCurve(this IElement2D element2D)
        {
            return new PolyCurve { Curves = element2D.IOutlineElements1D().Select(e => e.IGeometry()).ToList() };
        }

        /******************************************/

        [PreviousInputNames("elements1D", "edges")]
        [PreviousVersion("4.0", "BH.Engine.Structure.Query.Outline(System.Collections.Generic.List<BH.oM.Structure.Elements.Edge>)")]
        [PreviousVersion("3.2", "BH.Engine.Common.Query.IOutlineCurve(BH.oM.Dimensional.IElement1D)")]
        [Description("Returns a single polycurve outline created from the IElement1Ds.")]
        [Input("elements1D", "The IElement1Ds are expected to be provided in order in such a way that each elements end meets the start of the next.")]
        [Output("curve", "A single poly curve for the IElement1Ds where the next item in the list is set as the next curve in a single curve.")]
        public static PolyCurve OutlineCurve(this List<IElement1D> elements1D)
        {
            return new PolyCurve { Curves = elements1D.Select(e => e.IGeometry()).ToList() };
        }

        /******************************************/
    }
}


