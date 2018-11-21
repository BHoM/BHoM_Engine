using System;
using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Base;

using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> BuildingElements(this List<IBHoMObject> bhomObjects)
        {
            List<BuildingElement> bes = new List<BuildingElement>();

            foreach (IBHoMObject obj in bhomObjects)
            {
                if (obj is BuildingElement)
                    bes.Add(obj as BuildingElement);
            }

            return bes;
        }

        public static List<BuildingElement> UniqueBuildingElements(this List<List<BuildingElement>> elements)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            foreach (List<BuildingElement> lst in elements)
            {
                foreach (BuildingElement be in lst)
                {
                    BuildingElement beInList = rtn.Where(x => x.BHoM_Guid == be.BHoM_Guid).FirstOrDefault();
                    if (beInList == null)
                        rtn.Add(be);
                }
            }

            return rtn;
        }

        public static List<BuildingElement> ConnectedElementsByPoint(this BuildingElement element, List<BuildingElement> elements)
        {
            List<BuildingElement> connectedElement = new List<BuildingElement>();

            List<Point> vertexPts = element.PanelCurve.IControlPoints();

            foreach (BuildingElement be in elements)
            {
                if (be.BHoM_Guid == element.BHoM_Guid) continue; //Don't check the same element...

                List<Point> vPts = be.PanelCurve.IControlPoints();

                //Check if at least one point matches in some manner
                bool ptMatches = false;
                foreach (Point pt in vPts)
                {
                    ptMatches = vertexPts.Contains(pt);
                    if (ptMatches) break; //Have found a match so no need to check the rest
                }

                if (ptMatches) connectedElement.Add(be);
            }

            return connectedElement;
        }

        public static List<BuildingElement> SearchElements(this List<List<BuildingElement>> problemSpaces, List<List<BuildingElement>> allSpaces, List<BuildingElement> unusedElements)
        {
            List<BuildingElement> elementsToSearchWith = new List<BuildingElement>(unusedElements); //Initalise with all elements that are unused

            //Go through each problem space and find all connected spaces elements to narrow the search criteria for later
            foreach (List<BuildingElement> space in problemSpaces)
            {
                foreach (BuildingElement be in space)
                {
                    List<List<BuildingElement>> adjacentSpaces = be.AdjacentSpaces(allSpaces);
                    foreach (List<BuildingElement> sp in adjacentSpaces)
                        elementsToSearchWith.AddRange(sp);
                }
            }

            elementsToSearchWith = elementsToSearchWith.CullDuplicates();

            return elementsToSearchWith;
        }

        public static List<BuildingElement> ConnectedElementsByEdge(this BuildingElement element, List<BuildingElement> searchElements)
        {
            return searchElements.Where(x => x.Edges().Where(y => y.BooleanIntersection(element.Edges()).Count > 0).ToList().Count > 0).ToList();
        }

        public static List<BuildingElement> ConnectedElementsByEdge(this BuildingElement element1, BuildingElement element2, List<BuildingElement> searchElements)
        {
            List<Line> edges = element1.Edges();
            edges.AddRange(element2.Edges());

            return searchElements.Where(x => x.Edges().Where(y => y.BooleanIntersection(edges).Count > 0).ToList().Count > 0).ToList();
        }

        public static List<BuildingElement> SharedConnections(this BuildingElement element1, BuildingElement element2, List<BuildingElement> searchElements)
        {
            List<BuildingElement> e1Connections = element1.ConnectedElementsByEdge(searchElements);
            List<BuildingElement> e2Connections = element2.ConnectedElementsByEdge(searchElements);
            return e1Connections.Where(x => e2Connections.Contains(x)).ToList();
        }

        public static List<BuildingElement> SharedConnections(this List<BuildingElement> elements, List<BuildingElement> searchElements)
        {
            //Get the shared connections of all the elements in the list
            List<BuildingElement> sharedConnections = new List<BuildingElement>();

            

            return sharedConnections;
        }

        public static List<BuildingElement> ElementsByTilt(this List<BuildingElement> elements, double tilt)
        {
            return elements.Where(x => x.Tilt() == tilt).ToList();
        }

        public static List<BuildingElement> UnconnectedElements(this List<BuildingElement> space)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            foreach(BuildingElement be in space)
            {
                if (be.ConnectedElementsByEdge(space).Count != be.Edges().Count)
                    rtn.Add(be); //This Building Element has more connections to be made
            }

            return rtn;
        }

        public static List<BuildingElement> ConnectedElementsByUnconnectedEdge(this BuildingElement element, List<BuildingElement> space, List<BuildingElement> searchElements)
        {
            return searchElements.Where(x => x.Edges().Where(y => y.BooleanIntersection(element.UnconnectedEdges(space)).Count > 0).ToList().Count > 0).ToList();
        }

        public static List<BuildingElement> FullyUsedElements(this List<BuildingElement> elements, List<List<BuildingElement>> spaces)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            foreach(BuildingElement be in elements)
            {
                if (be.AdjacentSpaces(spaces).Count == 2) //This BE has been attributed to enough spaces
                        rtn.Add(be); 
            }

            return rtn;
        }

        public static List<BuildingElement> ShadingElements(this List<BuildingElement> elements)
        {
            //Isolate all of the shading elements in the list - shading elements are ones connected only along one edge
            List<BuildingElement> shadingElements = new List<BuildingElement>();

            foreach (BuildingElement be in elements)
            {
                if (be.BuildingElementProperties != null)
                {
                    if (be.BuildingElementProperties.CustomData.ContainsKey("SAM_BuildingElementType"))
                    {
                        object aObject = be.BuildingElementProperties.CustomData["SAM_BuildingElementType"];

                        if (aObject != null && aObject.ToString().ToLower().Contains("shade"))
                            shadingElements.Add(be);
                            
                    }
                }
            }

            return shadingElements;
        }

        public static List<BuildingElement> SearchElements(this List<BuildingElement> elements, List<BuildingElement> toRemove)
        {
            List<BuildingElement> search = new List<BuildingElement>();

            //Remove elements in the toRemove list from the search list
            foreach(BuildingElement be in elements)
            {
                if (!toRemove.Contains(be))
                    search.Add(be);
            }

            return search;
        }

        public static List<BuildingElement> ElementsByType(this List<BuildingElement> elements, BuildingElementType type)
        {
            return elements.Where(x => x.BuildingElementProperties.BuildingElementType == type).ToList();
        }

        public static List<BuildingElement> ElementsByWallType(this List<BuildingElement> elements)
        {
            return elements.ElementsByType(BuildingElementType.Wall);
        }

    }
}

