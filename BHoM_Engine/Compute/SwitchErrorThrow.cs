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

using BH.oM.Base.Debugging;
using BH.oM.Base.Attributes;
using System.Linq;
using System.ComponentModel;
using System;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Switch off throwing errors when they reach the event log.")]
        public static void SwitchErrorThrowOff()
        {
            m_ThrowError = false;
        }

        /***************************************************/

        [Description("Switch on throwing errors by the event log. By default, errors are NOT thrown when they reach the BHoM event log. Switching this on will result in errors hitting the event log and being thrown, and if a suitable try/catch is not in place to catch this, you may encounter crashes in your system. Use at your own risk. Please consult the documentation for more information.")] //ToDo: Write the documentation on BHoM.xyz for this system
        [Input("areYouSure", "Set this to true if you are sure you want to throw all errors recorded by the event log for try/catch statements to handle.")]
        public static void SwitchErrorThrowOn(bool areYouSure)
        {
            if (!areYouSure)
                return;

            m_ThrowError = true;
        }
    }
}
