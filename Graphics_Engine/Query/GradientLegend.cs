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
using BH.oM.Graphics.Colours;
using BH.oM.Graphics;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Geometry;
using System.Drawing;
using BH.oM.Dimensional;
using BH.Engine.Spatial;
using BH.Engine.Base;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Gradients                ****/
        /***************************************************/

        [Description("Constructs a RenderMesh corresponding to the gradient.")]
        [Input("gradient", "The gradient to construct a gradient legend for. Markers on gradient will be used for position and colour of verticies in returned RenderMesh.")]
        [Input("baseCoordinates", "The orientation and position of the returned RenderMesh. Will correspond to the bottom left corner. If nothing is provided, the global XY will be used.")]
        [Input("height", "The full height of the returned RenderMesh.")]
        [Input("width", "The width of the of the RenderMesh.")]
        [Output("legend", "The RenderMesh corresponding to the gradient.")]
        public static RenderMesh GradientLegend(this Gradient gradient, Cartesian baseCoordinates = null, double height = 10, double width = 1)
        {
            if (gradient == null)
                return null;

            //Default to world XY
            baseCoordinates = baseCoordinates ?? new Cartesian();

            RenderMesh mesh = new RenderMesh();

            foreach (KeyValuePair<decimal, System.Drawing.Color> marker in gradient.Markers)
            {
                mesh.Vertices.Add(new RenderPoint
                {
                    Point = baseCoordinates.Origin + ((double)marker.Key * height * baseCoordinates.Y),
                    Colour = marker.Value
                });

                mesh.Vertices.Add(new RenderPoint
                {
                    Point = baseCoordinates.Origin + ((double)marker.Key * height * baseCoordinates.Y) + baseCoordinates.X * width,
                    Colour = marker.Value
                });
            }

            for (int i = 0; i <= mesh.Vertices.Count - 4; i += 2)
            {
                //Add in clockwise order
                //  i-----i+1
                //  |      |
                //  i+2---i+3
                mesh.Faces.Add(new Face
                {
                    A = i,
                    B = i + 1,
                    C = i + 3,
                    D = i + 2
                });
            }

            return mesh;
        }

        /***************************************************/

        [Description("Constructs a RenderMesh corresponding to the gradient.")]
        [Input("gradient", "The gradient to construct a gradient legend for. Markers on gradient will be used for position and colour of verticies in returned RenderMesh.")]
        [Input("baseCoordinates", "The orientation and position of the returned RenderMesh. Will correspond to the bottom left corner. If nothing is provided, the global XY will be used.")]
        [Input("height", "The full height of the returned RenderMesh.")]
        [Input("width", "The width of the of the RenderMesh.")]
        [Output("legend", "The RenderMesh corresponding to the gradient.")]
        public static RenderMesh GradientLegend(this SteppedGradient gradient, Cartesian baseCoordinates = null, double height = 10, double width = 1)
        {
            if (gradient == null)
                return null;

            //Default to world XY
            baseCoordinates = baseCoordinates ?? new Cartesian();

            RenderMesh mesh = new RenderMesh();

            List<KeyValuePair<decimal, Color>> markerList = gradient.Markers.ToList();

            for (int i = 0; i < markerList.Count; i++)
            {
                Color col = markerList[i].Value;
                double lowBound = (double)markerList[i].Key;
                double upperBound;
                if (i < markerList.Count - 1)
                    upperBound = (double)markerList[i + 1].Key;
                else
                {
                    if (lowBound > 1 - Tolerance.Distance)
                        upperBound = 1 + 1.0 / markerList.Count;
                    else
                        upperBound = 1.0;
                }

                mesh.Vertices.Add(new RenderPoint
                {
                    Point = baseCoordinates.Origin + (lowBound * height * baseCoordinates.Y),
                    Colour = col
                }); ;

                mesh.Vertices.Add(new RenderPoint
                {
                    Point = baseCoordinates.Origin + (lowBound * height * baseCoordinates.Y) + baseCoordinates.X * width,
                    Colour = col
                });

                mesh.Vertices.Add(new RenderPoint
                {
                    Point = baseCoordinates.Origin + (upperBound * height * baseCoordinates.Y),
                    Colour = col
                }); ;

                mesh.Vertices.Add(new RenderPoint
                {
                    Point = baseCoordinates.Origin + (upperBound * height * baseCoordinates.Y) + baseCoordinates.X * width,
                    Colour = col
                });

            }

            for (int i = 0; i <= mesh.Vertices.Count - 4; i += 4)
            {
                //Add in clockwise order
                //  i-----i+1
                //  |      |
                //  i+2---i+3
                mesh.Faces.Add(new Face
                {
                    A = i,
                    B = i + 1,
                    C = i + 3,
                    D = i + 2
                });
            }

            return mesh;
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Constructs a RenderMesh corresponding to the gradient.")]
        [Input("gradient", "The gradient to construct a gradient legend for. Markers on gradient will be used for position and colour of verticies in returned RenderMesh.")]
        [Input("baseCoordinates", "The orientation and position of the returned RenderMesh. Will correspond to the bottom left corner. If nothing is provided, the global XY will be used.")]
        [Input("height", "The full height of the returned RenderMesh.")]
        [Input("width", "The width of the of the RenderMesh.")]
        [Output("legend", "The RenderMesh corresponding to the gradient.")]
        public static RenderMesh IGradientLegend(this IGradient gradient, Cartesian baseCoordinates = null, double height = 10, double width = 1)
        {
            if (gradient == null)
                return null;
            return GradientLegend(gradient as dynamic, baseCoordinates, height, width);
        }

        /***************************************************/
        /**** Public Methods - GradientOptions          ****/
        /***************************************************/

        [Description("Constructs a RenderMesh corresponding to the GradientOption with text markers corresponding to the numbers on the gradient markers.")]
        [Input("gradientOptions", "The gradientOptions to construct a gradient legend for. Markers on Gradient will be used for position and colour of verticies in returned RenderMesh and text markers. Name of GradientOptions will be used as legend title.")]
        [Input("baseCoordinates", "The orientation and position of the returned RenderMesh. Will correspond to the bottom left corner. If nothing is provided, the global XY will be used.")]
        [Input("height", "The full height of the returned RenderMesh.")]
        [Input("gradientWidth", "The width of the of the RenderMesh.")]
        [Input("textSize", "The size of the text markers and title.")]
        [Input("significantFigures", "Number of significant figures to be used for text markers.")]
        [MultiOutput(0, "legend", "The RenderMesh corresponding to the gradient.")]
        [MultiOutput(1, "markers", "Legend markers corresponding to values of the gradient option.")]
        [MultiOutput(2, "title", "Title of the legend takes from Name of GradientOptions.")]
        public static Output<RenderMesh, List<RenderText>, RenderText> GradientLegend(this GradientOptions gradientOptions, Cartesian baseCoordinates = null, double height = 10, double gradientWidth = 1, double textSize = 0.2, int significantFigures = 4)
        {
            if (gradientOptions == null)
                return new Output<RenderMesh, List<RenderText>, RenderText>();

            //Default to world XY
            baseCoordinates = baseCoordinates ?? new Cartesian();

            //Create the gradient legend
            RenderMesh mesh = IGradientLegend(gradientOptions.Gradient, baseCoordinates, height, gradientWidth);

            List<RenderText> textMarkers = new List<RenderText>();

            if (!double.IsNaN(gradientOptions.LowerBound) && !double.IsNaN(gradientOptions.UpperBound))
            {
                //Check maximimum number of decimals to be used.
                //This is done to make sure really small numbers do not display with a multitude of decimals when max and min are large numbers
                //Maximum number of decimals takes as number of significant figures + 2 -10 exponent of largest number
                //This means that if the largest value is 2500 and significant figures is 4 the maximum allowed decimals will be 4+2-4=2 decimals
                int maxNbDecimals = -(int)Math.Floor(Math.Log10(Math.Max(Math.Abs(gradientOptions.LowerBound), Math.Abs(gradientOptions.UpperBound)))) + significantFigures + 2;
                maxNbDecimals = Math.Max(maxNbDecimals, 0); //Only allowed to round to positive or 0 number of decimal places

                //Loop through and add text along the markers.
                foreach (KeyValuePair<decimal, Color> marker in gradientOptions.Gradient.Markers)
                {
                    Vector textTranslation = (double)marker.Key * height * baseCoordinates.Y + baseCoordinates.X * (gradientWidth + textSize);
                    double val = gradientOptions.LowerBound + (gradientOptions.UpperBound - gradientOptions.LowerBound) * (double)marker.Key;

                    if (maxNbDecimals <= 15)    //Only alows rounding up to 15 decimal places. Added as a saftey check
                        val = Math.Round(val, maxNbDecimals);

                    textMarkers.Add(new RenderText
                    {
                        Cartesian = baseCoordinates.Translate(textTranslation),
                        Height = textSize,
                        Colour = System.Drawing.Color.Black,
                        Text = ToSignificantDigits(val, significantFigures)
                    });
                }
            }
            //If set, add name on top of gradient
            RenderText title = null;
            if (!string.IsNullOrWhiteSpace(gradientOptions.Name))
            {
                title = new RenderText
                {
                    Cartesian = baseCoordinates.Translate(baseCoordinates.Y * height * 1.1),
                    Height = textSize,
                    Colour = System.Drawing.Color.Black,
                    Text = gradientOptions.Name
                };
            }


            return new Output<RenderMesh, List<RenderText>, RenderText> { Item1 = mesh, Item2 = textMarkers, Item3 = title };
        }

        /***************************************************/

        [Description("Constructs a RenderMesh corresponding to the GradientOption with text markers corresponding to the numbers on the gradient markers. Utilising input objects to calculate a bounding box to be used for automatic size and positioning of the gradient.")]
        [Input("gradientOptions", "The gradientOptions to construct a gradient legend for. Markers on Gradient will be used for position and colour of verticies in returned RenderMesh and text markers. Name of GradientOptions will be used as legend title.")]
        [Input("objects", "Objects used to calculate BoundingBox to be used for automatic positioning of the gradient legend.")]
        [Input("baseCoordinates", "Override of automatic base coordinates. If nothing in provided, the base coordinates will be set to the right of the Bounding box (positive global X) in the global XY plane.")]
        [Input("height", "Height override for the legend. Will use 80% of the bounding box Y direction depth if nothing is provided.")]
        [Input("gradientWidth", "Gradient with override. Will use 1/10 of the height if nothing is provided.")]
        [Input("textSize", "Text size override. Will use the minimum of 1/2 gradientwidth and 0.8*height/number of markers if nothing is provided.")]
        [Input("significantFigures", "Number of significant figures to be used for text markers.")]
        [MultiOutput(0, "legend", "The RenderMesh corresponding to the gradient.")]
        [MultiOutput(1, "markers", "Legend markers corresponding to values of the gradient option.")]
        [MultiOutput(2, "title", "Title of the legend takes from Name of GradientOptions.")]
        public static Output<RenderMesh, List<RenderText>, RenderText> GradientLegend(this GradientOptions gradientOptions, List<List<IObject>> objects, Cartesian baseCoordinates = null, double height = double.NaN, double gradientWidth = double.NaN, double textSize = double.NaN, int significantFigures = 4)
        {
            if (gradientOptions == null)
                return new Output<RenderMesh, List<RenderText>, RenderText>();

            //Get the overall boundingbox of the input geometry
            List<BoundingBox> boxes = new List<BoundingBox>();

            foreach (IObject item in objects.SelectMany(x => x))
            {
                if (item is IGeometry)
                    boxes.Add(((IGeometry)item).IBounds());
                else if (item is IRender)
                    boxes.Add(((IRender)item).IBounds());
                else if (item is IElement)
                    boxes.Add(((IElement)item).IBounds());
                else if (item is IBHoMObject)
                {
                    IGeometry geom = ((IBHoMObject)item).IGeometry();
                    if (geom != null)
                        boxes.Add(geom.IBounds());
                }
            }

            BoundingBox bounds = BH.Engine.Geometry.Query.Bounds(boxes);
            if (bounds == null)
            {
                Base.Compute.RecordWarning("Not able to compute the total bounding box of the provided items.");
                bounds = new BoundingBox();
            }

            if (double.IsNaN(height))
                height = 0.6 * (bounds.Max.Y - bounds.Min.Y);

            if (baseCoordinates == null)
            {
                BH.oM.Geometry.Point basePt = new oM.Geometry.Point { Y = (bounds.Max.Y + bounds.Min.Y) / 2 - height / 2, X = bounds.Max.X + (bounds.Max.X - bounds.Min.X) / 10 };
                baseCoordinates = new Cartesian();
                baseCoordinates.Origin = basePt;
            }

            if (double.IsNaN(gradientWidth))
                gradientWidth = height / 10;

            if (double.IsNaN(textSize))
                textSize = Math.Min(0.3 * height / gradientOptions.Gradient.Markers.Count, gradientWidth / 2);

            return GradientLegend(gradientOptions, baseCoordinates, height, gradientWidth, textSize, significantFigures);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Format the value with the indicated number of significant digits.
        private static string ToSignificantDigits(double value, int significant_digits)
        {
            //method taken from http://csharphelper.com/blog/2016/07/display-significant-digits-in-c/

            // Use G format to get significant digits.
            // Then convert to double and use F format.
            string format1 = "{0:G" + significant_digits.ToString() + "}";
            string result = System.Convert.ToDouble(
                String.Format(format1, value)).ToString("F99");

            // Rmove trailing 0s.
            result = result.TrimEnd('0');

            // Rmove the decimal point and leading 0s,
            // leaving just the digits.
            string test = result.Replace(".", "").TrimStart('0');

            // See if we have enough significant digits.
            if (significant_digits > test.Length)
            {
                // Add trailing 0s.
                result += new string('0', significant_digits - test.Length);
            }
            else
            {
                // See if we should remove the trailing decimal point.
                if ((significant_digits < test.Length) &&
                    result.EndsWith("."))
                    result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        /***************************************************/
    }
}

