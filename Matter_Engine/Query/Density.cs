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
using BH.oM.Physical.Elements;

using BH.Engine.Base;
using BH.oM.Physical.FramingProperties;
using BH.oM.Physical.Materials;
using BH.oM.Reflection;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the density of a Material by querying each of its IMaterialProperties for theirs." +
                     "The density is either gotten from a method or a property with that name." + 
                     "If the density is found on diffrent IMaterialProperties, the avarage is taken.")]
        [Input("material", "The material to get the density of")]
        [Input("type", "The kind of IMaterialProperties to cull the result by")]
        [Output("density", "The density of the material, further info on how the value was accuired is recorded in the warning", typeof(Density))]
        public static double Density(this Material material, Type type = null)
        {
            if (type == null)
                type = typeof(IMaterialProperties);

            List<double> densities = new List<double>();
            List<string> warnings = new List<string>() { "Density report for the Material " + material.Name + ":" };

            foreach (IMaterialProperties mat in material.Properties.Where(x => type.IsAssignableFrom(x.GetType())))
            {
                Output<double, string> result = IDensityWithReport(mat, 293, 0.8);
                if (!double.IsNaN(result.Item1))
                {
                    if (!string.IsNullOrWhiteSpace(result.Item2))
                        warnings.Add("  " + result.Item2);
                    densities.Add(result.Item1);
                }
            }

            if (densities.Count == 0)
            {
                Reflection.Compute.RecordWarning("no density on any of the fragments of " + material.Name + " by type " + type.Name);
                return 0;
            }
            if (densities.Count > 1)
                warnings.Add("The density for " + material.Name + " is a average of multiple Fragments");

            if (warnings.Count > 1)
                Reflection.Compute.RecordWarning(string.Join(System.Environment.NewLine, warnings.ToArray()));

            return densities.Average();
        }

        /***************************************************/

        [Description("Gets the density of a IMaterialProperties. The density is either gotten from a method or a property with that name.")]
        [Input("material", "The IMaterialProperties to get the density of")]
        [Input("temprature", "The temprature to get the density at", typeof(Temperature))]
        [Input("relativeHumidity", "The humidity to get the density at", typeof(Ratio))]
        [Output("density", "The density of the IMaterialProperties, further info on how the value was accuired is recorded in the warning", typeof(Density))]
        public static double IDensity(this IMaterialProperties materialProp, double temperature, double relativeHumidity)
        {
            Output<double, string> result = IDensityWithReport(materialProp, temperature, relativeHumidity);
            Reflection.Compute.RecordWarning(result.Item2);
            return result.Item1;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Output<double, string> IDensityWithReport(this IMaterialProperties materialProp, double temperature, double relativeHumidity)
        {
            object density = null;
            Output<double, string> result = new Output<double, string>();

            density = Reflection.Compute.RunExtensionMethod(materialProp, "Density", new object[] { temperature, relativeHumidity });
            if (density != null)
            {
                return new Output<double, string>()
                {
                    Item1 = System.Convert.ToDouble(density),
                    Item2 = ""
                };
            }
            // Does not work without more sofisticated use of reflection to check input param names

            //density = Reflection.Compute.RunExtensionMethod(materialProp, "Density", new object[] { temperature });
            //if (density != null)
            //{
            //    return new Output<double, string>()
            //    {
            //        Item1 = System.Convert.ToDouble(density),
            //        Item2 = UnUsedVaribles(materialProp.Name, "relativeHumidity")
            //    };
            //}

            //density = Reflection.Compute.RunExtensionMethod(materialProp, "Density", new object[] { relativeHumidity });
            //if (density != null)
            //{
            //    return new Output<double, string>()
            //    {
            //        Item1 = System.Convert.ToDouble(density),
            //        Item2 = UnUsedVaribles(materialProp.Name, "temperature")
            //    };
            //}

            density = Reflection.Compute.RunExtensionMethod(materialProp, "Density");
            if (density != null)
            {
                return new Output<double, string>()
                {
                    Item1 = System.Convert.ToDouble(density),
                    Item2 = UnUsedVaribles(materialProp.Name, "temperature and relativeHumidity")
                };
            }

            density = Reflection.Query.PropertyValue(materialProp, "Density");
            if (density != null)
            {
                return new Output<double, string>()
                {
                    Item1 = System.Convert.ToDouble(density),
                    Item2 = "The value of the density for the MaterialFragment: " + materialProp.Name + " was acquired through its properties"
                };
            }

            return new Output<double, string>()
            {
                Item1 = double.NaN,
                Item2 = "The density could not be acquired from the MaterialFragment " + materialProp.Name
            };
        }

        /***************************************************/

        private static string UnUsedVaribles(string name, string missing)
        {
            return "The value of the density for the MaterialFragment: " + name + " was acquired without the input: " + missing;
        }

        /***************************************************/

    }
}
