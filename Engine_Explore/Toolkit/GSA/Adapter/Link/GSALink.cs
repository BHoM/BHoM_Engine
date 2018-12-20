/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using Interop.gsa_8_7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Adapter.Link
{
    public partial class GSALink
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string Filename { get; }

        public List<string> ErrorLog { get; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GSALink()
        {
            m_Gsa = new ComAuto();
            ErrorLog = new List<string>();
        }

        /***************************************************/

        public GSALink(string filePath) : this()
        {
            short result;
            if (filePath != "")
                result = m_Gsa.Open(filePath);
            else
                result = m_Gsa.NewFile();
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Execute(string command)
        {
            var result = m_Gsa.GwaCommand(command);

            if ((int)result != 1)
            {
                ErrorLog.Add("Application of command " + command + " error. Invalid arguments?");
                return false;
            }

            return true;
        }

        /***************************************************/

        public List<GsaNode> PullNodes(IEnumerable<int> indices)
        {
            GsaNode[] nodes;
            m_Gsa.Nodes(indices.ToArray(), out nodes);
            return nodes.ToList();
        }

        /***************************************************/

        public int PullInt(string query)
        {
            return m_Gsa.GwaCommand(query);
        }

        /***************************************************/

        public bool DeleteAll()
        {
            return m_Gsa.Delete("RESULTS") == 0;
        }

        /*******************************************/
        /****  Private Fields                   ****/
        /*******************************************/

        private ComAuto m_Gsa;
    }
}

