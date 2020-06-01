/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.MaterialFragments;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a TransverseReinforcement placing rebars across a straight line along the ConcreteSection")]
        [InputFromProperty("rebarsCenterlinesLayout")]
        [InputFromProperty("diameter")]
        [InputFromProperty("spacing")]
        [InputFromProperty("adjustSpacingToFit", "Toggles if provided spacing value should be fixed or adjusted to fit reinforcement from bar's start to end.")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a default material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static TransverseReinforcement TransverseReinforcement(ICurveLaout curveLayout, double diameter, double spacing, bool adjustSpacingToFit, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            CheckEndLocations(ref startLocation, ref endLocation);
            return new TransverseReinforcement
            {
                CenterlineLayout = curveLayout,
                Diameter = diameter,
                Spacing = spacing,
                AdjustSpacingToFit = adjustSpacingToFit,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation,
            };
        }

        /***************************************************/
    }
}