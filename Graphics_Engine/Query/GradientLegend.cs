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

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static RenderMesh GradientLegend(this Gradient gradient, Cartesian baseCoordinates = null, double height = 10, double width = 1)
        {
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

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static RenderMesh GradientLegend(this SteppedGradient gradient, Cartesian baseCoordinates = null, double height = 10, double width = 1)
        {
            //Default to world XY
            baseCoordinates = baseCoordinates ?? new Cartesian();

            RenderMesh mesh = new RenderMesh();

            List<KeyValuePair<decimal,Color>> markerList = gradient.Markers.ToList();

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

        public static RenderMesh IGradientLegend(this IGradient gradient, Cartesian baseCoordinates = null, double height = 10, double width = 1)
        {
            return GradientLegend(gradient as dynamic, baseCoordinates, height, width);
        }

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static Output<RenderMesh, List<RenderText>> GradientLegend(this GradientOptions gradientOptions, Cartesian baseCoordinates = null, double height = 10, double gradientWidth = 1, double textSize = 0.2, int significantFigures = 4)
        {
            //Default to world XY
            baseCoordinates = baseCoordinates ?? new Cartesian();

            //Create the gradient legend
            RenderMesh mesh = IGradientLegend(gradientOptions.Gradient, baseCoordinates, height, gradientWidth);

            List<RenderText> textMarkers = new List<RenderText>();

            //Loop through and add text along the markers.
            foreach (KeyValuePair<decimal, System.Drawing.Color> marker in gradientOptions.Gradient.Markers)
            {
                Vector textTranslation = (double)marker.Key * height * baseCoordinates.Y + baseCoordinates.X * (gradientWidth + textSize);
                textMarkers.Add(new RenderText
                {
                    Cartesian = baseCoordinates.Translate(textTranslation),
                    Height = textSize,
                    Colour = System.Drawing.Color.Black,
                    Text = ToSignificantDigits((gradientOptions.LowerBound + (gradientOptions.UpperBound - gradientOptions.LowerBound) * (double)marker.Key), significantFigures)
                });
                
            }

            return new Output<RenderMesh, List<RenderText>> { Item1 = mesh, Item2 = textMarkers };
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
