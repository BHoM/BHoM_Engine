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
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Analytical;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Spatial.Layouts;
using BH.oM.Spatial.ShapeProfiles;
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


        [Description("Returns a Bar's GeneralMaterialTakeoff which contains information about the Bar's materiality and corresponding quantities.")]
        [Input("bar", "The Bar to evaluate.")]
        [Output("takeoff", "The GeneralMaterialTakeoff of the bar.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Bar bar)
        {
            if (bar.IsNull() || bar.SectionProperty.IsNull())
                return null;

            return bar.SectionProperty.IGeneralMaterialTakeoff(bar.Length(), bar.FindFragment<ReinforcementDensity>(), bar.FindFragment<ConnectionAllowance>());
        }

        /***************************************************/

        [Description("Returns a Pile's GeneralMaterialTakeoff which contains information about the Pile's materiality and corresponding quantities.")]
        [Input("pile", "The Pile to evaluate.")]
        [Output("takeoff", "The GeneralMaterialTakeoff of the pile.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Pile pile)
        {
            if (pile.IsNull() || pile.Section.IsNull())
                return null;

            return pile.Section.IGeneralMaterialTakeoff(pile.Length(), pile.FindFragment<ReinforcementDensity>(), pile.FindFragment<ConnectionAllowance>());
        }

        /***************************************************/

        [Description("Returns an Area Element's (Panel, FEMesh, Surface etc.) GeneralMaterialTakeoff which contains information about the element's materiality and corresponding quantities.")]
        [Input("areaElement", "The IAreaElement to evaluate.")]
        [Output("takeoff", "The GeneralMaterialTakeoff of the area element.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this IAreaElement areaElement)
        {
            if (areaElement.IIsNull() || areaElement.Property.IsNull())
                return null;

            GeneralMaterialTakeoff takeoff = areaElement.Property.IGeneralMaterialTakeoff(areaElement.IArea(), areaElement.FindFragment<ReinforcementDensity>(), areaElement.FindFragment<PanelRebarIntent>());

            takeoff.AddConnectionAllowance(areaElement.FindFragment<ConnectionAllowance>());

            return takeoff;
        }

        /***************************************************/

        [Description("Returns a PileFoundation's GeneralMaterialTakeoff, aggregating pile cap and piles. The takeoff contains information about the materiality and corresponding quantities of all the elements in the PileFoundation.")]
        [Input("pileFoundation", "The PileFoundation to evaluate.")]
        [Output("takeoff", "The aggregated GeneralMaterialTakeoff of the PileFoundation.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this PileFoundation pileFoundation)
        {
            if(pileFoundation.IsNull())
                return null;

            ReinforcementDensity topLevelReinforcementDensity = pileFoundation.FindFragment<ReinforcementDensity>();
            ConnectionAllowance topLevelConnectionAllowance = pileFoundation.FindFragment<ConnectionAllowance>();

            bool elementContainsConcrete = pileFoundation.PileCap.Property.Material is Concrete || pileFoundation.Piles.Any(x => x.Section.Material is Concrete);   //Check if any element contains concrete. This is to be able to apply the top level reinforcement density correctly for the case of for example piles being mixed between steel and concrete.

            PadFoundation pileCap = pileFoundation.PileCap;
            bool pileCapCloned = false;
            if (topLevelReinforcementDensity != null)
            {
                if (topLevelReinforcementDensity != null)
                {
                    if (pileCap.FindFragment<ReinforcementDensity>() != null)
                        Base.Compute.RecordWarning("A ReinforcementDensity Fragment is found on both the PileFoundation and on the pileCap. The reinforcement density applied directly to the pileCap is used.");
                    else if(!elementContainsConcrete || pileCap.Property.Material is Concrete)   //If the pile cap is concrete, or if none of the elements are concrete, apply the reinforcement density to the pile cap.
                    {
                        pileCap = pileCap.ShallowClone();
                        pileCapCloned = true;
                        pileCap.Fragments.Add(topLevelReinforcementDensity);
                    }
                }
            }

            if (topLevelConnectionAllowance != null)
            {
                if (pileCap.FindFragment<ConnectionAllowance>() != null)
                    Base.Compute.RecordWarning("A ConnectionAllowance Fragment is found on both the PileFoundation and on the pileCap. The connection allowance applied directly to the pileCap is used.");
                else
                {
                    if (!pileCapCloned)
                        pileCap = pileCap.ShallowClone();
                    pileCap.Fragments.Add(topLevelConnectionAllowance);
                }
            }

            List<GeneralMaterialTakeoff> takeoffs = new List<GeneralMaterialTakeoff>
            {
                pileCap.GeneralMaterialTakeoff()
            };

            bool pileReinforcementDensityOverlapFound = false;
            bool pileConnectionAllowanceOverlapFound = false;

            foreach (Pile pileInCollection in pileFoundation.Piles)
            {
                if (pileInCollection == null)
                {
                    BH.Engine.Base.Compute.RecordWarning("Null pile found in PileFoundation. Skipping this pile for general material takeoff.");
                    continue;
                }

                Pile pile = pileInCollection;

                bool pileCLoned = false;

                if (topLevelReinforcementDensity != null)
                {
                    if (pile.FindFragment<ReinforcementDensity>() != null)
                        pileReinforcementDensityOverlapFound = true;
                    else if(!elementContainsConcrete || pile.Section.Material is Concrete)   //If the Pile is Concrete or non of the elements in the pilecap is concrete, apply the reinforcement density to the pile. 
                    { 
                        pile = pile.ShallowClone();
                        pile.Fragments.Add(topLevelReinforcementDensity);
                    }
                }

                if(topLevelConnectionAllowance != null)
                {
                    if (pile.FindFragment<ConnectionAllowance>() != null)
                        pileConnectionAllowanceOverlapFound = true;
                    else
                    {
                        if (!pileCLoned)
                            pile = pile.ShallowClone();
                        pile.Fragments.Add(topLevelConnectionAllowance);
                    }
                }
                
                takeoffs.Add(pile.GeneralMaterialTakeoff());
            }

            if(pileReinforcementDensityOverlapFound)
                BH.Engine.Base.Compute.RecordWarning("A ReinforcementDensity Fragment is found on both the PileFoundation and on one or more of the piles. The reinforcement density applied directly to the pile(s) is used.");

            if (pileConnectionAllowanceOverlapFound)
                BH.Engine.Base.Compute.RecordWarning("A ConnectionAllowance Fragment is found on both the PileFoundation and on one or more of the piles. The connection allowance applied directly to the pile(s) is used.");

            GeneralMaterialTakeoff aggregateTakeoff = Matter.Compute.AggregateGeneralMaterialTakeoff(takeoffs);
            return aggregateTakeoff;
        }

        /***************************************************/

        [Description("Returns a Stem's GeneralMaterialTakeoff using the stem solid volume and material. The takeoff contains information about the materiality and corresponding quantities of the Stem.")]
        [Input("stem", "The Stem to evaluate.")]
        [Output("takeoff", "The GeneralMaterialTakeoff of the stem.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Stem stem)
        {
            if (stem.IsNull() || stem.Material.IsNull())
                return null;

            double volume = stem.SolidVolume();

            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff()
            {
                MaterialTakeoffItems = new List<TakeoffItem>()
                {
                    new TakeoffItem()
                    {
                        Material = Physical.Create.Material(stem.Material),
                        Volume = volume,
                        Mass = volume * stem.Material.Density
                    }
                }
            };

            takeoff.ApplyReinforcementDensity(stem.FindFragment<ReinforcementDensity>());

            return takeoff;
        }

        /***************************************************/

        [Description("Returns a RetainingWall's GeneralMaterialTakeoff by aggregating its stem and footing takeoffs. The takeoff contains information about the materiality and corresponding quantities of all the elements in the RetainingWall.")]
        [Input("retainingWall", "The RetainingWall to evaluate.")]
        [Output("takeoff", "The aggregated GeneralMaterialTakeoff of the retaining wall.")]
        public static GeneralMaterialTakeoff GeneralMaterialTakeoff(this RetainingWall retainingWall)
        {
            if (retainingWall.IsNull() && retainingWall.Stem.IsNull() && retainingWall.Footing.IsNull())
                return null;

            ReinforcementDensity topLevelReinforcementDensity = retainingWall.FindFragment<ReinforcementDensity>();

            Stem stem = retainingWall.Stem;
            PadFoundation footing = retainingWall.Footing;

            bool stemCloned = false;
            bool footingCloned = false;

            if (topLevelReinforcementDensity != null)
            {
                if (stem.FindFragment<ReinforcementDensity>() != null)
                    Base.Compute.RecordWarning("A ReinforcementDensity Fragment is found on both the RetainingWall and on the stem. The reinforcement density applied directly to the stem is used.");
                else
                {
                    stem = stem.ShallowClone();
                    stemCloned = true;
                    stem.Fragments.Add(topLevelReinforcementDensity);
                }

                if (footing.FindFragment<ReinforcementDensity>() != null)
                    Base.Compute.RecordWarning("A ReinforcementDensity Fragment is found on both the RetainingWall and on the footing. The reinforcement density applied directly to the footing is used.");
                else
                {
                    footing = footing.ShallowClone();
                    footingCloned = true;
                    footing.Fragments.Add(topLevelReinforcementDensity);
                }
            }

            ConnectionAllowance topLevelConnectionAllowance = retainingWall.FindFragment<ConnectionAllowance>();

            if (topLevelConnectionAllowance != null)
            { 
                if(stem.FindFragment<ConnectionAllowance>() != null)
                    Base.Compute.RecordWarning("A ConnectionAllowance Fragment is found on both the RetainingWall and on the stem. The connection allowance applied directly to the stem is used.");
                else
                {
                    if(!stemCloned)
                        stem = stem.ShallowClone();
                    stem.Fragments.Add(topLevelConnectionAllowance);
                }

                if (footing.FindFragment<ConnectionAllowance>() != null)
                    Base.Compute.RecordWarning("A ConnectionAllowance Fragment is found on both the RetainingWall and on the footing. The connection allowance applied directly to the footing is used.");
                else
                {
                    if(!footingCloned)
                        footing = footing.ShallowClone();
                    footing.Fragments.Add(topLevelConnectionAllowance);
                }
            }

            List<GeneralMaterialTakeoff> takeoffs = new List<GeneralMaterialTakeoff>
            {
                stem.GeneralMaterialTakeoff(),
                footing.GeneralMaterialTakeoff()
            };

            return Matter.Compute.AggregateGeneralMaterialTakeoff(takeoffs);
        }

        #region Fragments

        /***************************************************/
        /**** Private Methods - Add Fragments           ****/
        /***************************************************/

        [Description("Applies a reinforcement density fragment to a GeneralMaterialTakeoff, reducing concrete volumes and adding reinforcement as a separate takeoff item.")]
        [Input("takeoff", "The GeneralMaterialTakeoff to modify.")]
        [Input("reinforcementDensity", "The ReinforcementDensity fragment to apply.")]
        [Output("takeoff", "The modified GeneralMaterialTakeoff with reinforcement applied.")]
        private static void ApplyReinforcementDensity(this GeneralMaterialTakeoff takeoff, ReinforcementDensity reinforcementDensity)
        {
            if (reinforcementDensity == null || reinforcementDensity.Density == 0)
                return;

            //Check if any of the materials in the composition are concrete. If so, apply the reinforcement density just to those elements.
            //If not, apply the reinforcement density to all materials in the composition.
            List<TakeoffItem> reinforcedParts = takeoff.ReinforcedItems();

            double concreteVolume = reinforcedParts.Sum(x => x.Volume);
            double reinforcementMass = concreteVolume * reinforcementDensity.Density;
            double reinforcementVolume = reinforcementMass / reinforcementDensity.Material.Density;

            if(reinforcementVolume > concreteVolume)
                BH.Engine.Base.Compute.RecordWarning("Applied reinforcement density results in a reinforcement volume greater than the concrete volume. This will lead to negative concrete volumes in the takeoff.");

            foreach (TakeoffItem item in reinforcedParts)
            {
                double ratio = item.Volume / concreteVolume;
                item.Volume -= reinforcementVolume * ratio;
                item.Mass = item.Volume * item.Material.Density;
            }

            takeoff.MaterialTakeoffItems.Add(new TakeoffItem()
            {
                Material = Engine.Physical.Create.Material(reinforcementDensity.Material),
                Volume = reinforcementVolume,
                Mass = reinforcementMass,
            });
        }

        /***************************************************/

        [Description("Adds a connection allowance to a GeneralMaterialTakeoff either by scaling existing items or adding a dedicated allowance item.")]
        [Input("takeoff", "The GeneralMaterialTakeoff to modify.")]
        [Input("connectionAllowance", "The ConnectionAllowance fragment to apply.")]
        [Output("takeoff", "The modified GeneralMaterialTakeoff with the connection allowance applied.")]
        private static void AddConnectionAllowance(this GeneralMaterialTakeoff takeoff, ConnectionAllowance connectionAllowance)
        {
            if (connectionAllowance == null || connectionAllowance.Allowance == 0)
                return;

            if (connectionAllowance.Material == null && string.IsNullOrWhiteSpace(connectionAllowance.Name))    //No allowance material specified and no name specified -> simply scale up the mass of the existing materials in the takeoff by the allowance factor.
            {
                foreach (TakeoffItem item in takeoff.MaterialTakeoffItems)
                {
                    item.Volume = item.Volume * (1 + connectionAllowance.Allowance);
                    item.Mass = item.Mass * (1 + connectionAllowance.Allowance);
                }
            }
            else
            {
                double allowanceMass = takeoff.MaterialTakeoffItems.Sum(x => x.Mass) * connectionAllowance.Allowance;
                Material allowanceMaterial;

                if (connectionAllowance.Material != null)
                    allowanceMaterial = Engine.Physical.Create.Material(connectionAllowance.Material);
                else
                    allowanceMaterial = takeoff.MaterialTakeoffItems.OrderByDescending(x => x.Volume).Select(x => x.Material).FirstOrDefault().DeepClone();    //If no material is specified, use the material with the largest volume in the takeoff (assumed to be the primary material).

                //Set name if provided
                if (!string.IsNullOrWhiteSpace(connectionAllowance.Name))
                    allowanceMaterial.Name = connectionAllowance.Name;

                takeoff.MaterialTakeoffItems.Add(new TakeoffItem()
                {
                    Material = allowanceMaterial,
                    Mass = allowanceMass,
                    Volume = allowanceMass / allowanceMaterial.Density
                });
            }
        }

        #endregion

        #region Sections

        /***************************************************/
        /**** Private Methods - Section properties      ****/
        /***************************************************/

        [Description("Dispatches to the appropriate section property GeneralMaterialTakeoff implementation based on runtime type.")]
        [Input("sectionProperty", "The section property to evaluate.")]
        [Input("length", "The length over which to compute the section volume.", typeof(Length))]
        [Input("reinforcementDensity", "Optional ReinforcementDensity fragment to apply.")]
        [Input("connectionAllowance", "Optional ConnectionAllowance fragment to apply.")]
        [Output("takeoff", "The GeneralMaterialTakeoff of the section property.")]
        private static GeneralMaterialTakeoff IGeneralMaterialTakeoff(this ISectionProperty sectionProperty, double length, ReinforcementDensity reinforcementDensity = null, ConnectionAllowance connectionAllowance = null)
        {
            return GeneralMaterialTakeoff(sectionProperty as dynamic, length, reinforcementDensity, connectionAllowance);
        }

        /***************************************************/

        [Description("Returns a homogeneous ISectionProperty's GeneralMaterialTakeoff based on section area and length.")]
        [Input("sectionProperty", "The ISectionProperty to evaluate.")]
        [Input("length", "The length of the element.", typeof(Length))]
        [Input("reinforcementDensity", "Optional ReinforcementDensity fragment to apply.")]
        [Input("connectionAllowance", "Optional ConnectionAllowance fragment to apply.")]
        [Output("takeoff", "The GeneralMaterialTakeoff of the section property.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this ISectionProperty sectionProperty, double length, ReinforcementDensity reinforcementDensity = null, ConnectionAllowance connectionAllowance = null)
        {
            Material material = Physical.Create.Material(sectionProperty.Material);
            double volume = sectionProperty.ISolidVolume(length);

            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff();

            takeoff.MaterialTakeoffItems.Insert(0, new TakeoffItem()
            {
                Material = Physical.Create.Material(sectionProperty.Material),
                Volume = volume,
                Mass = volume * sectionProperty.Material.Density,
                Length = length,
                NumberItem = 1
            });

            takeoff.ApplyReinforcementDensity(reinforcementDensity);
            takeoff.AddConnectionAllowance(connectionAllowance);

            return takeoff;
        }

        /***************************************************/

        [Description("Returns a ConcreteSection's GeneralMaterialTakeoff including explicit bar reinforcement from the RebarIntent.")]
        [Input("sectionProperty", "The ConcreteSection to evaluate.")]
        [Input("length", "The length of the section to evaluate.", typeof(Length))]
        [Input("reinforcementDensity", "Optional ReinforcementDensity fragment to apply to the concrete.")]
        [Input("connectionAllowance", "Optional ConnectionAllowance fragment to apply.")]
        [Output("takeoff", "The GeneralMaterialTakeoff of the concrete section including reinforcement items.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this ConcreteSection sectionProperty, double length, ReinforcementDensity reinforcementDensity = null, ConnectionAllowance connectionAllowance = null)
        {
            if (sectionProperty.IsNull())
                return null;

            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff();

            double concreteVolume = sectionProperty.ISolidVolume(length);

            List<double> areas = new List<double>();
            List<Material> materials = new List<Material>();

            if (sectionProperty?.RebarIntent?.BarReinforcement != null && sectionProperty.RebarIntent.BarReinforcement.Any())
            {
                if(reinforcementDensity != null)
                    BH.Engine.Base.Compute.RecordWarning($"Reinforcement density is being applied to a {nameof(ConcreteSection)} with explicit reinforcement. This may lead to double counting of reinforcement in the takeoff.");

                List<ICurve> outerProfileEdges = new List<ICurve>();
                List<ICurve> innerProfileEdges = new List<ICurve>();

                //If the section contains transverse reinforcement with an offset layout, extract the inner and outer edges of the section profile to determine the length of the transverse reinforcement.
                //Only do this if the section contains transverse reinforcement with an offset layout, as this operation is relatively costly and is not required for longitudinal reinforcement or transverse reinforcement with a standard layout.
                if (sectionProperty.RebarIntent.BarReinforcement.OfType<TransverseReinforcement>().Any(x => x.CenterlineLayout is OffsetCurveLayout))
                    ExtractInnerAndOuterEdges(sectionProperty, out outerProfileEdges, out innerProfileEdges);

                foreach (IBarReinforcement reinforcement in sectionProperty.RebarIntent.BarReinforcement)
                {
                    TakeoffItem takeoffItem = reinforcement.IRebarTakeoff(length, sectionProperty.RebarIntent.MinimumCover, outerProfileEdges, innerProfileEdges);
                    if(takeoffItem == null)
                        continue;

                    //Check if the material already exists in the takeoff. If so, add the volumes together. If not, add a new item to the takeoff.
                    if (takeoff.MaterialTakeoffItems.Any(x => x.Material.Name == takeoffItem.Material.Name && x.Material.Properties.Any(p => takeoffItem.Material.Properties.First().Name == p.Name)))
                    {
                        TakeoffItem existingItem = takeoff.MaterialTakeoffItems.First(x => x.Material.Name == takeoffItem.Material.Name);
                        existingItem.Volume += takeoffItem.Volume;
                        existingItem.Mass += takeoffItem.Mass;
                        existingItem.Length += takeoffItem.Length;
                        existingItem.NumberItem += takeoffItem.NumberItem;
                    }
                    else
                        takeoff.MaterialTakeoffItems.Add(takeoffItem);

                    //Subtract the volume of the reinforcement from the concrete volume
                    concreteVolume -= takeoffItem.Volume;

                    if(concreteVolume < 0)
                        BH.Engine.Base.Compute.RecordWarning($"The total volume of reinforcement in the {nameof(ConcreteSection)} exceeds the concrete volume. This will lead to negative concrete volumes in the takeoff.");
                }
            }

            takeoff.MaterialTakeoffItems.Insert(0, new TakeoffItem()
            {
                Material = Physical.Create.Material(sectionProperty.Material),
                Volume = concreteVolume,
                Mass = concreteVolume * sectionProperty.Material.Density,
                Length = length,
                NumberItem = 1
            });

            takeoff.ApplyReinforcementDensity(reinforcementDensity);
            takeoff.AddConnectionAllowance(connectionAllowance);

            return takeoff;

        }

        /***************************************************/

        [Description("Returns a CompositeSection's GeneralMaterialTakeoff by combining concrete and steel section takeoffs.")]
        [Input("sectionProperty", "The CompositeSection to evaluate.")]
        [Input("length", "The length of the section to evaluate.", typeof(Length))]
        [Input("reinforcementDensity", "Optional ReinforcementDensity fragment to apply to concrete portion.")]
        [Input("connectionAllowance", "Optional ConnectionAllowance fragment to apply.")]
        [Output("takeoff", "The combined GeneralMaterialTakeoff of the composite section.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this CompositeSection sectionProperty, double length, ReinforcementDensity reinforcementDensity = null, ConnectionAllowance connectionAllowance = null)
        {
            //TODO: Handle embedment etc..
            GeneralMaterialTakeoff takeoff = sectionProperty.ConcreteSection.IGeneralMaterialTakeoff(length, reinforcementDensity, connectionAllowance);
            GeneralMaterialTakeoff steelTakeoff = sectionProperty.SteelSection.IGeneralMaterialTakeoff(length, null, connectionAllowance);  //Not applying any reinforcement density to the steel section
            takeoff.MaterialTakeoffItems.AddRange(steelTakeoff.MaterialTakeoffItems);
            return takeoff;
        }

        #endregion

        #region Surface Properties

        /***************************************************/
        /**** Private Methods - Surface Properties      ****/
        /***************************************************/

        [Description("Returns a SurfaceProperty's GeneralMaterialTakeoff.")]
        [Input("property", "The SurfaceProperty to evaluate.")]
        [Input("area", "The surface area to evaluate.", typeof(Area))]
        [Input("reinforcementDensity", "Optional ReinforcementDensity assigned to the SurfaceProperty.")]
        [Input("panelRebarIntent", "Optional explicit panel rebar intent to include in the takeoff.")]
        [Output("takeoff", "The GeneralMaterialTakeoff of the SurfaceProperty.")]
        private static GeneralMaterialTakeoff IGeneralMaterialTakeoff(this ISurfaceProperty property, double area, ReinforcementDensity reinforcementDensity = null, PanelRebarIntent panelRebarIntent = null)
        {
            if (property.IsNull()) //Specific GeneralMaterialTakeoff(SurfaceProp) methods must check for material null- some properties ignore the base material.
                return null;

            GeneralMaterialTakeoff takeoff = GeneralMaterialTakeoff(property as dynamic, area);

            if(takeoff == null)
                return null;

            if (panelRebarIntent?.PanelReinforcement != null && panelRebarIntent.PanelReinforcement.Count != 0)
            { 
                if(reinforcementDensity != null)
                    BH.Engine.Base.Compute.RecordWarning("Element contains both explicit reinforcement and a reinforcement density. This may lead to double counting of reinforcement in the takeoff.");

                List<TakeoffItem> reinforcementItems = panelRebarIntent.PanelReinforcement.Select(x => x.RebarTakeoff(area)).Where(x => x != null).ToList();
                double reinforcementVolume = reinforcementItems.Sum(x => x.Volume);

                //Get the parts of the takeoff which are reinforced (i.e. concrete) and subtract the volume of the reinforcement from the concrete volume.
                //If no concrete type materials are present, apply the reinforcement to all parts of the takeoff.
                List<TakeoffItem> reinforcedParts = takeoff.ReinforcedItems();
                double concreteVolume = reinforcedParts.Sum(x => x.Volume);

                //Subtract the volume of the reinforcement from the concrete volume
                foreach (TakeoffItem item in reinforcedParts)
                {
                    double ratio = item.Volume / concreteVolume;
                    item.Volume -= reinforcementVolume * ratio;
                    item.Mass = item.Volume * item.Material.Density;
                }

                takeoff.MaterialTakeoffItems.AddRange(reinforcementItems);
            }

            takeoff.ApplyReinforcementDensity(reinforcementDensity);
            return takeoff;
        }

        /***************************************************/

        [Description("Gets the GeneralMaterialTakeoff for homogenous SurfaceProperties.")]
        [Input("property", "The SurfaceProperty to evaluate.")]
        [Input("area", "The surface area to evaluate.", typeof(Area))]
        [Output("takeoff", "The GeneralMaterialTakeoff of the SurfaceProperty.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this ISurfaceProperty property, double area)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            double volume = area * property.IVolumePerArea();

            TakeoffItem item = new TakeoffItem
            {
                Material = Physical.Create.Material(property.Material),
                Volume = volume,
                Mass = volume * property.Material.Density,
                Area = area,
                NumberItem = 1
            };

            return new GeneralMaterialTakeoff { MaterialTakeoffItems = new List<TakeoffItem> { item } };
        }


        /***************************************************/

        [Description("Returns a Layered surface property's GeneralMaterialTakeoff by summing solid layers.")]
        [Input("property", "The Layered surface property to evaluate.")]
        [Input("area", "The surface area to evaluate.", typeof(Area))]
        [Output("takeoff", "The GeneralMaterialTakeoff of the layered property.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Layered property, double area)
        {
            if (property.IsNull() || property.Layers.All(x => x.Material.IsNull()))
                return null;

            if (property.Layers.All(x => x.Material == null)) //cull any null layers, raise a warning.            
            {
                Base.Compute.RecordError("Cannote evaluate GeneralMaterialTakeoff because all of the materials are null.");
                return null;
            }

            if (property.Layers.Any(x => x.Material == null)) //cull any null layers, raise a warning.            
                Base.Compute.RecordWarning("At least one Material in a Layered surface property was null. VolumePerArea excludes this layer, assuming it is void space.");

            IEnumerable<Layer> solidLayers = property.Layers.Where(x => x.Material != null); //Filter to only layers which are solid.
            return new GeneralMaterialTakeoff
            {
                MaterialTakeoffItems = solidLayers.Select(x => new TakeoffItem
                {
                    Material = Physical.Create.Material(x.Material),
                    Volume = area * x.Thickness,
                    Mass = area * x.Thickness * x.Material.Density,
                    Area = area,
                    NumberItem = 1
                }).ToList()
            };
        }

        /***************************************************/

        [Description("Returns a ToppedSlab property's GeneralMaterialTakeoff by combining base and topping layers.")]
        [Input("property", "The ToppedSlab property to evaluate.")]
        [Input("area", "The surface area to evaluate.", typeof(Area))]
        [Output("takeoff", "The GeneralMaterialTakeoff of the topped slab.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this ToppedSlab property, double area)
        {
            if (property.IsNull() || property.BaseProperty.IsNull() || property.Material.IsNull())
                return null;

            double baseVolume = property.BaseProperty.IVolumePerArea();
            double toppingThickness = property.ToppingThickness;

            //Generate takeoff for the base property
            GeneralMaterialTakeoff takeoff = property.BaseProperty.IGeneralMaterialTakeoff(area);

            //Add takeoff for topping layer
            takeoff.MaterialTakeoffItems.Add(new TakeoffItem {
                Material = Physical.Create.Material(property.Material),
                Volume = toppingThickness * area,
                Mass = toppingThickness * area * property.Material.Density,
                Area = area,
                NumberItem = 1
            });

            return takeoff;
        }

        /***************************************************/


        [Description("Returns a SlabOnDeck property's GeneralMaterialTakeoff by separating deck and slab volumes.")]
        [Input("property", "The SlabOnDeck property to evaluate.")]
        [Input("area", "The surface area to evaluate.", typeof(Area))]
        [Output("takeoff", "The GeneralMaterialTakeoff of the slab on deck.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this SlabOnDeck property, double area)
        {
            if (property.IsNull() || property.Material.IsNull() || property.DeckMaterial.IsNull())
            {
                return null;
            }

            double deckVolume = property.DeckThickness * property.DeckVolumeFactor * area;
            double slabVolume = property.VolumePerArea() * area - deckVolume;

            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff();

            takeoff.MaterialTakeoffItems.Add(new TakeoffItem
            {
                Material = Physical.Create.Material(property.Material),
                Volume = slabVolume,
                Mass = slabVolume * property.Material.Density,
                Area = area,
                NumberItem = 1
            });

            takeoff.MaterialTakeoffItems.Add(new TakeoffItem
            {
                Material = Physical.Create.Material(property.DeckMaterial),
                Volume = deckVolume,
                Mass = deckVolume * property.DeckMaterial.Density,
                Area = area,
                NumberItem = 1
            });

            return takeoff;
        }

        /***************************************************/

        [Description("Returns a Cassette surface property's GeneralMaterialTakeoff splitting top, bottom and rib zones.")]
        [Input("property", "The Cassette property to evaluate.")]
        [Input("area", "The surface area to evaluate.", typeof(Area))]
        [Output("takeoff", "The GeneralMaterialTakeoff of the cassette property.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this Cassette property, double area)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            double volPerAreaRibZone = property.RibHeight * (property.RibThickness / property.RibSpacing);

            double topVolume = property.TopThickness * area;
            double bottomVolume = property.BottomThickness * area;
            double ribVolume = volPerAreaRibZone * area;

            List<IMaterialFragment> materials = new List<IMaterialFragment>
            {
                property.Material
            };
            
            List<double> volumes = new List<double>
            {
                topVolume
            };

            if (property.BottomMaterial.IsTakeoffMaterialDifferent(property.Material))
            { 
                materials.Add(property.BottomMaterial);
                volumes.Add(bottomVolume);
            }
            else
                volumes[0] += bottomVolume; //Add bottom volume to top volume if no bottom material is specified as the bottom material is assumed to be the same as the top material.

            if(property.RibMaterial.IsTakeoffMaterialDifferent(property.Material))
            {
                materials.Add(property.RibMaterial);
                volumes.Add(ribVolume);
            }
            else
                volumes[0] += ribVolume; //Add rib volume to top volume if no rib material is specified as the rib material is assumed to be the same as the top material.


            return new GeneralMaterialTakeoff()
            {
                MaterialTakeoffItems = materials.Select((m, i) => new TakeoffItem
                {
                    Material = Physical.Create.Material(m),
                    Volume = volumes[i],
                    Mass = volumes[i] * m.Density,
                    Area = area,
                    NumberItem = 1
                }).ToList()
            };

        }

        /***************************************************/

        [Description("Returns a BuiltUpRibbed surface property's GeneralMaterialTakeoff splitting top and rib zones.")]
        [Input("property", "The BuiltUpRibbed property to evaluate.")]
        [Input("area", "The surface area to evaluate.", typeof(Area))]
        [Output("takeoff", "The GeneralMaterialTakeoff of the built-up ribbed property.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this BuiltUpRibbed property, double area)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            double volPerAreaRibZone = property.RibHeight * (property.RibThickness / property.RibSpacing);

            double topVolume = property.TopThickness * area;
            double ribVolume = volPerAreaRibZone * area;

            List<IMaterialFragment> materials = new List<IMaterialFragment>
            {
                property.Material
            };

            List<double> volumes = new List<double>
            {
                topVolume
            };

            if (property.RibMaterial.IsTakeoffMaterialDifferent(property.Material))
            {
                materials.Add(property.RibMaterial);
                volumes.Add(ribVolume);
            }
            else
                volumes[0] += ribVolume; //Add rib volume to top volume if no rib material is specified as the rib material is assumed to be the same as the top material.


            return new GeneralMaterialTakeoff()
            {
                MaterialTakeoffItems = materials.Select((m, i) => new TakeoffItem
                {
                    Material = Physical.Create.Material(m),
                    Volume = volumes[i],
                    Mass = volumes[i] * m.Density,
                    Area = area,
                    NumberItem = 1
                }).ToList()
            };

        }


        /***************************************************/

        [Description("Returns a BuiltUpDoubleRibbed surface property's GeneralMaterialTakeoff splitting top and rib zones.")]
        [Input("property", "The BuiltUpDoubleRibbed property to evaluate.")]
        [Input("area", "The surface area to evaluate.", typeof(Area))]
        [Output("takeoff", "The GeneralMaterialTakeoff of the built-up double ribbed property.")]
        private static GeneralMaterialTakeoff GeneralMaterialTakeoff(this BuiltUpDoubleRibbed property, double area)
        {
            if (property.IsNull() || property.Material.IsNull())
                return null;

            double volPerAreaRibZone = 2.0d * property.RibHeight * (property.RibThickness / property.RibSpacing);

            double topVolume = property.TopThickness * area;
            double ribVolume = volPerAreaRibZone * area;

            List<IMaterialFragment> materials = new List<IMaterialFragment>
            {
                property.Material
            };

            List<double> volumes = new List<double>
            {
                topVolume
            };

            if (property.RibMaterial.IsTakeoffMaterialDifferent(property.Material))
            {
                materials.Add(property.RibMaterial);
                volumes.Add(ribVolume);
            }
            else
                volumes[0] += ribVolume; //Add rib volume to top volume if no rib material is specified as the rib material is assumed to be the same as the top material.


            return new GeneralMaterialTakeoff()
            {
                MaterialTakeoffItems = materials.Select((m, i) => new TakeoffItem
                {
                    Material = Physical.Create.Material(m),
                    Volume = volumes[i],
                    Mass = volumes[i] * m.Density,
                    Area = area,
                }).ToList()
            };
        }

        /***************************************************/

        private static bool IsTakeoffMaterialDifferent(this IMaterialFragment material, IMaterialFragment reference)
        {
            if (material == null || reference == null)
                return false;
            return material.Name != reference.Name || material.BHoM_Guid != reference.BHoM_Guid || material.Hash() != reference.Hash(); //Hash will check the name and Guid as well, but quicker the check those explicitly first to avoid the hash calculation if possible.
        }


        #endregion


        #region Reinforcement

        /***************************************************/
        /**** Private Methods - Reinforcement           ****/
        /***************************************************/


        [Description("Dispatches to the appropriate rebar takeoff implementation based on reinforcement runtime type.")]
        [Input("reinforcement", "The IBarReinforcement to evaluate.")]
        [Input("length", "The length of the parent element.", typeof(Length))]
        [Input("cover", "Minimum cover to offset transverse reinforcement.")]
        [Input("outerProfileEdges", "Optional outer profile edges used for transverse reinforcement layout.")]
        [Input("innerProfileEdges", "Optional inner profile edges used for transverse reinforcement layout.")]
        [Output("takeoffItem", "The TakeoffItem for the reinforcement, or null if none.")]
        private static TakeoffItem IRebarTakeoff(this IBarReinforcement reinforcement, double length, double cover, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges)
        {
            return RebarTakeoff(reinforcement as dynamic, length, cover, outerProfileEdges, innerProfileEdges);
        }

        /***************************************************/

        [Description("Computes the takeoff for longitudinal reinforcement (bars) within a section.")]
        [Input("reinforcement", "The LongitudinalReinforcement to evaluate.")]
        [Input("length", "The length of the parent element.", typeof(Length))]
        [Input("cover", "Minimum cover (unused for longitudinal bars).")]
        [Input("outerProfileEdges", "Not used for longitudinal reinforcement.")]
        [Input("innerProfileEdges", "Not used for longitudinal reinforcement.")]
        [Output("takeoffItem", "The TakeoffItem representing longitudinal reinforcement.")]
        private static TakeoffItem RebarTakeoff(this LongitudinalReinforcement reinforcement, double length, double cover, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges)
        {
            int barCount = reinforcement.ReinforcingBarCount();
            double singleBarArea = reinforcement.Diameter * reinforcement.Diameter / 4 * Math.PI;   //Single bar area based on diameter

            //Scale lengths to account for reinforcement that does not span the entire length of the section
            double factor = Math.Min(reinforcement.EndLocation - reinforcement.StartLocation, 1);
            double barLength = length * factor;

            double totalBarLength = barLength * barCount;
            double reinforcementVolume = singleBarArea * totalBarLength;

            return new TakeoffItem()
            {
                Material = Physical.Create.Material(reinforcement.Material),
                Volume = reinforcementVolume,
                Mass = reinforcementVolume * reinforcement.Material.Density,
                Length = totalBarLength,
                NumberItem = barCount
            };
        }

        /***************************************************/

        [Description("Computes the takeoff for transverse reinforcement (stirrups/hoops) using layout outlines and spacing.")]
        [Input("reinforcement", "The TransverseReinforcement to evaluate.")]
        [Input("length", "The length of the parent element.", typeof(Length))]
        [Input("cover", "Minimum cover to offset stirrup centerline.")]
        [Input("outerProfileEdges", "Optional outer profile edges used for determining stirrup outlines.")]
        [Input("innerProfileEdges", "Optional inner profile edges used for determining stirrup outlines.")]
        [Output("takeoffItem", "The TakeoffItem representing transverse reinforcement.")]
        private static TakeoffItem RebarTakeoff(this TransverseReinforcement reinforcement, double length, double cover, List<ICurve> outerProfileEdges, List<ICurve> innerProfileEdges)
        {
            List<ICurve> rebarLines = new List<ICurve>();
            if (reinforcement.CenterlineLayout is OffsetCurveLayout offsetLayout)
            {
                cover += reinforcement.Diameter / 2;  //Add half the diameter of the stirrup to the cover to get the offset for the centerline of the stirrup
            }

            List<ICurve> stirrupOutline = reinforcement.ReinforcementLayout(cover, outerProfileEdges, innerProfileEdges);

            double stirrupRange = (reinforcement.EndLocation - reinforcement.StartLocation) * length - (2 * cover + reinforcement.Diameter);
            int count;
            double spacing = reinforcement.Spacing;

            if (reinforcement.AdjustSpacingToFit)
            {
                count = (int)Math.Ceiling(stirrupRange / reinforcement.Spacing);
                spacing = stirrupRange / count;
            }
            else
                count = (int)Math.Floor(stirrupRange / reinforcement.Spacing);

            count += 1; //Add one for the first stirrup at the start location

            double stirupLength = stirrupOutline.Sum(x => x.ILength());
            double stirupArea = reinforcement.Diameter * reinforcement.Diameter / 4 * Math.PI;
            double totalStirrupLength = stirupLength * count;
            double reinforcementVolume = stirupArea * totalStirrupLength;

            return new TakeoffItem()
            {
                Material = Physical.Create.Material(reinforcement.Material),
                Volume = reinforcementVolume,
                Mass = reinforcementVolume * reinforcement.Material.Density,
                Length = totalStirrupLength,
                NumberItem = count
            };

        }

        /***************************************************/

        [Description("Approximates panel reinforcement takeoff based on region area and reinforcement spacings.")]
        [Input("reinforcement", "The PanelReinforcement to evaluate.")]
        [Input("regionArea", "The region area to approximate reinforcement for (overridden by explicit perimeter if present).", typeof(Area))]
        [Output("takeoffItem", "The TakeoffItem representing panel reinforcement.")]
        private static TakeoffItem RebarTakeoff(this PanelReinforcement reinforcement, double regionArea)
        {
            
            if(reinforcement.Region?.Perimeter != null)
                regionArea = Engine.Geometry.Query.IArea(reinforcement.Region.Perimeter);

            //Approximates the total length of longitudinal reinforcement based on the area of the region and the spacing of the reinforcement. This is a rough estimate and may not be accurate for all shapes.
            double approxLongTotalLength = regionArea / reinforcement.LongitudinalSpacing;
            double longitudinalArea = reinforcement.LongitudinalDiameter * reinforcement.LongitudinalDiameter / 4 * Math.PI;
            double longitudinalVolume = approxLongTotalLength * longitudinalArea;


            //Approximates the total length of transverse reinforcement based on the area of the region and the spacing of the reinforcement. This is a rough estimate and may not be accurate for all shapes.
            double approxTransTotalLength = regionArea / reinforcement.TransverseSpacing;
            double transverseArea = reinforcement.TransverseDiameter * reinforcement.TransverseDiameter / 4 * Math.PI;
            double transverseVolume = approxLongTotalLength * transverseArea;

            double totalVolume = longitudinalVolume + transverseVolume;

            return new TakeoffItem()
            {
                Material = Physical.Create.Material(reinforcement.Material),
                Volume = totalVolume,
                Mass = totalVolume * reinforcement.Material.Density,
                Length = approxLongTotalLength + approxTransTotalLength,
            };

        }

        /***************************************************/

        [Description("Returns the parts of a GeneralMaterialTakeoff which should be considered for reinforcement (concrete by default).")]
        [Input("takeoff", "The GeneralMaterialTakeoff to inspect.")]
        [Output("reinforcedParts", "List of TakeoffItem considered reinforced.")]
        private static List<TakeoffItem> ReinforcedItems(this GeneralMaterialTakeoff takeoff)
        {
            List<TakeoffItem> takeoffItems = takeoff.MaterialTakeoffItems.Where(x => x.Material.Properties.Any(p => p is Concrete)).ToList();   //Try find parts made of concrete

            if (takeoffItems.Count == 0) //If no concrete parts found, return all items as reinforced items
            {
                BH.Engine.Base.Compute.RecordNote("Element does not contain any concrete materials. Reinforcement is assumed to be applied to all materials.");
                takeoffItems = takeoff.MaterialTakeoffItems;
            }
            return takeoffItems;
        }

        /***************************************************/

        #endregion
    }
}

