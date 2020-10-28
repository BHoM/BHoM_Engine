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

using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Base;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Constructors             ****/
        /***************************************************/
        public static Graph ProcessView(this Graph graph, double width, double height)
        {
            Graph clone = graph.DeepClone();
            clone.Entities.Values.ToList().ForEach(ent => ent.RemoveFragment(typeof(ProcessViewFragment)));
            IBHoMObject root = clone.AddRootEntity();

            Dictionary<Guid, int> depths = clone.DepthDictionary(root.BHoM_Guid);
            
            List<int> distinctDepths = depths.Values.Distinct().ToList();
            distinctDepths.Sort();
            //distinctDepths.Reverse();
            double cellY = (height *1.0) / distinctDepths.Count();
            double y = 0;
            
            foreach (int d in distinctDepths)
            {
                double wobbleY = height / 40;
                //all the entities at this level
                IEnumerable<Guid> level = depths.Where(kvp => kvp.Value == d).Select(kvp => kvp.Key);
                double x = 0;
                double cellX = (width * 1.0) / level.Count() ;
                foreach (Guid entity in level)
                {
                   
                    ProcessViewFragment view = new ProcessViewFragment();
                    
                    if (x % 2 == 0)
                        view.Position = Geometry.Create.Point(x * cellX + cellX / 2, y * cellY - wobbleY, 0);
                    else
                        view.Position = Geometry.Create.Point(x * cellX + cellX / 2, y * cellY + wobbleY, 0);

                    x++;
                    clone.Entities[entity] = clone.Entities[entity].AddFragment(view, true);
                }
                y--;
            }
            clone.RemoveRootEntity(root);
            return clone;
        }
        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
        private static IBHoMObject AddRootEntity(this Graph graph)
        {
            m_OriginalSources = graph.Sources();
            IBHoMObject root = graph.Entities.Values.ToList()[0].DeepClone();
            root.BHoM_Guid = Guid.NewGuid();
            graph.Entities.Add(root.BHoM_Guid, root);
            foreach (Guid guid in m_OriginalSources)
            {
                Relation relation = new Relation()
                {
                    Source = root.BHoM_Guid,
                    Target = guid
                };
                graph.Relations.Add(relation);
            }
            return root;
        }
        /***************************************************/
        private static IBHoMObject RemoveRootEntity(this Graph graph, IBHoMObject root)
        {
            graph.Entities.Remove(root.BHoM_Guid);
            foreach (Guid guid in m_OriginalSources)
            {
                IRelation relation = graph.Relations.Find(rel => rel.Source.Equals(root.BHoM_Guid) && rel.Target.Equals(guid));
                graph.Relations.Remove(relation);
            }
            return root;
        }
        /***************************************************/
        private static int biggestLevel(Dictionary<Guid, int> depths, List<int> distinctDepths)
        {
            int max = int.MinValue;
            foreach (int d in distinctDepths)
            {
                //all the entities at this level
                IEnumerable<Guid> level = depths.Where(kvp => kvp.Value == d).Select(kvp => kvp.Key);
                if (level.Count() > max)
                    max = level.Count();
            }
            return max;
        }
        /***************************************************/
        /****           Private Fields                  ****/
        /***************************************************/
        private static List<Guid> m_OriginalSources { get; set; }
    }
}
