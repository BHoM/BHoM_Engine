﻿/*
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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SectionProperties.Reinforcement;
using BH.oM.Structure.Elements;
using BH.oM.Physical.Reinforcement;
using BH.oM.Physical.Materials;
using BH.oM.Spatial.Layouts;
using BH.oM.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Geometry;
using BH.oM.Base;


namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Return the number of rebars in one particular Reinforcement object.")]
        [Input("reinforcement", "The reinforcement object to extract the count from.")]
        [Output("count", "Number of bars in the provided LongitudinalReinforcement.")]
        public static int ReinforcingBarCount(this LongitudinalReinforcement reinforcement)
        {
            return LayoutCount(reinforcement.RebarLayout as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static int LayoutCount(ExplicitLayout layout)
        {
            return layout.Points.Count;
        }

        /***************************************************/

        private static int LayoutCount(LinearLayout layout)
        {
            return layout.NumberOfPoints;
        }

        /***************************************************/

        private static int LayoutCount(MultiLinearLayout layout)
        {
            return layout.NumberOfPoints;
        }

        /***************************************************/

        private static int LayoutCount(PerimeterLayout layout)
        {
            return layout.NumberOfPoints;
        }

        /***************************************************/

        private static int LayoutCount(ILayout2D layout)
        {
            Engine.Reflection.Compute.RecordError("Could not extract number of points from layout of type " + layout.GetType().Name);
            return 0;
        }

        /***************************************************/
    }
}
