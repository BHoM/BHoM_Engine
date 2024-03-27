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

using BH.oM.Analytical.Graph;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Convert a graph to DotFormat for visualisation.")]
        [Input("graph", "The Graph to convert.")]
        [Input("shape", "The optional DotFormat shape to represent Graph entities. Default is \"box\".")]
        [Input("fontsize", "The optional DotFormat fontsize for text in the DotFormat. Default is 12.")]
        [Output("dotFormat", "The DotFormat string that can be copied and pasted in on line viewers like https://visjs.github.io/vis-network/examples/network/data/dotLanguage/dotPlayground.html for quick visualisation.")]
        public static string ToDotFormat(this Graph graph, string shape = "box", int fontsize = 12)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null graph to a Dot Format.");
                return "";
            }

            string pattern = "[\\~#%&*{}()/:<>?|\"-]";
            string replacement = "_";

            Regex regEx = new Regex(pattern);
            StringBuilder sb = new StringBuilder();
            sb.Append("digraph {\n");
            sb.Append("node [shape = " + shape + " fontsize=" + fontsize + "]\n");
            foreach (IRelation link in graph.Relations)
            {
                if (link.Weight == 0) continue;
                string start = Regex.Replace(regEx.Replace(graph.Entities[link.Source].Name, replacement), @"\s+", "");
                string end = Regex.Replace(regEx.Replace(graph.Entities[link.Target].Name, replacement), @"\s+", "");
                sb.Append(string.Format("{0} -> {1}", start, end));
                sb.Append(string.Format(" [ label = \"w:{0}\" ];\n", Math.Round(link.Weight, 2)));
            }
            sb.Append("}");
            Console.WriteLine(sb.ToString());
            return sb.ToString();
        }
    }
}




