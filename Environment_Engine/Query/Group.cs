/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Base;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Query  
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Groups a collection of Environment Panels by their panel type")]
        [Input("panels", "A collection of Environment Panels")]
        [MultiOutput(0, "undefined", "A collection of Environment Panels that match the type: Undefined")]
        [MultiOutput(1, "air", "A collection of Environment Panels that match the type: Air")]
        [MultiOutput(2, "ceiling", "A collection of Environment Panels that match the type: Ceiling")]
        [MultiOutput(3, "curtainWall", "A collection of Environment Panels that match the type: CurtainWall")]
        [MultiOutput(4, "floor", "A collection of Environment Panels that match the type: Floor, FloorExposed, FloorInternal, FloorRaised or SlabOnGrade")]
        [MultiOutput(5, "roof", "A collection of Environment Panels that match the type: Roof")]
        [MultiOutput(6, "shade", "A collection of Environment Panels that match the type: Shade")]
        [MultiOutput(7, "solarPanel", "A collection of Environment Panels that match the type: SolarPanel")]
        [MultiOutput(8, "undergroundPanel", "A collection of Environment Panels that match the type: UndergroundCeiling, UndergroundSlab or UndergroundWall")]
        [MultiOutput(9, "wall", "A collection of Environment Panels that match the type: Wall, WallExternal or WallInternal")]
        public static Output<List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>, List<Panel>> Group(this List<Panel> panels)
        {
            List<Panel> undefined = panels.Where(x => x.Type == PanelType.Undefined).ToList();
            List<Panel> air = panels.Where(x => x.Type == PanelType.Air).ToList();
            List<Panel> ceiling = panels.Where(x => x.Type == PanelType.Ceiling).ToList();
            List<Panel> curtainWall = panels.Where(x => x.Type == PanelType.CurtainWall).ToList();
            List<Panel> floor = panels.Where(x => x.Type == PanelType.Floor || x.Type == PanelType.FloorExposed || x.Type == PanelType.FloorInternal || x.Type == PanelType.FloorRaised || x.Type == PanelType.SlabOnGrade).ToList();
            List<Panel> roof = panels.Where(x => x.Type == PanelType.Roof).ToList();
            List<Panel> shade = panels.Where(x => x.IsShade()).ToList();
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

        [Description("Groups a collection of Openings by their opening type")]
        [Input("openings", "A collection of Openings")]
        [MultiOutput(0, "undefined", "A collection of Openings that match the type: Undefined")]
        [MultiOutput(1, "curtainWall", "A collection of Openings that match the type: CurtainWall")]
        [MultiOutput(2, "door", "A collection of Openings that match the type: Door")]
        [MultiOutput(3, "frame", "A collection of Openings that match the type: Frame")]
        [MultiOutput(4, "glazing", "A collection of Openings that match the type: Glazing")]
        [MultiOutput(5, "hole", "A collection of Openings that match the type: Hole")]
        [MultiOutput(6, "openingWithFrame", "A collection of Openings that match the type: RooflightWithFrame or WindowWithFrame")]
        [MultiOutput(7, "rooflight", "A collection of Openings that match the type: Rooflight")]
        [MultiOutput(8, "window", "A collection of Openings that match the type: Window")]
        [MultiOutput(9, "vehicleDoor", "A collection of Openings that match the type: VehicleDoor")]
        public static Output<List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>> Group(this List<Opening> openings)
        {
            List<Opening> undefined = openings.Where(x => x.Type == OpeningType.Undefined).ToList();
            List<Opening> curtainWall = openings.Where(x => x.Type == OpeningType.CurtainWall).ToList();
            List<Opening> door = openings.Where(x => x.Type == OpeningType.Door).ToList();
            List<Opening> frame = openings.Where(x => x.Type == OpeningType.Frame).ToList();
            List<Opening> glazing = openings.Where(x => x.Type == OpeningType.Glazing).ToList();
            List<Opening> hole = openings.Where(x => x.Type == OpeningType.Hole).ToList();
            List<Opening> openingWithFrame = openings.Where(x => x.Type == OpeningType.RooflightWithFrame || x.Type == OpeningType.WindowWithFrame).ToList();
            List<Opening> rooflight = openings.Where(x => x.Type == OpeningType.Rooflight).ToList();
            List<Opening> window = openings.Where(x => x.Type == OpeningType.Window).ToList();
            List<Opening> vehicleDoor = openings.Where(x => x.Type == OpeningType.VehicleDoor).ToList();

            return new Output<List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>, List<Opening>>
            {
                Item1 = undefined,
                Item2 = curtainWall,
                Item3 = door,
                Item4 = frame,
                Item5 = glazing,
                Item6 = hole,
                Item7 = openingWithFrame,
                Item8 = rooflight,
                Item9 = window,
                Item10 = vehicleDoor,
            };
        }
    }
}





