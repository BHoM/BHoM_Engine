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
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.Engine.Base;
using BH.Engine.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Sets the Outline Element1Ds of a PadFoundation, i.e. the ExternalBoundary. Method required for all IElement2Ds.\n" +
                     "The provided edges all need to be ICurves and should form a closed loop. No checking for planarity is made by the method.\n" +
                     "The Method will return a new PadFoundation with the provided edges as the TopSurface.")]
        [Input("padFoundation", "The PadFoundation to update the ExternalEdge of.")]
        [Input("curves", "A list of IElement1Ds which all should be of a type of ICurve.")]
        [Output("padFoundation", "A new PadFoundation with TopSurface matching the provided edges.")]
        public static PadFoundation SetOutlineElements1D(this PadFoundation padFoundation, IEnumerable<IElement1D> curves)
        {
            if (padFoundation.IsNull())
                return padFoundation;

            if (curves.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The list of IElement1D is null or empty.");
                return padFoundation;
            }

            PolyCurve curve = Geometry.Compute.IJoin(curves.Cast<ICurve>().ToList())[0];

            return Create.PadFoundation(curve, padFoundation.Property, padFoundation.OrientationAngle);
        }

        /***************************************************/


        [Description("Sets the Outline Element1Ds of a Stem, i.e. the ExternalBoundary. Method required for all IElement2Ds.\n" +
                     "The provided edges all need to be ICurves and should form a closed loop. No checking for planarity is made by the method.\n" +
                     "The Method will return a new Stem with the provided edges as the Outline.")]
        [Input("stem", "The Stem to update the ExternalEdge of.")]
        [Input("curves", "A list of IElement1Ds which all should be of a type of ICurve.")]
        [Output("stem", "A new Stem with Outline matching the provided edges.")]
        public static Stem SetOutlineElements1D(this Stem stem, IEnumerable<IElement1D> curves)
        {
            if (stem.IsNull())
                return stem;

            if (curves.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The list of IElement1D is null or empty.");
                return stem;
            }

            PolyCurve curve = Geometry.Compute.IJoin(curves.Cast<ICurve>().ToList())[0];

            stem.Outline = curve;

            return stem;
        }

    }
}




