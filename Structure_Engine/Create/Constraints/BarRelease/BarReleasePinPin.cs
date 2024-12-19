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

using BH.oM.Structure.Constraints;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a BarRelease that is pinned at both ends, i.e. all translational degrees of freedom will be fixed, and rotational released, with one exception. The rx release on the start will be fixed to avoid instability of the Bar, to prevent it from rotating about the local x-axis (the centreline axis).")]
        [Input("name", "Name of the BarRelease. Defaults to PinPin. This is required by most structural analysis software to create the object.")]
        [Output("release", "The created pinned BarRelease.")]
        public static BarRelease BarReleasePinPin(string name = "PinPin")
        {
            Constraint6DOF endRelease = PinConstraint6DOF();
            endRelease.RotationX = DOFType.Fixed;

            return new BarRelease
            {
                StartRelease = PinConstraint6DOF(),
                EndRelease = endRelease,
                Name = name
            };
        }

        /***************************************************/
    }
}






