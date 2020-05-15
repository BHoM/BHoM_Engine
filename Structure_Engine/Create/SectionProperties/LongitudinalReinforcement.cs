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

        [Description("Creates a LongitudinalReinforcement placing rebars along the perimiter of host ConcreteSection.")]
        [InputFromProperty("diameter")]
        [Input("barCount", "Number of Rebars along the perimiter.")]
        [Input("rebarsAtProfileDiscontinuities", "If true, bars will be placed at any discontinuities of the perimiter of the crossection.")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a defaul material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static LongitudinalReinforcement PerimiterReinforcement(double diameter, int barCount, bool rebarsAtProfileDiscontinuities, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            CheckEndLocations(ref startLocation, ref endLocation);
            return new LongitudinalReinforcement
            {
                RebarLayout = new PerimeterLayout() { NumberOfPoints = barCount, EnforceDiscontinuityPoints = rebarsAtProfileDiscontinuities },
                Diameter = diameter,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation,
            };
        }

        /***************************************************/

        [Description("Creates a LongitudinalReinforcement placing rebars along a straight line throughout the ConcreteSection")]
        [InputFromProperty("diameter")]
        [Input("barCount", "Number of bars along the along the linear distribution.")]
        [Input("direction", "Direction of the axis of the reinforcement. Should be a vector in the global XY-plane, defaults to the global X-axis.")]
        [Input("offset", "Offset of the linear layout in relation to the reference point, perpendicular to the Direction vector in the XY-plane.\n" +
                     "A positive value will mean an offset towards the centre of the boundingbox of the ConcreteSection.", typeof(Length))]
        [Input("referencePoint", "Controls, together with the offset, which point on the ConcreteSection that should be used for the layout.")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a defaul material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static LongitudinalReinforcement LayerReinforcement(double diameter, int barCount, Vector direction = null, double offset = 0, ReferencePoint referencePoint = ReferencePoint.BottomCenter, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            CheckEndLocations(ref startLocation, ref endLocation);
            return new LongitudinalReinforcement
            {
                RebarLayout = Spatial.Create.LinearLayout(barCount, direction, offset, referencePoint),
                Diameter = diameter,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation,
            };
        }

        /***************************************************/

        [Description("Creates a LongitudinalReinforcement placing rebars along multiple linear parallel axes, defined along a vector from one side of the perimeter of ConcreteSection to the other. \n" +
                 "Starts by fitting in as many points as possible in the first layer, then generates a new one and repeats.")]
        [InputFromProperty("diameter")]
        [Input("barCount", "Number of bars along the along the linear distribution axes.")]
        [Input("parallelSpacing", "Minimum spacing allowed between two rebars in a single layer", typeof(Length))]
        [Input("perpendicularSpacing", "Minimum spacing allowed between two rebars layers", typeof(Length))]
        [Input("direction", "Direction of the axis of the reinforcement. Should be a vector in the global XY-plane, defaults to the global X-axis.")]
        [Input("offset", "Offset of the linear layout in relation to the reference point, perpendicular to the Direction vector in the XY-plane.\n" +
                     "A positive value will mean an offset towards the centre of the boundingbox of the ConcreteSection.", typeof(Length))]
        [Input("referencePoint", "Controls, together with the offset, which point on the ConcreteSection that should be used for the layout.")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a defaul material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static LongitudinalReinforcement MultiLinearReinforcement(double diameter, int barCount, double parallelSpacing, double perpendicularSpacing, Vector direction = null, double offset = 0, ReferencePoint referencePoint = ReferencePoint.BottomCenter, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            CheckEndLocations(ref startLocation, ref endLocation);
            return new LongitudinalReinforcement
            {
                RebarLayout = Spatial.Create.MultiLinearLayout(barCount, parallelSpacing + diameter, perpendicularSpacing + diameter, direction, offset, referencePoint),
                Diameter = diameter,
                Material = material ?? Query.Default(MaterialType.Rebar),
                StartLocation = startLocation,
                EndLocation = endLocation,
            };
        }

        /***************************************************/

        [Description("Creates a LongitudinalReinforcement placing rebars along multiple linear parallel axes along the local y-axis of the ConcreteSection, defined along a vector from one side of the perimeter of ConcreteSection to the other. \n" +
                 "Starts by fitting in as many points as possible in a layer towards the bottom of the section, then generates a new one and repeats.")]
        [InputFromProperty("diameter")]
        [Input("area", "Total minimum required area of bottom reinforcement. Will be used to calculate required number of bars, based on their diameter, hence the resulting area can get higher the inout value.", typeof(Area))]
        [Input("spacing", "Minimum spacing allowed between any two rebars.")]
        [InputFromProperty("miniumCover")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a defaul material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static LongitudinalReinforcement BottomReinforcement(double diameter, double area, double spacing, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            int numberOfBars = (int)Math.Ceiling(area / (diameter * diameter * Math.PI / 4));
            return MultiLinearReinforcement(diameter, numberOfBars, spacing, spacing, Vector.XAxis, 0, ReferencePoint.BottomCenter, startLocation, endLocation, material);
        }

        /***************************************************/

        [Description("Creates a LongitudinalReinforcement placing rebars along multiple linear parallel axes along the local y-axis of the ConcreteSection, defined along a vector from one side of the perimeter of ConcreteSection to the other. \n" +
                 "Starts by fitting in as many points as possible in a layer towards the top of the section, then generates a new one and repeats.")]
        [InputFromProperty("diameter")]
        [Input("area", "Total minimum required area of bottom reinforcement. Will be used to calculate required number of bars, based on their diameter, hence the resulting area can get higher the inout value.", typeof(Area))]
        [Input("spacing", "Minimum spacing allowed between any two rebars.")]
        [InputFromProperty("startLocation")]
        [InputFromProperty("endLocation")]
        [Input("material", "Material of the Rebars. If null, a defaul material will be pulled from the Datasets.")]
        [Output("reinforcement", "The created Reinforcement to be applied to a ConcreteSection.")]
        public static LongitudinalReinforcement TopReinforcement(double diameter, double area, double spacing, double startLocation = 0, double endLocation = 1, IMaterialFragment material = null)
        {
            int numberOfBars = (int)Math.Ceiling(area / (diameter * diameter * Math.PI / 4));
            return MultiLinearReinforcement(diameter, numberOfBars, spacing, spacing, Vector.XAxis, 0, ReferencePoint.TopCenter, startLocation, endLocation, material);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void CheckEndLocations(ref double startLocation, ref double endLocation)
        {
            if (startLocation < 0)
            {
                startLocation = 0;
                Reflection.Compute.RecordWarning("Start location need to be larger or equal to 0. To accomodate, the start location has been set to 0.");
            }
            else if (startLocation > 1)
            {
                startLocation = 1;
                Reflection.Compute.RecordWarning("Start location need to be smaller or equal to 1. To accomodate, the start location has been set to 1.");
            }

            if (endLocation < 0)
            {
                startLocation = 0;
                Reflection.Compute.RecordWarning("End location need to be larger or equal to 0. To accomodate, the end location has been set to 0.");
            }
            else if (startLocation > 1)
            {
                startLocation = 1;
                Reflection.Compute.RecordWarning("End location need to be smaller or equal to 1. To accomodate, the end location has been set to 1.");
            }

            if (startLocation > endLocation)
            {
                double temp = startLocation;
                startLocation = endLocation;
                endLocation = temp;
                Reflection.Compute.RecordWarning("Start location need to be smaller or equal than the end location. To accomodate, the start and end location have been switched.");
            }
        }

        /***************************************************/
    }
}
