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

using BH.Engine.Humans.ViewQuality;
using BH.oM.Humans.ViewQuality;
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a default set of profile parameters")]
        [Input("scale", "Optional scale if working units are not metres")]
        
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

                BoardHeight = 1.1 * scale,

                RiserHeightRounding = 0.0,

                VomitoryParameters = VomitoryParameters(scale),

                EyePositionParameters = BH.Engine.Humans.ViewQuality.Create.EyePositionParameters(scale),

                SuperRiserParameters = SuperRiserParameters(scale)

            };
        }

        /***************************************************/
        [Description("Create a full set of profile parameters for a single tier, default values in metres")]
        [Input("startX", "The horizontal postion for the first spectator eye, ignored in the first tier if the profile depends on a plan geometry")]
        [Input("startZ", "The vertical postion for the first spectator eye")]
        [Input("rowWidth", "Row width")]
        [Input("targetC", "Target Cvalue")]
        [Input("seatWidth", "Seat width")]
        [Input("numRows", "Number of rows")]
        [Input("boardHeight", "Height of advertising signage infront of seating")]
        [Input("riserRounding", "Round riser heights are rounded to multiples of this value")]
        
       
        public static ProfileParameters ProfileParameters(double startX=5.0,double startZ=1.0,double rowWidth=0.8,double targetC =0.09,
            double seatWidth = 0.4,int numRows =20, double boardHeight = 1.1, double riserRounding = 0.0,
            EyePositionParameters eyePositionParameters=null, VomitoryParameters vomitoryParameters = null,SuperRiserParameters superRiserParameters =null )

        {
            //generate default parameters if null
            if (eyePositionParameters == null) eyePositionParameters = BH.Engine.Humans.ViewQuality.Create.EyePositionParameters(1.0);
            if (vomitoryParameters == null) vomitoryParameters = VomitoryParameters(1.0);
            if (superRiserParameters == null) superRiserParameters = SuperRiserParameters(1.0);
            return new ProfileParameters
            {
                //values in m below
                StartX = startX,

                StartZ = startZ,

                RowWidth = rowWidth,

                TargetCValue = targetC,

                SeatWidth = seatWidth,

                NumRows = numRows,

                BoardHeight = boardHeight,

                RiserHeightRounding = riserRounding,

                EyePositionParameters = eyePositionParameters,

                VomitoryParameters = vomitoryParameters,

                SuperRiserParameters = superRiserParameters,

            };
        }

        /***************************************************/
    }
}
