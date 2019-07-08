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

namespace BH.Engine.Architecture.Theatron
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static TheatronGeometry TheatronGeometry(TheatronPlan plan, TheatronFullProfile profile,StadiaParameters sParams, List<ProfileParameters> pParams)
        {
            var theatron = new TheatronGeometry();

            theatron.TotalTiers = profile.MappedProfiles.Count;
            setGeneratorblocks(ref theatron, profile, plan, sParams.TypeOfBowl,pParams);
            SetFloorMeshes(ref theatron, pParams);
            return theatron;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void setGeneratorblocks(ref TheatronGeometry theatronGeom, TheatronFullProfile fullprofile, TheatronPlan theatronPlan,StadiaType stadiatype, List<ProfileParameters> profileParameters)
        {
            //this defines the geometry of the seating blocks from which all others are created
            
            TierProfile tierToMap;
            for (int i = 0; i < theatronGeom.TotalTiers; i++)
            {
                tierToMap = fullprofile.BaseTierProfiles[i];
                theatronGeom.Tiers3d.Add(new Tier());
                var SectionBlock = getBlockStartIndexes(theatronPlan, stadiatype);
                foreach (var pair in SectionBlock)
                {
                    int index = pair.Key;
                    SeatingBlockType blockType = pair.Value;

                    var block = Create.SeatingBlock(theatronPlan.SectionOrigins[index], theatronPlan.VomitoryOrigins[index],
                        theatronPlan.SectionOrigins[index + 1], blockType, profileParameters[i].SeatWidth, profileParameters[i].AisleWidth);

                    theatronGeom.Tiers3d[i].Generatorblocks.Add(block);

                    if(blockType == SeatingBlockType.Side || blockType == SeatingBlockType.End || blockType == SeatingBlockType.Corner)
                    {
                        Create.SetBlockProfiles(ref block, tierToMap, fullprofile.FullProfileOrigin);
                    }
                    else
                    {
                        TierProfile start = new TierProfile();
                        TierProfile end = new TierProfile();
                        if (blockType == SeatingBlockType.Transition1)
                        {
                            start = theatronGeom.Tiers3d[i].Generatorblocks[0].Sections[2];//next to a side
                            end = theatronGeom.Tiers3d[i].Generatorblocks[2].Sections[0];//followed by a corner
                        }
                        if (blockType == SeatingBlockType.Transition2)
                        {
                            start = theatronGeom.Tiers3d[i].Generatorblocks[2].Sections[2];//next to a corner
                            end = theatronGeom.Tiers3d[i].Generatorblocks[1].Sections[0];// followed by end
                        }
                        Create.SetTransitionBlock(ref block, start, end, fullprofile.FullProfileOrigin);
                    }
                   
                }
                if (stadiatype == StadiaType.EightArc || stadiatype == StadiaType.Orthogonal)
                {
                    //setOtherBlockTypes(ref theatronGeom,i, fullprofile);//only needed for radial and othogonal
                }
            }

        }
        
        /***************************************************/

        private static void setOtherBlockTypes(ref TheatronGeometry theatronGeom, int tierNum, TheatronFullProfile fullprofile)
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

        private static Dictionary<int, SeatingBlockType> getBlockStartIndexes(TheatronPlan bp, StadiaType bowltype)
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
                    if(bt == SeatingBlockType.Side)//|| bt == SeatingBlockType.Corner|| bt == SeatingBlockType.End)
                    {
                        Create.setBlockFloor(ref block, parameters[i]);
                    }
                    else
                    {
                        //Create.setBlockFloorBasic(ref block);
                    }
                        
                }
            }
            
        }
    }
}
