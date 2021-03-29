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
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Spatial
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Modifies a IElement0D coordinates to be rounded to the number of provided decimal places.")]
        [Input("element0d", "The IElement0D to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("element", "The modified IElement0D.")]
        public static IElement0D RoundCoordinates(this IElement0D element0d, int decimalPlaces = 6)
        {
            return element0d.ISetGeometry(Geometry.Modify.RoundCoordinates(element0d.IGeometry(), decimalPlaces));
        }

        /***************************************************/

        [Description("Modifies a IElement1D defining curves to be rounded to the number of provided decimal places.")]
        [Input("element1d", "The IElement1D to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("element", "The modified IElement1D.")]
        public static IElement1D RoundCoordinates(this IElement1D element1d, int decimalPlaces = 6)
        {
            return element1d.ISetGeometry(Geometry.Modify.IRoundCoordinates(element1d.IGeometry(), decimalPlaces));
        }

        /***************************************************/

        [Description("Modifies a IElement2D's defining curves to be rounded to the number of provided decimal places.")]
        [Input("element2d", "The IElement2D to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("element", "The modified IElement2D.")]
        public static IElement2D RoundCoordinates(this IElement2D element2d, int decimalPlaces = 6)
        {
            Vector normal = element2d.Normal().Normalise();

            if (Math.Abs(Math.Abs(normal.X) - 1) < Tolerance.Angle ||
                Math.Abs(Math.Abs(normal.Y) - 1) < Tolerance.Angle ||
                Math.Abs(Math.Abs(normal.Z) - 1) < Tolerance.Angle)
            {
                Plane plane = new Plane() { Origin = Geometry.Modify.RoundCoordinates(element2d.OutlineCurve().StartPoint(), decimalPlaces), Normal = normal.RoundCoordinates(0) };

                element2d = element2d.ISetOutlineElements1D(element2d.IOutlineElements1D().Select(x => x.ISetGeometry(Geometry.Modify.IRoundCoordinates(x.IGeometry().IProject(plane), decimalPlaces))).ToList());

                return element2d.ISetInternalElements2D(element2d.IInternalElements2D().Select(y => y.ISetOutlineElements1D(y.IOutlineElements1D().Select(x => x.ISetGeometry(Geometry.Modify.IRoundCoordinates(x.IGeometry().IProject(plane), decimalPlaces))).ToList())).ToList());
            }
            else
            {
                IElement2D newElement2d = element2d.ISetOutlineElements1D(element2d.IOutlineElements1D().Select(x => x.ISetGeometry(Geometry.Modify.IRoundCoordinates(x.IGeometry(), decimalPlaces))).ToList());
                
                newElement2d.ISetInternalElements2D(newElement2d.IInternalElements2D().Select(y => y.ISetOutlineElements1D(y.IOutlineElements1D().Select(x => x.ISetGeometry(Geometry.Modify.IRoundCoordinates(x.IGeometry(), decimalPlaces))).ToList())).ToList());

                if (newElement2d.IsPlanar())
                    return newElement2d;
            }

            Reflection.Compute.RecordWarning("Rounding the coordinates of a planar surface couldn't be achieved without losing planarity. No action has been taken.");
            return element2d;
        }


        /***************************************************/
        /**** Interface Methods                         ****/
        /***************************************************/

        [Description("Modifies a IElement's defining curves to be rounded to the number of provided decimal places.")]
        [Input("element", "The IElement to modify.")]
        [Input("decimalPlaces", "The number of decimal places to round to, default 6.")]
        [Output("element", "The modified IElement.")]
        public static IElement IRoundCoordinates(this IElement element, int decimalPlaces = 6)
        {
            return RoundCoordinates(element as dynamic, decimalPlaces);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static IElement RoundCoordinates(this IElement element, int decimalPlaces = 6)
        {
            Engine.Reflection.Compute.RecordError("No RoundCoordinates method has been implemented for: " + element.GetType().Name + ". The object has not been modified.");
            return element;
        }

        /***************************************************/

    }
}

