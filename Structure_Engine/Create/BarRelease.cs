/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Properties.Constraint;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BarRelease BarRelease(Constraint6DOF startConstraint, Constraint6DOF endConstraint, string name = "")
        {
            return new BarRelease { StartRelease = startConstraint, EndRelease = endConstraint, Name = name };
        }

        /***************************************************/

        public static BarRelease BarReleaseFixFix( string name = "FixFix")
        {
            return new BarRelease { StartRelease = FixConstraint6DOF(), EndRelease = FixConstraint6DOF(), Name = name };
        }

        /***************************************************/

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

        public static BarRelease BarReleasePinSlip(string name = "PinSlip")
        {
            return new BarRelease
            {
                StartRelease = PinConstraint6DOF(),
                EndRelease = Constraint6DOF(false, true, true, false, false, false, "Slip"),
                Name = name
            };
        }

        /***************************************************/
    }
}
