using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Convert
    {
        public static Graph ToProcessRelation(this Graph graph)
        {
            Graph clone = graph.DeepClone();

            foreach(IRelation relation in graph.Relations)
                clone.Relations.Add(clone.ToProcessRelation(relation));

            return clone;
        }

        public static ProcessRelation ToProcessRelation(this Graph graph, IRelation relation)
        {
            ProcessViewFragment source = graph.Entities[relation.Source].FindFragment<ProcessViewFragment>();
            ProcessViewFragment target = graph.Entities[relation.Target].FindFragment<ProcessViewFragment>();

            if (source == null)
                Reflection.Compute.RecordWarning("ProcessViewFragment could not be found on entity " + graph.Entities[relation.Source].Name);

            if (target == null)
                Reflection.Compute.RecordWarning("ProcessViewFragment could not be found on entity " + graph.Entities[relation.Target].Name);

            ProcessRelation processRelation = new ProcessRelation();

            if (source != null || target != null)
            {
                processRelation.Source = relation.Source;
                processRelation.Target = relation.Target;
                processRelation.Curve = new Line() { Start = source.Position, End = target.Position };
            }
            return processRelation;
        }
    }
}
