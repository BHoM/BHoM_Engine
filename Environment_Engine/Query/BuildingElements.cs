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
using BH.oM.Environment.Elements;

using BH.oM.Base;

using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<BuildingElement> BuildingElements(this List<IBHoMObject> bhomObjects)
        {
            List<BuildingElement> bes = new List<BuildingElement>();

            foreach (IBHoMObject obj in bhomObjects)
            {
                if (obj is BuildingElement)
                    bes.Add(obj as BuildingElement);
            }

            return bes;
        }

        public static List<BuildingElement> UniqueBuildingElements(this List<List<BuildingElement>> elements)
        {
            List<BuildingElement> rtn = new List<BuildingElement>();

            foreach (List<BuildingElement> lst in elements)
            {
                foreach (BuildingElement be in lst)
                {
                    BuildingElement beInList = rtn.Where(x => x.BHoM_Guid == be.BHoM_Guid).FirstOrDefault();
                    if (beInList == null)
                        rtn.Add(be);
                }
            }

            return rtn;
        }

        public static List<BuildingElement> ShadingElements(this List<BuildingElement> elements)
        {
            //Isolate all of the shading elements in the list - shading elements are ones connected only along one edge
            List<BuildingElement> shadingElements = new List<BuildingElement>();

            foreach (BuildingElement be in elements)
            {
                if (be.BuildingElementProperties != null)
                {
                    if (be.BuildingElementProperties.CustomData.ContainsKey("SAM_BuildingElementType"))
                    {
                        object aObject = be.BuildingElementProperties.CustomData["SAM_BuildingElementType"];

                        if (aObject != null && aObject.ToString().ToLower().Contains("shade"))
                            shadingElements.Add(be);
                            
                    }
                }
            }

            return shadingElements;
        }

        public static List<BuildingElement> ElementsByType(this List<BuildingElement> elements, BuildingElementType type)
        {
            return elements.Where(x => x.BuildingElementProperties.BuildingElementType == type).ToList();
        }

        public static List<BuildingElement> ElementsByWallType(this List<BuildingElement> elements)
        {
            return elements.ElementsByType(BuildingElementType.Wall);
        }

    }
}

