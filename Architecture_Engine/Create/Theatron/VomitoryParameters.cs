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
        [Description("Create a default set of vomitory parameters")]
        [Input("scale", "Optional input to scale from default values")]
        public static VomitoryParameters VomitoryParameters(double scale = 1.0)
        {
            return new VomitoryParameters
            {
                //values in m below
                
                Vomitory = false,

                VomitoryStartRow = 10,

                VomitoryWidth = 1.2 * scale,

            };
        }
        [Description("Create a custom set of vomitory parameters")]
        [Input("vomitory", "Is there a vomitory?")]
        [Input("vomitoryStartRow", "What row does the vomitory start? (If there is a super riser the vomitory will start at the same row as the super riser)")]
        [Input("vomitoryWidth", "Width of aisle at vomitory")]
        
        public static VomitoryParameters VomitoryParameters(bool vomitory = false, int vomitoryStartRow = 10, double vomitoryWidth = 1.2)
        {
            return new VomitoryParameters
            {
                //values in m below

                Vomitory = vomitory,

                VomitoryStartRow = vomitoryStartRow,

                VomitoryWidth = vomitoryWidth,

            };
        }
    }
}
