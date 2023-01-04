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

        [Description("Creates a C-shaped profile based on input dimensions. Method generates edge curves based on the inputs.")]
        [InputFromProperty("height")]
        [InputFromProperty("width")]
        [InputFromProperty("webThickness")]
        [InputFromProperty("flangeThickness")]
        [InputFromProperty("flangeSlope")]
        [InputFromProperty("rootRadius")]
        [InputFromProperty("toeRadius")]
        [InputFromProperty("mirrorAboutLocalZ")]
        [Output("channel", "The created ChannelProfile.")]
        public static TaperFlangeChannelProfile TaperFlangeChannelProfile(double height, double width, double webThickness, double flangeThickness, double flangeSlope, double rootRadius = 0, double toeRadius = 0, bool mirrorAboutLocalZ = false)
        {
            if (height < 2 * ( flangeThickness + flangeSlope * (width/2 - webThickness))) // Assume no fillets
            {
                InvalidRatioError("height", "flangeThickness, width, and webThickness");
                return null;
            }

            if (width < webThickness) // Assume no fillets
            {
                InvalidRatioError("width", "webThickness");
                return null;
            }

            if (flangeSlope > flangeThickness / (width/2)) // Assume no fillets
            {
                InvalidRatioError("Width", "FlangeThickness and FlangeSlope");
                return null;
            }

            // check that the root radius doesn't take up the whole web
            if (height < 2 * (flangeThickness + flangeSlope * (width / 2 - webThickness - rootRadius) + Math.Sqrt(Math.Pow(rootRadius, 2) + Math.Pow(flangeSlope * rootRadius, 2))))
            {
                InvalidRatioError("rootRadius", "flangeThickness, flangeSlope, width, webThickness, and height");
                return null;
            }

            // check that the toe radius doesn't eliminate the face of the toe
            if (Math.Sqrt(Math.Pow(toeRadius, 2) + Math.Pow(flangeSlope * toeRadius, 2)) > flangeThickness - flangeSlope * (width / 2 - toeRadius))
            {
                InvalidRatioError("toeRadius", "flangeThickness, flangeSlope, and width");
                return null;
            }

            // check that the toe and root radii don't eliminate the inner flange face
            if (width < webThickness + (1 - flangeSlope) * (rootRadius + toeRadius))
            {
                InvalidRatioError("width", "webThickness, rootRadius, and toeRadius");
                return null;
            }

            if (height <= 0 || width <= 0 || webThickness <= 0 || flangeThickness <= 0 || flangeSlope < 0 || rootRadius < 0 || toeRadius < 0)
            {
                Engine.Base.Compute.RecordError("Input length less or equal to 0");
                return null;
            }

            List<ICurve> curves = TaperFlangeChannelProfileCurves(height, width, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius);
            curves = mirrorAboutLocalZ ? curves.MirrorAboutLocalZ() : curves;
            return new TaperFlangeChannelProfile(height, width, webThickness, flangeThickness, flangeSlope, rootRadius, toeRadius, mirrorAboutLocalZ, curves);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> TaperFlangeChannelProfileCurves(double height, double width, double wt, double ft, double slope, double r1, double r2)
        {
            List<ICurve> edges = new List<ICurve>();
            Point p = Point.Origin;

            Vector xAxis = oM.Geometry.Vector.XAxis;
            Vector yAxis = oM.Geometry.Vector.YAxis;

            Line l0 = new Line { Start = p, End = p = p + xAxis * width };
            Line l1 = new Line { Start = p, End = p = p + yAxis * (ft - width / 2 * slope) };
            Line l2 = new Line { Start = p, End = p = p - xAxis * (width - wt) + yAxis * (width - wt) * slope };
            Line l3 = new Line { Start = p, End = p = p + yAxis * (height - 2*(ft + (width / 2 - wt) * slope)) };
            Line l4 = new Line { Start = p, End = p = p + xAxis * (width - wt) + yAxis * (width - wt) * slope };
            Line l5 = new Line { Start = p, End = p = p + yAxis * (ft - width / 2 * slope) };
            Line l6 = new Line { Start = p, End = p = p - xAxis * width };
            Line l7 = new Line { Start = p, End = p = p - yAxis * height };

            edges.Add(l0);
            List<ICurve> fillet = Fillet(l1, l2, r2);
            edges.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l3, r1);
            edges.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l4, r1);
            edges.AddRange(fillet.GetRange(0, fillet.Count - 1));
            fillet = Fillet(fillet.Last() as Line, l5, r2);
            edges.AddRange(fillet);
            edges.Add(l6);
            edges.Add(l7);

            Point centroid = edges.IJoin().Centroid();
            Vector translation = Point.Origin - centroid;
            return edges.Select(x => x.ITranslate(translation)).ToList();
        }

        /***************************************************/

    }
}



