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

        [Description("Decide whether or not to have BHoM Events of type error thrown as C# excceptions or not. Default behaviour of BHoM Event log is for errors to NOT be thrown. Turn this off if you would like to catch BHoM Events of type error as C# exceptions.")]
        [Input("throwErrorsAsExceptions", "Set this to true if you want to have BHoM Events of type Error to be thrown as Exceptions when they are logged. Set this to false if you do NOT want this to happen (default BHoM Log behaviour).")]
        public static void ThrowErrorsAsExceptions(bool throwErrorsAsExceptions)
        {
            m_ThrowError = throwErrorsAsExceptions;
        }
    }
}
