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

using BH.oM.Architecture.Theatron;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a default set of super riser parameters")]
        [Input("scale", "Optional input to scale from default values")]
        public static SuperRiserParameters SuperRiserParameters(double scale = 1.0)
        {
            return new SuperRiserParameters
            {
                //values in m below
                
                SuperRiser = false,

                SuperRiserStartRow = 10,

                SuperRiserKerbWidth = 0.15 * scale,

            };
        }
        /***************************************************/
        [Description("Create a custom set of super riser parameters")]
        [Input("superRiser", "Is there a super riser (accessible row)")]
        [Input("superRiserStart", "What row does the super riser start?")]
        [Input("superRiserKerbWidth", "Optional scale if working units are not metres")]

        public static SuperRiserParameters SuperRiserParameters(bool superRiser = false, int superRiserStart = 10, double superRiserKerbWidth = 0.15)

        {
            return new SuperRiserParameters
            {
                //values in m below

                SuperRiser = superRiser,

                SuperRiserStartRow = superRiserStart,

                SuperRiserKerbWidth = superRiserKerbWidth,

            };
        }
        
    }
}
