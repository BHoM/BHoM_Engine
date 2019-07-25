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
using BH.oM.Environment.Fragments;
using BH.oM.Base;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.Engine.Base;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a collection of Environment Spaces from a list of generic BHoM objects")]
        [Input("bhomObjects", "A collection of generic BHoM objects")]
        [Output("spaces", "A collection of Environment Space objects")]
        public static List<Space> Spaces(this List<IBHoMObject> bhomObjects)
        {
            bhomObjects = bhomObjects.ObjectsByType(typeof(Space));
            List<Space> spaces = new List<Space>();
            foreach (IBHoMObject o in bhomObjects)
                spaces.Add(o as Space);

            return spaces;
        }

        [Description("Returns a collection of Environment Spaces that match the given element ID")]
        [Input("spaces", "A collection of Environment Spaces")]
        [Input("elementID", "The Element ID to filter by")]
        [Output("spaces", "A collection of Environment Space objects that match the element ID")]
        public static List<Space> SpacesByElementID(this List<Space> spaces, string elementID)
        {
            List<IEnvironmentObject> envObjects = new List<IEnvironmentObject>();
            foreach (Space s in spaces)
                envObjects.Add(s as IEnvironmentObject);

            envObjects = envObjects.ObjectsByFragment(typeof(OriginContextFragment));

            envObjects = envObjects.Where(x => (x.Fragments.Where(y => y.GetType() == typeof(OriginContextFragment)).FirstOrDefault() as OriginContextFragment).ElementID == elementID).ToList();

            List<Space> rtnSpaces = new List<Space>();
            foreach (IEnvironmentObject o in envObjects)
                rtnSpaces.Add(o as Space);

            return rtnSpaces;
        }

        [Description("Returns a collection of Environment Spaces that match the given name")]
        [Input("spaces", "A collection of Environment Spaces")]
        [Input("name", "The name to filter by")]
        [Output("spaces", "A collection of Environment Space objects that match the name")]
        public static List<Space> SpaceByName(this List<Space> spaces, string name)
        {
            return spaces.Where(x => x.Name == name).ToList();
        }

        [Description("Returns a nested collection of Environment Panels that represent fully closed spaces")]
        [Input("panelsAsSpaces", "A nested collection of Environment Panels representing spaces")]
        [Output("panelsAsSpaces", "A nested collection of Environment Panels that represent fully closed spaces")]
        public static List<List<Panel>> ClosedSpaces(this List<List<Panel>> panelsAsSpaces)
        {
            List<List<Panel>> closedSpaces = new List<List<Panel>>();

            foreach (List<Panel> space in panelsAsSpaces)
            {
                if (space.IsClosed())
                    closedSpaces.Add(space);
            }

            return closedSpaces;
        }

        [Description("Returns a nested collection of Environment Panels that represent spaces which aren't fully closed")]
        [Input("panelsAsSpaces", "A nested collection of Environment Panels representing spaces")]
        [Output("panelsAsSpaces", "A nested collection of Environment Panels that represent spaces which aren't fully closed")]
        public static List<List<Panel>> NotClosedSpaces(this List<List<Panel>> panelsAsSpaces)
        {
            List<List<Panel>> spacesNotClosed = new List<List<Panel>>();

            foreach (List<Panel> space in panelsAsSpaces)
            {
                if (!space.IsClosed())
                    spacesNotClosed.Add(space);
            }

            return spacesNotClosed;
        }
    }
}