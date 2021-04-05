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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.Geometry;
using BH.oM.MEP.System;
using BH.Engine.Spatial;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Returns the Hydraulic Diameter for any IProfile that are non-circular (ie the equivalent diameter for flow based equations if the duct were to be circular).")]
        [Input("profile", "IProfile used to define the element's cross section shape.")]
        [Output("hydraulicDiameter", "Hydraulic Diameter allows you to calculate the round equivalent hydraulic diameter for a non-round duct (rectangular/square).")]
        public static double HydraulicDiameter(this IProfile profile)
        {
            List<List<ICurve>> distCurves = Engine.Geometry.Compute.DistributeOutlines(Engine.Geometry.Compute.IJoin(profile.Edges.ToList()).Cast<ICurve>().ToList());
            List<ICurve> innerProfileEdges = new List<ICurve>();

            double area = profile.Area();

            foreach (List<ICurve> curves in distCurves)
            {
                innerProfileEdges.AddRange(curves.Skip(1));
            }
            return 4 * area / innerProfileEdges.Sum(x => x.ILength());
        }
        /***************************************************/

        [Description("Returns the Hydraulic Diameter for any Flow object that are non-circular (ie the equivalent diameter for flow based equations if the duct were to be circular).")]
        [Input("obj", "The object used to define the element's cross section shape.")]
        [Output("hydraulicDiameter", "Hydraulic Diameter allows you to calculate the round equivalent hydraulic diameter for a non-round duct (rectangular/square).")]
        public static double HydraulicDiameter(this IFlow obj)
        {
            ShapeType shapeType = obj.ElementSize.Shape;

            if (shapeType != ShapeType.Box)
            {
                BH.Engine.Reflection.Compute.RecordError("This Query only works with Box shaped elements.");
                return double.NaN;
            }
            else
            {
                // index 0 will always be ElementProfile
                IProfile profile = GetSectionProfiles(obj)[0];

                if (profile == null)
                {
                    BH.Engine.Reflection.Compute.RecordError("No profile was found for this object. Please provide an element size and section profile.");
                    return double.NaN;
                }

                double area = profile.Area();

                List<List<ICurve>> distCurves = Engine.Geometry.Compute.DistributeOutlines(Engine.Geometry.Compute.IJoin(profile.Edges.ToList()).Cast<ICurve>().ToList());
                List<ICurve> innerProfileEdges = new List<ICurve>();

                foreach (List<ICurve> curves in distCurves)
                {
                    innerProfileEdges.AddRange(curves.Skip(1));
                }
                return 4 * area / innerProfileEdges.Sum(x => x.ILength());
            }
        }
        /***************************************************/
    }
}

