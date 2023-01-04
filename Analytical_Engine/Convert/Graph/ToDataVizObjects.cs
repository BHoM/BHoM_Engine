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

using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.Engine.Serialiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Analytical
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Constructors             ****/
        /***************************************************/

        [Description("Convert a graph to CustomOBjects for visualisation.")]
        [Input("graph", "The Graph to convert.")]
        [Output("custom objects", "CUstom objects representing the Graph.")]
        public static List<CustomObject> ToDataVizObjects(this Graph graph)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot convert a null graph to DataViz objects.");
                return new List<CustomObject>();
            }

            List<CustomObject> objects = new List<CustomObject>();
            foreach (IBHoMObject entity in graph.Entities.Values.ToList())
            {
                
                string needed = "{ \"id\" :\"" + entity.Name + "\",";
                needed += "\"graphElement\" : \"node\"}";
                objects.Add((CustomObject)Serialiser.Convert.FromJson(needed));
            }
            foreach (IRelation link in graph.Relations)
            {
                
                string needed = "{ \"source\" :\"" + graph.Entities[link.Source].Name + "\",";
                needed += "\"target\" :\"" + graph.Entities[link.Target].Name + "\",";
                needed += "\"weight\" :\"" + link.Weight + "\",";
                needed += "\"graphElement\" : \"link\" }";
                objects.Add((CustomObject)Serialiser.Convert.FromJson(needed));
            }
            return objects;
        }

        /***************************************************/
    }
}



