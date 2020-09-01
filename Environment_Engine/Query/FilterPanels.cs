﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Geometry.SettingOut;
using BH.oM.Reflection;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels that match the given element ID")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("elementID", "The Element ID to filter by")]
        [Output("panels", "A collection of Environment Panel objects that match the element ID")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByElementID(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, System.String)")]
        public static List<Panel> FilterPanelsByElementID(this List<Panel> panels, string elementID)
        {
            List<IEnvironmentObject> envObjects = new List<IEnvironmentObject>();
            foreach (Panel p in panels)
                envObjects.Add(p as IEnvironmentObject);

            envObjects = envObjects.ObjectsByFragment(typeof(OriginContextFragment));

            envObjects = envObjects.Where(x => (x.Fragments.Where(y => y.GetType() == typeof(OriginContextFragment)).FirstOrDefault() as OriginContextFragment).ElementID == elementID).ToList();

            List<Panel> rtnPanels = new List<Panel>();
            foreach (IEnvironmentObject o in envObjects)
                rtnPanels.Add(o as Panel);

            return rtnPanels;
        }

        [Description("Returns a collection of Environment Panels that match the given geometry")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("searchGeometry", "The BHoM Geometry ICurve representation to search by")]
        [Output("panels", "A collection of Environment Panel where the external edges match the given search geometry")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByGeometry(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, BH.oM.Geometry.ICurve)")]
        public static List<Panel> FilterPanelsByGeometry(this List<Panel> panels, ICurve searchGeometry)
        {
            List<Point> searchPoints = searchGeometry.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).DiscontinuityPoints();
            return panels.Where(x => x.Polyline().DiscontinuityPoints().PointsMatch(searchPoints)).ToList();
        }

        [Description("Returns a collection of Environment Panels that sit entirely on a given levels elevation")]
        [Input("panels", "A collection of Environment Panels to filter")]
        [Input("searchLevel", "The Setting Out Level to search by")]
        [Output("panels", "A collection of Environment Panels which match the given level")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByLevel(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, BH.oM.Geometry.SettingOut.Level)")]
        public static List<Panel> FilterPanelsByLevel(this List<Panel> panels, Level searchLevel)
        {
            return panels.FilterPanelsByLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Panels that sit entirely on a given levels elevation")]
        [Input("panels", "A collection of Environment Panels to filter")]
        [Input("searchLevel", "The level to search by")]
        [Output("panels", "A collection of Environment Panels which match the given level")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByLevel(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, System.Double)")]
        public static List<Panel> FilterPanelsByLevel(this List<Panel> panels, double searchLevel)
        {
            return panels.Where(x => x.MinimumLevel() == searchLevel && x.MaximumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Panels where the maximum level of the panel matches the elevation of the given search level")]
        [Input("panels", "A collection of Environment Panels to filter")]
        [Input("searchLevel", "The Setting Level to search by")]
        [Output("panels", "A collection of Environment Panels where the maximum level meets the search level")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByMaximumLevel(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, BH.oM.Geometry.SettingOut.Level)")]
        public static List<Panel> FilterPanelsByMaximumLevel(this List<Panel> panels, Level searchLevel)
        {
            return panels.FilterPanelsByMaximumLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Panels where the maximum level of the panel matches the elevation of the given search level")]
        [Input("panels", "A collection of Environment Panels to filter")]
        [Input("searchLevel", "The level to search by")]
        [Output("panels", "A collection of Environment Panels where the maximum level meets the search level")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByMaximumLevel(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, System.Double)")]
        public static List<Panel> FilterPanelsByMaximumLevel(this List<Panel> panels, double searchLevel)
        {
            return panels.Where(x => x.MaximumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Panels where the minimum level of the panel matches the elevation of the given search level")]
        [Input("panels", "A collection of Environment Panels to filter")]
        [Input("searchLevel", "The Setting Out Level to search by")]
        [Output("panels", "A collection of Environment Panels where the minimum level meets the search level")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByMinimumLevel(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, BH.oM.Geometry.SettingOut.Level)")]
        public static List<Panel> FilterPanelsByMinimumLevel(this List<Panel> panels, Level searchLevel)
        {
            return panels.FilterPanelsByMinimumLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Panels where the minimum level of the panel matches the elevation of the given search level")]
        [Input("panels", "A collection of Environment Panels to filter")]
        [Input("searchLevel", "The level to search by")]
        [Output("panels", "A collection of Environment Panels where the minimum level meets the search level")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByMinimumLevel(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, System.Double)")]
        public static List<Panel> FilterPanelsByMinimumLevel(this List<Panel> panels, double searchLevel)
        {
            return panels.Where(x => x.MinimumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Panels that are match the given name")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("name", "The name of the panel to filter by")]
        [Output("panels", "A collection of Environment Panel that match the given name")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByName(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, System.String)")]
        public static List<Panel> FilterPanelsByName(this List<Panel> panels, string name)
        {
            return panels.Where(x => x.Name == name).ToList();
        }

        [Description("Returns a collection of Environment Panels that are match the given tilt")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("tilt", "The tilt to filter by")]
        [Output("panels", "A collection of Environment Panel that match the given tilt")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByTilt(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, System.Double)")]
        public static List<Panel> FilterPanelsByTilt(this List<Panel> panels, double tilt)
        {
            return panels.Where(x => x.Tilt() == tilt).ToList();
        }

        [Description("Returns a collection of Environment Panels that are tilted between the given tilt range")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("minTilt", "The minimum tilt to filter by")]
        [Input("maxTilt", "The maximum tilt to filter by")]
        [Output("panels", "A collection of Environment Panel that are between the given tilt range")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByTiltRange(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, System.Double, System.Double)")]
        public static List<Panel> FilterPanelsByTiltRange(this List<Panel> panels, double minTilt, double maxTilt)
        {
            return panels.Where(x => x.Tilt() >= minTilt && x.Tilt() <= maxTilt).ToList();
        }

        [Description("Returns a collection of Environment Panels that DO NOT match a given Panel Type")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("type", "A Panel Type to filter by from the Panel Type enum")]
        [Output("panels", "A collection of Environment Panel that DO NOT match the given type")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsNotByType(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, BH.oM.Environment.Elements.PanelType)")]
        [ToBeRemoved("3.3", "Replaced by FilterPanelsByType which provides the panels which match and don't match a given type")]
        public static List<Panel> FilterPanelsNotByType(this List<Panel> panels, PanelType type)
        {
            return panels.Where(x => x.Type != type).ToList();
        }

        [Description("Returns a collection of Environment Panels that match the provided type as the first output, and the panels which don't match the provided type as the second output")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("type", "A Panel Type to filter by from the Panel Type enum")]
        [MultiOutput(0, "panelsMatchingType", "A collection of Environment Panels that match the provided type")]
        [MultiOutput(1, "panelsNotMatchingType", "A collection of Environment Panel that DO NOT match the provided type")]
        [PreviousVersion("3.3", "BH.Engine.Environment.Query.PanelsByType(System.Collections.Generic.List<BH.oM.Environment.Elements.Panel>, BH.oM.Environment.Elements.PanelType)")]
        public static Output<List<Panel>, List<Panel>> FilterPanelsByType(this List<Panel> panels, PanelType type)
        {
            return new Output<List<Panel>, List<Panel>>
            {
                Item1 = panels.Where(x => x.Type == type).ToList(),
                Item2 = panels.Where(x => x.Type != type).ToList(),
            };
        }
    }
}
