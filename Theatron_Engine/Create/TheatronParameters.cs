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

using BH.oM.Theatron.Elements;
using BH.oM.Theatron.Parameters;

namespace BH.Engine.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static TheatronParameters TheatronParameters(double scale = 1.0)
        {
            return new TheatronParameters
            {
                StructBayWidth = 7.5 * scale,

                CornerRadius = 10.0 * scale,

                SideBound = 6.7 * scale,

                EndBound = 7.8 * scale,

                SideRadius = 240.0 * scale,

                EndRadius = 200.0 * scale,

                TheatronRadius = 100.0 * scale,

                NumCornerBays = 7,

                TypeOfBowl = BowlType.Radial,

                CornerFraction = 0.5 * scale,

            };
        }

        /***************************************************/

        public static TheatronParameters TheatronParameters(double structBayWidth = 7.5,double cornerRadius = 10.0,double sideBound = 6.7,
            double endBound = 7.8,double sideRadius = 240.0,double endRadius = 200.0,double theatronRadius = 100.0,int numCornerBays = 7,
            BowlType typeOfBowl = BowlType.Radial, double cornerFraction = 0.5,ActivityArea activityArea = null)
        {
            return new TheatronParameters
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
            };
        }

        /***************************************************/
    }
}
