/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Geometry;
using BH.oM.Graphics.Components;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Queries a set of Marker curves from the provided marker as well as endpoint and direction.")]
        [Input("end", "Point where the marker is attached.")]
        [Input("direction", "Vector representing the direction of the marker.")]
        [Output("marker curves", "Colection of curves to represent the marker.")]
        public static List<ICurve> IMarker(this IMarker marker, Point end, Vector direction)
        {
            if (marker == null || end == null || direction == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a marker if the basic arrow marker, end attachment point, or direction vector are null.");
                return new List<ICurve>();
            }

            return Marker(marker as dynamic, end , direction);
        }

        /***************************************************/

        [Description("Queries a set of Marker arrows from the provided marker as well as endpoint and direction.")]
        [Input("end", "Point where the marker is attached.")]
        [Input("direction", "Vector representing the direction of the marker.")]
        [Output("marker curves", "Collection of curves to represent the marker.")]
        public static List<ICurve> Marker(this BasicArrowMarker marker, Point end, Vector direction)
        {
            if(marker == null || end == null || direction == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create a marker if the basic arrow marker, end attachment point, or direction vector are null.");
                return new List<ICurve>();
            }

            direction.Normalise();
            Vector back = direction.Reverse() * marker.HeadLength;
            Vector perp = back.CrossProduct(Vector.ZAxis);
            if (perp.Length() == 0)
                perp = back.CrossProduct(Vector.YAxis);
            perp = perp.Normalise();

            perp = perp * marker.BaseWidth;

            Point p1 = end + (back + perp);
            Point p2 = end + (back - perp);

            List<ICurve> head = new List<ICurve>();
            head.Add(Engine.Geometry.Create.Line(p1, end));
            head.Add(Engine.Geometry.Create.Line(p2, end));

            if (marker.Closed)
                head.Add(Engine.Geometry.Create.Line(p1, p2));

            return head;
        }

        /***************************************************/

        public static List<ICurve> Marker(this IMarker markerr, Point end, Vector direction)
        {
            return new List<ICurve>();
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
    }
}




