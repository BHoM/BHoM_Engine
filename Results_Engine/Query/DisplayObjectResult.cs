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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using System.Linq;
using System.Reflection;
using BH.oM.Geometry;
using BH.oM.Structure.Results;
using BH.oM.Graphics;
using System;
using BH.Engine.Graphics;
using BH.Engine.Geometry;
using BH.Engine.Base;
using BH.Engine.Analytical;
using BH.oM.Analytical.Results;
using BH.oM.Analytical.Elements;
using BH.oM.Graphics.Colours;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generates a list of coloured geometry based on provided BHoMObjects and ObjectResults.")]
        [Input("objects", "BHoMObjects to colour.")]
        [Input("results", "The IObjectResults to colour by.")]
        [Input("objectIdentifier", "Should either be a string specifying what property on the object that should be used to map the objects to the results, or a type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Input("filter", "Optional filter for the results. If nothing is provided, all results will be used.")]
        [Input("displayProperty", "The name of the property on the result to colour by. If nothing is provided, the first available property will be used.")]
        [Input("gradientOptions", "How to color the mesh, null defaults to `BlueToRed` with automatic range.")]
        [MultiOutput(0, "results", "A List of Lists of RenderGeometry, where the outer list corresponds to the object and the inner list correspond to the matchis results..")]
        [MultiOutput(1, "gradientOptions", "The gradientOptions that were used to colour the meshes.")]
        public static Output<List<List<RenderGeometry>>, GradientOptions> DisplayObjectResult(this IEnumerable<IBHoMObject> objects, IEnumerable<IObjectResult> results, string displayProperty = "", object objectIdentifier = null, ResultFilter filter = null, GradientOptions gradientOptions = null)
        {
            if (objects == null || objects.Count() < 1)
            {
                Engine.Base.Compute.RecordError("No objects found. Make sure that your objects are input correctly.");
                return new Output<List<List<RenderGeometry>>, GradientOptions>
                {
                    Item1 = new List<List<RenderGeometry>>(),
                    Item2 = gradientOptions
                };
            }
            if (results == null || results.Count() < 1)
            {
                Engine.Base.Compute.RecordError("No results found. Make sure that your results are input correctly.");
                return new Output<List<List<RenderGeometry>>, GradientOptions>
                {
                    Item1 = new List<List<RenderGeometry>>(),
                    Item2 = gradientOptions
                };
            }

            if (gradientOptions == null)
                gradientOptions = new GradientOptions();

            //Get tuple with result name, property selector function and quantity attribute
            Output<string, Func<IResult, double>, QuantityAttribute> propName_selector_quantity = results.First().ResultValueProperty(displayProperty, filter);
            Func<IResult, double> resultPropertySelector = propName_selector_quantity?.Item2;

            if (resultPropertySelector == null)
            {
                return new Output<List<List<RenderGeometry>>, GradientOptions>
                {
                    Item1 = new List<List<RenderGeometry>>(),
                    Item2 = gradientOptions
                };
            }

            List<IBHoMObject> objectList = objects.ToList();

            // Map the Results to Objects
            List<List<IObjectResult>> mappedResults = objectList.MapResultsToObjects(results, "ObjectId", objectIdentifier, filter);

            if (mappedResults.Count == 0 || mappedResults.All(x => x.Count == 0))
            {
                Engine.Base.Compute.RecordError("No results able to be mapped to the objects or all filtered out. Please check the inputs.");
                return new Output<List<List<RenderGeometry>>, GradientOptions>();
            }

            //Extract result value to display
            List<List<double>> resValues = mappedResults.Select(l => l.Select(resultPropertySelector).ToList()).ToList();

            //Set up gradient based on values
            gradientOptions = gradientOptions.ApplyGradientOptions(resValues.SelectMany(x => x));
            double from = gradientOptions.LowerBound;
            double to = gradientOptions.UpperBound;
            IGradient gradient = gradientOptions.Gradient;

            //If unset, set name of gradient options to match property and unit
            if (string.IsNullOrWhiteSpace(gradientOptions.Name))
            {
                gradientOptions.Name = propName_selector_quantity.Item1;
                QuantityAttribute quantity = propName_selector_quantity.Item3;
                if (quantity != null)
                    gradientOptions.Name += $" [{quantity.SIUnit}]";
            }

            List<List<RenderGeometry>> renderGeometries = new List<List<RenderGeometry>>();
            for (int i = 0; i < objectList.Count; i++)
            {
                IGeometry geometry = objectList[i].IGeometry();
                List<RenderGeometry> resultDisplays = new List<RenderGeometry>();
                for (int j = 0; j < resValues[i].Count; j++)
                {
                    resultDisplays.Add(new RenderGeometry
                    {
                        Geometry = geometry,
                        Colour = gradient.Color(resValues[i][j], from, to)
                    });
                }
                renderGeometries.Add(resultDisplays);
            }

            return new Output<List<List<RenderGeometry>>, GradientOptions>
            {
                Item1 = renderGeometries,
                Item2 = gradientOptions
            };
        }

        /***************************************************/
    }
}

