/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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


using BH.oM.Structure.MaterialFragments;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.Reinforcement;
using System.Collections.Generic;
using System;
using System.Linq;
using BH.oM.Physical.Materials;
using BH.Engine.Spatial;
using BH.oM.Structure.Fragments;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a Bar's homogeneous MaterialComposition.")]
        [Input("bar", "The Bar to material from.")]
        [Output("materialComposition", "The kind of matter the Bar is composed of.")]
        public static MaterialComposition MaterialComposition(this Bar bar)
        {
            if (bar.IsNull())
                return null;

            if (bar.SectionProperty == null || bar.SectionProperty.Material == null)
            {
                Engine.Base.Compute.RecordError("The Bars MaterialComposition could not be calculated as no Material has been assigned.");
                return null;
            }

            ReinforcementDensity reinfDensity = bar.FindFragment<ReinforcementDensity>();

            if (reinfDensity == null)
                return bar.SectionProperty.IMaterialComposition();
            else
            {
                return bar.SectionProperty.Material.MaterialComposition(reinfDensity);
            }
        }

        /***************************************************/

        [Description("Returns an AreaElement's homogeneous MaterialComposition.")]
        [Input("areaElement", "The AreaElement to material from.")]
        [Output("materialComposition", "The kind of matter the AreaElement is composed of.")]
        public static MaterialComposition MaterialComposition(this IAreaElement areaElement)
        {
            if (areaElement.IIsNull())
                return null;

            if (areaElement.Property == null || areaElement.Property.Material == null)
            {
                Engine.Base.Compute.RecordError("The areaElements MaterialComposition could not be calculated as no Material has been assigned.");
                return null;
            }

            ReinforcementDensity reinfDensity = areaElement.FindFragment<ReinforcementDensity>();

            if (reinfDensity == null)
            {
                Material mat = Physical.Create.Material(areaElement.Property.Material);
                return (MaterialComposition)mat;
            }
            else
            {
                return areaElement.Property.Material.MaterialComposition(reinfDensity);
            }
        }

        /***************************************************/

        [Description("Returns a ConcreteSection's MaterialComposition, taking into account any LongitudinalReinforcement.")]
        public static MaterialComposition MaterialComposition(this ConcreteSection sectionProperty)
        {
            if (sectionProperty.IsNull())
                return null;

            double sectionArea = sectionProperty.Area;

            List<double> areas = new List<double>();
            List<Material> materials = new List<Material>();

            if (sectionProperty?.RebarIntent?.BarReinforcement != null)
            {
                //TODO: Resolve for stirups as well
                foreach (LongitudinalReinforcement reinforcement in sectionProperty.RebarIntent.BarReinforcement.OfType<LongitudinalReinforcement>())
                {
                    //Calculate reinforcement area for a section cut
                    double reinArea = reinforcement.Area();

                    //Scale area with distribution along the length
                    double factor = Math.Min(reinforcement.EndLocation - reinforcement.StartLocation, 1);
                    reinArea *= factor;

                    //Subtract reinforcement area from the section area
                    sectionArea -= reinArea;
                    areas.Add(reinArea);
                    Material reifMaterial;

                    if (reinforcement.Material != null)
                        reifMaterial = Physical.Create.Material(reinforcement.Material);
                    else
                        reifMaterial = new Material();

                    materials.Add(reifMaterial);
                }
            }

            if (materials.Count == 0)
            {
                return (MaterialComposition)Physical.Create.Material(sectionProperty.Material);
            }
            else
            {
                areas.Insert(0, sectionArea);
                materials.Insert(0, Physical.Create.Material(sectionProperty.Material));

                return Engine.Matter.Compute.AggregateMaterialComposition(materials.Select(x => (MaterialComposition)x), areas);
            }
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Returns a SectionProperty's MaterialComposition.")]
        public static MaterialComposition IMaterialComposition(this ISectionProperty sectionProperty)
        {
            return sectionProperty.IsNull() ? null : MaterialComposition(sectionProperty as dynamic);
        }

        /***************************************************/
        /**** Private methods - Default                 ****/
        /***************************************************/

        [Description("Returns a SectionProperty's homogeneous MaterialComposition.")]
        private static MaterialComposition MaterialComposition(this ISectionProperty sectionProperty)
        {
            return sectionProperty.IsNull() ? null : (MaterialComposition)Physical.Create.Material(sectionProperty.Material);
        }

        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private static MaterialComposition MaterialComposition(this IMaterialFragment baseMaterial, ReinforcementDensity reinforcementDensity)
        {
            if(reinforcementDensity.Material == null || reinforcementDensity.Material.Density == 0 || reinforcementDensity.Density == 0)
                return (MaterialComposition)Physical.Create.Material(baseMaterial);
      
            if(reinforcementDensity.Material.Density < 0)
                Engine.Base.Compute.RecordWarning("The Density of the Material of the ReinforcementDensity is less than 0. Please check the data to ensure its validity. Care should be taken using the MaterialComposition.");

            if (reinforcementDensity.Density < 0)
                Engine.Base.Compute.RecordWarning("The Density of the ReinforcementDensity is less than 0. Please check the data to ensure its validity. Care should be taken using the MaterialComposition.");

            //Calculate volume ratio of reinforcement
            double reinfRatio = reinforcementDensity.Density / reinforcementDensity.Material.Density;

            if (reinfRatio > 1)
            {
                Engine.Base.Compute.RecordWarning("The ReinforcementDensity fragment on an object gives a volume ratio larger than 1. This means the volume of reinforcement exceeds the volume of the host object. Please check the data to ensure its validity. Care should be taken using the MaterialComposition.");
            }

            //Remaining volume to base material
            double baseVolume = 1 - reinfRatio;

            return new MaterialComposition(
                new List<Material> { Physical.Create.Material(baseMaterial), Physical.Create.Material(reinforcementDensity.Material) },
                new List<double> { baseVolume, reinfRatio }
                );
        }

        /***************************************************/

    }
}


