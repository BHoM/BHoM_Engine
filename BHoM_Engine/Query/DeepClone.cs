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

using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using Force.DeepCloner;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Plane)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Point)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Vector)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.CoordinateSystem.Cartesian)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Arc)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Circle)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Ellipse)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Line)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.NurbsCurve)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.PolyCurve)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Polyline)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Extrusion)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Loft)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.NurbsSurface)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.PlanarSurface)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Pipe)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.PolySurface)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Mesh)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Face)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.BoundingBox)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.CompositeGeometry)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Quaternion)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.TransformMatrix)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.Basis)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.Clone(BH.oM.Geometry.SurfaceTrim)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.IClone(BH.oM.Geometry.IGeometry)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.IClone(BH.oM.Geometry.ICurve)")]
        [PreviousVersion("4.3", "BH.Engine.Geometry.Query.IClone(BH.oM.Geometry.ISurface)")]
        public static T DeepClone<T>(this T obj)
        {
            return DeepClonerExtensions.DeepClone(obj);
        }

        /***************************************************/
    }
}


