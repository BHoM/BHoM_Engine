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

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a rectangular hollow profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("topFlangeThickness")]
        [InputFromProperty("botFlangeThickness")]
        [InputFromProperty("weldSize")]
        [Output("fabBox", "The created BoxProfile.")]
        public static FabricatedBoxProfile FabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize = 0)
        {
            if (height < topFlangeThickness + botFlangeThickness + 2 * Math.Sqrt(2) * weldSize || height <= topFlangeThickness + botFlangeThickness)
            {
                InvalidRatioError("height", "topFlangeThickness, botFlangeThickness and weldSize");
                return null;
            }

            if (width < webThickness * 2 + 2 * Math.Sqrt(2) * weldSize || width <= webThickness * 2)
            {
                InvalidRatioError("width", "webThickness and weldSize");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || topFlangeThickness <= 0 || botFlangeThickness <= 0 || weldSize < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = FabricatedBoxProfileCurves(width, height, webThickness, topFlangeThickness, botFlangeThickness, weldSize);
            return new FabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> FabricatedBoxProfileCurves(double width, double height, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            List<ICurve> box = RectangleProfileCurves(width, height, 0);

            List<ICurve> welds = new List<ICurve>();
            double weldLength = weldSize * 2 / Math.Sqrt(2);
            Point q1 = new Point { X = (width / 2) - webThickness, Y = (height / 2) - topFlangeThickness, Z = 0 };
            Point q2 = new Point { X = -(width / 2) + webThickness, Y = (height / 2) - topFlangeThickness, Z = 0 };
            Point q3 = new Point { X = -(width / 2) + webThickness, Y = -(height / 2) + botFlangeThickness, Z = 0 };
            Point q4 = new Point { X = (width / 2) - webThickness, Y = -(height / 2) + botFlangeThickness, Z = 0 };
            Vector wx = new Vector { X = weldLength, Y = 0, Z = 0 };
            Vector wy = new Vector { X = 0, Y = weldLength, Z = 0 };

            List<ICurve> innerBox = new List<ICurve>();

            if (weldSize > 0)
            {
                welds.Add(new Line { Start = q1 - wx, End = q1 - wy });
                welds.Add(new Line { Start = q2 + wx, End = q2 - wy });
                welds.Add(new Line { Start = q3 + wx, End = q3 + wy });
                welds.Add(new Line { Start = q4 - wx, End = q4 + wy });
                innerBox.AddRange(welds);
            }


            innerBox.Add(new Line { Start = q1 - wy, End = q4 + wy });
            innerBox.Add(new Line { Start = q4 - wx, End = q3 + wx });
            innerBox.Add(new Line { Start = q3 + wy, End = q2 - wy });
            innerBox.Add(new Line { Start = q2 + wx, End = q1 - wx });

            Point centroid = box.IJoin().Centroid(innerBox.IJoin());
            Vector translation = Point.Origin - centroid;

            box.AddRange(innerBox);

            return box.Select(x => x.ITranslate(translation)).ToList();
        }

        /***************************************************/

    }
}




