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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Sets the Outline Element1Ds of an opening, i.e. the Edges of an Opening. Method required for all IElement2Ds.")]
        [Input("opening", "The Opening to update the Edges of.")]
        [Input("edges", "A list of IElement1Ds which all should be of type structural Edge.")]
        [Output("opening", "The opening with updated Edges.")]
        public static Opening SetOutlineElements1D(this Opening opening, List<IElement1D> edges)
        {
            Opening o = opening.GetShallowClone(true) as Opening;
            o.Edges = edges.Select(x => x is ICurve ? new Edge() { Curve = (x as ICurve) } : x as Edge).ToList();
            return o;
        }

        /***************************************************/

        [Description("Sets the outline Element1Ds of a Panel, i.e. the ExternalEdges of a Panel. Method required for all IElement2Ds.")]
        [Input("panel", "The Panel to update the ExternalEdges of.")]
        [Input("edges", "A list of IElement1Ds which all should be of type structural Edge.")]
        [Output("panel", "The Panel with updated ExternalEdges.")]
        public static Panel SetOutlineElements1D(this Panel panel, List<IElement1D> edges)
        {
            Panel pp = panel.GetShallowClone(true) as Panel;
            pp.ExternalEdges = edges.Select(x => x is ICurve ? new Edge() { Curve = (x as ICurve) } : x as Edge).ToList();
            return pp;
        }

        /***************************************************/
    }
}

