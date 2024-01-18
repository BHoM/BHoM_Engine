using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Environment.Configuration;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using ICSharpCode.Decompiler.IL.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        [Description("Creates openings for input Panels at the locations provided.\n The total area of the openings is equal to the total area of the external panels, multiplied by the glazing ratio.")]
        [Input("panelsAsSpaces", "Panels as spaces - A collection of Environment Panels which will be used to identify the host panel for the opening from the provided location point.")]
        [Input("glazingLocations", "The point in 3D space corresponding to the desired locations of the openings.")]
        [Input("glazingRatio", "Ratio of total external panel surface area and glazed area. Ratio as decimal {0-1}.")]
        [Input("panelsToIgnore", "Optional input for selecting a collection of Environment Panels to be ignored in the calculation")]
        [Input("sillHeight", "Optional input for defining the distance between the base of the panel and the bottom of the opening - default: 0.5m.")]
        [Input("openingHeight", "Optional input for defining the height of opening - default: 1.2m.")]
        [Input("openingType", "Optional input for defining the opening type of the output Openings, can be either 'Glazing' or 'Door' - default: Glazing.")]
        [Output("openings", "Returns the openings.")]
        public static List<Opening> OpeningsFromGlazingRatio(
            List<List<Panel>> panelsAsSpaces,
            List<Point> glazingLocations,
            double glazingRatio,
            List<Panel> panelsToIgnore = null,
            double sillHeight = 0.5,
            double openingHeight = 1.2,
            OpeningType openingType = OpeningType.Glazing
            ) 
        {
            switch (openingType)
            {
                case OpeningType.Door:
                    break;
                case OpeningType.Glazing:
                    break;
                default:
                    BH.Engine.Base.Compute.RecordError($"Only openings of type 'Glazing' and 'Door' are supported.");
                    return new List<Opening>();
            }

            List<Panel> wallPanels =  Query.FilterPanelsByType(panelsAsSpaces.SelectMany(x => x).Distinct().ToList(), new List<PanelType>() { PanelType.Wall }).Item1;
            List<Panel> externalPanels = wallPanels.IsExternal().Item1;
            List<Panel> filteredPanels = externalPanels.RemovePanels(panelsToIgnore ?? new List<Panel>());

            double totalArea = filteredPanels.Select(x=>x.Area()).Sum();
            double existingOpeningArea = filteredPanels.Select(x => x.Openings.Select(y => y.Polyline().Area()).Sum()).Sum();
            double windowWidth = WindowWidth(totalArea, glazingRatio, glazingLocations.Count(), openingHeight, existingOpeningArea);

            OpeningOption option = new OpeningOption()
            {
                Height = openingHeight,
                Width = windowWidth,
                SillHeight = sillHeight,
                Type = openingType,
            };

            List<Opening> openings = new List<Opening>();
            foreach(Point point in glazingLocations)
            {
                Opening opening = Create.Opening(point, option, filteredPanels);
                if(opening != null)
                {
                    openings.Add(opening);
                }
                else
                {
                    BH.Engine.Base.Compute.RecordWarning($"Could not find a host panel for the opening at ({point.X}, {point.Y}, {point.Z}).");
                }
            }
            return openings;
        }

        private static double WindowWidth(
            double area,
            double glazingRatio,
            double numberOfWindows,
            double height,
            double existingGlazingArea)
        {
            //area available for glazing is total area * glazing ratio
            //area avaialable per for new glazing is area avaialble minus area used by other galzing 
            //area per window avaialable is total area left divided for number of windows
            //window width is then area per window divided by the entered height
            double areaRequiredPerWindow = (area * glazingRatio - existingGlazingArea)/numberOfWindows;
            return areaRequiredPerWindow / height;
        }
    }
}