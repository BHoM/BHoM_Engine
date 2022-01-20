/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Constraints;
using BH.Engine.Geometry;
using BH.Engine.Reflection;
using BH.Engine.Spatial;

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Bar element from a Line, to be used as centre line of the Bar, and properties.")]
        [Input("line", "Geometrical Line used as centreline of the Bar. The StartNode and EndNode of the bar will be extracted from the Line.")]
        [InputFromProperty("sectionProperty")]
        [InputFromProperty("orientationAngle")]
        [InputFromProperty("release")]
        [InputFromProperty("feaType", "FEAType")]
        [Input("name", "The name of the created Bar.")]
        [Output("bar", "The created Bar with a centreline matching the provided geometrical Line.")]
        public static Bar Bar(Line line, ISectionProperty sectionProperty = null, double orientationAngle = 0, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural, string name = "")
        {
            return line.IsNull() ? null : new Bar
            {
                Name = name,
                StartNode = (Node)line.Start,
                EndNode = (Node)line.End,
                SectionProperty = sectionProperty,
                Release = release == null ? BarReleaseFixFix() : release,
                FEAType = feaType,
                OrientationAngle = orientationAngle
            };
        }

        /***************************************************/

        [Description("Creates a Bar element from a Line, to be used as centre line of the Bar, and properties.")]
        [Input("line", "Geometrical Line used as centreline of the Bar. The StartNode and EndNode of the bar will be extracted from the Line.")]
        [InputFromProperty("sectionProperty")]
        [Input("normal", "Vector to be used as normal of the Bar. This vector should generally be orthogonal to the Bar, if it is not, it will be made orthogonal by projecting it to the section plane of the Bar (a plane that has that Bar tangent as its normal). This means that the Normal cannot be paralell to the Tangent of the Bar. \n" +
                         "Vector will be used to determain the orientation angle of the Bar. This is done by measuring the counter clockwise angle in the section plane of the Bar between a reference Vector and the provided Vector. For a non-vertical Bar, the reference vector will be the global Z-axis. For a vertical bar the reference vector will be a vector that is orthogonal to the tangent vector of the Bar and the global Y-axis.")]
        [InputFromProperty("release")]
        [InputFromProperty("feaType", "FEAType")]
        [Input("name", "The name of the created Bar.")]
        [Output("bar", "The created Bar with a centreline matching the provided geometrical Line.")]
        public static Bar Bar(Line line, ISectionProperty sectionProperty = null, Vector normal = null, BarRelease release = null, BarFEAType feaType = BarFEAType.Flexural, string name = "")
        {
            double orientationAngle = normal.OrientationAngleLinear(line);

            if (double.IsNaN(orientationAngle))
                return null;

            return Bar(line, sectionProperty, orientationAngle, release, feaType, name);
        }

        /***************************************************/
    }
}




