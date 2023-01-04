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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Reinforcement;
using BH.oM.Structure.Elements;
using BH.oM.Physical.Reinforcement;
using BH.oM.Physical.Materials;
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

        [Description("Extract all physical ReinforcingBars from the structural Bar. Only extracts reinforcement for bars owning a ConcreteSection, for other section types, an empty list will be returned.")]
        [Input("bar", "The structural Bar to extract Reinforcement from.")]
        [Output("rebars", "All physical ReinforcingBar on the provided Bar.")]
        public static List<IReinforcingBar> ReinforcingBars(this Bar bar)
        {
            if (bar.IsNull())
                return null;

            List<IReinforcingBar> rebars = new List<IReinforcingBar>();
            ConcreteSection section = bar.SectionProperty as ConcreteSection;

            List<ICurve> outerProfileEdges;
            List<ICurve> innerProfileEdges;
            List<LongitudinalReinforcement> longReif;
            List<TransverseReinforcement> tranReif;
            double longCover, tranCover;

            if (section.CheckSectionAndExtractParameters(out outerProfileEdges, out innerProfileEdges, out longReif, out tranReif, out longCover, out tranCover))
            {
                TransformMatrix transformation = bar.BarSectionTranformation();
                double length = bar.Length();

                List<IBarReinforcement> barReinf = new List<IBarReinforcement>();
                barReinf.AddRange(longReif);
                barReinf.AddRange(tranReif);

                foreach (IBarReinforcement reif in barReinf)
                {
                    Material material;
                    if (reif.Material != null)
                        material = Physical.Create.Material(reif.Material);
                    else
                        material = new Material();

                    if (reif is LongitudinalReinforcement)
                    {
                        foreach (ICurve centreLine in reif.IReinforcementLayout(longCover, outerProfileEdges, innerProfileEdges, length, transformation))
                        {
                            PrimaryReinforcingBar rebar = Physical.Create.PrimaryReinforcingBar(centreLine, reif.Diameter, material);
                            rebars.Add(rebar);
                        }
                    }
                    else if (reif is TransverseReinforcement)
                    {
                        foreach (ICurve centreLine in reif.IReinforcementLayout(tranCover, outerProfileEdges, innerProfileEdges, length, transformation))
                        {
                            Stirrup rebar = Physical.Create.Stirrup(centreLine, reif.Diameter, material);
                            rebars.Add(rebar);
                        }
                    }
                }
            }

            return rebars;
        }

        /***************************************************/
    }
}



