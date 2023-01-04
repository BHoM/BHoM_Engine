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
using BH.oM.Base.Attributes;
using BH.oM.Structure.Reinforcement;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a LongitudinalReinforcement placing rebars along multiple linear parallel axes along the local y-axis of the ConcreteSection, defined along a vector from one side of the perimeter of ConcreteSection to the other. \n" +
                 "Starts by fitting in as many points as possible in a layer towards the top of the section, then generates a new one and repeats.")]
        [InputFromProperty("diameter")]
        [Input("area", "Total minimum required area of bottom reinforcement. Will be used to calculate required number of bars, based on their diameter, hence the resulting area may be larger than the input value.", typeof(Area))]
        [Input("spacing", "Minimum spacing allowed between any two rebars.")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a default material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static LongitudinalReinforcement TopReinforcement(double diameter, double area, double spacing, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            if (diameter < Tolerance.Distance || area < Math.Pow(Tolerance.Distance, 2) || spacing < Tolerance.Distance)
            {
                Base.Compute.RecordError("The diameter, area or spacing values are less than the tolerance. Please check your inputs.");
                return null;
            }

            int numberOfBars = (int)Math.Ceiling(area / (diameter * diameter * Math.PI / 4));
            return MultiLinearReinforcement(diameter, numberOfBars, spacing, spacing, Vector.XAxis, 0, ReferencePoint.TopCenter, startLocation, endLocation, material);
        }

        /***************************************************/
    }
}



