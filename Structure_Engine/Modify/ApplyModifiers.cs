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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.SectionProperties;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets modifiers to the SurfaceProperty. The modifiers are used to scale one or more of the property constants for analysis. Constants are multiplied with the modifiers, hence a modifier value of 1 means no change.")]
        [Input("prop", "The SurfaceProperty to apply modifiers to.")]
        [Input("f11", "Modifier of the axial stiffness along the local x-axis.", typeof(Ratio))]
        [Input("f12", "Modifier of the the in-plane shear.", typeof(Ratio))]
        [Input("f22", "Modifier of the axial stiffness along the local y-axis.", typeof(Ratio))]
        [Input("m11", "Modifier of the bending stiffness about the local x-axis.", typeof(Ratio))]
        [Input("m12", "Modifier of the in-plane twist stiffness.", typeof(Ratio))]
        [Input("m22", "Modifier of the bending stiffness about the local y-axis.", typeof(Ratio))]
        [Input("v13", "Modifier of the out of plane shear stiffness along the local x-axis.", typeof(Ratio))]
        [Input("v23", "Modifier of the out of plane shear stiffness along the local y-axis.", typeof(Ratio))]
        [Input("mass", "Modifier of the mass.", typeof(Ratio))]
        [Input("weight", "Modifier of the weight.", typeof(Ratio))]
        [Output("prop", "SurfaceProperty with applied modifiers.")]
        public static ISurfaceProperty ApplyModifiers(this ISurfaceProperty prop, double f11 = 1, double f12 = 1, double f22 = 1, double m11 = 1, double m12 = 1, double m22 = 1, double v13 = 1, double v23 = 1, double mass = 1, double weight = 1)
        {
            ISurfaceProperty clone = prop.GetShallowClone() as ISurfaceProperty;

            double[] modifiers = new double[] { f11, f12, f22, m11, m12, m22, v13, v23, mass, weight };

            clone.CustomData["Modifiers"] = modifiers;

            return clone;
        }

        /***************************************************/

        [Description("Sets modifiers to the SectionProperty. The modifiers are used to scale one or more of the property constants for analysis. Constants are multiplied with the modifiers, hence a modifier value of 1 means no change.")]
        [Input("prop", "The SectionProperty to apply modifiers to.")]
        [Input("area", "Modifier of the area.", typeof(Ratio))]
        [Input("iy", "Modifier of the second moment about the local y-axis (generally major axis).", typeof(Ratio))]
        [Input("iz", "Modifier of the second moment about the local z-axis (generally minor axis).", typeof(Ratio))]
        [Input("j", "Modifier of the torsional constant.", typeof(Ratio))]
        [Input("asy", "Modifier of the shear area along the local y-axis (generally minor axis).", typeof(Ratio))]
        [Input("asz", "Modifier of the shear area along the local z-axis (generally major axis).", typeof(Ratio))]
        [Output("prop", "SectionProperty with applied modifiers.")]
        public static ISectionProperty ApplyModifiers(this ISectionProperty prop, double area = 1, double iy = 1, double iz = 1, double j = 1, double asy = 1, double asz = 1)
        {
            ISectionProperty clone = prop.GetShallowClone() as ISectionProperty;

            double[] modifiers = new double[] { area, iy, iz, j, asy, asz };

            clone.CustomData["Modifiers"] = modifiers;

            return clone;
        }

        /***************************************************/
    }
}

