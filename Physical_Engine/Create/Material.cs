/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.oM.Physical.Materials;
using BH.oM.Physical.Materials.Options;
using BH.Engine.Matter;

using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Physical
{
    public static partial class Create
    {

        /***************************************************/

        [PreviousVersion("6.0", "BH.Engine.Physical.Create.Material(System.String, System.Collections.Generic.List<BH.oM.Physical.Materials.IMaterialProperties>)")]
        [Description("Returns a Material object")]
        [Input("name", "The name of the material, default empty string")]
        [Input("properties", "A collection of the specific properties of the material to be created, default null")]
        [Output("A Material object")]
        public static Material Material(string name = "", List<IMaterialProperties> properties = null, double density = double.NaN, DensityExtractionOptions densityOptions = null)
        {
            properties = properties ?? new List<IMaterialProperties>();

            if (double.IsNaN(density))
            {
                List<IDensityProvider> densityProviders = properties.OfType<IDensityProvider>().ToList();
                density = densityProviders.Density(densityOptions, densityOptions != null); //Only raise warnings and errors if options provided
            }

            return new Material
            {
                Name = name,
                Properties = properties,
                Density = density
            };
        }

        /***************************************************/

        [Description("Returns a Material object")]
        [Input("property", "The specific property of the material to be created, its name will be carried over to the Material")]
        [Output("A Material object")]
        public static Material Material(IMaterialProperties property)
        {
            if(property == null)
            {
                Base.Compute.RecordError("Cannot create a Physical.Material from a null set of material properties.");
                return null;
            }

            IDensityProvider densityProp = property as IDensityProvider;
            double density = double.NaN;
            if (densityProp != null)
                density = densityProp.Density;

            return new Material
            {
                Name = property.Name,
                Properties = new List<IMaterialProperties>() { property },
                Density = density

            };
        }

        /***************************************************/

    }
}



