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

using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /******************************************/
        /****            IElement1D            ****/
        /******************************************/

        [Description("Sets the IElement0Ds of the Bar, i.e. its two end Nodes or Points. Method required for IElement1Ds.")]
        [Input("bar", "The Bar to set the IElement0Ds to.")]
        [Input("newElements0D", "The new IElement0Ds of the Bar. Should be a list of length two, containing exactly two structural Nodes or Geometrical Points. \n" +
                                "Points will assigin default end properties to the Bar, i.e. Fixed releases, no support.")]
        [Output("bar","The bar with updated Nodes.")]
        public static Bar SetElements0D(this Bar bar, List<IElement0D> newElements0D)
        {
            if (newElements0D.Count != 2)
            {
                Base.Compute.RecordError("A bar is defined by 2 nodes.");
                return null;
            }

            Bar clone = bar.DeepClone() as Bar;

            // Default the Bars end if the input is an Point
            if (newElements0D[0] is Point)
            {
                clone.StartNode = Create.Node(newElements0D[0] as Point);
                if (clone.Release != null)
                    clone.Release.StartRelease = Create.FixConstraint6DOF();
            }
            else
                clone.StartNode = newElements0D[0] as Node;

            // Default the Bars end if the input is an Point
            if (newElements0D[1] is Point)
            {
                clone.EndNode = Create.Node(newElements0D[1] as Point);
                if (clone.Release != null)
                    clone.Release.EndRelease = Create.FixConstraint6DOF();
            }
            else
                clone.EndNode = newElements0D[1] as Node;

            return clone;
        }

        /******************************************/
    }
}




