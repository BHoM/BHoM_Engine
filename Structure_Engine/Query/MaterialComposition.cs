/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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


using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Fragments;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Reinforcement;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

            return Engine.Matter.Create.MaterialComposition(bar.GeneralMaterialTakeoff());
        }

        /***************************************************/

        [Description("Returns an AreaElement's homogeneous MaterialComposition.")]
        [Input("areaElement", "The AreaElement to material from.")]
        [Output("materialComposition", "The kind of matter the AreaElement is composed of.")]
        public static MaterialComposition MaterialComposition(this IAreaElement areaElement)
        {
            if (areaElement.IIsNull() || areaElement.Property.IsNull())
                return null;

            return Engine.Matter.Create.MaterialComposition(areaElement.GeneralMaterialTakeoff());
        }

        /***************************************************/

        [Description("Returns a ConcreteSection's MaterialComposition, taking into account any Reinfrocement.")]
        [Input("property", "The ConcreteSection to query.")]
        [Output("materialComposition", "The MaterialComposition of the ConcreteSection.")]
        public static MaterialComposition MaterialComposition(this ConcreteSection property)
        {
            if (property.IsNull())
                return null;

            GeneralMaterialTakeoff takeoff = property.GeneralMaterialTakeoff(1, null, null);
            return Matter.Create.MaterialComposition(takeoff);
        }

        /***************************************************/

        [Description("Returns a Pile's homogeneous MaterialComposition.")]
        [Input("pile", "The Pile to get material from.")]
        [Output("materialComposition", "The kind of matter the Pile is composed of.")]
        public static MaterialComposition MaterialComposition(this Pile pile)
        {
            if (pile.IsNull())
                return null;

            return Engine.Matter.Create.MaterialComposition(pile.GeneralMaterialTakeoff());
        }

        /***************************************************/

        [Description("Returns a PileFoundation's homogeneous MaterialComposition.")]
        [Input("pileFoundation", "The PileFoundation to get material from.")]
        [Output("materialComposition", "The kind of matter the Pile is composed of.")]
        public static MaterialComposition MaterialComposition(this PileFoundation pileFoundation)
        {
            if (pileFoundation.IsNull())
                return null;

            return Engine.Matter.Create.MaterialComposition(pileFoundation.GeneralMaterialTakeoff());
        }

        /***************************************************/

        [Description("Returns a Stem's homogeneous MaterialComposition.")]
        [Input("stem", "The Stem to query.")]
        [Output("materialComposition", "The MaterialComposition of the Stem.")]
        public static MaterialComposition MaterialComposition(this Stem stem)
        {
            if (stem.IsNull() || stem.Material.IsNull())
                return null;

            return Engine.Matter.Create.MaterialComposition(stem.GeneralMaterialTakeoff());
        }

        /***************************************************/

        [Description("Returns a RetainingWall's homogeneous MaterialComposition based on the Stem and Footing.")]
        [Input("retainingWall", "The RetainingWall to query.")]
        [Output("materialComposition", "The MaterialComposition of the RetainingWall.")]
        public static MaterialComposition MaterialComposition(this RetainingWall retainingWall)
        {
            if (retainingWall.IsNull() && retainingWall.Stem.IsNull() && retainingWall.Footing.IsNull())
                return null;


            return Engine.Matter.Create.MaterialComposition(retainingWall.GeneralMaterialTakeoff());
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Returns a SectionProperty's MaterialComposition.")]
        [Input("property", "The SectionProperty to query.")]
        [Output("materialComposition", "The MaterialComposition of the SectionProperty.")]
        public static MaterialComposition IMaterialComposition(this ISectionProperty property)
        {
            return property.IsNull() ? null : MaterialComposition(property as dynamic);
        }

        /***************************************************/

        [PreviousVersion("9.3", "BH.Engine.Structure.Query.IMaterialComposition(BH.oM.Structure.SurfaceProperties.ISurfaceProperty, BH.oM.Structure.Fragments.ReinforcementDensity)")]
        [PreviousVersion("9.3", "BH.Engine.Structure.Query.MaterialComposition(BH.oM.Structure.SurfaceProperties.BuiltUpDoubleRibbed, BH.oM.Structure.Fragments.ReinforcementDensity)")]
        [PreviousVersion("9.3", "BH.Engine.Structure.Query.MaterialComposition(BH.oM.Structure.SurfaceProperties.BuiltUpRibbed, BH.oM.Structure.Fragments.ReinforcementDensity)")]
        [PreviousVersion("9.3", "BH.Engine.Structure.Query.MaterialComposition(BH.oM.Structure.SurfaceProperties.Cassette, BH.oM.Structure.Fragments.ReinforcementDensity)")]
        [PreviousVersion("9.3", "BH.Engine.Structure.Query.MaterialComposition(BH.oM.Structure.SurfaceProperties.SlabOnDeck, BH.oM.Structure.Fragments.ReinforcementDensity)")]
        [PreviousVersion("9.3", "BH.Engine.Structure.Query.MaterialComposition(BH.oM.Structure.SurfaceProperties.ToppedSlab, BH.oM.Structure.Fragments.ReinforcementDensity)")]
        [PreviousVersion("9.3", "BH.Engine.Structure.Query.MaterialComposition(BH.oM.Structure.SurfaceProperties.CorrugatedDeck, BH.oM.Structure.Fragments.ReinforcementDensity)")]
        [PreviousVersion("9.3", "BH.Engine.Structure.Query.MaterialComposition(BH.oM.Structure.SurfaceProperties.Layered, BH.oM.Structure.Fragments.ReinforcementDensity)")]
        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the SurfaceProperty.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition MaterialComposition(this ISurfaceProperty property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull()) //Specific MaterialComposition(SurfaceProp) methods must check for material null- some properties ignore the base material.
                return null;

            return Engine.Matter.Create.MaterialComposition(property.IGeneralMaterialTakeoff(1, reinforcementDensity));
        }

        /***************************************************/
        /**** Private methods - Default                 ****/
        /***************************************************/

        [Description("Returns a SectionProperty's homogeneous MaterialComposition.")]
        [Input("property", "The SectionProperty to query.")]
        [Output("materialComposition", "The MaterialComposition of the SectionProperty.")]
        private static MaterialComposition MaterialComposition(this ISectionProperty sectionProperty)
        {
            return sectionProperty.IsNull() ? null : (MaterialComposition)Physical.Create.Material(sectionProperty.Material);
        }


        /***************************************************/
   


    }
}

