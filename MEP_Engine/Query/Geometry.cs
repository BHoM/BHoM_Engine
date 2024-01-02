/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.MEP.System;
using BH.oM.MEP.Fixtures;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using BH.oM.MEP.System.Fittings;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /****             Public Methods                ****/
        /***************************************************/
        
        [Description("Queries an IFlow object for its geometry.")]
        [Input("flowObj", "The object to query geometry from.")]
        [Output("geometry", "The geometry queried from the object.")]
        public static IGeometry Geometry(this IFlow flowObj)
        {
            if (flowObj?.StartPoint == null || flowObj?.EndPoint == null)
                return null;
            else
                return new Line { Start = flowObj.StartPoint, End = flowObj.EndPoint};
        }

        /***************************************************/

        [Description("Queries a Fitting object for its geometry.")]
        [Input("fitting", "The object to query geometry from.")]
        [Output("geometry", "The geometry queried from the object.")]
        public static List<Line> Geometry(this Fitting fitting)
        {
            if (fitting == null)
                return null;
            
            List<Line> result = new List<Line>();
            if (fitting.ConnectionsLocation.Count == 2)
                result.Add(BH.Engine.Geometry.Create.Line(fitting.ConnectionsLocation[0], fitting.ConnectionsLocation[1]));
            
            else
            {
                foreach (Point point in fitting.ConnectionsLocation)
                {
                    result.Add(BH.Engine.Geometry.Create.Line(fitting.Location, point));
                }
            }

            return result;
        }
        
        /***************************************************/
    }
}



