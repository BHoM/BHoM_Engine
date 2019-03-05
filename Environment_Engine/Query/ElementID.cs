/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Environment.Properties;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Opening OpeningByElementID(this List<BuildingElement> elements, string openingElementID)
        {
            foreach (BuildingElement be in elements)
            {
                foreach (Opening o in be.Openings)
                {
                    if ((o.EnvironmentContextProperties() as EnvironmentContextProperties) != null && (o.EnvironmentContextProperties() as EnvironmentContextProperties).ElementID == openingElementID)
                        return o;
                }
            }

            return null;
        }

        /***************************************************/

        public static Opening OpeningByElementID(this List<List<BuildingElement>> elementsAsSpaces, string openingElementID)
        {
            return elementsAsSpaces.SelectMany(x => x).ToList().OpeningByElementID(openingElementID);
        }

        /***************************************************/

        public static BuildingElement BuildingElementByOpeningElementID(this List<BuildingElement> elements, string openingElementID)
        {
            foreach (BuildingElement be in elements)
            {
                foreach (Opening o in be.Openings)
                {
                    if ((o.EnvironmentContextProperties() as EnvironmentContextProperties) != null && (o.EnvironmentContextProperties() as EnvironmentContextProperties).ElementID == openingElementID)
                        return be;
                }
            }

            return null;
        }

        /***************************************************/

        public static BuildingElement BuildingElementByOpeningElementID(this List<List<BuildingElement>> elementsAsSpaces, string openingElementID)
        {
            return elementsAsSpaces.SelectMany(x => x).ToList().BuildingElementByOpeningElementID(openingElementID);
        }

        /***************************************************/

        public static BuildingElement BuildingElementByElementID(this List<BuildingElement> elements, string elementID)
        {
            foreach(BuildingElement be in elements)
            {
                if ((be.EnvironmentContextProperties() as EnvironmentContextProperties) != null && (be.EnvironmentContextProperties() as EnvironmentContextProperties).ElementID == elementID)
                    return be;
            }

            return null;
        }

        /***************************************************/

        public static BuildingElement BuildingElementByElementID(this List<List<BuildingElement>> elementsAsSpaces, string elementID)
        {
            return elementsAsSpaces.SelectMany(x => x).ToList().BuildingElementByElementID(elementID);
        }

        /***************************************************/

        public static List<BuildingElement> BuildingElementsByElementID(this List<BuildingElement> elements, string elementID)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            foreach (BuildingElement be in elements)
            {
                if ((be.EnvironmentContextProperties() as EnvironmentContextProperties) != null && (be.EnvironmentContextProperties() as EnvironmentContextProperties).ElementID == elementID)
                    rtn.Add(be);
            }

            return rtn;
        }

        /***************************************************/

        public static List<BuildingElement> BuildingElementsByElementID(this List<List<BuildingElement>> elementsAsSpaces, string elementID)
        {
            return elementsAsSpaces.SelectMany(x => x).ToList().BuildingElementsByElementID(elementID);
        }

        /***************************************************/

        public static Space SpaceByElementID(this List<Space> spaces, string elementID)
        {
            foreach(Space s in spaces)
            {
                if ((s.EnvironmentContextProperties() as EnvironmentContextProperties) != null && (s.EnvironmentContextProperties() as EnvironmentContextProperties).ElementID == elementID)
                    return s;
            }

            return null;
        }
    }
}
