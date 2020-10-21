using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Reflection;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Query  
    {
        [Description("Groups a collection of Environment Panels by their panel type")]
        [Input("panels", "A collection of Environment Panels")]
        [MultiOutput(0, "undefined", "A collection of Environment Panels that match the type: Undefined")]
        [MultiOutput(1, "air", "A collection of Environment Panels that match the type: Air")]
        [MultiOutput(2, "ceiling", "A collection of Environment Panels that match the type: Ceiling")]
        [MultiOutput(3, "curtainWall", "A collection of Environment Panels that match the type: CurtainWall")]
        [MultiOutput(4, "floor", "A collection of Environment Panels that match the type: Floor, FloorExposed, FloorInternal, FloorRaised, SlabOnGrade")]
        [MultiOutput(5, "roof", "A collection of Environment Panels that match the type: Roof")]
        [MultiOutput(6, "shade", "A collection of Environment Panels that match the type: Shade")]
        [MultiOutput(7, "solarPanel", "A collection of Environment Panels that match the type: SolarPanel")]
        [MultiOutput(8, "undergroundPanel", "A collection of Environment Panels that match the type: UndergroundCeiling, UndergroundSlab, UndergroundWall")]
        [MultiOutput(9, "wall", "A collection of Environment Panels that match the type: Wall, WallExternal, WallInternal")]
        public static Output<List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>> Group(this List<Panel> panels)
        {
            List<Panel> undefined = panels.Where(x => x.Type == PanelType.Undefined).ToList();
            List<Panel> air = panels.Where(x => x.Type == PanelType.Air).ToList();
            List<Panel> ceiling = panels.Where(x => x.Type == PanelType.Ceiling).ToList();
            List<Panel> curtainWall = panels.Where(x => x.Type == PanelType.CurtainWall).ToList();
            List<Panel> floor = panels.Where(x => x.Type == PanelType.Floor || x.Type == PanelType.FloorExposed || x.Type == PanelType.FloorInternal || x.Type == PanelType.FloorRaised || x.Type == PanelType.SlabOnGrade).ToList();
            List<Panel> roof = panels.Where(x => x.Type == PanelType.Roof).ToList();
            List<Panel> shade = panels.Where(x => x.Type == PanelType.Shade).ToList();
            List<Panel> solarPanel = panels.Where(x => x.Type == PanelType.SolarPanel).ToList();
            List<Panel> undergroundPanel = panels.Where(x => x.Type == PanelType.UndergroundCeiling || x.Type == PanelType.UndergroundSlab || x.Type == PanelType.UndergroundWall).ToList();
            List<Panel> wall = panels.Where(x => x.Type == PanelType.Wall || x.Type == PanelType.WallExternal || x.Type == PanelType.WallInternal).ToList();

            return new Output<List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>>
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
                Item10 = wall,
            };
        }
        

    }
}
