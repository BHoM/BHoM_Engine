/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Reinforcement;

using BH.Engine.Spatial;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a LongitudinalReinforcement placing rebars along a straight line throughout the ConcreteSection.")]
        [InputFromProperty("rebarLayout")]
        [InputFromProperty("diameter")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a default material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static LongitudinalReinforcement LongitudinalReinforcement(ILayout2D rebarLayout, double diameter, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            if (rebarLayout.IsNull())
                return null;
            else if (diameter < Tolerance.Distance)
            {
                Base.Compute.RecordError("The diameter is less than the tolerance. Please check your inputs.");
                return null;
            }


            CheckEndLocations(ref startLocation, ref endLocation);
            return new LongitudinalReinforcement
            {
                RebarLayout = rebarLayout,
                Diameter = diameter,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation,
            };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Private checking methods used by all LongitudinalReinforcement create methods to check start and end.")]
        private static void CheckEndLocations(ref double startLocation, ref double endLocation)
        {
            if (startLocation < 0)
            {
                startLocation = 0;
                Base.Compute.RecordWarning("Start location need to be larger or equal to 0. To accommodate, the start location has been set to 0.");
            }
            else if (startLocation > 1)
            {
                startLocation = 1;
                Base.Compute.RecordWarning("Start location need to be smaller or equal to 1. To accommodate, the start location has been set to 1.");
            }

            if (endLocation < 0)
            {
                startLocation = 0;
                Base.Compute.RecordWarning("End location need to be larger or equal to 0. To accommodate, the end location has been set to 0.");
            }
            else if (startLocation > 1)
            {
                startLocation = 1;
                Base.Compute.RecordWarning("End location need to be smaller or equal to 1. To accommodate, the end location has been set to 1.");
            }

            if (startLocation > endLocation)
            {
                double temp = startLocation;
                startLocation = endLocation;
                endLocation = temp;
                Base.Compute.RecordWarning("Start location need to be smaller or equal than the end location. To accommodate, the start and end location have been switched.");
            }
        }

        /***************************************************/

    }
}




