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
using System.Collections.Generic;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static StadiaParameters StadiaParameters(double scale = 1.0)
        {
            return new StadiaParameters
            {
                StructBayWidth = 7.5 * scale,

                CornerRadius = 10.0 * scale,

                SideBound = 6.7 * scale,

                EndBound = 7.8 * scale,

                SideRadius = 240.0 * scale,

                EndRadius = 200.0 * scale,

                TheatronRadius = 100.0 * scale,

                NumCornerBays = 7,

                TypeOfBowl = StadiaType.EightArc,

                CornerFraction = 0.5 * scale,

                ActivityArea = Create.ActivityArea(scale),

            };
        }

        /***************************************************/

        public static StadiaParameters StadiaParameters(double structBayWidth = 7.5,double cornerRadius = 10.0,double sideBound = 6.7,
            double endBound = 7.8,double sideRadius = 240.0,double endRadius = 200.0,double theatronRadius = 100.0,int numCornerBays = 7,
            StadiaType typeOfBowl = StadiaType.EightArc, double cornerFraction = 0.5,ActivityArea activityArea = null)
        {
            if (activityArea == null) activityArea = Create.ActivityArea(1);

            return new StadiaParameters
            {
                StructBayWidth = structBayWidth,

                CornerRadius = cornerRadius,

                SideBound = sideBound,

                EndBound = endBound,

                SideRadius = sideRadius,

                EndRadius = endRadius,

                TheatronRadius = theatronRadius,

                NumCornerBays = numCornerBays,

                TypeOfBowl = typeOfBowl,

                CornerFraction = cornerFraction,

                ActivityArea = activityArea,
            };
        }

        /***************************************************/
    }
}
