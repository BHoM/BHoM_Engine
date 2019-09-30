/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /******************************************************************/
        /*** Those methods were written to work with offset method only ***/
        /***        They were not tested with anything else!            ***/
        /******************************************************************/

        /***************************************************/
        /**** private Methods - Curves                   ****/
        /***************************************************/

        private static PolyCurve ExtendTangent(this Line curve, Point startPoint, Point endPoint)
        {
            List<ICurve> result = new List<ICurve>();
            //if(curve.ClosestPoint(startPoint)==curve.ClosestPoint(endPoint))
            //{
            //    result.Add(new Line { Start = startPoint, End = endPoint });
            //    return new PolyCurve
            //    {
            //        Curves=result
            //    };
            //}
            //if(!startPoint.IsOnCurve(curve))
            //    result.Add(new Line { Start = startPoint, End = curve.Start });
            //result.Add(curve);
            //if(!endPoint.IsOnCurve(curve))
            //    result.Add(new Line { Start = curve.End, End = endPoint});
            result.Add(curve.Extend(startPoint, endPoint));
            return new PolyCurve
            {
                Curves = result
            }
            ;
        }

        /***************************************************/

        private static PolyCurve ExtendTangent(this Arc curve, Point startPoint, Point endPoint)
        {
            List<ICurve> result = new List<ICurve>();
            if(!startPoint.IsOnCurve(curve))
                result.Add(new Line { Start = startPoint, End = curve.StartPoint() });
            result.Add(curve);
            if (!endPoint.IsOnCurve(curve))
                result.Add(new Line { Start = curve.EndPoint(), End = endPoint });
            return new PolyCurve
            {
                Curves = result
            };
        }
        
        /***************************************************/

        private static Circle ExtendTangent(this Circle curve, Point startPoint, Point endPoint)
        {
            Reflection.Compute.RecordNote("Can't Extend closed models.");
            return curve;
        }
        
        /***************************************************/

        private static Ellipse ExtendTangent(this Ellipse curve, Point startPoint, Point endPoint)
        {
            Reflection.Compute.RecordNote("Can't Extend closed models.");
            return curve;
        }
        
        /***************************************************/

        [NotImplemented]
        private static NurbsCurve ExtendTangent(this NurbsCurve curve, Point startPoint, Point endPoint)
        {
            throw new NotImplementedException();
        }
        
        /***************************************************/

        private static Polyline ExtendTangent(this Polyline curve, Point startPoint, Point endPoint)
        {
            if (curve.IsClosed())
            {
                Reflection.Compute.RecordNote("Can't Extend closed models.");
                return curve;
            }
            List<Point> result = new List<Point>();
            if(curve.ControlPoints[0]!=startPoint)
                result.Add(startPoint);
            foreach(Point a in curve.ControlPoints)
            {
                result.Add(a);
            }
            if (result[result.Count - 1] != endPoint)
                result.Add(endPoint);
            return new Polyline
            {
                ControlPoints = result
            };
        }
        
        /***************************************************/

        private static PolyCurve ExtendTangent(this PolyCurve curve, Point startPoint, Point endPoint)
        {
            if (curve.IsClosed())
            {
                Reflection.Compute.RecordNote("Can't Extend closed models.");
                return curve;
            }
            List<ICurve> result = new List<ICurve>();
            if (curve.Curves[0].IStartPoint() != startPoint)
                result.Add(new Line { Start = startPoint, End = curve.Curves[0].IStartPoint() });
            foreach (ICurve a in curve.Curves)
                result.Add(a);
            if (result[result.Count - 1].IEndPoint() != endPoint)
                result.Add(new Line { Start=result[result.Count - 1].IEndPoint(), End = endPoint });
            return new PolyCurve
            {
                Curves = result
            };

        }


        /***************************************************/
        /**** private Methods - Interfaces               ****/
        /***************************************************/

        private static ICurve IExtendTangent(this ICurve curve, Point startPoint, Point endPoint)
        {
            return ExtendTangent(curve as dynamic, startPoint, endPoint);

        }
        /***************************************************/

    }
}
