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
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        [Description("Returns if the curve is Linear or not. For a Line this always returns true.")]
        [Input("line", "The Line to check for linearity.")]
        [Input("tolerance", "Distance tolerance to use for check whether the curve is linear or not.", typeof(Length))]
        [Output("isLinear", "Return true if the curve is linear. For a line this always return true.")]
        public static bool IsLinear(this Line line, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Returns if the curve is Linear or not. For a Arc this always returns false.")]
        [Input("arc", "The Arc to check for linearity.")]
        [Input("tolerance", "Distance tolerance to use for check whether the curve is linear or not.", typeof(Length))]
        [Output("isLinear", "Return true if the curve is linear. For a Arc this always return false.")]
        public static bool IsLinear(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Returns if the curve is Linear or not. For a Circle this always returns false.")]
        [Input("circle", "The Circle to check for linearity.")]
        [Input("tolerance", "Distance tolerance to use for check whether the curve is linear or not.", typeof(Length))]
        [Output("isLinear", "Return true if the curve is linear. For a Circle this always return false.")]
        public static bool IsLinear(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Returns if the curve is Linear or not. For a Ellipse this always returns false.")]
        [Input("ellipse", "The Ellipse to check for linearity.")]
        [Input("tolerance", "Distance tolerance to use for check whether the curve is linear or not.", typeof(Length))]
        [Output("isLinear", "Return true if the curve is linear. For a Ellipse this always return false.")]
        public static bool IsLinear(this Ellipse ellipse, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Returns if the curve is Linear or not, i.e. if all the controlpoints of the Polyline are colinear.")]
        [Input("curve", "The Polyline to check for linearity.")]
        [Input("tolerance", "Distance tolerance to use for check whether the curve is linear or not.", typeof(Length))]
        [Output("isLinear", "Return true if the curve is linear.")]
        public static bool IsLinear(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsCollinear(tolerance);
        }

        /***************************************************/

        [Description("Returns if the curve is Linear or not, i.e. if the Polycurve is built up of a set of colinear curves.")]
        [Input("curve", "The PolyCurve to check for linearity.")]
        [Input("tolerance", "Distance tolerance to use for check whether the curve is linear or not.", typeof(Length))]
        [Output("isLinear", "Return true if the curve is linear.")]
        public static bool IsLinear(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints().IsCollinear(tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Returns if the curve is Linear or not.")]
        [Input("curve", "The curve to check for linearity.")]
        [Input("tolerance", "Distance tolerance to use for check whether the curve is linear or not.", typeof(Length))]
        [Output("isLinear", "Return true if the curve is linear.")]
        public static bool IIsLinear(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return IsLinear(curve as dynamic, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsLinear(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsLinear is not implemented for ICurves of type: {curve.GetType().Name}.");
        }

        /***************************************************/
    }
}


