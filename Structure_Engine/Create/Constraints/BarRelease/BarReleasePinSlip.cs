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

        [Description("Creates a BarRelease that is pinned at one end and axially released at the other. \n" +
                     "This means that the start node will have all tranlational degrees fixed as well as the rx, to avoid instability, i.e. to prevent it from rotating about the local x-axis (the centreline axis). \n" +
                     "The end node will be fixed in the y-axis and z-axis translationally, all other degrees of freedom will be free, i.e. it will be free to rotate as well as translate along the local x-axis (the centreline axis) of the Bar.")]
        [Input("name", "Name of the BarRelease. Defaults to PinSlip. This is required by most structural analysis software to create the object.")]
        [Output("release", "The created pin-slip BarRelease.")]
        public static BarRelease BarReleasePinSlip(string name = "PinSlip")
        {
            Constraint6DOF startRelease = PinConstraint6DOF();
            startRelease.RotationX = DOFType.Fixed;

            return new BarRelease
            {
                StartRelease = startRelease,
                EndRelease = Constraint6DOF(false, true, true, false, false, false, "Slip"),
                Name = name
            };
        }

        /***************************************************/
    }
}






