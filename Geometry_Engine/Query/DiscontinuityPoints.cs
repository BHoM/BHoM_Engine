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

using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;
using System;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Gets the points at kinks of the curve. For an Arc this is the start and end points.")]
        [Input("curve", "The Arc to get the discontinuity points from.")]
        [Input("distanceTolerance", "Distance tolerance for extracting discontinuity points. Not used for Arcs.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance for extracting discontinuity points. Not used for Arcs.", typeof(Length))]
        [Output("discPoints", "The list of discontinuity points.")]
        public static List<Point> DiscontinuityPoints(this Arc curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Input curve is null. Discontinuity points can not be evaluated.");
                return new List<Point>();
            }
            return new List<Point> { curve.StartPoint(), curve.EndPoint() };
        }

        /***************************************************/

        [Description("Gets the points at kinks of the curve. A Circle does not have any discontinuity points, why this method is returning an empty list.")]
        [Input("curve", "The Circle to get the discontinuity points from.")]
        [Input("distanceTolerance", "Distance tolerance for extracting discontinuity points. Not used for Circle.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance for extracting discontinuity points. Not used for Circle.", typeof(Length))]
        [Output("discPoints", "The list of discontinuity points.")]
        public static List<Point> DiscontinuityPoints(this Circle curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            return new List<Point>();
        }

        /***************************************************/

        [Description("Gets the points at kinks of the curve. An Ellipse does not have any discontinuity points, why this method is returning an empty list.")]
        [Input("curve", "The Ellipse to get the discontinuity points from.")]
        [Input("distanceTolerance", "Distance tolerance for extracting discontinuity points. Not used for Ellipse.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance for extracting discontinuity points. Not used for Ellipse.", typeof(Length))]
        [Output("discPoints", "The list of discontinuity points.")]
        public static List<Point> DiscontinuityPoints(this Ellipse curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            return new List<Point>();
        }

        /***************************************************/

        [Description("Gets the points at kinks of the curve. For an Line this is the start and end points.")]
        [Input("curve", "The Line to get the discontinuity points from.")]
        [Input("distanceTolerance", "Distance tolerance for extracting discontinuity points. Not used for Line.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance for extracting discontinuity points. Not used for Line.", typeof(Length))]
        [Output("discPoints", "The list of discontinuity points.")]
        public static List<Point> DiscontinuityPoints(this Line curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Input curve is null. Discontinuity points can not be evaluated.");
                return new List<Point>();
            }
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        [Description("Gets the points at kinks of the curve, i.e. Points where the tangent between two sub curves coming in to the same point is outside of the provided tolerance.")]
        [Input("curve", "The IPolyCurve to get the discontinuity points from.")]
        [Input("distanceTolerance", "Distance tolerance for extracting discontinuity points.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance for extracting discontinuity points. Not used for IPolyCurve.", typeof(Length))]
        [Output("discPoints", "The list of discontinuity points.")]
        public static List<Point> DiscontinuityPoints(this IPolyCurve curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Input curve is null. Discontinuity points can not be evaluated.");
                return new List<Point>();
            }
            List<Point> result = new List<Point>();
            List<ICurve> curves = curve.ISubParts().Where(c => !(c is Circle || c is Ellipse)).ToList();
            bool closed = curve.IIsClosed(distanceTolerance);

            if (curves.Count == 0)
                return result;

            int j;
            for (int i = 0; i < curves.Count; i++)
            {
                j = (i - 1 + curves.Count) % curves.Count;
                if (i > 0 || closed)
                {
                    if (!curves[j].IEndDir().IsEqual(curves[i].IStartDir(), distanceTolerance))
                        result.Add(curves[i].IStartPoint());
                }
                else
                    result.Add(curves[i].IStartPoint());
            }

            if (!closed)
                result.Add(curve.EndPoint());

            return result;
        }

        /***************************************************/

        [Description("Gets the points at kinks of the curve, i.e. points where the tangent between two segments coming in to the same point is outside of the provided tolerance.")]
        [Input("curve", "The IPolyline to get the discontinuity points from.")]
        [Input("distanceTolerance", "Distance tolerance for extracting discontinuity points.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance for extracting discontinuity points.", typeof(Length))]
        [Output("discPoints", "The list of discontinuity points.")]
        public static List<Point> DiscontinuityPoints(this IPolyline curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Input curve is null. Discontinuity points can not be evaluated.");
                return new List<Point>();
            }
            List<Point> ctrlPts = new List<Point>(curve.IControlPoints());

            if (ctrlPts.Count < 3)
                return ctrlPts;

            double sqTol = distanceTolerance * distanceTolerance;
            int j = 0;
            if (!curve.IIsClosed(distanceTolerance))
                j += 2;

            for (int i = j; i < ctrlPts.Count; i++)
            {
                int cc = ctrlPts.Count;
                int i1 = (i - 1 + cc) % cc;
                int i2 = (i - 2 + cc) % cc;
                Vector v1 = ctrlPts[i1] - ctrlPts[i2];
                Vector v2 = ctrlPts[i] - ctrlPts[i1];
                double angle = v1.Angle(v2);

                if (angle <= angleTolerance || angle >= (2 * Math.PI) - angleTolerance || ctrlPts[i2].SquareDistance(ctrlPts[i1]) <= sqTol)
                {
                    ctrlPts.RemoveAt(i1);
                    i--;
                }
            }

            return ctrlPts;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Gets the points at kinks of the curve, i.e. points where the tangent between two segments coming in to the same point is outside of the provided tolerance.")]
        [Input("curve", "The IPolyline to get the discontinuity points from.")]
        [Input("distanceTolerance", "Distance tolerance for extracting discontinuity points.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance for extracting discontinuity points.", typeof(Length))]
        [Output("discPoints", "The list of discontinuity points.")]
        public static List<Point> IDiscontinuityPoints(this ICurve curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (curve == null)
            {
                Engine.Base.Compute.RecordError("Input curve is null. Discontinuity points can not be evaluated.");
                return new List<Point>();
            }
            return DiscontinuityPoints(curve as dynamic, distanceTolerance, angleTolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<Point> DiscontinuityPoints(this ICurve curve, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            Base.Compute.RecordError($"DiscontinuityPoints is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}


