using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.Engine.Serialiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Analytical
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Constructors             ****/
        /***************************************************/

        [Description("Convert a graph to CustomOBjects for visualisation")]
        [Input("graph", "The Graph to convert.")]
        [Output("custom objects", "CUstom objects representing the Graph.")]
        public static List<CustomObject> ToDataVizObjects(this Graph graph)
        {
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
