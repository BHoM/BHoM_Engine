using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Reflection;

namespace BH.Engine.Environment
{
    public static partial class Query  
    {
        public static Output<List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>> GroupingPanels(List<Panel> panelObject)
        {
            List<Panel> undefined = panelObject.Where(x => x.Type == PanelType.Undefined).ToList();
            List<Panel> air = panelObject.Where(x => x.Type == PanelType.Air).ToList();
            List<Panel> ceiling = panelObject.Where(x => x.Type == PanelType.Ceiling && x.Type == PanelType.FloorInternal && x.Type == PanelType.FloorRaised).ToList();
            List<Panel> curtainWall = panelObject.Where(x => x.Type == PanelType.CurtainWall).ToList();
            List<Panel> floor = panelObject.Where(x => x.Type == PanelType.Floor && x.Type == PanelType.FloorExposed && x.Type == PanelType.SlabOnGrade).ToList();
            List<Panel> roof = panelObject.Where(x => x.Type == PanelType.Roof).ToList();
            List<Panel> shade = panelObject.Where(x => x.Type == PanelType.Shade).ToList();
            List<Panel> solarPanel = panelObject.Where(x => x.Type == PanelType.SolarPanel).ToList();
            List<Panel> undergroundPanel = panelObject.Where(x => x.Type == PanelType.UndergroundCeiling && x.Type == PanelType.UndergroundSlab && x.Type == PanelType.UndergroundWall).ToList();
            List<Panel> wall = panelObject.Where(x => x.Type == PanelType.Wall && x.Type == PanelType.WallExternal && x.Type == PanelType.WallInternal).ToList();


            return new Output<List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>>
            {
                Item1 = undefined,
                Item2 = air,
                Item3 = ceiling,
                Item4 = curtainWall,
                Item5 = floor,
                Item6 = roof,
                Item7 = shade,
                Item8 = solarPanel,
                Item9 = undergroundPanel,
            };
        }
            
        

    }
}
