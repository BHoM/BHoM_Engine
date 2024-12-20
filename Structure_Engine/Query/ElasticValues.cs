/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Structure.Constraints;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the spring values from a Constraint6DOF as a double array. Values returned in the following order: TransX, TranxY, TransZ, RotX, RotY, RotZ.")]
        [Input("constraint", "The Constraint6DOF to spring values from.")]
        [Output("springValues", "The elastic values, or spring values, from a constraint as a double array in the following order: TransX, TransY, TransZ, RotX, RotY, RotZ.")]
        public static double[] ElasticValues(this Constraint6DOF constraint)
        {
            return constraint.IsNull() ? null : new double[]
            {
                constraint.TranslationalStiffnessX,
                constraint.TranslationalStiffnessY,
                constraint.TranslationalStiffnessZ,
                constraint.RotationalStiffnessX,
                constraint.RotationalStiffnessY,
                constraint.RotationalStiffnessZ
            };
        }

        /***************************************************/
    }
}






