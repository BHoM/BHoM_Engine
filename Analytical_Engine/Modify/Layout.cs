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

using BH.oM.Analytical.Elements;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Constructors             ****/
        /***************************************************/

        [Description("Modifies a Graph by configuring ViewFragments for each entity.")]
        [Input("graph", "The Graph to modify.")]
        [Input("layout", "ILayout for the view of the of the Graph.")]
        [Output("graph", "Graph with ILayout configured.")]
        public static Graph ILayout(this Graph graph, ILayout layout)
        {
            Layout(layout as dynamic, graph);
            return graph;
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static void Layout(this ILayout layout, Graph graph)
        {
            // Do nothing
        }

        /***************************************************/
    }
}
