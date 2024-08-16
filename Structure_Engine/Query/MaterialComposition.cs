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


using BH.oM.Structure.MaterialFragments;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SectionProperties;
using BH.oM.Structure.SurfaceProperties;
using BH.oM.Structure.Reinforcement;
using System.Collections.Generic;
using System;
using System.Linq;
using BH.oM.Physical.Materials;
using BH.oM.Dimensional;
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
            if (areaElement.IIsNull() || areaElement.Property.IsNull())
                return null;

            if (areaElement.FindFragment<PanelRebarIntent>() != null)
                Engine.Base.Compute.RecordWarning("The areaElement has a PanelRebarIntent, which will not be included in the MaterialComposition. Please account for replacement of concrete volume with reinforcement externally.");

            ReinforcementDensity reinfDensity = areaElement.FindFragment<ReinforcementDensity>();

            return areaElement.Property.IMaterialComposition(reinfDensity);
        }

        /***************************************************/

        [Description("Returns a ConcreteSection's MaterialComposition, taking into account any LongitudinalReinforcement.")]
        [Input("property", "The ConcreteSection to query.")]
        [Output("materialComposition", "The MaterialComposition of the ConcreteSection.")]
        public static MaterialComposition MaterialComposition(this ConcreteSection property)
        {
            if (property.IsNull())
                return null;

            double sectionArea = property.Area;

            List<double> areas = new List<double>();
            List<Material> materials = new List<Material>();

            if (property?.RebarIntent?.BarReinforcement != null)
            {
                //TODO: Resolve for stirups as well
                foreach (LongitudinalReinforcement reinforcement in property.RebarIntent.BarReinforcement.OfType<LongitudinalReinforcement>())
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
                return (MaterialComposition)Physical.Create.Material(property.Material);
            }
            else
            {
                areas.Insert(0, sectionArea);
                materials.Insert(0, Physical.Create.Material(property.Material));

                return Engine.Matter.Compute.AggregateMaterialComposition(materials.Select(x => (MaterialComposition)x), areas);
            }
        }

        /***************************************************/

        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the Panel.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition MaterialComposition(this Layered property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull() || property.Layers.All(x => x.Material.IsNull()))
                return null;

            if (property.Layers.All(x => x.Material == null)) //cull any null layers, raise a warning.            
            {
                Base.Compute.RecordError("Cannote evaluate MaterialComposition because all of the materials are null.");
                return null;
            }

            if (reinforcementDensity != null)
                Base.Compute.RecordWarning("The layered property has a ReinforcementDensity which will not be included in the MaterialComposition, because it is not known which layer to replace. Please account for this reinforcement externally.");

            if (property.Layers.Any(x => x.Material == null)) //cull any null layers, raise a warning.            
                Base.Compute.RecordWarning("At least one Material in a Layered surface property was null. VolumePerArea excludes this layer, assuming it is void space.");

            IEnumerable<Layer> solidLayers = property.Layers.Where(x => x.Material != null); //filter to only layers which are solid.
            return Matter.Create.MaterialComposition(solidLayers.Select(x => Physical.Create.Material(x.Material)), solidLayers.Select(x => x.Thickness));
        }

        /***************************************************/

        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the Panel.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition MaterialComposition(this CorrugatedDeck property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            if (reinforcementDensity != null)
                Base.Compute.RecordWarning("the CorrugatedDeck property has a ReinforcementDensity which will not be included in the MaterialComposition, because it is inconceivable.");

            return (MaterialComposition)Physical.Create.Material(property.Material);
        }

        /***************************************************/

        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the Panel.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition MaterialComposition(this ToppedSlab property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull() || property.BaseProperty.IsNull() || property.Material.IsNull())
                return null;

            double baseVolume = property.BaseProperty.IVolumePerArea();
            double toppingVol = property.ToppingThickness;

            MaterialComposition baseComposition = property.BaseProperty.IMaterialComposition(reinforcementDensity);

            MaterialComposition toppingComposition;
            if (reinforcementDensity == null)
                toppingComposition = (MaterialComposition)Physical.Create.Material(property.Material);
            else
                toppingComposition = property.Material.MaterialComposition(reinforcementDensity);

            return Matter.Compute.AggregateMaterialComposition(new List<MaterialComposition> { baseComposition, toppingComposition }, new List<double> { baseVolume, toppingVol });
        }

        /***************************************************/


        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the Panel.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition MaterialComposition(this SlabOnDeck property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull() || property.Material.IsNull() || property.DeckMaterial.IsNull())
            {
                return null;
            }

            double deckVolume = property.DeckThickness * property.DeckVolumeFactor;
            double slabVolume = property.VolumePerArea() - deckVolume;

            if (reinforcementDensity != null && reinforcementDensity.Material != null)
            {
                MaterialComposition slabComposition = property.Material.MaterialComposition(reinforcementDensity);

                double rebarVolume = slabVolume * slabComposition.Ratios[1];
                slabVolume -= rebarVolume;

                return Matter.Create.MaterialComposition(
                    new List<Material>() {
                    Physical.Create.Material(property.Material),
                    Physical.Create.Material(property.DeckMaterial),
                    Physical.Create.Material(reinforcementDensity.Material)
                    },
                    new List<double>() { slabVolume, deckVolume, rebarVolume }
                    );
            }

            return Matter.Create.MaterialComposition(
                new List<Material>() {
                    Physical.Create.Material(property.Material),
                    Physical.Create.Material(property.DeckMaterial)
                },
                new List<double>() { slabVolume, deckVolume }
                );
        }

        /***************************************************/

        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the Panel.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition MaterialComposition(this Cassette property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            //If only main material provided, use it for all parts
            if (property.RibMaterial == null && property.BottomMaterial == null)
                return property.Material.MaterialComposition(reinforcementDensity);

            IMaterialFragment topMat = property.Material;
            IMaterialFragment bottomMat = property.BottomMaterial ?? property.Material;
            IMaterialFragment ribMat = property.RibMaterial ?? property.Material;
            double volPerAreaRibZone = property.RibHeight * (property.RibThickness / property.RibSpacing);

            return Matter.Compute.AggregateMaterialComposition(new List<MaterialComposition>
            { 
                topMat.MaterialComposition(reinforcementDensity),
                bottomMat.MaterialComposition(reinforcementDensity),
                ribMat.MaterialComposition(reinforcementDensity)
            },
            new List<double> { property.TopThickness, property.BottomThickness, volPerAreaRibZone });
        }

        /***************************************************/

        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the Panel.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition MaterialComposition(this BuiltUpRibbed property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            //If only main material provided, use it for all parts
            if (property.RibMaterial == null)
                return property.Material.MaterialComposition(reinforcementDensity);

            IMaterialFragment topMat = property.Material;
            IMaterialFragment ribMat = property.RibMaterial ?? property.Material;
            double volPerAreaRibZone = property.RibHeight * (property.RibThickness / property.RibSpacing);
            return Matter.Compute.AggregateMaterialComposition(new List<MaterialComposition>
            {
                topMat.MaterialComposition(reinforcementDensity),
                ribMat.MaterialComposition(reinforcementDensity)
            },
            new List<double> { property.TopThickness, volPerAreaRibZone });
        }

        /***************************************************/

        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the Panel.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition MaterialComposition(this BuiltUpDoubleRibbed property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            IMaterialFragment topMat = property.Material;
            IMaterialFragment ribMat = property.RibMaterial ?? property.Material;

            bool reinfToSlab = false;
            bool reinfToRib = false;
            if (reinforcementDensity != null)
            {
                bool ribIsConcrete = ribMat is Concrete;
                bool topIsConcrete = topMat is Concrete;

                if (ribIsConcrete && topIsConcrete) //Both concrete
                {
                    reinfToSlab = true;
                    reinfToRib = true;
                }
                else if (ribIsConcrete) //Only rib concrete
                {
                    reinfToRib = true;
                    Base.Compute.RecordNote($"Only the ribs in the in the {nameof(BuiltUpDoubleRibbed)} is Concrete. Provided {nameof(ReinforcementDensity)} is only applied to the ribs.");
                }
                else if (topIsConcrete) //Only slab concrete
                {
                    reinfToRib = true;
                    Base.Compute.RecordNote($"Only the slab in the in the {nameof(BuiltUpDoubleRibbed)} is Concrete. Provided {nameof(ReinforcementDensity)} is only applied to the slab.");
                }
                else    //Neither is concrete. Add to both and record a note
                {
                    reinfToSlab = true;
                    reinfToRib = true;
                    Base.Compute.RecordNote($"Neither ribs or slab in the {nameof(BuiltUpDoubleRibbed)} is Concrete. Provided {nameof(ReinforcementDensity)} is applied to both.");

                }
            }

            //If only main material provided, use it for all parts
            if (property.RibMaterial == null)
                return property.Material.MaterialComposition(reinforcementDensity);

            double volPerAreaRibZone = property.RibHeight * (property.RibThickness * 2 / property.RibSpacing);
            return Matter.Compute.AggregateMaterialComposition(new List<MaterialComposition>
            {
                topMat.MaterialComposition(reinfToSlab ? reinforcementDensity : null),
                ribMat.MaterialComposition(reinfToRib ? reinforcementDensity : null)
            },
            new List<double> { property.TopThickness, volPerAreaRibZone });
        }

        /***************************************************/

        [Description("Returns a Pile's homogeneous MaterialComposition.")]
        [Input("pile", "The Pile to get material from.")]
        [Output("materialComposition", "The kind of matter the Pile is composed of.")]
        public static MaterialComposition MaterialComposition(this Pile pile)
        {
            if (pile.IsNull())
                return null;

            if (pile.Section.IsNull() || pile.Section.Material.IsNull())
                return null;

            return pile.Section.IMaterialComposition();
        }

        /***************************************************/

        [Description("Returns a PileFoundation's homogeneous MaterialComposition.")]
        [Input("pileFoundation", "The PileFoundation to get material from.")]
        [Output("materialComposition", "The kind of matter the Pile is composed of.")]
        public static MaterialComposition MaterialComposition(this PileFoundation pileFoundation)
        {
            if (pileFoundation.IsNull())
                return null;

            MaterialComposition pileCap = pileFoundation.PileCap.MaterialComposition();
            List<MaterialComposition> piles = pileFoundation.Piles.Select(x => x.MaterialComposition()).ToList();

            List<IElementM> elements = new List<IElementM>();
            elements.Add(pileFoundation.PileCap);
            elements.AddRange(pileFoundation.Piles);

            return Matter.Compute.AggregateMaterialComposition(elements);
        }

        /***************************************************/

        [Description("Returns a Stem's homogeneous MaterialComposition.")]
        [Input("stem", "The Stem to query.")]
        [Output("materialComposition", "The MaterialComposition of the Stem")]
        public static MaterialComposition MaterialComposition(this Stem stem)
        {
            if (stem.IsNull() || stem.Material.IsNull())
                return null;

            ReinforcementDensity reinfDensity = stem.FindFragment<ReinforcementDensity>();

            return MaterialComposition(stem.Material, reinfDensity);
        }

        /***************************************************/

        [Description("Returns a RetainingWall's homogeneous MaterialComposition based on the Stem and Footing.")]
        [Input("retainingWall", "The RetainingWall to query.")]
        [Output("materialComposition", "The MaterialComposition of the RetainingWall")]
        public static MaterialComposition MaterialComposition(this RetainingWall retainingWall)
        {

            if (retainingWall.IsNull() && retainingWall.Stem.IsNull() && retainingWall.Footing.IsNull())
                return null;


            List<IElementM> elements = new List<IElementM>
            {
                retainingWall.Stem,
                retainingWall.Footing
            };

            //Case for when ReinforcementDensity is attached to both higher and lower level objects.
            if ((retainingWall.Stem.FindFragment<ReinforcementDensity>() != null || retainingWall.Footing.FindFragment<ReinforcementDensity>() != null) && retainingWall.FindFragment<ReinforcementDensity>() != null)
            {
                Base.Compute.RecordWarning("A ReinforcementDensity Fragment is found on both the RetainingWall and on at least one of its defining objects. The ReinforcementDensity of the defining objects has been used.");
                return Matter.Compute.AggregateMaterialComposition(elements);
            }

            //Case when ReinforcementDensity is atatched to lower level objects.
            //Gives an error becasue the ReinforcementDensity is null on the retaining wall even though it works fine.
            else if (retainingWall.Stem.FindFragment<ReinforcementDensity>() != null || retainingWall.Footing.FindFragment<ReinforcementDensity>() != null)
                return Matter.Compute.AggregateMaterialComposition(elements);

            //Case when ReinforcementDensity is atatched to higher level object.
            //Gives an error becasue the ReinforcementDensity is null on the retaining wall even though it works fine.
            else if (retainingWall.FindFragment<ReinforcementDensity>() != null)
            {
                ReinforcementDensity reinfDensity = retainingWall.FindFragment<ReinforcementDensity>();

                List<IElementM> reinforcedElements = new List<IElementM>
                {
                    (IElementM)retainingWall.Stem.AddFragment(reinfDensity),
                    (IElementM)retainingWall.Footing.AddFragment(reinfDensity)
                };

                return Matter.Compute.AggregateMaterialComposition(reinforcedElements);
            }

            return Matter.Compute.AggregateMaterialComposition(elements);
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



        [Description("Returns a SurfaceProperty's MaterialComposition.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        public static MaterialComposition IMaterialComposition(this ISurfaceProperty property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull()) //Specific MaterialComposition(SurfaceProp) methods must check for material null- some properties ignore the base material.
                return null;

            return MaterialComposition(property as dynamic, reinforcementDensity);
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

        [Description("Gets the MaterialComposition for homogenous SurfaceProperties. Multi-material properties will not be reported correctly.")]
        [Input("property", "The SurfaceProperty to query.")]
        [Input("reinforcementDensity", "ReinforcementDensity assigned to the Panel.")]
        [Output("materialComposition", "The MaterialComposition of the SurfaceProperty.")]
        private static MaterialComposition MaterialComposition(this ISurfaceProperty property, ReinforcementDensity reinforcementDensity = null)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            if (reinforcementDensity == null)
                return (MaterialComposition)Physical.Create.Material(property.Material);
            else
                return property.Material.MaterialComposition(reinforcementDensity);
        }


        /***************************************************/
        /**** Private methods                           ****/
        /***************************************************/

        private static MaterialComposition MaterialComposition(this IMaterialFragment baseMaterial, ReinforcementDensity reinforcementDensity)
        {
            if (reinforcementDensity?.Material == null || reinforcementDensity.Material.Density == 0 || reinforcementDensity.Density == 0)
                return (MaterialComposition)Physical.Create.Material(baseMaterial);

            if (reinforcementDensity.Material.Density < 0)
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




