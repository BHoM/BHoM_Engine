using BH.oM.Geometry;
using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node SetGeometry(this Node node, Point point)
        {
            Node clone = node.GetShallowClone() as Node;
            clone.Position = point;
            return clone;
        }

        /***************************************************/

        public static Bar SetGeometry(this Bar bar, ICurve curve)
        {
            Line line = curve as Line;
            if (line == null)
                return null;

            Bar clone = bar.GetShallowClone() as Bar;
            clone.StartNode = clone.StartNode.SetGeometry(line.Start);
            clone.EndNode = clone.EndNode.SetGeometry(line.End);
            return clone;
        }

        /***************************************************/

        public static Edge SetGeometry(this Edge edge, ICurve curve)
        {
            Edge clone = edge.GetShallowClone() as Edge;
            clone.Curve = curve;
            return clone;
        }

        /***************************************************/

        //public static Opening SetGeometry(this Opening opening, PolyCurve newOutline)
        //{
        //    Opening clone = opening.GetShallowClone() as Opening;
        //    if (clone.Edges.Count != newOutline.Curves.Count)
        //    {
        //        Reflection.Compute.RecordWarning("The method could not be executed due to a Get/Set geometry issue.");
        //        return null;
        //    }

        //    List<Edge> newEdges = new List<Edge>();
        //    for (int i = 0; i < clone.Edges.Count; i++)
        //    {
        //        newEdges.Add(clone.Edges[i].SetGeometry(newOutline.Curves[i]));
        //    }
        //    clone.Edges = newEdges;
        //    return clone;
        //}

        ///***************************************************/

        //public static PanelPlanar SetGeometry(this PanelPlanar panelPlanar, List<PolyCurve> newOutlines)
        //{
        //    if (newOutlines.Count == 0)
        //    {
        //        Reflection.Compute.RecordWarning("An empty geometry was attempted to be set to a panel.");
        //        return null;
        //    }

        //    PanelPlanar clone = panelPlanar.GetShallowClone() as PanelPlanar;
        //    if (clone.ExternalEdges.Count != newOutlines[0].Curves.Count)
        //    {
        //        Reflection.Compute.RecordWarning("The method could not be executed due to a Get/Set geometry issue.");
        //        return null;
        //    }

        //    List<Edge> newEdges = new List<Edge>();
        //    for (int i = 0; i < clone.ExternalEdges.Count; i++)
        //    {
        //        newEdges.Add(clone.ExternalEdges[i].SetGeometry(newOutlines[0].Curves[i]));
        //    }
        //    clone.ExternalEdges = newEdges;

        //    if (clone.Openings.Count != newOutlines.Count - 1)
        //    {
        //        Reflection.Compute.RecordWarning("The method could not be executed due to a Get/Set geometry issue.");
        //        return null;
        //    }

        //    List<Opening> newOpenings = new List<Opening>();
        //    for (int i = 0; i < clone.Openings.Count; i++)
        //    {
        //        newOpenings.Add(panelPlanar.Openings[i].SetGeometry(newOutlines[i + 1]));
        //    }
        //    clone.Openings = newOpenings;

        //    return clone;
        //}

        ///***************************************************/

        //public static PanelFreeForm SetGeometry(this PanelFreeForm contour, ISurface surface)
        //{
        //    PanelFreeForm clone = contour.GetShallowClone() as PanelFreeForm;
        //    clone.Surface = surface as ISurface;
        //    return clone;
        //}

        /***************************************************/

    }
}
