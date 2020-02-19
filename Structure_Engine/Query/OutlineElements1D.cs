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
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Gets the elements from an Opening defining the boundary of the element. Method required for all IElement2Ds. \n" +
             "For an opening this will return a list of its edges.")]
        [Input("opening", "The Opening to get outline elements from.")]
        [Output("elements", "Outline elements of the Opening, i.e. the Edges of the Opening.")]
        public static List<IElement1D> OutlineElements1D(this Opening opening)
        {
            return new List<IElement1D>(opening.Edges);
        }

        /***************************************************/

        [Description("Gets the elements from a Panel defining the boundary of the element. Method required for all IElement2Ds. \n" +
                     "For a Panel this will return a list of its external Edges.")]
        [Input("panel", "The Panel to get outline elements from.")]
        [Output("elements", "Outline elements of the Panel, i.e. the external Edges of the Panel.")]
        public static List<IElement1D> OutlineElements1D(this Panel panel)
        {
            return new List<IElement1D>(panel.ExternalEdges);
        }

        /***************************************************/
    }
}

