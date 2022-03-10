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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Facade.Elements;
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Base;
using BH.oM.Facade.Fragments;
using BH.oM.Facade.Results;
using BH.oM.Facade.SectionProperties;
using BH.oM.Physical.FramingProperties;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Changes the depth of mullion represented as SectionProperties")]
        [Input("FrameEdgeProperty", "FrameEdgeProperty with SectionProperties to modify and FrameExtensionBox fragment")]
        [Input("MullionDepth", "New depth for mullion. Measured from interior face of glazing.")]
        [Output("FrameEdgeProperty", "FrameEdgeProperty with modified SectionProperties depth")] //UPDATE
        public static IProfile ExtendProfile(this IProfile profile, ICurve extBox, double extDist)
        {
            List<ICurve> profileCrv = new List<ICurve>(profile.Edges);
            Vector vector = BH.Engine.Geometry.Create.Vector(extDist, 0, 0);

            foreach (ICurve crv in profileCrv)
            {
                List<Point> intersectionPts = Geometry.Query.ICurveIntersections(crv, extBox);

                if(intersectionPts.Count >= 1)
                {
                    if (!Geometry.Query.IIsLinear(crv))
                    {
                        BH.Engine.Base.Compute.RecordError($"Extension cannot be performed on non-linear profile edge.");
                        return profile;
                    }
                    Point endPt = Geometry.Query.IEndPoint(crv);
                    Point strtPt = Geometry.Query.IStartPoint(crv);
                    if(endPt.X >= strtPt.X)
                    {
                        Geometry.Modify.Translate(endPt, vector);
                    }
                    else
                    {
                        Geometry.Modify.Translate(strtPt, vector);
                    }
                }
                else if (Geometry.Query.IIsContaining(extBox, crv)) //Why isn't this running with ICurves like stated
                {
                    Geometry.Modify.ITranslate(crv, vector);
                }
            }
            IProfile newProfile = BH.Engine.Spatial.Create.FreeFormProfile(profileCrv,false);
            return newProfile;
        }
    }
}




