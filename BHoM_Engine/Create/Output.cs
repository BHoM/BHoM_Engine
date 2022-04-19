/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using System;
using BH.oM.Base;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Output<T> Output<T>(T item1)
        {
            return new Output<T> { Item1 = item1 };
        }

        /***************************************************/

        public static Output<T1, T2> Output<T1, T2>(T1 item1, T2 item2)
        {
            return new Output<T1, T2> { Item1 = item1, Item2 = item2 };
        }

        /***************************************************/

        public static Output<T1, T2, T3> Output<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new Output<T1, T2, T3> { Item1 = item1, Item2 = item2, Item3 = item3 };
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4> Output<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new Output<T1, T2, T3, T4> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4 };
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4, T5> Output<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            return new Output<T1, T2, T3, T4, T5> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4, Item5 = item5 };
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4, T5, T6> Output<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            return new Output<T1, T2, T3, T4, T5, T6> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4, Item5 = item5, Item6 = item6 };
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4, T5, T6, T7> Output<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            return new Output<T1, T2, T3, T4, T5, T6, T7> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4, Item5 = item5, Item6 = item6 , Item7 = item7};
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4, T5, T6, T7, T8> Output<T1, T2, T3, T4, T5, T6, T7, T8>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
        {
            return new Output<T1, T2, T3, T4, T5, T6, T7, T8> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4, Item5 = item5, Item6 = item6, Item7 = item7, Item8 = item8};
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4, T5, T6, T7, T8, T9> Output<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9)
        {
            return new Output<T1, T2, T3, T4, T5, T6, T7, T8, T9> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4, Item5 = item5, Item6 = item6, Item7 = item7, Item8 = item8, Item9 = item9 };
        }

        /***************************************************/

        public static Output<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Output<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10)
        {
            return new Output<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> { Item1 = item1, Item2 = item2, Item3 = item3, Item4 = item4, Item5 = item5, Item6 = item6, Item7 = item7, Item8 = item8, Item9 = item9, Item10 = item10 };
        }

        /***************************************************/
    }
}



