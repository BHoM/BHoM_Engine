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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Reinforcement;
using BH.Engine.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Verifies whether the reinforcement is valid by performing null checks, checking the bend radius and shape code compliance.")]
        [Input("reinforcement", "The reinforcement containing the ShapeCode, reinforcement and bending radius to be verified.")]
        [Output("bool", "True if the shape code is valid.")]
        public static bool IsValid(this Reinforcement reinforcement)
        {
            if (reinforcement.IsNull())
                return false;
            else if (reinforcement.CoordinateSystem.IsNull())
                return false;
            else if (reinforcement.Diameter <= 0)
            {
                Reflection.Compute.RecordError("The Reinforcement is invalid because the diameter is less than zero.");
                return false;
            }
            else if (reinforcement.BendRadius < reinforcement.SchedulingRadius())
            {
                reinforcement.BendRadius = reinforcement.SchedulingRadius();
                Reflection.Compute.RecordWarning("The bend radius of the Reinforcement is less than the minimum scheduling radius and has been assigned the minimum value.");
            }
            else if (!reinforcement.ShapeCode.IIsCompliant(reinforcement.Diameter))
                return false;

            return true;
        }

    }
}