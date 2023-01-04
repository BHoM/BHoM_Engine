/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Reinforcement;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a LongitudinalReinforcement placing rebars along a straight line throughout the ConcreteSection.")]
        [InputFromProperty("diameter")]
        [Input("barCount", "Number of bars along the along the linear distribution.")]
        [Input("direction", "Direction of the axis of the reinforcement. Should be a vector in the global XY-plane. Defaults to the global X-axis.")]
        [Input("offset", "Offset of the linear layout in relation to the reference point, perpendicular to the Direction vector in the XY-plane.\n" +
                     "A positive value will mean an offset towards the centre of the boundingbox of the ConcreteSection.", typeof(Length))]
        [Input("referencePoint", "Controls, together with the offset, which point on the ConcreteSection that should be used for the layout.")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a default material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static LongitudinalReinforcement LayerReinforcement(double diameter, int barCount, Vector direction = null, double offset = 0, ReferencePoint referencePoint = ReferencePoint.BottomCenter, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            if (diameter < Tolerance.Distance || barCount <= 0)
            {
                Base.Compute.RecordError("The diameter or bar count is less than the tolerance. Please check your inputs.");
                return null;
            }

            return LongitudinalReinforcement(Spatial.Create.LinearLayout(barCount, direction, offset, referencePoint), diameter, startLocation, endLocation, material);
        }

        /***************************************************/

    }
}


