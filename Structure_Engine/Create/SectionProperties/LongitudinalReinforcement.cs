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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Base;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static LongitudinalReinforcement PerimiterReinforcement(double diameter, int barCount, bool rebarsAtProfileDiscontinuities, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            return new LongitudinalReinforcement
            {
                RebarLayout = new PerimeterLayout() { NumberOfPoints = barCount, EnforceDiscontinuityPoints = rebarsAtProfileDiscontinuities },
                Diameter = diameter,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation
            };
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static LongitudinalReinforcement LayerReinforcement(double diameter, int barCount, Vector direction = null, double offset = 0, ReferencePoint referencePoint = ReferencePoint.BottomCenter, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            return new LongitudinalReinforcement
            {
                RebarLayout = Spatial.Create.LinearLayout(barCount, direction, offset, referencePoint),
                Diameter = diameter,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation
            };
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static LongitudinalReinforcement BottomReinforcement(double diameter, double area, double spacing, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            int numberOfBars = (int)Math.Ceiling(area / (diameter * diameter * Math.PI / 4));
            return new LongitudinalReinforcement
            {
                RebarLayout = Spatial.Create.MultiLinearLayout(numberOfBars, spacing + diameter, Vector.XAxis, 0, ReferencePoint.BottomCenter),
                Diameter = diameter,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation
            };
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static LongitudinalReinforcement TopReinforcement(double diameter, double area, double spacing, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            int numberOfBars = (int)Math.Ceiling(area / (diameter * diameter * Math.PI / 4));
            return new LongitudinalReinforcement
            {
                RebarLayout = Spatial.Create.MultiLinearLayout(numberOfBars, spacing + diameter, Vector.XAxis, 0, ReferencePoint.TopCenter),
                Diameter = diameter,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation
            };
        }

        /***************************************************/
    }
}
