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

using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Structure.Properties.Section;
using BH.oM.Structure.Properties.Constraint;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Bar Bar(Line line, ISectionProperty property = null, double orientationAngle = 0, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural, string name = "")
        {          
            return Bar(new Node { Position = line.Start }, new Node { Position = line.End }, property, orientationAngle, release, feaType, name);
        }

        /***************************************************/

        public static Bar Bar(Node startNode, Node endNode, ISectionProperty property = null, double orientationAngle = 0, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural,  string name = "")
        {
            return new Bar
            {
                Name = name,
                StartNode = startNode,
                EndNode = endNode,
                SectionProperty = property,
                Release = release == null ? BarReleaseFixFix() : release,
                FEAType = feaType,
                OrientationAngle = orientationAngle
            };
        }


        /***************************************************/

        public static Bar Bar(Line line, ISectionProperty property = null, Vector normal =  null, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural, string name = "")
        {
            double orientationAngle;

            if (normal == null)
                orientationAngle = 0;
            else
            {
                normal = normal.Normalise();
                Vector tan = (line.End - line.Start).Normalise();

                double dot = normal.DotProduct(tan);

                if (Math.Abs(1 - dot) < oM.Geometry.Tolerance.Angle)
                {
                    Reflection.Compute.RecordError("The normal is parallell to the centreline of the bar");
                    return null;
                }
                else if (Math.Abs(dot) > oM.Geometry.Tolerance.Angle)
                {
                    Reflection.Compute.RecordWarning("Normal not othogonal to the centreline and will get projected");
                }

                Vector reference;

                if (!line.IsVertical())
                    reference = Vector.ZAxis;
                else
                {
                    reference = tan.CrossProduct(Vector.YAxis);
                }

                orientationAngle = reference.Angle(normal, new Plane { Normal = tan });

            }
                

            return Bar(new Node { Position = line.Start }, new Node { Position = line.End }, property, orientationAngle, release, feaType, name);
        }

        /***************************************************/
    }
}
