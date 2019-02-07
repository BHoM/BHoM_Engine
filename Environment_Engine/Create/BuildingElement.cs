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

using System.Collections.Generic;

using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties properties)
        {
            return new BuildingElement
            {
                BuildingElementProperties = properties,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(ICurve panelCurve)
        {
            return new BuildingElement
            {
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(Opening opening)
        {
            return BuildingElement(new List<Opening> { opening });
        }

        /***************************************************/

        public static BuildingElement BuildingElement(List<Opening> openings)
        {
            return new BuildingElement
            {
                Openings = openings,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(ICurve crv, Opening opening)
        {
            return BuildingElement(new List<ICurve> { crv }, new List<Opening> { opening });
        }

        /***************************************************/

        public static BuildingElement BuildingElement(IEnumerable<ICurve> curves, List<Opening> openings)
        {
            return new BuildingElement
            {
                PanelCurve = BH.Engine.Geometry.Create.PolyCurve(curves),
                Openings = openings,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, IEnumerable<Polyline> panelPolyLines)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                PanelCurve = Geometry.Create.PolyCurve(panelPolyLines),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(BuildingElementProperties buildingElementProperties, ICurve panelCurve)
        {
            return new BuildingElement
            {
                BuildingElementProperties = buildingElementProperties,
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(IEnumerable<ICurve> panelCurves)
        {
            return new BuildingElement
            {
                PanelCurve = Geometry.Create.PolyCurve(panelCurves),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(PolyCurve panelCurve)
        {
            return new BuildingElement
            {
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(IEnumerable<Polyline> panelPolyLines)
        {
            return new BuildingElement
            {
                PanelCurve = Geometry.Create.PolyCurve(panelPolyLines),
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(Polyline panelPolyLine)
        {
            return new BuildingElement
            {
                PanelCurve = panelPolyLine,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(string name, ICurve panelCurve, BuildingElementProperties properties)
        {
            return new BuildingElement
            {
                Name = name,
                PanelCurve = panelCurve,
                BuildingElementProperties = properties,
            };
        }

        /***************************************************/

        public static BuildingElement BuildingElement(ICurve crv, Opening opening, BuildingElementProperties properties)
        {
            return BuildingElement(crv, new List<Opening> { opening }, properties);
        }

        /***************************************************/

        public static BuildingElement BuildingElement(ICurve crv, List<Opening> openings, BuildingElementProperties properties)
        {
            return new BuildingElement
            {
                PanelCurve = crv,
                Openings = openings,
                BuildingElementProperties = properties,
            };
        }
    }
}
