/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using System;
using BH.Engine.Reflection;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a rectangular hollow profile with outstands based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("topFlangeThickness")]
        [InputFromProperty("botFlangeThickness")]
        [Input("topCorbelWidth", "Width of the outstands of each side at the top of the profile.", typeof(Length))]
        [Input("botCorbelWidth", "Width of the outstands of each side at the bottom of the profile.", typeof(Length))]
        [Output("fabBox", "The created GeneralisedFabricatedBoxProfile.")]
        public static GeneralisedFabricatedBoxProfile GeneralisedFabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness = 0.0, double botFlangeThickness = 0.0, double topCorbelWidth = 0.0, double botCorbelWidth = 0.0)
        {
            if (webThickness >= width / 2)
            {
                InvalidRatioError("webThickness", "width");
                return null;
            }

            if (height <= topFlangeThickness + botFlangeThickness)
            {
                InvalidRatioError("height", "topFlangeThickness and botFlangeThickness");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || topFlangeThickness <= 0 || botFlangeThickness <= 0 || topCorbelWidth < 0 || botCorbelWidth < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = GeneralisedFabricatedBoxProfileCurves(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, topCorbelWidth, botCorbelWidth, botCorbelWidth);
            return new GeneralisedFabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, topCorbelWidth, topCorbelWidth, botCorbelWidth, botCorbelWidth, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> GeneralisedFabricatedBoxProfileCurves(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double topLeftCorbelWidth, double topRightCorbelWidth, double botLeftCorbelWidth, double botRightCorbelWidth)
        {

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            List<ICurve> externalEdges = new List<ICurve>();
            List<ICurve> internalEdges = new List<ICurve>();
            List<ICurve> group = new List<ICurve>();
            Point p1 = new Point { X = 0, Y = 0, Z = 0 };
            Point p2 = new Point { X = 0, Y = botFlangeThickness, Z = 0 };

            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + xAxis * ((width / 2) + botRightCorbelWidth) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + yAxis * botFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - xAxis * botRightCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + yAxis * (height - botFlangeThickness - topFlangeThickness) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + xAxis * topRightCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + yAxis * topFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - xAxis * (width + topRightCorbelWidth + topLeftCorbelWidth) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - yAxis * topFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + xAxis * topLeftCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - yAxis * (height - botFlangeThickness - topFlangeThickness) });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - xAxis * botLeftCorbelWidth });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 - yAxis * botFlangeThickness });
            externalEdges.Add(new Line { Start = p1, End = p1 = p1 + xAxis * ((width / 2) + botLeftCorbelWidth) });

            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + xAxis * ((width / 2) - webThickness) });
            internalEdges.Add(new Line { Start = p2, End = p2 = p2 + yAxis * (height - botFlangeThickness - topFlangeThickness) });
            internalEdges.Add(new Line { Start = p2, End = p2 = p2 - xAxis * ((width / 2) - webThickness) });

            int intCount = internalEdges.Count;
            for (int i = 0; i < intCount; i++)
            {
                internalEdges.Add(internalEdges[i].IMirror(new Plane { Origin = origin, Normal = xAxis }));
            }

            Point centroid = externalEdges.IJoin().Centroid(internalEdges.IJoin());
            Vector translation = Point.Origin - centroid;

            group.AddRange(externalEdges);
            group.AddRange(internalEdges);

            return group.Select(x => x.ITranslate(translation)).ToList();
        }

        /***************************************************/

    }
}




