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

using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns arrow geometry used to represent a Relation.")]
        [Input("relation", "The IRelation to represent with geometry.")]
        [Input("headLength", "Optional length of the arrow head. Default is 1.")]
        [Input("baseWidth", "Optional width of the arrow head. Default is 0.3.")]
        [Input("closed", "Optional boolean to set if the arrow head is closed or open. Default is false.")]
        [Output("composite geometry", "CompositeGeometry of the arrow.")]
        public static CompositeGeometry RelationArrow(this IRelation relation, double headLength = 1, double baseWidth = 0.3, bool closed = false)
        {
            List<IGeometry> geometries = new List<IGeometry>();

            if (headLength == 0)
            {
                Base.Compute.RecordWarning("Unable to create arrow with headLength of zero.");
                return BH.Engine.Geometry.Create.CompositeGeometry(geometries);
            }
                

            ICurve curve = relation.Curve;
            geometries.Add(curve);
            if (curve is NurbsCurve)
            {
                NurbsCurve nurbsCurve = curve as NurbsCurve;
                curve = Engine.Geometry.Create.Polyline(nurbsCurve.ControlPoints);
            }
            List<ICurve> nonNurbs = new List<ICurve>();

            foreach (ICurve sub in curve.ISubParts())
            {
                if (sub is NurbsCurve)
                {
                    NurbsCurve nurbsCurve = sub as NurbsCurve;
                    nonNurbs.Add(Engine.Geometry.Create.Polyline(nurbsCurve.ControlPoints));
                }
                else
                {
                    nonNurbs.Add(sub);
                }
            }

            foreach (ICurve c in nonNurbs)
            {
                double length = c.ILength() - headLength;
                geometries.Add(ArrowHead(c.IPointAtLength(length), c.IEndPoint(), baseWidth, closed));
            }
                
            return BH.Engine.Geometry.Create.CompositeGeometry(geometries);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Polyline ArrowHead(Point start, Point end, double width = 0.3, bool closed = false)
        {
            Vector back = start - end;
            Vector perp = back.CrossProduct(Vector.ZAxis);
            if (perp.Length() == 0)
                perp = back.CrossProduct(Vector.YAxis);
            perp = perp.Normalise();

            perp = perp * width;

            Point p1 = end + (back + perp);
            Point p2 = end + (back - perp);

            Polyline head = new Polyline();
            head.ControlPoints.Add(p1);
            head.ControlPoints.Add(end);
            head.ControlPoints.Add(p2);
            
            if (closed)
                head.ControlPoints.Add(p1);

            return head;
        }

        /***************************************************/
    }
}




