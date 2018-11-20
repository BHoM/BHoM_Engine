using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.Engine.Environment;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> CullOverlaps(this List<BuildingElement> elements)
        {
            List<BuildingElement> ori = new List<BuildingElement>(elements);
            List<BuildingElement> toReturn = new List<BuildingElement>();
            
            while(ori.Count > 0)
            {
                BuildingElement current = ori[0];
                List<BuildingElement> overlaps = current.IdentifyOverlaps(elements);

                foreach (BuildingElement be in overlaps)
                    ori.Remove(be);

                toReturn.Add(current);
                ori.RemoveAt(0);
            }

            return toReturn;
        }

        public static List<BuildingElement> CullDuplicates(this List<BuildingElement> elements)
        {
            //Go through each building element and compare vertices and centre points - if there is a matching element, remove it
            for(int x = 0; x < elements.Count; x++)
            {
                if (elements[x] == null) continue;

                for(int y = x + 1; y < elements.Count; y++)
                {
                    if (elements[x].IsIdentical(elements[y]))
                        elements[y] = null;
                }
            }

            return elements.Where(x => x != null).ToList();
        }

        public static List<List<BuildingElement>> CullDuplicates(this List<List<BuildingElement>> spaces)
        {
            //Go through each set of building elements and find those that match
            for(int x = 0; x < spaces.Count; x++)
            {
                List<BuildingElement> space = spaces[x];
                for(int y = x+1; y < spaces.Count; y++)
                {
                    List<BuildingElement> space2 = spaces[y];
                    if (space2.Count != space.Count) continue; //Numbers don't match so no point checking equality
                    bool allMatch = true;
                    
                    foreach(BuildingElement be in space)
                    {
                        allMatch &= space2.Contains(be);
                        if (!allMatch) break; //No point checking everything if we find a non-match
                    }

                    if(allMatch)
                    {
                        //This space matches another space, set all the BEs to null
                        spaces[y] = new List<BuildingElement>(); //Empty list
                    }                    
                }
            }

            return spaces.Where(x => x.Count > 0).ToList();
        }

        public static List<Line> CullDuplicateLines(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            List<Line> result = lines.Select(l => l).ToList();
            for (int i = lines.Count - 2; i >= 0; i--)
            {
                for (int j = lines.Count - 1; j > i; j--)
                {
                    Line l1 = lines[i];
                    Line l2 = lines[j];
                    {
                        if ((l1.Start.SquareDistance(l2.Start) <= sqTol && l1.End.SquareDistance(l2.End) <= sqTol) || (l1.Start.SquareDistance(l2.End) <= sqTol && l1.End.SquareDistance(l2.Start) <= sqTol))
                        {
                            lines.RemoveAt(j);
                        }
                    }
                }
            }
            return lines;
        }
    }
}
