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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Rationalises an ICurve into a Polyline. The number of subdivisions is automatically derived depending on the type.")]
        [Input("minSubdivisions", "Minimum numbers of subdivisions (segments) to perform on the input ICurve.")]
        [Input("refinement", "(Optional, defaults to 1) When the number of subdivisions is derived for a specific type, it gets multiplied by this factor.")]
        public static Polyline IRationalise(this ICurve curve, int minSubdivisions = 3, int refinement = 1)
        {
            if (curve is Polyline) // no need to rationalise
                return curve as Polyline;

            if (curve is Line) // no need to rationalise
                return new Polyline() { ControlPoints = (curve as Line).ControlPoints() };

            return Rationalise(curve as dynamic, refinement, minSubdivisions);
        }

        /***************************************************/

        [Description("Rationalises the Polycurve into a Polyline. Currently limited functionality.")]
        public static Polyline Rationalise(this PolyCurve curve, int minSubdivisions = 3, int refinement = 1)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot rationalise a null {nameof(PolyCurve)}.");
                return null;
            }

            EnforceMinimumValues(ref minSubdivisions, ref refinement);

            if (curve.Curves.Count == 0)
                return new Polyline();

            Polyline polyline = new Polyline();
            polyline.ControlPoints.Add(curve.SubParts()[0].IStartPoint());

            foreach (ICurve c in curve.SubParts())
            {
                Line line = c as Line;
                if (line != null)
                {
                    polyline.ControlPoints.Add(line.End);
                    continue;
                }

                Polyline rationalised = IRationalise(c, minSubdivisions, refinement);

                List<Point> points = rationalised.ControlPoints.Skip(1).ToList();

                polyline.ControlPoints.AddRange(points);
            }

            if (polyline == null || polyline.ControlPoints.Count < 2)
                BH.Engine.Base.Compute.RecordError("Rationalisation of curves currently only supports Arcs.");

            return polyline;
        }

        /***************************************************/

        [Description("Rationalises the Arc into a Polyline.")]
        public static Polyline Rationalise(this Arc arc, int minSubdivisions = 3, int refinement = 1)
        {
            EnforceMinimumValues(ref minSubdivisions, ref refinement);

            Polyline polyline = new Polyline();

            List<Point> controlPoints = new List<Point> { arc.IStartPoint() };

            double arcAngle = Math.Round(Math.Abs(Math.Abs(arc.StartAngle - arc.EndAngle)), 4);

            double minRadiusForSubdivision = 0.02;
            double minAngleForSubdivision = 0.1;

            minRadiusForSubdivision = minRadiusForSubdivision / refinement;
            minAngleForSubdivision = minAngleForSubdivision / refinement;

            double length = arc.Length();

            if (arc.Radius < minRadiusForSubdivision || arcAngle < minAngleForSubdivision) // a very small arc should not be subdivided.
                controlPoints.Add(arc.IEndPoint());
            else
            {
                // If not, subdivide the arc.
                int numSubdivisions = (int)Math.Abs(Math.Ceiling(1.5708 / (arcAngle) * refinement * length / 2) - 1);

                // Scale the number of subdivisions based on the refinement
                numSubdivisions = (int)Math.Ceiling((double)(numSubdivisions * refinement));

                // Check the number of subdivisions is over the minimum acceptable
                numSubdivisions = numSubdivisions < minSubdivisions ? minSubdivisions : numSubdivisions;

                List<double> pointParams = Enumerable.Range(0, numSubdivisions).Select(i => (double)((double)i / (double)numSubdivisions)).ToList();

                controlPoints.AddRange(pointParams.Select(par => arc.IPointAtParameter(par)));

                controlPoints.Add(arc.IEndPoint());
            }

            polyline.ControlPoints = controlPoints;

            return polyline;
        }

        /***************************************************/

        [Description("Rationalises the Circle into a Polyline.")]
        public static Polyline Rationalise(this Circle circle, int minSubdivisions = 3, int refinement = 1)
        {
            EnforceMinimumValues(ref minSubdivisions, ref refinement);

            Polyline polyline = new Polyline();

            List<Point> controlPoints = new List<Point> { circle.IStartPoint() };

            // Subdivide the circle.
            // Empyrical formula to extract a reasonable amount of segments
            int numSubdvision = (int)(Math.Ceiling(circle.Radius * 10));

            // Scale the number of subdivisions based on the Options
            numSubdvision = (int)Math.Ceiling((double)numSubdvision * refinement);

            // Check the number of subdivisions is over the minimum acceptable
            numSubdvision = numSubdvision < minSubdivisions ? minSubdivisions : numSubdvision;

            List<double> pointParams = Enumerable.Range(0, numSubdvision).Select(i => (double)((double)i / (double)numSubdvision)).ToList();
            pointParams.Add(1);

            controlPoints.AddRange(pointParams.Select(par => circle.IPointAtParameter(par)));

            polyline.ControlPoints = controlPoints;

            return polyline;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Fallback
        private static Polyline Rationalise(this ICurve curve, int minSubdivisions = 3, int refinement = 1)
        {
            BH.Engine.Base.Compute.RecordError($"Could not find a method to rationalise the curve {curve.GetType().Name}." +
                $"The method {nameof(Rationalise)} currently only supports ICurves composed of Arc and/or Circle and/or straight lines.");
            return null;
        }

        /***************************************************/

        private static void EnforceMinimumValues(ref int minSubdivisions, ref int refinement)
        {
            refinement = refinement < 1 ? 1 : refinement;
            minSubdivisions = minSubdivisions < 3 ? 3 : minSubdivisions;
        }
    }
}


