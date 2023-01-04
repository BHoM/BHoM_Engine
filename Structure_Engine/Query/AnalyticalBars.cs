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

using BH.oM.Structure.Elements;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;
using System;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Quantities.Attributes;

using BHP = BH.oM.Physical;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Create a series of analytical Bar elements from the framing element. The relationship of Bars to FramingElements is not necessarily one to one and this is dictated by property and curve type of the FramingElement.")]
        [Input("elements", "The framing element to generate bars from.")]
        [Input("angleTolerance", "Angle tolerance to control the splitting up of non-linear curves. Unused for line based FramingElements.", typeof(Angle))]
        [Input("maxNbBarsPerArc", "The maximum number of bars that each arc segement of the element will be split up into. Unused for line based FramingElements.")]
        [Output("bars", "A list of bars per framing element. For straight framing elements with prismatic sections this will be a single bar.")]
        public static List<List<Bar>> AnalyticalBars(this List<BHP.Elements.IFramingElement> elements, double angleTolerance = 0.05 * Math.PI, int maxNbBarsPerArc = 10)
        {
            //Store the converted properties as the elements are being converted.
            Dictionary<BHP.FramingProperties.IFramingElementProperty, object> convertedProps = new Dictionary<BHP.FramingProperties.IFramingElementProperty, object>();

            List<List<Bar>> bars = new List<List<Bar>>();

            foreach (BHP.Elements.IFramingElement element in elements)
            {
                try
                {
                    bars.Add(AnalyticalBars(element.Property as dynamic, element.Location, element.Name, angleTolerance, maxNbBarsPerArc, ref convertedProps));
                }
                catch (Exception e)
                {
                    bars.Add(new List<Bar>());
                    string nameMsg = "an element with no name.";
                    if (element != null && element.Name != null)
                        nameMsg = "an element named " + element.Name;

                    string errorMessage = "";
                    if (e.InnerException != null && !string.IsNullOrEmpty(e.InnerException.Message))
                        errorMessage = e.InnerException.Message;
                    else
                        errorMessage = e.Message;

                    Base.Compute.RecordError("Failed to get analytical bars from " + nameMsg + ". The following error was thrown by the method: " + errorMessage);
                }
            }

            return bars;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Bar> AnalyticalBars(BHP.FramingProperties.ConstantFramingProperty property, ICurve centreLine, string name, double angleTolerance, int maxNbBars, ref Dictionary<BHP.FramingProperties.IFramingElementProperty, object> convertedProps)
        {
            if (centreLine is NurbsCurve)
            {
                Engine.Base.Compute.RecordError("The analytical bars method is currently not supported for NurbsCurves. Please use another method to split up the nurbs to polylines that can be used to construct the bars.");
                return new List<Bar>();
            }

            bool isLinear = centreLine.IIsLinear();
            Plane curvePlane = null;

            if (!isLinear)
                curvePlane = centreLine.IFitPlane();

            ISectionProperty section;

            if (convertedProps.ContainsKey(property))
            {
                section = convertedProps[property] as ISectionProperty;
            }
            else
            {
                section = ToSectionProperty(property);
                convertedProps[property] = section;
            }

            List<Bar> bars = new List<Bar>();

            foreach (ICurve part in centreLine.ISubParts())
            {
                foreach (Line line in part.ICollapseToPolyline(angleTolerance, maxNbBars).SubParts())
                {
                    if (isLinear)
                    {
                        bars.Add(Create.Bar(line, section, property.OrientationAngle, Create.BarReleaseFixFix(), BarFEAType.Flexural, name));
                    }
                    else
                    {
                        Vector nomal = curvePlane.Normal.Rotate(property.OrientationAngle, line.Direction());
                        bars.Add(Create.Bar(line, section, nomal, Create.BarReleaseFixFix(), BarFEAType.Flexural, name));
                    }
                }
            }

            return bars;
        }

        /***************************************************/

        private static ISectionProperty ToSectionProperty(BHP.FramingProperties.ConstantFramingProperty property)
        {
            BHP.Materials.Material material = property.Material;

            IMaterialFragment fragment;

            if (material == null)
            {
                Base.Compute.RecordError("The FramingElement does not contain any material. An empty generic isotropic material has been used");
                fragment = new GenericIsotropicMaterial();
            }
            else if (!material.IsValidStructural())
            {
                string matName = material.Name ?? "";
                Base.Compute.RecordWarning("The material with name " + matName + " is not a valid structural material as it does not contain exactly one structural material fragment. An empty generic isotropic material has been assumed");
                fragment = new GenericIsotropicMaterial { Name = matName };
            }
            else
            {
                fragment = material.StructuralMaterialFragment();
            }

            return Create.SectionPropertyFromProfile(property.Profile, fragment, property.Name);
        }

        /***************************************************/
    }
}




