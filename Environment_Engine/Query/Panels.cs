/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Environment.Properties;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.Engine.Geometry;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Panels from a list of generic BHoM objects")]
        [Input("objects", "A collection of generic BHoM objects")]
        [Output("panels", "A collection of Environment Panel objects")]
        public static List<Panel> Panels(this List<IBHoMObject> objects)
        {
            objects = objects.ObjectsByType(typeof(Panel));
            List<Panel> spaces = new List<Panel>();
            foreach (IBHoMObject o in objects)
                spaces.Add(o as Panel);

            return spaces;
        }

        [Description("Returns a collection of unique Environment Panels from a list of panels representing spaces")]
        [Input("panelsAsSpaces", "A collection of Environment Panels that represent spaces")]
        [Output("uniquePanels", "A collection of Environment Panel without duplicates")]
        public static List<Panel> UniquePanels(this List<List<Panel>> panelsAsSpaces)
        {
            List<Panel> rtn = new List<Panel>();

            foreach (List<Panel> lst in panelsAsSpaces)
            {
                foreach (Panel be in lst)
                {
                    Panel beInList = rtn.Where(x => x.BHoM_Guid == be.BHoM_Guid).FirstOrDefault();
                    if (beInList == null)
                        rtn.Add(be);
                }
            }

            return rtn;
        }

        [Description("Returns a collection of Environment Panels that match a given Panel Type")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("type", "A Panel Type to filter by from the Panel Type enum")]
        [Output("panels", "A collection of Environment Panel that match the given type")]
        public static List<Panel> PanelsByType(this List<Panel> panels, PanelType type)
        {
            return panels.Where(x => x.Type == type).ToList();
        }

        [Description("Returns a collection of Environment Panels that DO NOT match a given Panel Type")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("type", "A Panel Type to filter by from the Panel Type enum")]
        [Output("panels", "A collection of Environment Panel that DO NOT match the given type")]
        public static List<Panel> PanelsNotByType(this List<Panel> panels, PanelType type)
        {
            return panels.Where(x => x.Type != type).ToList();
        }

        [Description("Returns a collection of Environment Panels that are match the given name")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("name", "The name of the panel to filter by")]
        [Output("panels", "A collection of Environment Panel that match the given name")]
        public static List<Panel> PanelsByName(this List<Panel> panels, string name)
        {
            return panels.Where(x => x.Name == name).ToList();
        }

        [Description("Returns a collection of Environment Panels that are match the given tilt")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("tilt", "The tilt to filter by")]
        [Output("panels", "A collection of Environment Panel that match the given tilt")]
        public static List<Panel> PanelsByTilt(this List<Panel> panels, double tilt)
        {
            return panels.Where(x => x.Tilt() == tilt).ToList();
        }

        [Description("Returns a collection of Environment Panels that are tilted between the given tilt range")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("minTilt", "The minimum tilt to filter by")]
        [Input("maxTilt", "The maximum tilt to filter by")]
        [Output("panels", "A collection of Environment Panel that are between the given tilt range")]
        public static List<Panel> PanelsByTiltRange(this List<Panel> panels, double minTilt, double maxTilt)
        {
            return panels.Where(x => x.Tilt() >= minTilt && x.Tilt() <= maxTilt).ToList();
        }

        [Description("Returns a collection of Environment Panels containing the given search point")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("searchPoint", "The BHoM Geometry Point to search by")]
        [Output("panels", "A collection of Environment Panel where the external edges contain the given search point")]
        public static List<Panel> PanelsContainingPoint(this List<Panel> panels, Point searchPoint)
        {
            return panels.Where(x => x.ToPolyline().IsContaining(new List<Point>() { searchPoint })).ToList();
        }

        [Description("Returns a collection of Environment Panels that match the given geometry")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("searchGeometry", "The BHoM Geometry ICurve representation to search by")]
        [Output("panels", "A collection of Environment Panel where the external edges match the given search geometry")]
        public static List<Panel> PanelsByGeometry(this List<Panel> panels, ICurve searchGeomtry)
        {
            List<Point> searchPoints = searchGeomtry.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle).DiscontinuityPoints();
            return panels.Where(x => x.ToPolyline().DiscontinuityPoints().PointsMatch(searchPoints)).ToList();
        }

        [Description("Returns a collection of Environment Panels that match the given element ID")]
        [Input("panels", "A collection of Environment Panels")]
        [Input("elementID", "The Element ID to filter by")]
        [Output("panels", "A collection of Environment Panel objects that match the element ID")]
        public static List<Panel> PanelsByElementID(this List<Panel> panels, string elementID)
        {
            List<IEnvironmentObject> envObjects = new List<IEnvironmentObject>();
            foreach (Panel p in panels)
                envObjects.Add(p as IEnvironmentObject);

            envObjects = envObjects.ObjectsByFragment(typeof(OriginContextFragment));

            envObjects = envObjects.Where(x => (x.FragmentProperties.Where(y => y.GetType() == typeof(OriginContextFragment)).FirstOrDefault() as OriginContextFragment).ElementID == elementID).ToList();

            List<Panel> rtnPanels = new List<Panel>();
            foreach (IEnvironmentObject o in envObjects)
                rtnPanels.Add(o as Panel);

            return rtnPanels;
        }
    }
}