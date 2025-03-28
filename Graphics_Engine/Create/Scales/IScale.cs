/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Data.Collections;
using BH.oM.Graphics.Scales;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Create 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Create a scale based on an input domain and out range.")]
        [Input("domain", "The configuration properties for the box representation.")]
        [Input("range", "Dataset of a BH.oM.Analytical.Elements.Graph where Graph.Entities are one element of type BHoMGroup in Dataset.Data and Graph.Relations are another element of type BHoMGroup in Dataset.Data.")]
        [Output("scale", "The created scale.")]
        public static IScale IScale(List<object> domain, List<object> range)
        {
            IScale scale = null;

            if (domain.All(d => d.IsNumericType()) && range.All(d => d.IsNumericType()))
            {
                List<double> d = new List<double>();
                domain.ForEach(x => d.Add(System.Convert.ToDouble(x)));
                List<double> r = new List<double>();
                range.ForEach(x => r.Add(System.Convert.ToDouble(x)));

                scale = new ScaleLinear()
                {
                    Domain = Data.Create.Domain(d),
                    Range = Data.Create.Domain(r)
                };
                return scale;
            }

            else
            {
                List<string> d = domain.ToStringList().Distinct().ToList();

                scale = new ScaleOrdinal()
                {
                    Domain = d,
                    Range = range
                };
            }

            
            //date time
            //non linear
            //others to do
            return scale;
        }

        /***************************************************/

        private static List<string> ToStringList(this List<object> objs)
        {
            List<string> strings = new List<string>();
            objs.ForEach(o => strings.Add(o.ToString()));
            return strings;
        }

    }
}




