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
using System.Text;
using System.Threading.Tasks;
using BH.oM.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Fragments;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Base;
using BH.oM.Spatial.SettingOut;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Returns a collection of Environment Panels that match the given element ID.")]
        [Input("panels", "A collection of Environment Panels.")]
        [Input("elementID", "The Element ID to filter by.")]
        [Output("panels", "A collection of Environment Panel objects that match the element ID.")]
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

        [Description("Returns a collection of Environment Panels that match the given geometry.")]
        [Input("panels", "A collection of Environment Panels.")]
        [Input("searchGeometry", "The BHoM Geometry ICurve representation to search by.")]
        [Output("panels", "A collection of Environment Panel where the external edges match the given search geometry.")]
        public static List<Panel> FilterPanelsByGeometry(this List<Panel> panels, ICurve searchGeometry)
        {
            List<Point> searchPoints = searchGeometry.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).DiscontinuityPoints();
            return panels.Where(x => x.Polyline().DiscontinuityPoints().PointsMatch(searchPoints)).ToList();
        }

        [Description("Returns a collection of Environment Panels that sit entirely on a given levels elevation.")]
        [Input("panels", "A collection of Environment Panels to filter.")]
        [Input("searchLevel", "The Setting Out Level to search by.")]
        [Output("panels", "A collection of Environment Panels which match the given level.")]
        public static List<Panel> FilterPanelsByLevel(this List<Panel> panels, Level searchLevel)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the panels related to a level if the panels are null.");
                return null;
            }

            if (searchLevel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the level that panels are on if the search level is null.");
                return null;
            }

            return panels.FilterPanelsByLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Panels that sit entirely on a given levels elevation.")]
        [Input("panels", "A collection of Environment Panels to filter.")]
        [Input("searchLevel", "The level to search by.")]
        [Output("panels", "A collection of Environment Panels which match the given level.")]
        public static List<Panel> FilterPanelsByLevel(this List<Panel> panels, double searchLevel)
        {
            return panels.Where(x => x.MinimumLevel() == searchLevel && x.MaximumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Panels where the maximum level of the panel matches the elevation of the given search level.")]
        [Input("panels", "A collection of Environment Panels to filter.")]
        [Input("searchLevel", "The Setting Level to search by.")]
        [Output("panels", "A collection of Environment Panels where the maximum level meets the search level.")]
        public static List<Panel> FilterPanelsByMaximumLevel(this List<Panel> panels, Level searchLevel)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the panels related to a maximum level if the panels are null.");
                return null;
            }

            if (searchLevel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the panels are on the maximum level if the search level is null.");
                return null;
            }

            return panels.FilterPanelsByMaximumLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Panels where the maximum level of the panel matches the elevation of the given search level.")]
        [Input("panels", "A collection of Environment Panels to filter.")]
        [Input("searchLevel", "The level to search by.")]
        [Output("panels", "A collection of Environment Panels where the maximum level meets the search level.")]
        public static List<Panel> FilterPanelsByMaximumLevel(this List<Panel> panels, double searchLevel)
        {
            return panels.Where(x => x.MaximumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Panels where the minimum level of the panel matches the elevation of the given search level.")]
        [Input("panels", "A collection of Environment Panels to filter.")]
        [Input("searchLevel", "The Setting Out Level to search by.")]
        [Output("panels", "A collection of Environment Panels where the minimum level meets the search level.")]
        public static List<Panel> FilterPanelsByMinimumLevel(this List<Panel> panels, Level searchLevel)
        {
            if (panels == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the panels related to a minimum level if the panels are null.");
                return null;
            }

            if (searchLevel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the panels are on the minimum level if the search level is null.");
                return null;
            }

            return panels.FilterPanelsByMinimumLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Panels where the minimum level of the panel matches the elevation of the given search level.")]
        [Input("panels", "A collection of Environment Panels to filter.")]
        [Input("searchLevel", "The level to search by.")]
        [Output("panels", "A collection of Environment Panels where the minimum level meets the search level.")]
        public static List<Panel> FilterPanelsByMinimumLevel(this List<Panel> panels, double searchLevel)
        {
            return panels.Where(x => x.MinimumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Panels that match the given name.")]
        [Input("panels", "A collection of Environment Panels.")]
        [Input("name", "The name of the panel to filter by.")]
        [Output("panels", "A collection of Environment Panels that match the given name.")]
        public static List<Panel> FilterPanelsByName(this List<Panel> panels, string name)
        {
            return panels.Where(x => x.Name == name).ToList();
        }

        [Description("Returns a collection of Environment Panels that match the given tilt.")]
        [Input("panels", "A collection of Environment Panels.")]
        [Input("tilt", "The tilt to filter by.")]
        [Output("panels", "A collection of Environment Panels that match the given tilt.")]
        public static List<Panel> FilterPanelsByTilt(this List<Panel> panels, double tilt)
        {
            return panels.Where(x => x.Tilt() == tilt).ToList();
        }

        [Description("Returns a collection of Environment Panels that are tilted between the given tilt range.")]
        [Input("panels", "A collection of Environment Panels.")]
        [Input("minTilt", "The minimum tilt to filter by.")]
        [Input("maxTilt", "The maximum tilt to filter by.")]
        [Output("panels", "A collection of Environment Panel that are between the given tilt range.")]
        public static List<Panel> FilterPanelsByTiltRange(this List<Panel> panels, double minTilt, double maxTilt)
        {
            return panels.Where(x => x.Tilt() >= minTilt && x.Tilt() <= maxTilt).ToList();
        }

        [Description("Returns a collection of Environment Panels that match the provided types as the first output, and the panels which don't match the provided types as the second output.")]
        [Input("panels", "A collection of Environment Panels.")]
        [Input("types", "One or more Panel Types to filter by from the Panel Type enum.")]
        [MultiOutput(0, "panelsMatchingType", "A collection of Environment Panels that match the provided types.")]
        [MultiOutput(1, "panelsNotMatchingType", "A collection of Environment Panel that DO NOT match the provided types.")]
        [PreviousInputNames("types", "type")]
        public static Output<List<Panel>, List<Panel>> FilterPanelsByType(this List<Panel> panels, List<PanelType> types)
        {
            return new Output<List<Panel>, List<Panel>>{Item1 = panels.Where(x => types.Contains(x.Type)).ToList(), Item2 = panels.Where(x => !types.Contains(x.Type)).ToList(), };
        }
    }
}