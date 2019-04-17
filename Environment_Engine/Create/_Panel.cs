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
using BH.oM.Environment.Properties;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Panel Panel(ICurve panelCurve, IEnumerable<Opening> openings)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
                Openings = openings.ToList(),
            };
        }

        /***************************************************/

        public static Panel Panel(ICurve panelCurve, PanelProperties properties)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
                PanelProperties = properties,
            };
        }

        /***************************************************/

        public static Panel Panel(ICurve panelCurve, PanelProperties properties, Opening opening)
        {
            return Panel(panelCurve, properties, new List<Opening> { opening });
        }

        /***************************************************/

        public static Panel Panel(ICurve panelCurve, PanelProperties properties, IEnumerable<Opening> openings)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
                PanelProperties = properties,
                Openings = openings as List<Opening>,
            };
        }

        /***************************************************/

        public static Panel Panel(PanelProperties properties)
        {
            return new Panel
            {
                PanelProperties = properties,
            };
        }

        /***************************************************/

        public static Panel Panel(Opening opening)
        {
            return Panel(new List<Opening> { opening });
        }

        /***************************************************/

        public static Panel Panel(IEnumerable<Opening> openings)
        {
            return new Panel
            {
                Openings = openings as List<Opening>,
            };
        }

        /***************************************************/

        public static Panel Panel(ICurve panelCurve)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static Panel Panel(IEnumerable<ICurve> panelCurves)
        {
            return new Panel
            {
                PanelCurve = Geometry.Create.PolyCurve(panelCurves),
            };
        }

        /***************************************************/

        public static Panel Panel(PolyCurve panelCurve)
        {
            return new Panel
            {
                PanelCurve = panelCurve,
            };
        }

        /***************************************************/

        public static Panel Panel(IEnumerable<Polyline> panelPolylines)
        {
            return new Panel
            {
                PanelCurve = Geometry.Create.PolyCurve(panelPolylines),
            };
        }

        /***************************************************/

        public static Panel Panel(Polyline panelPolyline)
        {
            return new Panel
            {
                PanelCurve = panelPolyline,
            };
        }

        /***************************************************/
    }
}
