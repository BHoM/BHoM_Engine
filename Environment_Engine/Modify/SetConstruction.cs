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

using BH.Engine.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Environment;
using BH.oM.Geometry;

using BH.oM.Physical.Constructions;

using System.Collections.Generic;
using System.Linq;
using BH.oM.Environment.Fragments;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Update a new construction to a collection of panels which match the given type from their Origin Context Fragment")]
        [Input("panels", "A collection of Environment Panels to set the construction of")]
        [Input("type", "The type name of the panels to update")]
        [Input("newConstruction", "The new construction to assign to the panels")]
        [Output("panels", "The collection of Environment Panels with an updated construction")]
        public static List<Panel> SetConstructions(this List<Panel> panels, string type, IConstruction newConstruction)
        {
            List<Panel> returnPanels = new List<Panel>();

            foreach (Panel p in panels)
            {
                OriginContextFragment context = p.FindFragment<OriginContextFragment>(typeof(OriginContextFragment));
                if (context == null || context.TypeName != type)
                    returnPanels.Add(p);
                else if (context != null && context.TypeName == type)
                {
                    p.Construction = newConstruction;
                    returnPanels.Add(p);
                }
            }

            return returnPanels;
        }

        [Description("Update a new construction to a collection of openings which match the given type from their Origin Context Fragment")]
        [Input("openings", "A collection of Environment Openings to set the construction of")]
        [Input("type", "The type name of the openings to update")]
        [Input("newConstruction", "The new construction to assign to the openings")]
        [Output("openings", "The collection of Environment Openings with an updated construction")]
        public static List<Opening> SetConstructions(this List<Opening> openings, string type, IConstruction newConstruction)
        {
            List<Opening> returnOpenings = new List<Opening>();

            foreach (Opening o in openings)
            {
                OriginContextFragment context = o.FindFragment<OriginContextFragment>(typeof(OriginContextFragment));
                if (context == null || context.TypeName != type)
                    returnOpenings.Add(o);
                else if (context != null && context.TypeName == type)
                {
                    o.OpeningConstruction = newConstruction;
                    returnOpenings.Add(o);
                }
            }

            return returnOpenings;
        }
    }
}
