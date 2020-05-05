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

        [Description("Extract all ReinforcingBars from the structural Bar. Only extract reinforcement for bars owning a ConcreteSection, for other sectiontypes, an empty list will be returned.")]
        [Input("bar", "THe structural bar to extract Reinforcement from.")]
        [Output("rebars", "All ReinforcingBar on the provided Bar.")]
        public static List<IReinforcingBar> ReinforcingBars(this Bar bar)
        {
            ConcreteSection section = bar.SectionProperty as ConcreteSection;
            if (section == null)
                return new List<IReinforcingBar>();

            //Extract Longitudinal reinforcement
            List<LongitudinalReinforcement> longReif = section.LongitudinalReinforcement();

            //No longitudinal reinforcement available
            if (longReif.Count == 0)
                return new List<IReinforcingBar>();

            List<ICurve> outerProfileEdges;
            List<ICurve> innerProfileEdges;

            ExtractInnerAndOuterEdges(section, out outerProfileEdges, out innerProfileEdges);

            //Need at least one external edge curve
            if (outerProfileEdges.Count == 0)
                return new List<IReinforcingBar>();


            TransformMatrix transformation = bar.BarSectionTranformation();
            double length = bar.Length();

            //TODO: include stirups for offset distance
            double stirupOffset = 0;

            List<IReinforcingBar> rebars = new List<IReinforcingBar>();

            foreach (LongitudinalReinforcement reif in longReif)
            {
                Material material;
                if (reif.Material != null)
                    material = Engine.Physical.Create.Material(reif.Material);
                else
                    material = new Material();

                foreach (Line centreLine in reif.ReinforcementLayout(stirupOffset, outerProfileEdges, innerProfileEdges, length, transformation))
                {
                    PrimaryReinforcingBar rebar = new PrimaryReinforcingBar
                    {
                        CentreCurve = centreLine,
                        Diameter = reif.Diameter,
                        Material = material,
                    };
                    rebars.Add(rebar);
                }
            }

            return rebars;
        }

        /***************************************************/
    }
}
