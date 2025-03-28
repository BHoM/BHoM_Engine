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
using System.Text;
using System.Threading.Tasks;

using BH.oM.Lighting;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Lighting.Elements;

namespace BH.Engine.Lighting
{
    public static partial class Query
    {
        [Description("Gets the geometry of a Luminaire as a Point.")]
        [Input("luminaire", "Element to get the Point from.")]
        [Output("point", "The geometry of the Element.")]
        public static Point Geometry(this Luminaire luminaire)
        {
            if(luminaire == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the geometry of a null luminaire.");
                return null;
            }

            return luminaire.Position;
        }
    }
}


