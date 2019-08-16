/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Humans.ViewQuality;
using System.Collections.Generic;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a default set of eye position parameters")]
        [Input("scale", "Optional input to scale from default values")]

        public static EyePositionParameters EyePositionParameters(double scale =1.0)
        {
            return new EyePositionParameters
            {

                WheelChairEyePositionX = 1 * scale,

                WheelChairEyePositionZ = 1 * scale,

                EyePositionZ = 1.2 * scale,

                EyePositionX = 0.15 * scale,

                StandingEyePositionX = 0.4 * scale,

                StandingEyePositionZ = 1.4 * scale,

            };
        }
        /***************************************************/
        [Description("Create a custom set of eye position parameters")]
        [Input("wheelChairEyePositionX", "Horizontal seated eye postion on super riser row")]
        [Input("wheelChairEyePositionZ", "Vertical seated eye postion on super riser row")]
        [Input("eyePositionX", "Horizontal seated eye postion on standard row")]
        [Input("eyePositionZ", "Vertical seated eye postion on standard row")]
        [Input("standingEyePositionX", "Horizontal standing eye postion on standard row")]
        [Input("standingEyePositionZ", "Vertical standing eye postion on standard row")]
        public static EyePositionParameters EyePositionParameters(double wheelChairEyePositionX = 1.1, double wheelChairEyePositionZ = 1.1,
            double eyePositionX = 0.15, double eyePositionZ = 1.2, double standingEyePositionX = 0.4, double standingEyePositionZ = 1.4)
        {
            return new EyePositionParameters
            {

                WheelChairEyePositionX = wheelChairEyePositionX,

                WheelChairEyePositionZ = wheelChairEyePositionZ,

                EyePositionZ = eyePositionZ,

                EyePositionX = eyePositionX,

                StandingEyePositionX = standingEyePositionX,

                StandingEyePositionZ = standingEyePositionZ,

            };
        }
    }
}
