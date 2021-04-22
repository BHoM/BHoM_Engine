using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        public static List<List<Panel>> SetPanelTypeByDwelling(List<Panel> panels, List<Dwelling> dwellings)
        {
            List<Panel> fixedPanels = new List<Panel>();
            List<Guid> handledPanels = new List<Guid>();
            for (int i = 0; i < dwellings.Count; i++)
            {

                List<Panel> wallPanels = panels.Where(x => x.Type == PanelType.Wall).ToList();
                List<Panel> floors = panels.Where(x => x.Type == PanelType.Floor).ToList();
                List<Panel> ceilings = panels.Where(x => x.Type == PanelType.Ceiling).ToList();

                List < Panel > exteriorWalls = wallPanels.Where(x => (x.Bottom() as Polyline).ControlPoints.Where(y => y.IIsOnCurve(dwellings[i].Perimeter as Polyline)).Count() == 2).ToList();
                foreach (Panel p in exteriorWalls)
                {
                    if (p != null && (handledPanels.Contains(p.BHoM_Guid) == false))
                    {
                        p.Type = PanelType.WallExternal;
                        fixedPanels.Add(p);
                        handledPanels.Add(p.BHoM_Guid);
                    }
                }

                foreach (Panel p in floors)
                {
                    if (p != null && (handledPanels.Contains(p.BHoM_Guid) == false))
                    {
                        p.Type = PanelType.Floor;
                        fixedPanels.Add(p);
                        handledPanels.Add(p.BHoM_Guid);
                    }
                }

                foreach (Panel p in ceilings)
                {
                    if (p != null && (handledPanels.Contains(p.BHoM_Guid) == false))
                    {
                        p.Type = PanelType.Ceiling;
                        fixedPanels.Add(p);
                        handledPanels.Add(p.BHoM_Guid);
                    }
                }


            }

            foreach (Panel p in panels)
            {
                if (handledPanels.Contains(p.BHoM_Guid) == false && p.Type == PanelType.Wall)
                {
                    p.Type = PanelType.WallInternal;
                    handledPanels.Add(p.BHoM_Guid);
                    fixedPanels.Add(p);
                }
            }

            return fixedPanels.ToSpaces();
        }
    }
    
}
