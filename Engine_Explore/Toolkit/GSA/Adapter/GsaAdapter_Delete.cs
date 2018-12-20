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

using Engine_Explore.BHoM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHE = Engine_Explore.Engine;

namespace Engine_Explore.Adapter
{
    public partial class GsaAdapter
    {
        public bool Delete(string filter = "", string config = "")
        {
            if (filter.Count() == 0)
                return DeleteAll(config);
            else
            {
                // Need to identify that we want to delete tags
                // Delete Element with tag, Remove tag from element with multiple tags

                //Other types of delete filters
                return true;
            }
        }

        /*******************************************/

        public bool DeleteAll(string config = "")
        {
            return m_Link.DeleteAll();
        }
    }
}
