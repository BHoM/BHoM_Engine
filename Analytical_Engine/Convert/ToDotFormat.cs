using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
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
        public static string ToDotFormat(this Graph graph, string shape = "box", int fontsize = 12)
        {
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
