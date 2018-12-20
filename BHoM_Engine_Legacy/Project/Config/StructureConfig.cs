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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Project
{
    public partial class Config
    {
        private string m_SectionDB = "UK_Sections";
        private string m_MaterialDB = "Europe";
        private string m_PfeiferFullLockedDB = "PfeiferFullLocked";
        private string m_StyliteForkSocketDB = "BridonSTF";
        private string m_StyliteAdjustableForkSocketDB = "BridonSTAF";
        private string m_StyliteRingConnectorDB = "BridonSTRC";
        
        public string SectionDatabase
        {
            get
            {
                return m_SectionDB;
            }
            set
            {
                m_SectionDB = value;
            }
        }

        public string MaterialDatabase
        {
            get
            {
                return m_MaterialDB;
            }
            set
            {
                m_MaterialDB = value;
            }
        }

        public string CableDataBase
        {
            get
            {
                return m_PfeiferFullLockedDB;
            }
            set
            {
                m_PfeiferFullLockedDB = value;
            }
        }

        public string StyliteForkSocketDataBase
        {
            get
            {
                return m_StyliteForkSocketDB;
            }
            set
            {
                m_StyliteForkSocketDB = value;
            }
        }

        public string StyliteAdjustableForkSocketDataBase
        {
            get
            {
                return m_StyliteAdjustableForkSocketDB;
            }
            set
            {
                m_StyliteAdjustableForkSocketDB = value;
            }
        }

        public string StyliteRingConnectorDataBase
        {
            get
            {
                return m_StyliteRingConnectorDB;
            }
            set
            {
                m_StyliteRingConnectorDB = value;
            }
        }
    }
}
