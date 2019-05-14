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
using BH.oM.Physical.Materials;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Assigns a material fragemnt to a material. First removes any previous instances of the IMaterialFragment from the list, then adds the new")]
        [Input("material", "The physical material to add a structural fragment to.")]
        [Input("structuralFragment", "The structural fragment to be appended to the material. Will replace any previous instance")]
        public static Material SetStructuralFragment(this Material material, IMaterialFragment structuralFragment)
        {
            //Clone the object
            Material clone = material.GetShallowClone() as Material;

            //null check for the list
            clone.Properties = clone.Properties ?? new List<IMaterialProperties>();

            //Remove any reference to old structural material fragment. Only one is allowed
            clone.Properties = clone.Properties.Where(x => !(x is IMaterialFragment)).ToList();

            //Assign the new fragment
            clone.Properties.Add(structuralFragment);

            return clone;
        }

        /***************************************************/


        [Description("Creates a aluminium material. First constructs a Aluminium material fragment, then applies it to a new Material class")]
        [Input("E", "Youngs modulus. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("v", "Poissons Ratio. Will be stored on the material fragment")]
        [Input("tC", "Modulus of thermal expansion. Given in [1/°C] or [1/K]. Will be stored on the material fragment")]
        [Input("density", "Given as [kg/m3]. Will be stored on the base material")]
        [Input("dampingRatio", "Dynamic damping ratio of the material. Will be stored on the material fragment")]
        [Output("Material", "The created material with a aluminium fragment")]
        public static Material SetAluminium(this Material material, string name, double E = 70000000000, double v = 0.34, double tC = 0.000023, double density = 2710, double dampingRatio = 0)
        {
            Aluminium alumniniumFragment = new Aluminium()
            {
                Name = name,
                YoungsModulus = E,
                PoissonsRatio = v,
                ThermalExpansionCoeff = tC,
                DampingRatio = dampingRatio,
            };

            Material clone = SetStructuralFragment(material, alumniniumFragment);
            clone.Density = density;
            return clone;
        }

        /***************************************************/

        [Description("Creates a concrete material. First constructs a concrete material fragment, then applies it to a new Material class")]
        [Input("E", "Youngs modulus. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("v", "Poissons Ratio. Will be stored on the material fragment")]
        [Input("tC", "Modulus of thermal expansion. Given in [1/°C] or [1/K]. Will be stored on the material fragment")]
        [Input("density", "Given as [kg/m3]. Will be stored on the base material")]
        [Input("dampingRatio", "Dynamic damping ratio of the material. Will be stored on the material fragment")]
        [Input("cubeStrength", "Cube strength of the concrete material. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("cylinderStrength", "Cylinder strength of the concrete material. Given in [Pa] or [N/m].Will be stored on the material fragment")]
        [Output("Material", "The created material with a concrete fragment")]
        public static Material SetConcrete(this Material material, string name, double E = 33000000000, double v = 0.2, double tC = 0.00001, double density = 2550, double dampingRatio = 0, double cubeStrength = 0, double cylinderStrength = 0)
        {
            Concrete concreteFragment = new Concrete()
            {
                Name = name,
                YoungsModulus = E,
                PoissonsRatio = v,
                ThermalExpansionCoeff = tC,
                CubeStrength = cubeStrength,
                DampingRatio = dampingRatio,
                CylinderStrength = cylinderStrength
            };

            Material clone = SetStructuralFragment(material, concreteFragment);
            clone.Density = density;
            return clone;
        }

        /***************************************************/

        [Description("Creates a steelmaterial. First constructs a Steel material fragment, then applies it to a new Material class")]
        [Input("E", "Youngs modulus. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("v", "Poissons Ratio. Will be stored on the material fragment")]
        [Input("tC", "Modulus of thermal expansion. Given in [1/°C] or [1/K]. Will be stored on the material fragment")]
        [Input("density", "Given as [kg/m3]. Will be stored on the base material")]
        [Input("dampingRatio", "Dynamic damping ratio of the material. Will be stored on the material fragment")]
        [Input("yieldStress", "Stress level at yield for the material. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("ultimateStress", "Stress level at break for the material. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Output("Material", "The created material with a steel fragment")]
        public static Material SetSteel(this Material material, string name, double E = 210000000000, double v = 0.3, double tC = 0.000012, double density = 7850, double dampingRatio = 0, double yieldStress = 0, double ultimateStress = 0)
        {
            Steel steelFragment = new Steel()
            {
                Name = name,
                YoungsModulus = E,
                PoissonsRatio = v,
                ThermalExpansionCoeff = tC,
                DampingRatio = dampingRatio,
                YieldStress = yieldStress,
                UltimateStress = ultimateStress
            };

            Material clone = SetStructuralFragment(material, steelFragment);
            clone.Density = density;
            return clone;
        }

        /***************************************************/

        [Description("Creates a timber material. First constructs a timber material fragment, then applies it to a new Material class")]
        [Input("E", "Youngs modulus as Vector. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("G", "Youngs modulus as Vector. Given in [Pa] or [N/m]. Will be stored on the material fragment")]
        [Input("v", "Poissons Ratio as Vector. Will be stored on the material fragment")]
        [Input("tC", "Modulus of thermal expansion as Vector. Given in [1/°C] or [1/K]. Will be stored on the material fragment")]
        [Input("density", "Given as [kg/m3]. Will be stored on the base material")]
        [Input("dampingRatio", "Dynamic damping ratio of the material. Will be stored on the material fragment")]
        [Output("Material", "The created material with a timber fragment")]
        public static Material SetTimber(this Material material, string name, Vector E, Vector v, Vector G, Vector tC, double density, double dampingRatio)
        {
            Timber timberFragment = new Timber()
            {
                Name = name,
                YoungsModulus = E,
                PoissonsRatio = v,
                ShearModulus = G,
                ThermalExpansionCoeff = tC,
                DampingRatio = dampingRatio,
            };

            Material clone = SetStructuralFragment(material, timberFragment);
            clone.Density = density;
            return clone;
        }

        /***************************************************/
    }
}
