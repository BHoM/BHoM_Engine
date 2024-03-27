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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        [Description("Creates a PlanarSurface based on boundary curves. Only processing done by this method is checking (co)planarity and that the curves are closed. Internal edges will be assumed to be inside the External.")]
        [Input("externalBoundary", "The outer boundary curve of the surface. Needs to be closed and planar.")]
        [Input("internalBoundaries", "Optional internal boundary curves descibing any openings inside the external. All internal edges need to be closed and co-planar with the external edge.")]
        [Input("tolerance", "Distance tolerance used for checking the validity of the inputs for PlanarSurface creation.", typeof(Length))]
        [Output("PlanarSurface", "Planar surface corresponding to the provided edge curves.")]
        public static PlanarSurface PlanarSurface(ICurve externalBoundary, List<ICurve> internalBoundaries = null, double tolerance = Tolerance.Distance)
        {
            if (externalBoundary == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot create planar surface from a null outline.");
                return null;
            }

            //--------------Planar-External-Boundary-----------------------//
            if (!externalBoundary.IIsPlanar(tolerance))
            {
                Base.Compute.RecordError("External edge curve is not planar");
                return null;
            }

            //---------------Closed-External-Boundary-----------------------//
            if (!externalBoundary.IIsClosed(tolerance))
            {
                Base.Compute.RecordError("External edge curve is not closed");
                return null;
            }

            //--------------SelfIntersecting-External-Boundary--------------//
            if (!externalBoundary.ISubParts().Any(y => y is NurbsCurve) && externalBoundary.IIsSelfIntersecting(tolerance))
            {
                Base.Compute.RecordError("The provided external boundary is self-intersecting.");
                return null;
            }

            internalBoundaries = internalBoundaries ?? new List<ICurve>();

            //----------------Closed-Internal-Boundaries--------------------//
            int count = internalBoundaries.Count;

            internalBoundaries = internalBoundaries.Where(x => x.IIsClosed(tolerance)).ToList();

            if (internalBoundaries.Count != count)
            {
                Base.Compute.RecordWarning("At least one of the internal boundaries is not closed and has been ignored on creation of the planar surface.");
            }

            //---------------Coplanar-Internal-Boundaries-------------------//
            count = internalBoundaries.Count;

            Plane p = externalBoundary.IFitPlane(tolerance);
            internalBoundaries = internalBoundaries.Where(x => x.IIsInPlane(p, tolerance)).ToList();

            if (internalBoundaries.Count != count)
            {
                Base.Compute.RecordWarning("At least one of the internal boundaries is not coplanar with the external edge curve and has been ignored on creation of the planar surface.");
            }

            //--------------Unsupported-Internal-Boundaries-Warning---------//
            if (internalBoundaries.SelectMany(x => x.ISubParts()).Any(x => x is NurbsCurve || x is Ellipse))
                Base.Compute.RecordWarning("At least one of the internal boundaries is a NurbsCurve or Ellipse and has not been checked for validity on creation of the planar surface.");

            //--------------Self-Intersecting-Internal-Boundaries-----------//
            count = internalBoundaries.Count;

            internalBoundaries = internalBoundaries.Where(x => x.ISubParts().Any(y => y is NurbsCurve) || !x.IIsSelfIntersecting(tolerance)).ToList();

            if (internalBoundaries.Count != count)
            {
                Base.Compute.RecordWarning("At least one of the internal boundaries is self-intersecting and has been ignored on creation of the planar surface.");
            }

            //--------------Overlapping-Internal-Boundaries-----------------//
            count = internalBoundaries.Count;

            internalBoundaries = internalBoundaries.Where(x => x.ISubParts().Any(y => y is NurbsCurve || y is Ellipse))
                                .Concat(internalBoundaries.Where(x => x.ISubParts().All(y => !(y is NurbsCurve) && !(y is Ellipse))).BooleanUnion()).ToList();

            if (internalBoundaries.Count != count)
            {
                Base.Compute.RecordWarning("At least one of the internalBoundaries was overlapping another one. BooleanUnion was used to resolve it.");
            }

            //--------------Unsupported-External-Boundary-------------------//
            if (externalBoundary.ISubParts().Any(x => x is NurbsCurve ||  x is Ellipse))
            {
                Base.Compute.RecordWarning("External boundary is a nurbs curve or Ellipse. Necessary checks to ensure validity of a planar surface based on nurbs curve cannot be run, therefore correctness of the surface boundaries is not guaranteed.");
                // External done
                return new PlanarSurface(externalBoundary, internalBoundaries);
            }

            //-------------------Internal-Boundary-Curves-------------------//
            //--------------Overlapping-External-Boundary-Curve-------------//
            for (int i = 0; i < internalBoundaries.Count; i++)
            {
                ICurve intCurve = internalBoundaries[i];
                if (intCurve.ISubParts().Any(x => x is NurbsCurve || x is Ellipse))
                    continue;

                if (externalBoundary.ICurveIntersections(intCurve, tolerance).Count != 0)
                {
                    List<PolyCurve> regions = externalBoundary.BooleanDifference(new List<ICurve>() { intCurve });
                    if (regions.Count > 1)
                    {
                        BH.Engine.Base.Compute.RecordError("Cannot create a single planar surface: One of the internal boundaries splits the external boundary into more than one surface.\nTry running BooleanDifference on the external and internal boundaries and creating planar surfaces from the output regions.");
                        return null;
                    }
                    else if (regions.Count == 0)
                    {
                        BH.Engine.Base.Compute.RecordWarning("Cannot create a single planar surface: One of the internal boundaries encompasses the external boundary. BooleanDifference was used to resolve the issue");
                        return null;
                    }
                    else
                        externalBoundary = regions.Single();
                    internalBoundaries.RemoveAt(i);
                    i--;
                    Base.Compute.RecordWarning("At least one of the internalBoundaries is intersecting the externalBoundary. BooleanDifference was used to resolve the issue.");
                }
            }

            //--------------------Internal-Boundaries-----------------------//
            //---------------Contained-By-External-Boundary-----------------//
            count = internalBoundaries.Count;

            internalBoundaries = internalBoundaries.Where(x => x.ISubParts().Any(y => y is NurbsCurve || y is Ellipse) || externalBoundary.IIsContaining(x, true, tolerance)).ToList();

            if (internalBoundaries.Count != count)
            {
                Base.Compute.RecordWarning("At least one of the internalBoundaries is not contained by the externalBoundary. And have been disregarded.");
            }

            externalBoundary = externalBoundary.TryGetBoundaryCurve(tolerance);
            internalBoundaries = internalBoundaries.Select(x => x.TryGetBoundaryCurve(tolerance)).ToList();

            //------------------Return-Valid-Surface------------------------//
            return new PlanarSurface(externalBoundary, internalBoundaries);
        }

        /***************************************************/
        
        [Description("Distributes the edge curve and creates a set of boundary planar surfaces.")]
        [Input("boundaryCurves", "Boundary curves to be used. Non-planar and non-closed curves are ignored.")]
        [Input("tolerance", "Distance tolerance used for checking the validity of the inputs for PlanarSurface creation.", typeof(Length))]
        [Output("PlanarSurface", "List of planar surfaces created.")]
        public static List<PlanarSurface> PlanarSurface(List<ICurve> boundaryCurves, double tolerance = Tolerance.Distance)
        {
            if (boundaryCurves == null || boundaryCurves.Count == 0 || boundaryCurves.All(x => x == null))
            {
                BH.Engine.Base.Compute.RecordError("Cannot create planar surface from a null or empty collection of boundary curves.");
                return null;
            }

            List<ICurve> checkedCurves = boundaryCurves.ValidateCurves(tolerance);

            
            if (checkedCurves.Count == 0)
            {
                Base.Compute.RecordError("Planar surface could not be created because all input boundary curves are invalid (null, unsupported type, non-planar, not closed within the tolerance).");
                return new List<PlanarSurface>();
            }

            List<List<ICurve>> distributed = Compute.DistributeOutlines(checkedCurves, tolerance);

            List<PlanarSurface> surfaces = new List<PlanarSurface>();
            for (int i = 0; i < distributed.Count; i++)
            {
                PlanarSurface srf = new PlanarSurface(
                    distributed[i][0],
                    distributed[i].Skip(1).ToList()
                );

                surfaces.Add(srf);
            }
            
            return surfaces;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<ICurve> ValidateCurves(this List<ICurve> boundaryCurves, double tolerance)
        {
            //--------------Null-Boundary-Curves-----------------------//
            int count = boundaryCurves.Count;
            List<ICurve> checkedCurves = boundaryCurves.Where(x => x != null).ToList();
            if (checkedCurves.Count != count)
                Base.Compute.RecordWarning("Some of the input boundary curves were null and have been ignored on planar surface creation. Please make sure if the output is correct.");

            //--------------Unsupported-Boundary-Curves-----------------------//
            count = checkedCurves.Count;
            checkedCurves = checkedCurves.Where(x => !(x is NurbsCurve) && !(x is Ellipse)).ToList();
            if (checkedCurves.Count != count)
                Base.Compute.RecordWarning("Some of the input boundary curves were nurbs curves or ellipses, which are unsupported, and have been ignored on planar surface creation. Please make sure if the output is correct.");

            //--------------Planar-Boundary-Curves-----------------------//
            count = checkedCurves.Count;
            checkedCurves = checkedCurves.Where(x => x.IIsPlanar(tolerance)).ToList();
            if (checkedCurves.Count != count)
                Base.Compute.RecordWarning("Some of the input boundary curves were not planar within the tolerance and have been ignored on planar surface creation. Please make sure if the output is correct and tweak the input tolerance if needed.");

            //--------------Closed-Boundary-Curves-----------------------//
            count = checkedCurves.Count;
            checkedCurves = checkedCurves.Where(x => x.IIsClosed(tolerance)).ToList();
            if (checkedCurves.Count != count)
                Base.Compute.RecordWarning("Some of the input boundary curves were not closed within the tolerance and have been ignored on planar surface creation. Please make sure if the output is correct and tweak the input tolerance if needed.");


            //--------------Self-intersecting-Boundary-Curves-----------------------//
            count = checkedCurves.Count;
            checkedCurves = checkedCurves.Where(x => !x.IIsSelfIntersecting(tolerance)).ToList();
            if (checkedCurves.Count != count)
                Base.Compute.RecordWarning("Some of the input boundary curves were self-intersecting within the tolerance and have been ignored on planar surface creation. Please make sure if the output is correct and tweak the input tolerance if needed.");


            return checkedCurves.Select(x => x.TryGetBoundaryCurve(tolerance)).ToList();
        }

        /***************************************************/

        [Description("Try get out the curve as suitable type of IBoundary curve. Method assumes curves to have been pre-checked for validity (Closed, planar, non-self intersecting).")]
        private static ICurve TryGetBoundaryCurve(this ICurve curve, double tolerance)
        {
            if (curve is IBoundary)
            {
                return curve;
            }
            else if (curve is PolyCurve)
            {
                PolyCurve pCurve = curve as PolyCurve;
                if (pCurve.Curves.Count == 1)
                    return TryGetBoundaryCurve(pCurve.Curves[0], tolerance);

                List<ICurve> subParts = curve.ISubParts().ToList();
                if (subParts.Any(x => x is NurbsCurve))
                    return curve;

                if (subParts.Count == 1)
                    return TryGetBoundaryCurve(subParts[0], tolerance);

                return new BoundaryCurve(Compute.IJoin(subParts, tolerance).First().Curves);
            }
            else if (curve is Polyline)
            {
                Polyline pLine = curve as Polyline;
                return new Polygon(pLine.ControlPoints.GetRange(0, pLine.ControlPoints.Count - 1));
            }
            else
            {
                return curve;
            }    

        }

        /***************************************************/
    }
}




