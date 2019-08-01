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
        [Description("Create a default set of stadia parameters")]
        [Input("scale", "Optional input to scale from default values")]
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

                ActivityArea = ActivityArea(scale),

                PitchLength = 90 *scale,

                PitchWidth = 60 * scale,

            };
        }

        /***************************************************/
        [Description("Create a full set of stadia parameters, default values in metres")]
        [Input("structBayWidth", "Target width for structural bays measure at front of the first row")]
        [Input("cornerRadius", "Radius for the corners if using EightArc or Orthogonal stadia types")]
        [Input("sideBound", "Dimension from the side of the playing area to the front row at the half way line")]
        [Input("endBound", "Dimension from the end of playing area to the front row at the middle of the goal mouth")]
        [Input("sideRadius", "Side arc radius when using EightArc stadia type")]
        [Input("endRadius", "End arc radius when using EightArc stadia type")]
        [Input("theatronRadius", "Radius of bowl for Circular stadia type")]
        [Input("numCornerBays", "Number of structural bays through corners if using EightArc or Orthogonal stadia types")]
        [Input("typeOfBowl", "What is the stadia type defined by a StadiaType enum")]
        [Input("cornerFraction", "Proportional width of the transition bays between corners and sides and corners and ends")]
        [Input("activityArea", "ActivityArea defines playing area and a focal point")]
        [Input("pitchLength", "Length of the pitch")]
        [Input("pitchWidth", "Width of the pitch")]
        public static StadiaParameters StadiaParameters(double structBayWidth = 7.5,double cornerRadius = 10.0,double sideBound = 6.7,
            double endBound = 7.8,double sideRadius = 240.0,double endRadius = 200.0,double theatronRadius = 100.0,int numCornerBays = 7,
            StadiaType typeOfBowl = StadiaType.EightArc, double cornerFraction = 0.5,ActivityArea activityArea = null, double pitchLength = 90, double pitchWidth =45)
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

                PitchLength = pitchLength,

                PitchWidth = pitchWidth,
            };
        }

        /***************************************************/
    }
}
