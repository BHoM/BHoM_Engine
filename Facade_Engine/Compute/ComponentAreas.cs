/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Analytical.Elements;
using BH.oM.Facade.Elements;
using BH.oM.Facade.SectionProperties;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Facade
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns frame and clear opening areas for an Opening.")]
        [Input("opening", "Opening to find areas for.")]
        [MultiOutput(0, "openingArea", "Area of the portion of the opening not covered by the frame as per a projected elevation of the opening.")]
        [MultiOutput(1, "frameArea", "Adjacent Elements per adjacent edge")]
        public static Output<double, double> ComponentAreas(this Opening opening)
        {
            if (opening == null)
            {
                Base.Compute.RecordWarning("Component areas can not be calculated for a null opening.");
                return new Output<double, double>
                {
                    Item1 = double.NaN,
                    Item2 = double.NaN,
                };
            }

            IGeometry frameGeo = opening.FrameGeometry2D();
            double frameArea = 0;

            if (frameGeo is PlanarSurface)
            {
                PlanarSurface frameSrf = frameGeo as PlanarSurface;
                frameArea = frameSrf.Area();
            }
            double openArea = opening.Area()-frameArea;

            return new Output<double, double>
            {
                Item1 = openArea,
                Item2 = frameArea,
            };
        }

        /***************************************************/
    }
}





