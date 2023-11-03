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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Physical.Reinforcement;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Physical.FramingProperties;
using BH.Engine.Spatial;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether the Piles are within the extents defined by the PadFoundation (both depth and extents). Piles located on the boundary are accepted within tolerance" +
            "if their profile does not intersect with the pad foundation boundary.")]
        [Input("padFoundation", "The PadFoundation that defines the extents for the Piles to be located.")]
        [Input("piles", "The Piles to check if they are located within the extents of the PadFoundation.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Piles are deemed to be on the edge of the PadFoundation if they are within this distance from the curve.", typeof(Length))]
        [Output("withinPileCap", "True if all Piles are contained within the boundary and depth of the PileCap. False if one or more Piles are not.")]
        public static bool IsWithinPileCap(this PadFoundation padFoundation, List<Pile> piles, double tolerance = Tolerance.Distance)
        {
            if (padFoundation.IsNull())
                return false;
            else if (piles.IsNullOrEmpty())
                return false;
            else if (piles.Any(x => x.IsNull()))
                return false;
            else if (padFoundation.Construction.IThickness() == 0)
            {
                Base.Compute.RecordError("The PadFoundation does not contain a thickness, the method will return false.");
                return false;
            }

            // Get the top of the piles
            List<Point> tops = piles.Select(p => p.Location.IControlPoints().OrderBy(pt => pt.Z).Last()).ToList();

            // Project all points to top surface of the pad
            Plane padTop = padFoundation.Location.FitPlane();
            List<Point> pTops = tops.Select(x => x.Project(padTop)).ToList(); //project points only used for IsContaining

            // Get the thickness of the PadFoundation to compare against Z coordinates of the pile tops
            double maxZ = padFoundation.Location.Centroid().Z;
            double minZ = maxZ - padFoundation.Construction.IThickness();

            ICurve topOutline = padFoundation.Location.ExternalBoundary;

            // Check if all piles are within the curve of the pad
            if (topOutline.IIsContaining(pTops, true, tolerance))
            {
                // Check if the piles are within the depth of the pad
                if (tops.Any(pt => pt.Z > maxZ || pt.Z < minZ))
                {
                    Base.Compute.RecordError("One or more the Piles tops is not located within the depth of the PileCap.");
                    return false;
                }
                else
                {
                    // Check if the profile pushes the pile outside the curve of the pad
                    for (int i = 0; i < pTops.Count; i++)
                    {
                        ConstantFramingProperty property = (ConstantFramingProperty)piles[i].Property;
                        ICurve profile = Engine.Geometry.Compute.IJoin(property.Profile.Edges.ToList()).OrderBy(c => c.Length()).Last()
                            .ITranslate(Engine.Geometry.Create.Vector(pTops[i])).IRotate(pTops[i], Vector.ZAxis, property.OrientationAngle);
                        
                        
                        if (profile.ICurveIntersections(topOutline).Count > 0)
                        {
                            Base.Compute.RecordError("One or more the Pile profiles is located outside the edge of the pile cap.");
                            return false;
                        }
                    }
                }
            }
            else
            {
                Base.Compute.RecordError("One or more the Piles centre points is located outside the extents of the PileCap.");
                return false;
            }

            return true;
        }

        /***************************************************/

    }
}

