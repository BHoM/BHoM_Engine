/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static GeometricalRepresentation Representation(Vector v, VectorRepresentationOptions vectOpts)
        {
            List<ICurve> arrow = new List<ICurve>();

            Point pt = vectOpts.BasePoint;

            pt = pt + v * vectOpts.Shift;
            Point end = pt + v;

            arrow.Add(Create.Line(pt, end));

            double length = v.Length();

            Vector norm = v / length;

            Vector v1 = Vector.XAxis;

            double dot = v1.DotProduct(norm);

            if (Math.Abs(1 - Math.Abs(dot)) < Tolerance.Angle)
            {
                v1 = Vector.YAxis;
                dot = v1.DotProduct(norm);
            }

            v1 = (v1 - dot * norm).Normalise();

            Vector v2 = v1.CrossProduct(norm).Normalise();

            v1 /= 2;
            v2 /= 2;

            double factor = length / 10;

            int m = 0;

            while (m < 1)
            {
                arrow.Add(Engine.Geometry.Create.Line(pt, (v1 + norm) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (-v1 + norm) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (v2 + norm) * factor));
                arrow.Add(Engine.Geometry.Create.Line(pt, (-v2 + norm) * factor));

                pt = pt + norm * factor;
                m++;
            }

            CompositeGeometry compositeGeometry = new CompositeGeometry() { Elements = arrow.OfType<IGeometry>().ToList() };
            GeometricalRepresentation repr = new GeometricalRepresentation() { Geometry = compositeGeometry };
            return repr;
        }

        /***************************************************/

        private static GeometricalRepresentation ArcArrow(Point pt, Vector v)
        {
            List<ICurve> arrow = new List<ICurve>();

            double length = v.Length();

            Vector cross;
            if (v.IsParallel(Vector.ZAxis) == 0)
                cross = Vector.ZAxis;
            else
                cross = Vector.YAxis;

            Vector yAxis = v.CrossProduct(cross);
            Vector xAxis = yAxis.CrossProduct(v);

            double pi4over3 = Math.PI * 4 / 3;
            Arc arc = Engine.Geometry.Create.Arc(Engine.Geometry.Create.CartesianCoordinateSystem(pt, xAxis, yAxis), length / pi4over3, 0, pi4over3);

            arrow.Add(arc);

            Vector tan = -arc.EndDir();

            Vector v1 = Vector.XAxis;

            double dot = v1.DotProduct(tan);

            if (Math.Abs(1 - dot) < Tolerance.Angle)
            {
                v1 = Vector.YAxis;
                dot = v1.DotProduct(tan);
            }

            v1 = (v1 - dot * tan).Normalise();

            Vector v2 = v1.CrossProduct(tan).Normalise();

            v1 /= 2;
            v2 /= 2;

            double factor = length / 10;

            pt = arc.EndPoint();

            arrow.Add(Engine.Geometry.Create.Line(pt, (v1 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (-v1 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (v2 + tan) * factor));
            arrow.Add(Engine.Geometry.Create.Line(pt, (-v2 + tan) * factor));

            CompositeGeometry compositeGeometry = new CompositeGeometry() { Elements = arrow.OfType<IGeometry>().ToList() };
            GeometricalRepresentation repr = new GeometricalRepresentation() { Geometry = compositeGeometry };
            return repr;
        }
    }
}
