using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<Opening> openings = null, IProperty2D property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();

            return PanelPlanar(externalEdges, openings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<ICurve> openings = null, IProperty2D property = null, string name = "")
        {
            if (!outline.IIsClosed()) return null;
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            List<Edge> externalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();
            return PanelPlanar(externalEdges, pOpenings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(List<Edge> externalEdges, List<ICurve> openings = null, IProperty2D property = null, string name = "")
        {
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            return PanelPlanar(externalEdges, pOpenings, property, name);
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(List<Edge> externalEdges, List<Opening> openings = null, IProperty2D property = null, string name = "")
        {
            return new PanelPlanar
            {
                ExternalEdges = externalEdges,
                Openings = openings ?? new List<Opening>(),
                Property = property,
                Name = name
            };
        }

        /***************************************************/

        public static List<PanelPlanar> PanelPlanar(List<Polyline> outlines, IProperty2D property = null, string name = "")
        {
            List<PanelPlanar> result = new List<PanelPlanar>();
            List<List<Polyline>> sortedOutlines = outlines.DistributeOutlines();
            foreach (List<Polyline> panelOutlines in sortedOutlines)
            {
                List<Edge> externalEdges = panelOutlines[0].SubParts().Select(o => new Edge { Curve = o }).ToList();
                List<Opening> openings = new List<Opening>();
                foreach (Polyline p in panelOutlines.Skip(1))
                {
                    List<Edge> openingEdges = p.SubParts().Select(o => new Edge { Curve = o }).ToList();
                    openings.Add(new Opening { Edges = openingEdges });
                }
                result.AddRange((new PanelPlanar { ExternalEdges = externalEdges, Openings = openings, Property = property, Name = name }).RecomputeEdges());
            }
            return result;
        }

        /***************************************************/

        public static List<PanelPlanar> PanelPlanar(List<ICurve> outlines, IProperty2D property = null, string name = "")
        {
            List<PanelPlanar> result = new List<PanelPlanar>();
            List<List<ICurve>> sortedOutlines = outlines.DistributeOutlines();
            foreach (List<ICurve> panelOutlines in sortedOutlines)
            {
                List<Edge> externalEdges = panelOutlines[0].ISubParts().Select(o => new Edge { Curve = o }).ToList();
                List<Opening> openings = new List<Opening>();
                foreach (ICurve p in panelOutlines.Skip(1))
                {
                    List<Edge> openingEdges = p.ISubParts().Select(o => new Edge { Curve = o }).ToList();
                    openings.Add(new Opening { Edges = openingEdges });
                }
                result.Add(new PanelPlanar { ExternalEdges = externalEdges, Openings = openings, Property = property, Name = name });
            }
            return result;
        }

        /***************************************************/
    }
}

