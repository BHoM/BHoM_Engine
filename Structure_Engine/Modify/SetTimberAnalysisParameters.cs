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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Structure.SurfaceProperties;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
      
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material of the ISectionProperty based on the product specific properties. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("sectionProperty", "SectionProperty on which to update the timber analysis properties of the assigned material. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner. Not used for all timber products.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void SetTimberAnalysisParameters(this ISectionProperty sectionProperty, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            ITimber timber = sectionProperty.Material as ITimber;
            if(timber != null)
                timber.ISetAnalysisParameters(stiffnessType, orientationType);
        }

        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material of the ISurfaceProperty based on the product specific properties. No action is taken if the material of the property is not a type of ITimber.")]
        [Input("surfaceProperty", "SurfaceProperty on which to update the timber analysis properties of the assigned material. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner. Not used for all timber products.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void ISetTimberAnalysisParameters(this ISurfaceProperty surfaceProperty, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            SetTimberAnalysisParameters(surfaceProperty as dynamic, stiffnessType, orientationType);
        }

        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material of the ISectionProperty of the Bar based on the product specific properties. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("bar", "Bar on which to update the timber analysis properties of material assigned to the SectionProperty. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner. Not used for all timber products.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void SetTimberAnalysisParameters(this Bar bar, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            bar.SectionProperty.SetTimberAnalysisParameters(stiffnessType, orientationType);
        }

        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material of the ISurfaceProperty of the Panel based on the product specific properties. No action is taken if the material of the property is not a type of ITimber.")]
        [Input("panel", "Panel on which to update the timber analysis properties of material assigned to the Property. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner. Not used for all timber products.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void SetTimberAnalysisParameters(this Panel panel, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            panel.Property.ISetTimberAnalysisParameters(stiffnessType, orientationType);
        }

        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material of the ISurfaceProperty of the FEMesh based on the product specific properties. No action is taken if the material of the property is not a type of ITimber.")]
        [Input("feMesh", "FEMesh on which to update the timber analysis properties of material assigned to the Property. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner. Not used for all timber products.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        public static void SetTimberAnalysisParameters(this FEMesh feMesh, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            feMesh.Property.ISetTimberAnalysisParameters(stiffnessType, orientationType);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material of the ISectionProperty based on the product specific properties. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("sectionProperty", "Timber material to update the Analysis proeprties of.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner. Not used for all timber products.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        private static void SetTimberAnalysisParameters(this ISurfaceProperty surfaceProperty, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            ITimber timber = surfaceProperty.Material as ITimber;
            if (timber != null)
                timber.ISetAnalysisParameters(stiffnessType, orientationType);
        }

        /***************************************************/

        [Description("Sets YoungsModulus and ShearModulus used for analysis of the material of the ISectionProperty based on the product specific properties. No action is taken if the material of the section is not a type of ITimber.")]
        [Input("sectionProperty", "Timber material to update the Analysis proeprties of.")]
        [Input("stiffnessType", "Controls if mean or Characteristic values should be used when assigning stiffness values.")]
        [Input("orientationType", "Controls if material is oriented in a flatwise or edgewise manner. Not used for all timber products.")]
        [Output("timber", "The timber material with updated stiuffness properties based on the inputs and product specific values.")]
        private static void SetTimberAnalysisParameters(this Layered surfaceProperty, TimberAnalysisStiffness stiffnessType, TimberAnalysisOrientation orientationType = TimberAnalysisOrientation.Unknown)
        {
            foreach (Layer layer in surfaceProperty.Layers)
            {
                ITimber timber = layer.Material as ITimber;
                if (timber != null)
                    timber.ISetAnalysisParameters(stiffnessType, orientationType);
            }

        }

        /***************************************************/
    }
}
