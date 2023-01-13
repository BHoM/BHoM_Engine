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
using BH.oM.Geometry;
using BH.oM.Physical.Reinforcement.BS8666;
using BH.oM.Base.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a ShapeCode object using the parameters provided. Refer to the object descriptions for alignment.")]
        [Output("shapeCode", "A ShapeCode to be used with Reinforcement objects.")]
        public static ShapeCode22 ShapeCode22(double a, double b, double c, double d, double diameter, double bendRadius)
        {
            if (a < Tolerance.Distance || b < Tolerance.Distance || c < Tolerance.Distance || d < Tolerance.Distance)
            {
                Base.Compute.RecordError("One or more of the parameters given is zero and therefore the ShapeCode cannot be created.");
                return null;
            }

            ShapeCode22 shapeCode = new ShapeCode22(a, b, c, d, diameter, bendRadius);

            return shapeCode.IIsCompliant() ? shapeCode : null;
        }

        /***************************************************/
    }
}


