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
using BH.oM.Environment.Interface;

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
                BH.oM.Environment.Properties.ElementProperties props = be.ElementProperties() as BH.oM.Environment.Properties.ElementProperties;
                if (props != null)
                {
                    if(props.BuildingElementType == BuildingElementType.Shade)
                            shadingElements.Add(be);
                }
            }

            return shadingElements;
        }

        public static List<BuildingElement> ElementsByWallType(this List<BuildingElement> elements)
        {
            List<IBuildingObject> objs = elements.ConvertAll(x => (IBuildingObject)x).ToList();
            return objs.ObjectsByElementType(BuildingElementType.Wall).ConvertAll(x => (BuildingElement)x).ToList();
        }

        public static List<BuildingElement> ElementsByType(this List<BuildingElement> elements, BuildingElementType type)
        {
            List<IBuildingObject> objs = elements.ConvertAll(x => (IBuildingObject)x).ToList();
            return objs.ObjectsByElementType(type).ConvertAll(x => (BuildingElement)x).ToList();
        }

        public static List<BuildingElement> ElementsWithoutType(this List<BuildingElement> elements, BuildingElementType type)
        {
            List<IBuildingObject> objs = elements.ConvertAll(x => (IBuildingObject)x).ToList();
            return objs.ObjectsWithoutElementType(type).ConvertAll(x => (BuildingElement)x).ToList();
        }

        public static List<BuildingElement> ElementsByName(this List<BuildingElement> elements, string name)
        {
            List<IBuildingObject> objs = elements.ConvertAll(x => (IBuildingObject)x).ToList();
            return objs.ObjectsByName(name).ConvertAll(x => (BuildingElement)x).ToList();
        }

        public static List<BuildingElement> ElementsByTilt(this List<BuildingElement> elements, double tilt)
        {
            return elements.Where(x => x.Tilt() == tilt).ToList();
        }

        public static List<BuildingElement> ElementsByTiltRange(this List<BuildingElement> elements, double minTilt, double maxTilt)
        {
            return elements.Where(x => x.Tilt() >= minTilt && x.Tilt() <= maxTilt).ToList();
        }

        public static List<BuildingElement> ElementsByPoint(this List<BuildingElement> elements, Point pt)
        {
            return elements.Where(x => x.PanelCurve.IIsContaining(new List<Point> { pt }, true)).ToList();
        }

        public static List<BuildingElement> ElementsByGeometry(this List<BuildingElement> elements, ICurve geometry)
        {
            return elements.Where(x => x.PanelCurve.IDiscontinuityPoints().PointsMatch(geometry.IDiscontinuityPoints())).ToList();
        }

        public static List<BuildingElement> RemoveBuildingElement(this List<BuildingElement> elements, BuildingElement elementToRemove)
        {
            List<BuildingElement> rtnElements = new List<BuildingElement>(elements);
            rtnElements.Remove(elementToRemove);

            if (rtnElements.Count == elements.Count)
                rtnElements = elements.Where(x => x.BHoM_Guid != elementToRemove.BHoM_Guid).ToList(); //Back up in case the element isn't removed the first time

            return rtnElements;
        }

        public static List<BuildingElement> RemoveBuildingElements(this List<BuildingElement> elements, List<BuildingElement> elementsToRemove)
        {
            foreach(BuildingElement be in elementsToRemove)
                elements = elements.RemoveBuildingElement(be);

            return elements;
        }
    }
}

