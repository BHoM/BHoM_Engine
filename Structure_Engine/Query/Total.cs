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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Structure.Results;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousInputNames("displacement", "disp")]
        [Description("Gets the total resolved translational displacement of the three components of the result.")]
        [Input("displacement", "The displacement result to get the resolved rotation from.")]
        [Output("UTot", "The total resolved translational displacement of the result.", typeof(oM.Quantities.Attributes.Length))]
        public static double UTotal(this IDisplacement displacement)
        {
            if (displacement == null)
                return double.NaN;
            return Math.Sqrt(displacement.UX * displacement.UX + displacement.UY * displacement.UY + displacement.UZ * displacement.UZ);
        }

        /***************************************************/

        [Description("Gets the total resolved rotational displacement of the three components of the result.")]
        [Input("displacement", "The displacement result to get the resolved rotation from.")]
        [Output("RTot", "The total resolved rotational displacement of the result.", typeof(oM.Quantities.Attributes.Angle))]
        public static double RTotal(this IDisplacement displacement)
        {
            if (displacement == null)
                return double.NaN;
            return Math.Sqrt(displacement.RX * displacement.RX + displacement.RY * displacement.RY + displacement.RZ * displacement.RZ);
        }

        /***************************************************/

        [Description("Gets the total resolved translational displacement of the three components of the result.")]
        [Input("displacement", "The displacement result to get the resolved translation from.")]
        [Output("UTot", "The total resolved translational displacement of the result.", typeof(oM.Quantities.Attributes.Length))]
        public static double UTotal(this IMeshDisplacement displacement)
        {
            if (displacement == null)
                return double.NaN;
            return Math.Sqrt(displacement.UXX * displacement.UXX + displacement.UYY * displacement.UYY + displacement.UZZ * displacement.UZZ);
        }

        /***************************************************/

        [Description("Gets the total resolved rotational displacement of the three components of the result.")]
        [Input("displacement", "The displacement result to get the resolved translation from.")]
        [Output("RTot", "The total resolved rotational displacement of the result.", typeof(oM.Quantities.Attributes.Angle))]
        public static double RTotal(this IMeshDisplacement displacement)
        {
            if (displacement == null)
                return double.NaN;
            return Math.Sqrt(displacement.RXX * displacement.RXX + displacement.RYY * displacement.RYY + displacement.RZZ * displacement.RZZ);
        }

        /***************************************************/

        [Description("Gets the resolved reaction force of the three components of the result.")]
        [Input("reaction", "The reaction result to get the resolved total reaction force from.")]
        [Output("FTot", "The total resolved reaction force of the result.", typeof(oM.Quantities.Attributes.Force))]
        public static double FTotal(this IReaction reaction)
        {
            if (reaction == null)
                return double.NaN;
            return Math.Sqrt(reaction.FX * reaction.FX + reaction.FY * reaction.FY + reaction.FZ * reaction.FZ);
        }

        /***************************************************/

        [Description("Gets the resolved reaction moment of the three components of the result.")]
        [Input("reaction", "The reaction result to get the resolved total reaction moment from.")]
        [Output("MTot", "The total resolved reaction moment of the result.", typeof(oM.Quantities.Attributes.Moment))]
        public static double MTotal(this IReaction reaction)
        {
            if (reaction == null)
                return double.NaN;
            return Math.Sqrt(reaction.MX * reaction.MX + reaction.MY * reaction.MY + reaction.MZ * reaction.MZ);
        }

        /***************************************************/

        [Description("Gets the resolved bending moment of the major and minor components of the result.")]
        [Input("force", "The BarForce result to get the resolved bending moment from.")]
        [Output("MTot", "The total resolved bending moment of the result.", typeof(oM.Quantities.Attributes.Moment))]
        public static double MTotal(this BarForce force)
        {
            if (force == null)
                return double.NaN;
            return Math.Sqrt(force.MY * force.MY + force.MZ * force.MZ);
        }

        /***************************************************/

    }
}



