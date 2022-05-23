/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a BoundaryCurve from a set of curves defining its shape. The curves are required to be co-planar and form a non self-intersecting closed loop. Sub-parts of the incoming curves will be extracted, and joined to ensure the validity of the inputs. This means the order of the Curves on the BoundaryCurve might be different than the input.")]
        [Input("curves", "Set of Curves forming a closed, planar, non self-intersecting loop. If not a valid loop, null is returned.")]
        [Input("tolerance", "Tolerance used for checking the validity of the incoming curves.", typeof(Length))]
        [Output("boundaryCurve", "The created BoundaryCurve.")]
        public static BoundaryCurve BoundaryCurve(List<ICurve> curves, double tolerance = Tolerance.Distance)
        {
            if (curves.IsNullOrEmpty())
                return null;

            if (curves.Count == 1 && (curves[0] is Circle || curves[0] is Ellipse)) //Special case of single circle or ellipse
                return new BoundaryCurve(curves);

            //Try joining all segments into a single polycurve
            List<ICurve> subParts = curves.SelectMany(x => x.ISubParts()).ToList();

            if (subParts.Any(x => x is NurbsCurve))
            {
                Engine.Base.Compute.RecordError("Input curves contaisn NubrsCurves which cannot be validated. BoundaryCurve not created.");
                return null;
            }


            List<PolyCurve> joinedCurves = Compute.IJoin(curves.SelectMany(x => x.ISubParts()).ToList(),tolerance);

            if (joinedCurves.Count != 1)
            {
                Engine.Base.Compute.RecordError("Provided curves are not joined. Unable to create BoundaryCurve.");
                return null;
            }

            PolyCurve polyCurve = joinedCurves[0];

            if (!polyCurve.IsClosed(tolerance))
            {
                Engine.Base.Compute.RecordError("Provided curves does not form a closed loop. Unable to create BoundaryCurve.");
                return null;
            }

            if (!polyCurve.IsPlanar(tolerance))
            {
                Engine.Base.Compute.RecordError("Provided curves are not co-planar. Unable to create BoundaryCurve.");
                return null;
            }

            if (polyCurve.IsSelfIntersecting(tolerance))
            {
                Engine.Base.Compute.RecordError("Provided curves are self-intersecting or intersecting each other. Unable to create BoundaryCurve.");
                return null;
            }

            return new BoundaryCurve(polyCurve.Curves);
        }

        /***************************************************/
    }
}
