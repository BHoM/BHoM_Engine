/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using BH.oM.Environment;
using BH.Engine.Geometry;
using BH.oM.Geometry;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the range of altitude of an Environment Object taken as the maximum z value minus minimum z value from the bounding box of the geometry")]
        [Input("environmentObject", "Any object implementing the IEnvironmentObject interface that can have an altitude range")]
        [Output("altitudeRange", "The altitude range of the object")]
        public static double AltitudeRange(this IEnvironmentObject environmentObject)
        {
            if(environmentObject == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the altitude range of a null environment object.");
                return -1;
            }

            BoundingBox panelBoundingBox = BH.Engine.Geometry.Query.IBounds(environmentObject.Polyline());
            double altitudeRange = panelBoundingBox.Max.Z - panelBoundingBox.Min.Z;

            return altitudeRange;
        }

        /***************************************************/
    }
}






