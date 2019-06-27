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

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node SetGeometry(this Node node, Point point)
        {
            Node clone = node.GetShallowClone(true) as Node;
            clone.Position = point;
            return clone;
        }

        /***************************************************/

        public static Bar SetGeometry(this Bar bar, ICurve curve)
        {
            Line line = curve.IClone() as Line;
            if (line == null)
            {
                Reflection.Compute.RecordError("The bar needs to be linear.");
                return null;
            }

            Bar clone = bar.GetShallowClone(true) as Bar;
            clone.StartNode = clone.StartNode.SetGeometry(line.Start);
            clone.EndNode = clone.EndNode.SetGeometry(line.End);
            return clone;
        }

        /***************************************************/

        public static Edge SetGeometry(this Edge edge, ICurve curve)
        {
            Edge clone = edge.GetShallowClone(true) as Edge;
            clone.Curve = curve.IClone();
            return clone;
        }

        /***************************************************/

        public static Surface SetGeometry(this Surface contour, ISurface surface)
        {
            Surface clone = contour.GetShallowClone(true) as Surface;
            clone.Extents = surface.IClone() as ISurface;
            return clone;
        }

        /***************************************************/
    }
}
