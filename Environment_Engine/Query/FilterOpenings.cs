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
using BH.oM.Base;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

using BH.oM.Physical.Elements;
using BH.Engine.Geometry;

using BH.oM.Spatial.SettingOut;
 

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Openings that match the given element ID")]
        [Input("openings", "A collection of Environment Openings")]
        [Input("elementID", "The Element ID to filter by")]
        [Output("openings", "A collection of Environment Opening objects that match the element ID")]
        public static List<Opening> FilterOpeningsByElementID(this List<Opening> openings, string elementID)
        {
            List<IEnvironmentObject> envObjects = new List<IEnvironmentObject>();
            foreach (Opening o in openings)
                envObjects.Add(o as IEnvironmentObject);

            envObjects = envObjects.ObjectsByFragment(typeof(OriginContextFragment));

            envObjects = envObjects.Where(x => (x.Fragments.Where(y => y.GetType() == typeof(OriginContextFragment)).FirstOrDefault() as OriginContextFragment).ElementID == elementID).ToList();

            List<Opening> rtnOpenings = new List<Opening>();
            foreach (IEnvironmentObject o in envObjects)
                rtnOpenings.Add(o as Opening);

            return rtnOpenings;
        }

        [Description("Returns a collection of Environment Openings that match the given element ID")]
        [Input("panels", "A collection of Environment Panels to query for openings")]
        [Input("elementID", "The Element ID to filter by")]
        [Output("openings", "A collection of Environment Opening objects that match the element ID")]
        public static List<Opening> FilterOpeningsByElementID(this List<Panel> panels, string elementID)
        {
            List<Opening> allOpenings = new List<Opening>();
            foreach (Panel p in panels)
                allOpenings.AddRange(p.Openings);

            return allOpenings.FilterOpeningsByElementID(elementID);
        }

        [Description("Returns a collection of Environment Opening that sit entirely on a given levels elevation")]
        [Input("openings", "A collection of Environment Openings to filter")]
        [Input("searchLevel", "The Setting Out Level to search by")]
        [Output("openings", "A collection of Environment Openings which match the given level")]
        public static List<Opening> FilterOpeningsByLevel(this List<Opening> openings, Level searchLevel)
        {
            if(openings == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the openings related to a level if the openings are null.");
                return null;
            }

            if(searchLevel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the level that openings are on if the search level is null.");
                return null;
            }

            return openings.FilterOpeningsByLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Openings that sit entirely on a given levels elevation")]
        [Input("openings", "A collection of Environment Openings to filter")]
        [Input("searchLevel", "The level to search by")]
        [Output("openings", "A collection of Environment Openings which match the given level")]
        public static List<Opening> FilterOpeningsByLevel(this List<Opening> openings, double searchLevel)
        {
            return openings.Where(x => x.MinimumLevel() == searchLevel && x.MaximumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Openings where the maximum level of the opening matches the elevation of the given search level")]
        [Input("openings", "A collection of Environment Openings to filter")]
        [Input("searchLevel", "The Setting Level to search by")]
        [Output("openings", "A collection of Environment Openings where the maximum level meets the search level")]
        public static List<Opening> FilterOpeningsByMaximumLevel(this List<Opening> openings, Level searchLevel)
        {
            if (openings == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the openings related to a maximum level if the openings are null.");
                return null;
            }

            if (searchLevel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the openings are on the maximum level if the search level is null.");
                return null;
            }

            return openings.FilterOpeningsByMaximumLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Openings where the maximum level of the panel matches the elevation of the given search level")]
        [Input("openings", "A collection of Environment Openings to filter")]
        [Input("searchLevel", "The level to search by")]
        [Output("openings", "A collection of Environment Openings where the maximum level meets the search level")]
        public static List<Opening> FilterOpeningsByMaximumLevel(this List<Opening> openings, double searchLevel)
        {
            return openings.Where(x => x.MaximumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Openings where the minimum level of the Opening matches the elevation of the given search level")]
        [Input("openings", "A collection of Environment Openings to filter")]
        [Input("searchLevel", "The Setting Out Level to search by")]
        [Output("openings", "A collection of Environment Openings where the minimum level meets the search level")]
        public static List<Opening> FilterOpeningsByMinimumLevel(this List<Opening> openings, Level searchLevel)
        {
            if (openings == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the openings related to a minimum level if the openings are null.");
                return null;
            }

            if (searchLevel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the openings are on the minimum level if the search level is null.");
                return null;
            }

            return openings.FilterOpeningsByMinimumLevel(searchLevel.Elevation);
        }

        [Description("Returns a collection of Environment Openings where the minimum level of the Opening matches the elevation of the given search level")]
        [Input("openings", "A collection of Environment Opening to filter")]
        [Input("searchLevel", "The level to search by")]
        [Output("openings", "A collection of Environment Opening where the minimum level meets the search level")]
        public static List<Opening> FilterOpeningsByMinimumLevel(this List<Opening> openings, double searchLevel)
        {
            return openings.Where(x => x.MinimumLevel() == searchLevel).ToList();
        }

        [Description("Returns a collection of Environment Openings that match the given opening name")]
        [Input("openings", "A collection of Environment Openings")]
        [Input("openingName", "The Opening Name to filter by")]
        [Output("openings", "A collection of Environment Opening objects that match the name")]
        public static List<Opening> FilterOpeningsByName(this List<Opening> openings, string openingName)
        {
            return openings.Where(x => x.Name == openingName).ToList();
        }

        [Description("Returns a collection of Environment Openings that match the provided type as the first output, and the openings which don't match the provided type as the second output")]
        [Input("openings", "A collection of Environment Openings")]
        [Input("type", "An Opening Type to filter by from the Opening Type enum")]
        [MultiOutput(0, "openingsMatchingType", "A collection of Environment Panels that match the provided type")]
        [MultiOutput(1, "openingsNotMatchingType", "A collection of Environment Panel that DO NOT match the provided type")]
        public static Output<List<Opening>, List<Opening>> FilterOpeningsByType(this List<Opening> openings, OpeningType type)
        {
            return new Output<List<Opening>, List<Opening>>
            {
                Item1 = openings.Where(x => x.Type == type).ToList(),
                Item2 = openings.Where(x => x.Type != type).ToList(),
            };
        }
    }
}





