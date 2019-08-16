/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Architecture.Theatron;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Base;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Create a full stadium TheatronGeometry based on predefined plan types, Cvalue is used to define the TheatronFullProfile")]
        [Input("planFull", "The theatron plan")]
        [Input("profile", "The TheatronFullProfile used in defining the plan")]
        [Input("sParams", "The StadiaParameters")]
        [Input("pParams", "List of the ProfileParameters")]
        public static TheatronGeometry TheatronGeometry(TheatronPlan planFull, TheatronFullProfile profile,StadiaParameters sParams, List<ProfileParameters> pParams)
        {
            var theatron = CreateGeometry(planFull, profile, pParams,sParams.TypeOfBowl);
            CopyGeneratorBlocks(ref theatron, planFull, sParams.TypeOfBowl);

            return theatron;
        }

        /***************************************************/
        [Description("Create a partial TheatronGeometry based on structural locations and a TheatronFullProfile, Cvalue is not used to define the TheatronFullProfile")]
        [Input("structuralOrigins", "List of ProfileOrigins to orientate of the structural sections")]
        [Input("profile", "The TheatronFullProfile")]
        [Input("pParams", "List of the ProfileParameters")]
        public static TheatronGeometry TheatronGeometry(List<ProfileOrigin> structuralOrigins, TheatronFullProfile profile, List<ProfileParameters> pParams)
        {
            var plan = PlanGeometry(structuralOrigins, null);
            var theatron = CreateGeometry(plan, profile, pParams,StadiaType.Undefined);
            
            theatron.Tiers3d.ForEach(t => t.Generatorblocks.ForEach(g => { t.TierBlocks.Add(g); theatron.Audience.Add(g.Audience); }));
            
            return theatron;
        }

        /***************************************************/
        [Description("Create a partial TheatronGeometry based on a partial plan and a profile, Cvalue is used to define the TheatronFullProfile")]
        [Input("planPart", "The partial TheatronPlan")]
        [Input("profile", "The TheatronFullProfile")]
        [Input("pParams", "List of the ProfileParameters")]
        public static TheatronGeometry TheatronGeometry(TheatronPlan planPart, TheatronFullProfile profile, List<ProfileParameters> pParams)
        {
            var theatron = CreateGeometry(planPart, profile, pParams, StadiaType.Undefined);
            theatron.Tiers3d.ForEach(t => t.Generatorblocks.ForEach(g => { t.TierBlocks.Add(g); theatron.Audience.Add(g.Audience); }));
            return theatron;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static TheatronGeometry CreateGeometry(TheatronPlan plan, TheatronFullProfile profile, List<ProfileParameters> pParams, StadiaType stadiaType)
        {
            var theatron = new TheatronGeometry();
            theatron.TotalTiers = profile.MappedProfiles.Count;
            SetGeneratorblocks(ref theatron, profile, plan, stadiaType, pParams);
            SetFloorMeshes(ref theatron, pParams);
            SetGeneratorEyes(ref theatron);
           
            return theatron;
        }
        /***************************************************/
        private static void SetGeneratorblocks(ref TheatronGeometry theatronGeom, TheatronFullProfile fullprofile, TheatronPlan theatronPlan,StadiaType stadiatype, List<ProfileParameters> profileParameters)
        {
            //this defines the geometry of the seating blocks from which all others are created
            
            TierProfile tierToMap;
            for (int i = 0; i < theatronGeom.TotalTiers; i++)
            {
                tierToMap = fullprofile.BaseTierProfiles[i];
                theatronGeom.Tiers3d.Add(new Tier());
                var SectionBlock = GetBlockStartIndexes(theatronPlan, stadiatype);
                foreach (var pair in SectionBlock)
                {
                    int index = pair.Key;
                    SeatingBlockType blockType = pair.Value;

                    var block = SeatingBlock(theatronPlan.SectionOrigins[index], theatronPlan.VomitoryOrigins[index],
                        theatronPlan.SectionOrigins[index + 1], blockType, profileParameters[i].SeatWidth, profileParameters[i].VomitoryParameters.VomitoryWidth);

                    theatronGeom.Tiers3d[i].Generatorblocks.Add(block);
                    if (block.TypeOfSeatingBlock == SeatingBlockType.Transition1 || block.TypeOfSeatingBlock == SeatingBlockType.Transition2)
                    {
                        //cannot use vomitory location to define profile scaling 
                        SetTransitionProfiles(ref block, tierToMap, fullprofile.FullProfileOrigin, theatronPlan.VomitoryOrigins[index-1], theatronPlan.VomitoryOrigins[index + 1]);
                    }
                    else
                    {
                        SetBlockProfiles(ref block, tierToMap, fullprofile.FullProfileOrigin);
                    }
                    
                }
                if (stadiatype == StadiaType.EightArc || stadiatype == StadiaType.Orthogonal)
                {
                    SetOtherBlockTypes(ref theatronGeom,i, fullprofile);//only needed for radial and othogonal
                }
            }

        }

        /***************************************************/

        private static void SetGeneratorEyes(ref TheatronGeometry theatronGeom)
        {
            for (int i = 0; i < theatronGeom.TotalTiers; i++)
            {
                for (int j = 0; j < theatronGeom.Tiers3d[i].Generatorblocks.Count; j++)
                {
                    var block = theatronGeom.Tiers3d[i].Generatorblocks[j];
                    SetEyesBasic(ref block);
                }
            }
        }

        /***************************************************/

        private static void SetOtherBlockTypes(ref TheatronGeometry theatronGeom, int tierNum, TheatronFullProfile fullprofile)
        {
            ////no Vomitory corner block
            var corner = theatronGeom.Tiers3d[tierNum].Generatorblocks.Find(x => x.TypeOfSeatingBlock == SeatingBlockType.Corner);
            var cornerNoVom = corner.DeepClone();
            cornerNoVom.TypeOfSeatingBlock = SeatingBlockType.CornerNoVom;
            theatronGeom.Tiers3d[tierNum].Generatorblocks.Add(cornerNoVom);

            //// transition 1 block
            var toMirror = theatronGeom.Tiers3d[tierNum].Generatorblocks.Find(x => x.TypeOfSeatingBlock == SeatingBlockType.Transition1);
            var transMirror1 = Create.MirrorSeatingBlock(toMirror, SeatingBlockType.Transition1mirrored);
            theatronGeom.Tiers3d[tierNum].Generatorblocks.Add(transMirror1);

            //// transition 2 block
            toMirror = theatronGeom.Tiers3d[tierNum].Generatorblocks.Find(x => x.TypeOfSeatingBlock == SeatingBlockType.Transition2);
            var transMirror2 = Create.MirrorSeatingBlock(toMirror, SeatingBlockType.Transition2mirrored);
            theatronGeom.Tiers3d[tierNum].Generatorblocks.Add(transMirror2);
        }
       
        /***************************************************/

        private static Dictionary<int, SeatingBlockType> GetBlockStartIndexes(TheatronPlan bp, StadiaType bowltype)
        {
            //gets a dictionary where key is index of SectionOrigin and value is the SeatingBlockType
            Dictionary<int, SeatingBlockType> dict = new Dictionary<int, SeatingBlockType>();
            BayType current, next;
            if (bowltype == StadiaType.EightArc || bowltype == StadiaType.Orthogonal)
            {
                //radial and ortho
                for (int j = 0; j < bp.SectionOrigins.Count; j++)
                {
                    current = bp.StructBayType[j];
                    if (j == bp.SectionOrigins.Count - 1)
                    {
                        next = bp.StructBayType[0];
                    }
                    else
                    {
                        next = bp.StructBayType[j + 1];
                    }
                    if (!dict.ContainsValue(SeatingBlockType.Side)) if (current == 0 && next == 0) dict.Add(j, SeatingBlockType.Side);//side standard
                    if (!dict.ContainsValue(SeatingBlockType.End)) if (current == BayType.End && next == BayType.End) dict.Add(j, SeatingBlockType.End);//end standard
                    if (!dict.ContainsValue(SeatingBlockType.Corner)) if (current == BayType.Corner && next == BayType.Corner) dict.Add(j, SeatingBlockType.Corner);//corner standard
                    if (!dict.ContainsValue(SeatingBlockType.Transition1)) if (current == 0 && next == BayType.Corner) dict.Add(j, SeatingBlockType.Transition1);//side to corner transition
                    if (!dict.ContainsValue(SeatingBlockType.Transition2)) if (current == BayType.Corner && next == BayType.End) dict.Add(j, SeatingBlockType.Transition2);//corner to end transition

                }
            }
            if (bowltype == StadiaType.NoCorners)//no corner
            {
                for (int j = 0; j < bp.SectionOrigins.Count; j++)
                {
                    current = bp.StructBayType[j];
                    if (j == bp.SectionOrigins.Count - 1)
                    {
                        next = bp.StructBayType[0];
                    }
                    else
                    {
                        next = bp.StructBayType[j + 1];
                    }
                    if (!dict.ContainsValue(SeatingBlockType.Side)) if (current == 0 && next == 0) dict.Add(j, SeatingBlockType.Side);//side standard
                    if (!dict.ContainsValue(SeatingBlockType.End)) if (current == BayType.End && next == BayType.End) dict.Add(j, SeatingBlockType.End);//end standard

                }
            }
            if (bowltype == StadiaType.Circular)//circular
            {
                dict.Add(0, SeatingBlockType.Side);//side standard
            }
            if(bowltype == StadiaType.Undefined)
            {
                for (int j = 0; j < bp.SectionOrigins.Count-1; j++)
                {
                    dict.Add(j, SeatingBlockType.Undefined);
                }
            }
            var sortedDict = dict.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value); 
            return sortedDict;
        }

        /***************************************************/

        private static void SetFloorMeshes(ref TheatronGeometry theatron, List<ProfileParameters> parameters)
        {
            for (int i = 0; i < theatron.TotalTiers; i++)
            {
                 
                for (int j = 0; j < theatron.Tiers3d[i].Generatorblocks.Count; j++)
                {
                    SeatingBlockType bt = theatron.Tiers3d[i].Generatorblocks[j].TypeOfSeatingBlock;
                    var block = theatron.Tiers3d[i].Generatorblocks[j];
                    if(bt == SeatingBlockType.Side || bt == SeatingBlockType.Corner|| bt == SeatingBlockType.End || bt == SeatingBlockType.Undefined)
                    {
                        SetBlockFloor(ref block, parameters[i]);
                    }
                    else
                    {
                        SetBlockFloorBasic(ref block);
                    }
                        
                }
            }
            
        }

        /***************************************************/

        
        private static void CopyGeneratorBlocks(ref TheatronGeometry theatron, TheatronPlan bp, StadiaType stadiatype)
        {
            for (int i = 0; i < theatron.TotalTiers; i++)
            {
                BayType current, next;
                int cornerCount = -1;
                for (int j = 0; j < bp.SectionOrigins.Count; j++)
                {
                    current = bp.StructBayType[j];
                    if (j == bp.SectionOrigins.Count - 1)
                    {
                        next = bp.StructBayType[0];
                    }
                    else
                    {
                        next = bp.StructBayType[j + 1];
                    }
                    if (stadiatype == StadiaType.NoCorners && current != next) continue;//no corner bowl corner position
                    SeatingBlockType bt = DetermineBlockToCopy(current, next, stadiatype, cornerCount);

                    if (current == BayType.Corner) cornerCount++;
                    else cornerCount = -1;

                    SeatingBlock blockToCopy = theatron.Tiers3d[i].Generatorblocks.Find(b => b.TypeOfSeatingBlock == bt).DeepClone();
                    double angle = Geometry.Query.Angle(blockToCopy.Start.Direction, bp.SectionOrigins[j].Direction,Plane.XY);
                    SeatingBlock copy = TransformSeatingBlock(blockToCopy, blockToCopy.Start.Origin, bp.SectionOrigins[j].Origin, angle);
                    theatron.Tiers3d[i].TierBlocks.Add(copy);
                    theatron.Audience.Add(copy.Audience);
                }
            }
        }

        /***************************************************/
    
        private static SeatingBlockType DetermineBlockToCopy(BayType current, BayType next, StadiaType bowlType, int cornerCount)
        {
            SeatingBlockType bt = SeatingBlockType.Side;
            if (bowlType == StadiaType.EightArc || bowlType == StadiaType.Orthogonal)
            {
                if (current == 0 && next == 0) bt = SeatingBlockType.Side;//side standard
                if (current == BayType.End && next == BayType.End) bt = SeatingBlockType.End;//end standard
                if (current == BayType.Corner && next == BayType.Corner) bt = SeatingBlockType.CornerNoVom;//corner standard
                                                                                    //corner vom or no vom should be determined by number of seats on last row 14 either side of vomitory max
                if (current == BayType.Corner && cornerCount % 2 == 0) bt = SeatingBlockType.Corner;//corner vomitory standard
                if (current == 0 && next == BayType.Corner) bt = SeatingBlockType.Transition1;//side to corner transition
                if (current == BayType.Corner && next == BayType.End) bt = SeatingBlockType.Transition2;//corner to end transition
                if (current == BayType.End && next == BayType.Corner) bt = SeatingBlockType.Transition2mirrored;//end to corner transition
                if (current == BayType.Corner && next == 0) bt = SeatingBlockType.Transition1mirrored;//corner to sidetransition
            }
            if (bowlType == StadiaType.NoCorners)
            {
                if (current == BayType.End) bt = SeatingBlockType.End;//end standard otherwise a side is returned
            }
            //if bowlType is circular side is returned
            return bt;
        }
    }
}
