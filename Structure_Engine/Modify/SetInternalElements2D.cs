/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Linq;
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        [Description("Sets internal IElement2Ds of an Opening. Method required for all IElement2Ds. \n" +
                     "As the opening does not contain any internal IElement2Ds, this method if called will simply raise an error and return a shallow clone of the Opening.")]
        [Input("opening", "The Opening, nothing will be updated as no Property available to be updated.")]
        [Input("internalElements2D", "The internal IElement2Ds to set. Will be unused, as nothing can be set to the Opening.")]
        [Output("opening", "The Opening cloned. No other changes applied.")]
        public static Opening SetInternalElements2D(this Opening opening, List<IElement2D> internalElements2D)
        {
            if (internalElements2D.Count != 0)
                Reflection.Compute.RecordError("Cannot set internal 2D elements to an Opening.");

            return opening.GetShallowClone() as Opening;
        }

        /***************************************************/

        [Description("Sets internal IElement2Ds of a Panel, i.e. sets the Openings of a Panel. Method required for all IElement2Ds.")]
        [Input("panel", "The Panel to update the ")]
        [Input("openings", "The internal IElement2Ds to set. For a Panel this should be a list of structural Openings.")]
        [Output("panel", "The Panel with updated Openings.")]
        public static Panel SetInternalElements2D(this Panel panel, List<IElement2D> openings)
        {
            Panel pp = panel.GetShallowClone() as Panel;
            pp.Openings = new List<Opening>(openings.Cast<Opening>().ToList());
            return pp;
        }

        /***************************************************/
    }
}

