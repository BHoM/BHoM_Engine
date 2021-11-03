/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.CoordinateSystem;
using System;
using BH.Engine.Reflection;
using BH.oM.Reflection.Attributes;
using BH.Engine.Geometry;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a I-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [Output("I", "The created ISectionProfile.")]
        public static TaperFlangeISectionProfile TaperFlangeISectionProfile(double height, double width, double webThickness, double flangeThickness, double flangeSlope, double rootRadius, double toeRadius)
        {
            if (height < flangeThickness * 2 + rootRadius * 2 || height <= flangeThickness * 2)
            {
                InvalidRatioError("height", "flangeThickness and rootRadius");
                return null;
            }

            if (width < webThickness + rootRadius * 2 + toeRadius * 2)
            {
                InvalidRatioError("width", "webthickness, rootRadius and toeRadius");
                return null;
            }

            if (flangeSlope < 0)
            {
                Reflection.Compute.RecordError("Flange slope must be positive. Suggest approximately 0.16 radians");
                return null;
            }            

            if (flangeSlope > Math.Atan(4 * flangeThickness / width))
            {
                InvalidRatioError("Width", "FlangeThickness");
                return null;
            }

            if (toeRadius > flangeThickness - width / 4 * Math.Tan(flangeSlope))
            {
                InvalidRatioError("toeRadius", "flangeThickness");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || flangeThickness <= 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Reflection.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = TaperFlangeIProfileCurves(flangeThickness, width, flangeThickness, width, flangeSlope, webThickness, height, rootRadius, toeRadius);
            return new TaperFlangeISectionProfile(height, width, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> TaperFlangeIProfileCurves(double tft, double tfw, double bft, double bfw, double slope, double wt, double height, double r1, double r2)
        {
            List<ICurve> perimeter = new List<ICurve>();
            Point p = new Point { X = bfw / 2, Y = 0, Z = 0 };

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            Line l1 = new Line { Start = p, End = p = p + yAxis * (bft - bfw / 4 * Math.Tan(slope)) };
            Line l2 = new Line { Start = p, End = p = p - xAxis * (bfw - wt) / 2 + yAxis * ((bfw - wt) / 2 * Math.Tan(slope)) };
            Line l3 = new Line { Start = p, End = p = p + yAxis * (height - (bft + bfw/4 * Math.Tan(slope)) - (tft + tfw / 4 * Math.Tan(slope)))};
            Line l4 = new Line { Start = p, End = p = p + xAxis * (tfw - wt) / 2 + yAxis * ((tfw - wt) / 2 * Math.Tan(slope)) };
            Line l5 = new Line { Start = p, End = p = p + yAxis * (tft - tfw / 4 * Math.Tan(slope)) };

            //perimeter = new List<ICurve> { l1, l2, l3, l4, l5 };

            List<ICurve> fillet = Fillet(l1, l2, r2);
            perimeter.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l3, r1);
            perimeter.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l4, r1);
            perimeter.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l5, r2);
            perimeter.AddRange(fillet);

            int count = perimeter.Count;
            for (int i = 0; i < count; i++)
            {
                perimeter.Add(perimeter[i].IMirror(new Plane { Origin = origin, Normal = xAxis }));
            }
            perimeter.Add(new Line { Start = p, End = p - xAxis * (tfw) });
            perimeter.Add(new Line { Start = origin + xAxis * (-bfw / 2), End = origin + xAxis * (bfw / 2) });


            return perimeter;
        }

        /***************************************************/

        private static List<ICurve> Fillet(Line l1, Line l2, double r)
        {
            // Check that the radius is not absurd- if it is, return the two lines un-modified. If the radius is sensible, return l1 trimmed, arc, l2 trimmed.
            if (r > 0 && l1.End.Distance(l2.Start) == 0)
            {
                Arc fillet = Fillet(l1.Start, l1.End, l2.End, r);
                if (fillet != null)
                {
                    l1 = new Line { Start = l1.Start, End = fillet.StartPoint() };
                    l2 = new Line { Start = fillet.EndPoint(), End = l2.End };

                    return new List<ICurve> { l1, fillet, l2 };
                }
            }
            return new List<ICurve> { l1, l2 };
        }

        private static Arc Fillet(Point a, Point b, Point c, double r)
        {
            double len1 = a.Distance(b);
            double len2 = b.Distance(c);

            if (len1 == 0.0 || len2 == 0.0)
            {
                return null;
            }

            Vector vector1 = (a - b).Normalise();
            Vector vector2 = (c - b).Normalise();

            double angle = vector1.Angle(vector2);

            Vector localZ = vector1.CrossProduct(vector2).Normalise();

            double trim = r / Math.Tan(angle / 2);

            if (trim > len1 || trim > len2)
            {
                return null;
            }

            Point p = b + vector1 * trim + r * localZ.CrossProduct(vector1);

            Vector localX = vector1.CrossProduct(localZ);
            Vector localY = -vector1;

            Cartesian center = Geometry.Create.CartesianCoordinateSystem(p, localX, localY);

            return Geometry.Create.Arc(center, r, 0, Math.PI - angle);
        }
    }
}