using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;

using BH.oM.Environment.Elements;

using BH.Engine.Geometry;

using System;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<BuildingElement>> BuildSpaces(this List<BuildingElement> unmatchedElements, List<BuildingElement> allElements, List<Point> unmatchedSpaces)
        {
            //Using 3D flood fill techniques to identify the remaining spaces...
            List<List<BuildingElement>> rtn = new List<List<BuildingElement>>();

            List<Point> ctrPoints = allElements.SelectMany(x => x.PanelCurve.IControlPoints()).ToList();
            BoundingBox boundingBox = BH.Engine.Geometry.Query.Bounds(ctrPoints);

            foreach (Point p in unmatchedSpaces)
                rtn.Add(BuildSpaces(p, allElements, (boundingBox.Max - boundingBox.Min).Length()));
                //rtn.Add(BuildSpaces(p, allElements));

            return rtn;
        }

        public static List<List<BuildingElement>> BuildSpacesByEdges(this List<BuildingElement> searchElements)
        {
            //Loop through each element and build up the connected elements piece by piece
            List<List<BuildingElement>> spaces = new List<List<BuildingElement>>();

            List<BuildingElement> floorElements = searchElements.Where(x => x.Tilt() == 0 || x.Tilt() == 180).ToList();

            for(int x = 0; x < floorElements.Count; x++)
            {
                BuildingElement be2 = floorElements.Where(a => a.BHoM_Guid != floorElements[x].BHoM_Guid && a.SharedConnections(floorElements[x], searchElements).Count > 1 && a.MinimumLevel() != floorElements[x].MinimumLevel() && a.MaximumLevel() != floorElements[x].MaximumLevel()).FirstOrDefault();

                if (be2 == null) continue;

                List<BuildingElement> sharedElements = floorElements[x].SharedConnections(be2, searchElements);
                List<BuildingElement> space = new List<BuildingElement>();
                space.Add(floorElements[x]);
                space.Add(be2);
                space.AddRange(sharedElements);

                //space = space.CleanSpace();
                int lastCount = 0;
                while(!space.IsClosed() && lastCount != space.Count)
                {
                    lastCount = space.Count;
                    //Continue finding elements until space is closed
                    List<BuildingElement> awaitingConnection = space.UnconnectedElements();
                    foreach(BuildingElement be in awaitingConnection)
                    {
                        List<BuildingElement> sharedConnections = floorElements[x].SharedConnections(be, searchElements);
                        sharedConnections.AddRange(be2.SharedConnections(be, searchElements));
                        sharedConnections = sharedConnections.Where(a => !space.Contains(a)).ToList();
                        sharedConnections = sharedConnections.Where(a => a.Edges().EdgeIntersects(floorElements[x].UnconnectedEdges(space)) || a.Edges().EdgeIntersects(be2.UnconnectedEdges(space))).ToList();
                        space.AddRange(sharedConnections);
                    }
                }

                space = space.CleanSpace();

                spaces.Add(space);
            }

            spaces = spaces.CullDuplicates();

            return spaces;
        }

        public static List<BuildingElement> BuildSpacesSimple(Point startPt, List<BuildingElement> searchElements, int degreeJump = 1, int lineLength = 100)
        {
            /*
             * Degree jump is used to decide the search space to be used. The higher the number, the less precise the search
             * Line Length is used to decide how far away from the search point we should be looking. Typically this should be to the edge of the bounding box of all elements for high accuracy 
             */

            List<BuildingElement> foundElements = new List<BuildingElement>();
            searchElements = new List<BuildingElement>(searchElements); //Make a copy of the list

            //Build spaces by doing a 3 dimensional ray trace in the three distinct dimensions (not a sphere)
            List<Line> rays = new List<Line>();
            for(int x = 0; x < 360; x += degreeJump)
            {
                double rad = (Math.PI / 180) * x;
                rays.Add(BH.Engine.Geometry.Create.Line(startPt, BH.Engine.Geometry.Create.Point(startPt.X + (lineLength * Math.Cos(rad)), startPt.Y + (lineLength * Math.Sin(rad)), startPt.Z)));
                rays.Add(BH.Engine.Geometry.Create.Line(startPt, BH.Engine.Geometry.Create.Point(startPt.X + (lineLength * Math.Cos(rad)), startPt.Y, startPt.Z + (lineLength * Math.Sin(rad)))));
                rays.Add(BH.Engine.Geometry.Create.Line(startPt, BH.Engine.Geometry.Create.Point(startPt.X, startPt.Y + (lineLength * Math.Cos(rad)), startPt.Z + (lineLength * Math.Sin(rad)))));
            }

            foreach (Line ray in rays)
            {
                List<BuildingElement> found = searchElements.Where(a => ray.PlaneIntersection(a.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IFitPlane()) != null && a.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsContaining(new List<Point> { ray.PlaneIntersection(a.PanelCurve.IFitPlane()) }, false)).ToList();

                if (found.Count > 0)
                {
                    BuildingElement foundElement = found[0];
                    for (int a = 1; a < found.Count; a++)
                    {
                        if (startPt.Distance(ray.PlaneIntersection(foundElement.PanelCurve.IFitPlane())) > startPt.Distance(ray.PlaneIntersection(found[a].PanelCurve.IFitPlane())))
                            foundElement = found[a];
                    }

                    if (foundElement != null && !foundElements.Contains(foundElement))
                    {
                        foundElements.Add(foundElement);
                        if (foundElements.IsClosed()) break;
                    }
                }
            }

            return foundElements;
        }

        public static List<BuildingElement> BuildSpaces(Point pt, List<BuildingElement> searchElements, double jump = 100, int angle = 5)
        {
            List<BuildingElement> foundElements = new List<BuildingElement>();

            //Create a sphere of rays...
            List<Line> rays = new List<Line>();
            for(int x = 0; x < 360; x++)
            {
                double radian = (Math.PI / 180) * x;

                for(int y = 0; y < 360; y++)
                {
                    double phi = (Math.PI / 180) * y;
                    rays.Add(BH.Engine.Geometry.Create.Line(pt, BH.Engine.Geometry.Create.Point(pt.X + (jump * Math.Cos(radian) * Math.Sin(phi)), pt.Y + (jump * Math.Sin(radian) * Math.Sin(phi)), pt.Z + (jump * Math.Cos(phi)))));
                }
            }

            foreach(Line ray in rays)
            {
                List<BuildingElement> found = searchElements.Where(a => ray.PlaneIntersection(a.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IFitPlane()) != null && a.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsContaining(new List<Point> { ray.PlaneIntersection(a.PanelCurve.IFitPlane()) }, false)).ToList();

                if (found.Count > 0)
                {
                    BuildingElement foundElement = found[0];
                    for (int a = 1; a < found.Count; a++)
                    {
                        if (pt.Distance(ray.PlaneIntersection(foundElement.PanelCurve.IFitPlane())) > pt.Distance(ray.PlaneIntersection(found[a].PanelCurve.IFitPlane())))
                            foundElement = found[a];
                    }

                    if (foundElement != null && !foundElements.Contains(foundElement))
                    {
                        foundElements.Add(foundElement);
                        if (foundElements.IsClosed()) break;
                    }
                }
            }

            return foundElements;
        }

        public static List<BuildingElement> BuildSpaces(Point startPt, List<BuildingElement> allElements)
        {
            List<BuildingElement> foundElements = new List<BuildingElement>();

            List<Point> searchPts = new List<Point>();
            List<Point> donePts = new List<Point>();
            searchPts.Add(startPt);

            while(searchPts.Count > 0)
            {
                Point sPt = searchPts[0];
                donePts.Add(sPt);
                searchPts.RemoveAt(0);

                Point westPt = BH.Engine.Geometry.Create.Point(sPt.X, sPt.Y - 1, sPt.Z);
                Line lineTest = BH.Engine.Geometry.Create.Line(sPt, westPt);
                List<BuildingElement> searchedBEs = allElements.Where(x => westPt.IsInPlane(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IFitPlane()) && x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsContaining(new List<Point> { westPt })).ToList();
                //List<BuildingElement> searchedBEs = allElements.Where(x => x.IsContaining(lineTest.PlaneIntersection(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).FitPlane(), false), true) || x.IsContaining(westPt, true)).ToList();
                if (searchedBEs.Count == 0)
                {
                    Point p = searchPts.Where(x => x.X == westPt.X && x.Y == westPt.Y && x.Z == westPt.Z).FirstOrDefault();
                    Point p2 = donePts.Where(x => x.X == westPt.X && x.Y == westPt.Y && x.Z == westPt.Z).FirstOrDefault();
                    if (p == null && p2 == null) searchPts.Add(westPt);
                    //if (!searchPts.Contains(westPt) && !donePts.Contains(westPt)) searchPts.Add(westPt);
                }
                else
                {
                    if (!foundElements.Contains(searchedBEs[0]))
                    {
                        foundElements.Add(searchedBEs[0]);
                        if (foundElements.IsClosed()) break; //Space has been successfully found, no need to keep searching now
                    }
                }

                Point eastPt = BH.Engine.Geometry.Create.Point(sPt.X, sPt.Y + 1, sPt.Z);
                lineTest = BH.Engine.Geometry.Create.Line(sPt, eastPt);
                searchedBEs = allElements.Where(x => eastPt.IsInPlane(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IFitPlane()) && x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsContaining(new List<Point> { eastPt })).ToList();
                //searchedBEs = allElements.Where(x => x.IsContaining(lineTest.PlaneIntersection(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).FitPlane(), false), true) || x.IsContaining(eastPt, true)).ToList();
                if (searchedBEs.Count == 0)
                {
                    Point p = searchPts.Where(x => x.X == eastPt.X && x.Y == eastPt.Y && x.Z == eastPt.Z).FirstOrDefault();
                    Point p2 = donePts.Where(x => x.X == eastPt.X && x.Y == eastPt.Y && x.Z == eastPt.Z).FirstOrDefault();
                    if (p == null && p2 == null) searchPts.Add(eastPt);
                    //if (!searchPts.Contains(eastPt) && !donePts.Contains(eastPt)) searchPts.Add(eastPt);
                }
                else
                {
                    if (!foundElements.Contains(searchedBEs[0]))
                    {
                        foundElements.Add(searchedBEs[0]);
                        if (foundElements.IsClosed()) break; //Space has been successfully found, no need to keep searching now
                    }
                }

                Point northPt = BH.Engine.Geometry.Create.Point(sPt.X + 1, sPt.Y, sPt.Z);
                lineTest = BH.Engine.Geometry.Create.Line(sPt, northPt);
                searchedBEs = allElements.Where(x => northPt.IsInPlane(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IFitPlane()) && x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsContaining(new List<Point> { northPt })).ToList();
                //searchedBEs = allElements.Where(x => x.IsContaining(lineTest.PlaneIntersection(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).FitPlane(), false), true) || x.IsContaining(northPt, true)).ToList();
                if (searchedBEs.Count == 0)
                {
                    Point p = searchPts.Where(x => x.X == northPt.X && x.Y == northPt.Y && x.Z == northPt.Z).FirstOrDefault();
                    Point p2 = donePts.Where(x => x.X == northPt.X && x.Y == northPt.Y && x.Z == northPt.Z).FirstOrDefault();
                    if (p == null && p2 == null) searchPts.Add(northPt);
                    //if (!searchPts.Contains(northPt) && !donePts.Contains(northPt)) searchPts.Add(northPt);
                }
                else
                {
                    if (!foundElements.Contains(searchedBEs[0]))
                    {
                        foundElements.Add(searchedBEs[0]);
                        if (foundElements.IsClosed()) break; //Space has been successfully found, no need to keep searching now
                    }
                }

                Point southPt = BH.Engine.Geometry.Create.Point(sPt.X - 1, sPt.Y, sPt.Z);
                lineTest = BH.Engine.Geometry.Create.Line(sPt, southPt);
                searchedBEs = allElements.Where(x => southPt.IsInPlane(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IFitPlane()) && x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsContaining(new List<Point> { southPt })).ToList();
                //searchedBEs = allElements.Where(x => x.IsContaining(lineTest.PlaneIntersection(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).FitPlane(), false), true) || x.IsContaining(southPt, true)).ToList();
                if (searchedBEs.Count == 0)
                {
                    Point p = searchPts.Where(x => x.X == southPt.X && x.Y == southPt.Y && x.Z == southPt.Z).FirstOrDefault();
                    Point p2 = donePts.Where(x => x.X == southPt.X && x.Y == southPt.Y && x.Z == southPt.Z).FirstOrDefault();
                    if (p == null && p2 == null) searchPts.Add(southPt);
                    //if (!searchPts.Contains(southPt) && !donePts.Contains(southPt)) searchPts.Add(southPt);
                }
                else
                {
                    if (!foundElements.Contains(searchedBEs[0]))
                    {
                        foundElements.Add(searchedBEs[0]);
                        if (foundElements.IsClosed()) break; //Space has been successfully found, no need to keep searching now
                    }
                }

                Point upPt = BH.Engine.Geometry.Create.Point(sPt.X, sPt.Y, sPt.Z + 1);
                lineTest = BH.Engine.Geometry.Create.Line(sPt, upPt);
                searchedBEs = allElements.Where(x => upPt.IsInPlane(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IFitPlane()) && x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsContaining(new List<Point> { upPt })).ToList();
                //searchedBEs = allElements.Where(x => x.IsContaining(lineTest.PlaneIntersection(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).FitPlane(), false), true) || x.IsContaining(upPt, true)).ToList();
                if (searchedBEs.Count == 0)
                {
                    Point p = searchPts.Where(x => x.X == upPt.X && x.Y == upPt.Y && x.Z == upPt.Z).FirstOrDefault();
                    Point p2 = donePts.Where(x => x.X == upPt.X && x.Y == upPt.Y && x.Z == upPt.Z).FirstOrDefault();
                    if (p == null && p2 == null) searchPts.Add(upPt);
                    //if (!searchPts.Contains(upPt) && !donePts.Contains(upPt)) searchPts.Add(upPt);
                }
                else
                {
                    if (!foundElements.Contains(searchedBEs[0]))
                    {
                        foundElements.Add(searchedBEs[0]);
                        if (foundElements.IsClosed()) break; //Space has been successfully found, no need to keep searching now
                    }
                }

                Point downPt = BH.Engine.Geometry.Create.Point(sPt.X, sPt.Y, sPt.Z - 1);
                lineTest = BH.Engine.Geometry.Create.Line(sPt, downPt);
                searchedBEs = allElements.Where(x => downPt.IsInPlane(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IFitPlane()) && x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).IsContaining(new List<Point> { downPt })).ToList();
                //searchedBEs = allElements.Where(x => x.IsContaining(lineTest.PlaneIntersection(x.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).FitPlane(), false), true) || x.IsContaining(downPt, true)).ToList();
                if (searchedBEs.Count == 0)
                {
                    Point p = searchPts.Where(x => x.X == downPt.X && x.Y == downPt.Y && x.Z == downPt.Z).FirstOrDefault();
                    Point p2 = donePts.Where(x => x.X == downPt.X && x.Y == downPt.Y && x.Z == downPt.Z).FirstOrDefault();
                    if (p == null && p2 == null) searchPts.Add(downPt);
                    //if (!searchPts.Contains(downPt) && !donePts.Contains(downPt)) searchPts.Add(downPt);
                }
                else
                {
                    if (!foundElements.Contains(searchedBEs[0]))
                    {
                        foundElements.Add(searchedBEs[0]);
                        if (foundElements.IsClosed()) break; //Space has been successfully found, no need to keep searching now
                    }
                }
            }

            return foundElements;
        }

        public static List<List<BuildingElement>> JoinBuildingElements(this List<BuildingElement> unmatchedElements, List<BuildingElement> allElements, List<Point> unmatchedSpaces)
        {
            List<List<BuildingElement>> newSpaces = new List<List<BuildingElement>>();

            //Use Dijkstra's algorithm to connect building element 'nodes' in the shortest path from the start back to their starting point

            Dictionary<Point, int> pointsInPlane = new Dictionary<Point, int>();

            BuildingElement firstElement = unmatchedElements.Where(x => x.Tilt() == 90).FirstOrDefault(); //Get first wall
            List<Line> pLines = firstElement.PanelCurve.ISubParts() as List<Line>;
            Line topLine = pLines.Where(x => x.Start.Z == firstElement.MaximumLevel() && x.End.Z == firstElement.MaximumLevel()).FirstOrDefault();

            if(topLine != null)
            {
                //This is the top line
            }



            /*
             * foreach building element in the list
             *      find the next connected element that is closest to the space and not equal to the previous element
             *      if the element already has been found before, it is time to return back out
             * 
             * 
             * 
             * */


            return newSpaces;
        }

        public static List<BuildingElement> BuildSpace(BuildingElement be, List<BuildingElement> inElements, List<BuildingElement> allElements, Point space)
        {
            //Find the next connected element closest to the space
            List<BuildingElement> connectedElements = be.ConnectedElementsByPoint(allElements);

            Polyline panelCurve = be.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);

            //BuildingElement bestMatch = connectedElements.Where(x => x.Pan)

            return inElements;
        }
    }
}