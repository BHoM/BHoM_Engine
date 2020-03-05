/*
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
using BH.oM.Base;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

using BH.oM.Physical.Elements;
using BH.Engine.Geometry;

using BH.oM.Geometry.SettingOut;

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
        public static List<Opening> OpeningsByElementID(this List<Opening> openings, string elementID)
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
        public static List<Opening> OpeningsByElementID(this List<Panel> panels, string elementID)
        {
            List<Opening> allOpenings = new List<Opening>();
            foreach (Panel p in panels)
                allOpenings.AddRange(p.Openings);

            return allOpenings.OpeningsByElementID(elementID);
        }
    }
}
