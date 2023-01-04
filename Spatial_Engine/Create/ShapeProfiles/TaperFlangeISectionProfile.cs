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
using System.Linq;
using System.Collections.ObjectModel;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
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

        [Description("Creates a I-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("flangeSlope")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [Output("I", "The created ISectionProfile.")]
        public static TaperFlangeISectionProfile TaperFlangeISectionProfile(double height, double width, double webThickness, double flangeThickness, double flangeSlope, double rootRadius, double toeRadius)
        {
            if (height < 2 * (flangeThickness + flangeSlope * (width / 4 - webThickness / 2))) // assume no fillets
            {
                InvalidRatioError("height", "flangeThickness, flangeSlope, webThickness");
                return null;
            }

            if (width < webThickness) // Assume no fillets
            {
                InvalidRatioError("width", "webThickness");
                return null;
            }

            if (flangeSlope > flangeThickness / (width/4)) // Assume no fillets
            {
                InvalidRatioError("Width", "FlangeThickness and flangeSlope");
                return null;
            }

            // check that the root radius doesn't take up the whole web
            if (height < 2 * (flangeThickness + flangeSlope * (width / 4 - webThickness / 2 - rootRadius) + Math.Sqrt(Math.Pow(rootRadius, 2) + Math.Pow(flangeSlope * rootRadius, 2))))
            {
                InvalidRatioError("rootRadius", "flangeThickness, flangeSlope, flangeWidth, webThickness, and height");
                return null;
            }

            // check that the toe radius doesn't eliminate the face of the toe
            if (Math.Sqrt(Math.Pow(toeRadius,2) + Math.Pow(flangeSlope * toeRadius, 2)) > flangeThickness - flangeSlope * (width / 4 - toeRadius))
            {
                InvalidRatioError("toeRadius", "flangeThickness, flangeSlope, and width");
                return null;
            }

            // check that the toe and root radii don't eliminate the inner flange face
            if (width < webThickness + 2 * (1 - flangeSlope)*( rootRadius + toeRadius ))
            {
                InvalidRatioError("width", "webthickness, rootRadius, and toeRadius");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || flangeThickness <= 0 || flangeSlope < 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
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
            List<ICurve> edges = new List<ICurve>();
            Point p = new Point { X = bfw / 2, Y = 0, Z = 0 };

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;
            Point origin = oM.Geometry.Point.Origin;

            Line l1 = new Line { Start = p, End = p = p + yAxis * (bft - bfw / 4 * slope) };
            Line l2 = new Line { Start = p, End = p = p - xAxis * (bfw - wt) / 2 + yAxis * ((bfw - wt) / 2 * slope) };
            Line l3 = new Line { Start = p, End = p = p + yAxis * (height - (bft + (bfw/4 - wt/2) * slope + (tft + (tfw/4 - wt/2) * slope)))};
            Line l4 = new Line { Start = p, End = p = p + xAxis * (tfw - wt) / 2 + yAxis * ((tfw - wt) / 2 * slope) };
            Line l5 = new Line { Start = p, End = p = p + yAxis * (tft - tfw / 4 * slope) };

            List<ICurve> fillet = Fillet(l1, l2, r2);
            edges.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l3, r1);
            edges.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l4, r1);
            edges.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l5, r2);
            edges.AddRange(fillet);

            int count = edges.Count;
            for (int i = 0; i < count; i++)
            {
                edges.Add(edges[i].IMirror(new Plane { Origin = origin, Normal = xAxis }));
            }
            edges.Add(new Line { Start = p, End = p - xAxis * (tfw) });
            edges.Add(new Line { Start = origin + xAxis * (-bfw / 2), End = origin + xAxis * (bfw / 2) });

            Point centroid = edges.IJoin().Centroid();
            Vector translation = Point.Origin - centroid;
            return edges.Select(x => x.ITranslate(translation)).ToList();
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

