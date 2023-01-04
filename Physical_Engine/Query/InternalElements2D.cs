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
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;
using BH.oM.Dimensional;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the internal 2D elements of an ISurface.")]
        [Input("surface", "The ISurface to get the internal elements from.")]
        [Output("openings", "The IOpenings of the ISurface.")]
        public static List<IElement2D> InternalElements2D(this oM.Physical.Elements.ISurface surface)
        {
            if(surface == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the internal 2D elements of a null surface.");
                return new List<IElement2D>();
            }

            return new List<IElement2D>(surface.Openings);
        }

        /***************************************************/

    }
}




