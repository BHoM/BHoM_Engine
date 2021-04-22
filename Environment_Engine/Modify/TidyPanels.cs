using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        public static List<List<Panel>> TidyPanels(List<Panel> panels)
        {
            List<Panel> fixedPanels = new List<Panel>();

            List<Panel> splitPanels = panels.SplitPanelsByOverlap();
            List<List<Panel>> overlappingPanels = splitPanels.Select(x => x.IdentifyOverlaps(splitPanels)).ToList();
            List<Guid> handledPanels = new List<Guid>();

            for (int x = 0; x < splitPanels.Count; x++)
            {
                if (handledPanels.Contains(splitPanels[x].BHoM_Guid))
                    continue; //This panel has already been handled
                if (splitPanels[x].Type == PanelType.Floor || splitPanels[x].Type == PanelType.Ceiling)
                {
                    fixedPanels.Add(splitPanels[x]);
                    handledPanels.Add(splitPanels[x].BHoM_Guid);
                    continue;
                }

                if (overlappingPanels[x].Count == 0)
                {
                    fixedPanels.Add(splitPanels[x]);
                    handledPanels.Add(splitPanels[x].BHoM_Guid);
                    continue;
                }

                Panel p = splitPanels[x];
                for (int y = 0; y < overlappingPanels[x].Count; y++)
                {
                    p = p.MergePanels(overlappingPanels[x][y]);
                    handledPanels.Add(overlappingPanels[x][y].BHoM_Guid);
                }

                fixedPanels.Add(p);
                handledPanels.Add(p.BHoM_Guid);
            }
            
            //List<List<Panel>> panelsAsSpaces = fixedPanels.ToSpaces();
            //foreach (List<Panel> l in panelsAsSpaces)
            //{
            //    List<List<Panel>> overlappingPanels = splitPanels.Select(x => x.IdentifyOverlaps(splitPanels)).ToList();
            //    // if tow panels have the same two connecte spaces and not the same bottom controlpoints, merge them
            //    // What does the merge panels part of the code do?

            //    string spaceName = l.ConnectedSpaceName();
            //    BH.oM.Environment.SAP.Space space = spaces.Where(x => x.BHoM_Guid.ToString() == spaceName).FirstOrDefault();
            //    if (space != null)
            //        space.Panels = l;
            //}



            return fixedPanels.ToSpaces();
        }

    }

}
