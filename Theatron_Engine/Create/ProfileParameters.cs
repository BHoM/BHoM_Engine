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

using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using BH.oM.Theatron.Parameters;

namespace BH.Engine.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        public static ProfileParameters ProfileParameters(double scale = 1.0)
        {
            return new ProfileParameters
            {
                //values in m below
                StartX = 5.0 * scale,

                StartZ = 1.0 * scale,

                RowWidth = 0.8 * scale,

                TargetCValue = 0.09 * scale,

                SeatWidth = 0.4 * scale,

                NumRows = 25,

                SuperRiser = false,

                SuperRiserStartRow = 10,

                Vomitory = false,

                VomitoryStartRow = 10,

                AisleWidth = 1.2 * scale,

                SuperRiserKerbWidth = 0.15 * scale,

                SuperRiserEyePositionX = 1 * scale,

                SuperRiserEyePositionZ = 1 * scale,

                EyePositionZ = 1.2 * scale,

                EyePositionX = 0.15 * scale,

                BoardHeight = 1.1 * scale,

                StandingEyePositionX = 0.4 * scale,

                StandingEyePositionZ = 1.4 * scale

            };
        }

        /***************************************************/

        public static ProfileParameters ProfileParameters(double startX=5.0,double startZ=1.0,double rowWidth=0.8,double targetC =0.9,
            double seatWidth = 0.4,int numRows =20,bool superRiser=false,int superRiserStart = 10, bool vomitory =false,int vomitoryStartRow=10,
            double aisleWidth = 1.2,double superRiserKerbWidth=0.15,double superRiserEyePositionX = 1.1, double superRiserEyePositionZ = 1.1,
            double eyePositionZ = 1.2,double eyePositionX = 0.15,double boardHeight =1.1,double standingEyePositionX =0.4, double standingEyePositionZ=1.4)
        {
            return new ProfileParameters
            {
                //values in m below
                StartX = startX,

                StartZ = startZ,

                RowWidth = rowWidth,

                TargetCValue = targetC,

                SeatWidth = seatWidth,

                NumRows = numRows,

                SuperRiser = superRiser,

                SuperRiserStartRow = superRiserStart,

                Vomitory = vomitory,

                VomitoryStartRow = vomitoryStartRow,

                AisleWidth = aisleWidth,

                SuperRiserKerbWidth = superRiserKerbWidth,

                SuperRiserEyePositionX = superRiserEyePositionX,

                SuperRiserEyePositionZ = superRiserEyePositionZ,

                EyePositionZ = eyePositionZ,

                EyePositionX = eyePositionX,

                BoardHeight = boardHeight,

                StandingEyePositionX = standingEyePositionX,

                StandingEyePositionZ = standingEyePositionZ

            };
        }

        /***************************************************/
    }
}
