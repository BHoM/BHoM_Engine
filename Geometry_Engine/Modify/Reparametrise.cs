/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Reparametrises a NurbsCurve to have a normalised domain from 0 to 1.")]
        [Input("curve", "The NurbsCurve to reparametrise.")]
        [Output("curve", "The reparametrised NurbsCurve with domain [0,1].")]
        public static NurbsCurve Reparametrise(this NurbsCurve curve)
        {
            if (curve == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't reparametrise a null curve.");
                return null;
            }

            int degree = curve.Degree();
            double min = curve.Knots[degree - 1];
            double max = curve.Knots[curve.Knots.Count - degree];
            return new NurbsCurve { ControlPoints = curve.ControlPoints, Weights = curve.Weights, Knots = curve.Knots.Select(t => (t - min) / (max - min)).ToList() };
        }

        /***************************************************/

        [Description("Reparametrises a NurbsSurface to have normalised U and V domains from 0 to 1.")]
        [Input("surface", "The NurbsSurface to reparametrise.")]
        [Output("surface", "The reparametrised NurbsSurface with U and V domains [0,1].")]
        public static NurbsSurface Reparametrise(this NurbsSurface surface)
        {
            if (surface == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't reparametrise a null surface.");
                return null;
            }

            List<ICurve> innerTrims2D = surface.InnerTrims.Select(x => x.Curve2d).ToList();
            List<ICurve> outerTrims2D = surface.OuterTrims.Select(x => x.Curve2d).ToList();

            // Reparametrise U knots
            double uMin = surface.UKnots[surface.UDegree - 1];
            double uMax = surface.UKnots[surface.UKnots.Count - surface.UDegree];
            double uTranslation = -uMin;
            double uScale = 1 / (uMax - uMin);
            List<double> reparametrisedUKnots = surface.UKnots.Select(u => (u + uTranslation) * uScale).ToList();
            innerTrims2D = innerTrims2D.Select(x => x.ITranslate(new Vector { X = uTranslation }).IScale(Point.Origin, new Vector { X = uScale, Y = 1, Z = 1 })).ToList();
            outerTrims2D = outerTrims2D.Select(x => x.ITranslate(new Vector { X = uTranslation }).IScale(Point.Origin, new Vector { X = uScale, Y = 1, Z = 1 })).ToList();

            // Reparametrise V knots
            double vMin = surface.VKnots[surface.VDegree - 1];
            double vMax = surface.VKnots[surface.VKnots.Count - surface.VDegree];
            double vTranslation = -vMin;
            double vScale = 1 / (vMax - vMin);
            List<double> reparametrisedVKnots = surface.VKnots.Select(v => (v + vTranslation) * vScale).ToList();
            innerTrims2D = innerTrims2D.Select(x => x.ITranslate(new Vector { Y = vTranslation }).IScale(Point.Origin, new Vector { X = 1, Y = vScale, Z = 1 })).ToList();
            outerTrims2D = outerTrims2D.Select(x => x.ITranslate(new Vector { Y = vTranslation }).IScale(Point.Origin, new Vector { X = 1, Y = vScale, Z = 1 })).ToList();

            List<SurfaceTrim> innerTrims = surface.InnerTrims.Select((x, i) => new SurfaceTrim(x.Curve3d, innerTrims2D[i])).ToList();
            List<SurfaceTrim> outerTrims = surface.OuterTrims.Select((x, i) => new SurfaceTrim(x.Curve3d, outerTrims2D[i])).ToList();

            return new NurbsSurface(
                surface.ControlPoints,
                surface.Weights,
                reparametrisedUKnots,
                reparametrisedVKnots,
                surface.UDegree,
                surface.VDegree,
                innerTrims,
                outerTrims
            );
        }

        /***************************************************/
    }
}

