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

        [Description("Update a new construction to a collection of panels. OPTIONAL - filter the panels to receive the new construction by their Origin Context Fragment type name")]
        [Input("panels", "A collection of Environment Panels to set the construction of")]
        [Input("newConstruction", "The new construction to assign to the panels")]
        [Input("typeNames", "OPTIONAL: The type names of the panels to update - if any panels type name is contained in the list given it will have its construction updated, default null")]
        [Output("panels", "The collection of Environment Panels with an updated construction")]
        public static List<Panel> SetConstructions(this List<Panel> panels, IConstruction newConstruction, List<string> typeNames = null)
        {
            List<Panel> returnPanels = new List<Panel>();

            foreach (Panel p in panels)
            {
                if (typeNames == null)
                    p.Construction = newConstruction;
                else
                {
                    OriginContextFragment context = p.FindFragment<OriginContextFragment>(typeof(OriginContextFragment));
                    if (context != null && typeNames.Contains(context.TypeName))
                        p.Construction = newConstruction;
                }

                returnPanels.Add(p);
            }

            return returnPanels;
        }

        [Description("Update a new construction to a collection of openings. OPTIONAL - filter the openings to receive the new construction by their Origin Context Fragment type name")]
        [Input("openings", "A collection of Environment Openings to set the construction of")]
        [Input("newConstruction", "The new construction to assign to the openings")]
        [Input("typeNames", "OPTIONAL: The type names of the openings to update - if any openings type name is contained in the list given it will have its construction updated, default null")]
        [Output("openings", "The collection of Environment Openings with an updated construction")]
        public static List<Opening> SetConstructions(this List<Opening> openings, IConstruction newConstruction, List<string> typeNames = null)
        {
            List<Opening> returnOpenings = new List<Opening>();

            foreach (Opening o in openings)
            {
                if (typeNames == null)
                    o.OpeningConstruction = newConstruction;
                else
                {
                    OriginContextFragment context = o.FindFragment<OriginContextFragment>(typeof(OriginContextFragment));
                    if (context != null && typeNames.Contains(context.TypeName))
                        o.OpeningConstruction = newConstruction;
                }

                returnOpenings.Add(o);
            }

            return returnOpenings;
        }

        [Description("Update a new construction to openings hosted on panels. OPTIONAL - filter the openings to receive the new construction by their Origin Context Fragment type name")]
        [Input("panels", "A collection of Environment Panels to update the hosted openings constructions of")]
        [Input("newConstruction", "The new construction to assign to the openings")]
        [Input("typeNames", "OPTIONAL: The type names of the openings to update - if any openings type name is contained in the list given it will have its construction updated, default null")]
        [Output("panels", "The collection of Environment Panels with updated construction on the hosted openings")]
        public static List<Panel> SetOpeningConstruction(this List<Panel> panels, IConstruction newConstruction, List<string> typeNames = null)
        {
            foreach(Panel p in panels)
                p.Openings = p.Openings.SetConstructions(newConstruction, typeNames);

            return panels;
        }
    }
}
