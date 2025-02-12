/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.Data;
using KellermanSoftware;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;

//Minor modification of file from https://github.com/GregFinzer/Compare-Net-Objects/blob/master/Compare-NET-Objects/TypeComparers/DataRowComparer.cs
//Reason for inclusion in here is to allow for inclusion of these windows specific comparers in netstandard2.0
//Link to issue: https://github.com/BHoM/BHoM_Engine/issues/3455
//Code below under lincence as stated by https://github.com/GregFinzer/Compare-Net-Objects?tab=MS-PL-1-ov-file

namespace BH.Engine.Diffing
{
    /// <summary>
    /// Compare all columns in a data row
    /// </summary>
    public class DataRowComparer : BaseTypeComparer
    {
        /// <summary>
        /// Constructor that takes a root comparer
        /// </summary>
        /// <param name="rootComparer"></param>
        public DataRowComparer(RootComparer rootComparer)
            : base(rootComparer)
        { }

        /// <summary>
        /// Returns true if this is a DataRow
        /// </summary>
        /// <param name="type1">The type of the first object</param>
        /// <param name="type2">The type of the second object</param>
        /// <returns></returns>
        public override bool IsTypeMatch(Type type1, Type type2)
        {
            if(type1 == null || type2 == null)
                return false;

            return type1 == typeof(DataRow) && type2 == typeof(DataRow);
        }

        /// <summary>
        /// Compare two data rows
        /// </summary>
        public override void CompareType(CompareParms parms)
        {
            DataRow dataRow1 = parms.Object1 as DataRow;
            DataRow dataRow2 = parms.Object2 as DataRow;

            //This should never happen, null check happens one level up
            if (dataRow1 == null || dataRow2 == null)
                return;

            for (int i = 0; i < Math.Min(dataRow2.Table.Columns.Count, dataRow1.Table.Columns.Count); i++)
            {
                //Only compare specific column names
                if (parms.Config.MembersToInclude.Count > 0 && !parms.Config.MembersToInclude.Contains(dataRow1.Table.Columns[i].ColumnName))
                    continue;

                //If we should ignore it, skip it
                if (parms.Config.MembersToInclude.Count == 0 && parms.Config.MembersToInclude.Contains(dataRow1.Table.Columns[i].ColumnName))
                    continue;

                //If we should ignore read only, skip it
                if (!parms.Config.CompareReadOnly && dataRow1.Table.Columns[i].ReadOnly)
                    continue;

                //Both are null
                if (dataRow1.IsNull(i) && dataRow2.IsNull(i))
                    continue;

                string currentBreadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, string.Empty, string.Empty, dataRow1.Table.Columns[i].ColumnName);

                //Check if one of them is null
                if (dataRow1.IsNull(i))
                {
                    Difference difference = new Difference
                    {
                        ParentObject1 = parms.ParentObject1,
                        ParentObject2 = parms.ParentObject2,
                        PropertyName = currentBreadCrumb,
                        Object1Value = "(null)",
                        Object2Value = NiceString(dataRow2[i]),
                        Object1 = parms.Object1,
                        Object2 = parms.Object2
                    };

                    AddDifference(parms.Result, difference);
                    return;
                }

                if (dataRow2.IsNull(i))
                {
                    Difference difference = new Difference
                    {
                        ParentObject1 = parms.ParentObject1,
                        ParentObject2 = parms.ParentObject2,
                        PropertyName = currentBreadCrumb,
                        Object1Value = NiceString(dataRow1[i]),
                        Object2Value = "(null)",
                        Object1 = parms.Object1,
                        Object2 = parms.Object2
                    };

                    AddDifference(parms.Result, difference);
                    return;
                }

                //Check if one of them is deleted
                if (dataRow1.RowState == DataRowState.Deleted ^ dataRow2.RowState == DataRowState.Deleted)
                {
                    Difference difference = new Difference
                    {
                        ParentObject1 = parms.ParentObject1,
                        ParentObject2 = parms.ParentObject2,
                        PropertyName = currentBreadCrumb,
                        Object1Value = dataRow1.RowState.ToString(),
                        Object2Value = dataRow2.RowState.ToString(),
                        Object1 = parms.Object1,
                        Object2 = parms.Object2
                    };

                    AddDifference(parms.Result, difference);
                    return;
                }

                CompareParms childParms = new CompareParms();
                childParms.Result = parms.Result;
                childParms.Config = parms.Config;
                childParms.ParentObject1 = parms.Object1;
                childParms.ParentObject2 = parms.Object2;
                childParms.Object1 = dataRow1[i];
                childParms.Object2 = dataRow2[i];
                childParms.BreadCrumb = currentBreadCrumb;

                RootComparer.Compare(childParms);

                if (parms.Result.ExceededDifferences)
                    return;
            }
        }
    }
}
