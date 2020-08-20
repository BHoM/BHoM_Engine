/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Base;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static Panel SetLocalOrientation(this Panel panel, Vector localX)
        {
            Panel clone = panel.GetShallowClone() as Panel;
            Vector normal = Engine.Spatial.Query.Normal(panel);

            double dot = normal.DotProduct(localX);

            if (Math.Abs(1 - dot) < oM.Geometry.Tolerance.Angle)
            {
                Reflection.Compute.RecordError("The provided localX is parallel to the normal of the Panel. The local orientation could not be updated.");
                return null;
            }
            else if (Math.Abs(dot) > oM.Geometry.Tolerance.Angle)
            {
                Reflection.Compute.RecordWarning("The provided localX in the Plane of the panel and will get projected");
                localX = localX.Project(new Plane { Normal = normal });
            }

            Vector refVec;

            if (normal.IsParallel(Vector.XAxis) == 0)
            {
                //Normal is not paralell to the global X-axis
                refVec = Vector.XAxis;
            }
            else
            {
                //Normal _is_ paralell to the global X-axis
                refVec = Vector.YAxis.CrossProduct(normal);
            }

            clone.OrientationAngle = refVec.Angle(localX, new Plane { Normal = normal });

            return clone;
        }

        /***************************************************/
    }
}
