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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Compute
    {
        /******************************************/
        /****            IElement0D            ****/
        /******************************************/

        [PreviousVersion("4.2", "BH.Engine.ModelLaundry.Compute.FitPlane(BH.oM.Dimensional.IElement0D, System.Double)")]
        [Description("Computes plane that fits best in the IElements control points using least square fitting. Always returns null for IElement0Ds.")]
        [Input("element0D", "Element to fit the plane to. Impossible with IElement0D.")]
        [Input("tolerance", "Tolerance used to evaluate planarity (and linearity for special cases), as explained in Compute.FitPlane method in Geometry_Engine.", typeof(Length))]
        [Output("plane", "Plane that fits best into the element's geometry. null if no singular solution exists.")]
        public static Plane FitPlane(this IElement0D element0D, double tolerance = Tolerance.Distance)
        {
            return null;
        }


        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [PreviousVersion("4.2", "BH.Engine.ModelLaundry.Compute.FitPlane(BH.oM.Dimensional.IElement1D, System.Double)")]
        [Description("Computes plane that fits best in the IElements control points using least square fitting. Returns null for linear IElement1Ds.")]
        [Input("element1D", "Element to fit the plane to. No singular solution for linear IElement1Ds")]
        [Input("tolerance", "Tolerance used to evaluate planarity (and linearity for special cases), as explained in Compute.FitPlane method in Geometry_Engine.", typeof(Length))]
        [Output("plane", "Plane that fits best into the element's geometry. Null if no singular solution exists.")]
        public static Plane FitPlane(this IElement1D element1D, double tolerance = Tolerance.Distance)
        {
            List<Point> controlPoints = element1D.ControlPoints();

            return controlPoints.FitPlane(tolerance);
        }


        /******************************************/
        /****            IElement2D            ****/
        /******************************************/

        [PreviousVersion("4.2", "BH.Engine.ModelLaundry.Compute.FitPlane(BH.oM.Dimensional.IElement2D, System.Boolean, System.Double)")]
        [Description("Computes plane that fits best in the IElements control points using least square fitting. Returns null if no singular solution exists.")]
        [Input("element2D", "Element to fit the plane to.")]
        [Input("externalOnly", "If true, only the external outline of an element is taken into account.")]
        [Input("tolerance", "Tolerance used to evaluate planarity (and linearity for special cases), as explained in Compute.FitPlane method in Geometry_Engine.", typeof(Length))]
        [Output("plane", "Plane that fits best into the element's geometry. Null if no singular solution exists.")]
        public static Plane FitPlane(this IElement2D element2D, bool externalOnly = false, double tolerance = Tolerance.Distance)
        {
            List<Point> controlPoints = element2D.OutlineCurve().ControlPoints();

            if (!externalOnly)
            {
                foreach (PolyCurve internalOutline in element2D.InternalOutlineCurves())
                {
                    controlPoints.AddRange(internalOutline.ControlPoints());
                }
            }

            return controlPoints.FitPlane(tolerance);
        }


        /******************************************/
        /****            Interfaces            ****/
        /******************************************/

        [PreviousVersion("4.2", "BH.Engine.ModelLaundry.Compute.IFitPlane(BH.oM.Dimensional.IElement, System.Boolean, System.Double)")]
        [Description("Computes plane that fits best in the IElements control points using least square fitting.")]
        [Input("element", "Element to fit the plane to. Returns null for elements whose geometry can't return a singular Plane.")]
        [Input("externalOnly", "If true, only the external outline of an element is taken into account.")]
        [Input("tolerance", "Tolerance used to evaluate planarity (and linearity for special cases), as explained in Compute.FitPlane method in Geometry_Engine.", typeof(Length))]
        [Output("plane", "Plane that fits best into the element's geometry. Null if no singular solution exists.")]
        public static Plane IFitPlane(this IElement element, bool externalOnly = false, double tolerance = Tolerance.Distance)
        {
            return FitPlane(element as dynamic, externalOnly, tolerance);
        }


        /******************************************/
        /****         Private methods          ****/
        /******************************************/

        private static Plane FitPlane(this IElement0D element0D, bool externalOnly = false, double tolerance = Tolerance.Distance)
        {
            return element0D.FitPlane(tolerance);
        }

        /******************************************/

        private static Plane FitPlane(this IElement1D element1D, bool externalOnly = false, double tolerance = Tolerance.Distance)
        {
            return element1D.FitPlane(tolerance);
        }

        /******************************************/
    }
}
