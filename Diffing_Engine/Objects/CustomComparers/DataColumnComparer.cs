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
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;

//Minor modification of file from https://github.com/GregFinzer/Compare-Net-Objects/blob/master/Compare-NET-Objects/TypeComparers/DataColumnComparer.cs
//Reason for inclusion in here is to allow for inclusion of these windows specific comparers in netstandard2.0
//Link to issue: https://github.com/BHoM/BHoM_Engine/issues/3455
//Code below under lincence as stated by https://github.com/GregFinzer/Compare-Net-Objects?tab=MS-PL-1-ov-file

namespace BH.Engine.Diffing
{
    /// <summary>
    /// Compare a data column
    /// </summary>
    public class DataColumnComparer : BaseTypeComparer
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="rootComparer"></param>
        public DataColumnComparer(RootComparer rootComparer) : base(rootComparer)
        {
        }

        /// <summary>
        /// Returns true if both types compared are a DataColumn
        /// </summary>
        /// <param name="type1"></param>
        /// <param name="type2"></param>
        /// <returns></returns>
        public override bool IsTypeMatch(Type type1, Type type2)
        {
            if (type1 == null || type2 == null)
                return false;

            return type1 == typeof(DataColumn) && type2 == typeof(DataColumn);
        }

        /// <summary>
        /// Compare a Data Column
        /// </summary>
        /// <param name="parms"></param>
        public override void CompareType(CompareParms parms)
        {
            //This should never happen, null check happens one level up
            if (parms.Object1 == null || parms.Object2 == null)
                return;

            DataColumn col1 = (DataColumn)parms.Object1;
            DataColumn col2 = (DataColumn)parms.Object2;

            CompareProp(parms, col1, col2, col1.AllowDBNull, col2.AllowDBNull, "AllowDBNull");
            CompareProp(parms, col1, col2, col1.AutoIncrement, col2.AutoIncrement, "AutoIncrement");
            CompareProp(parms, col1, col2, col1.AutoIncrementSeed, col2.AutoIncrementSeed, "AutoIncrementSeed");
            CompareProp(parms, col1, col2, col1.AutoIncrementStep, col2.AutoIncrementStep, "AutoIncrementStep");
            CompareProp(parms, col1, col2, col1.Caption, col2.Caption, "Caption");
            CompareProp(parms, col1, col2, col1.ColumnName, col2.ColumnName, "ColumnName");
            CompareProp(parms, col1, col2, col1.DataType, col2.DataType, "DataType");
            CompareProp(parms, col1, col2, col1.DefaultValue, col2.DefaultValue, "DefaultValue");
            CompareProp(parms, col1, col2, col1.Expression, col2.Expression, "Expression");
            CompareProp(parms, col1, col2, col1.MaxLength, col2.MaxLength, "MaxLength");
            CompareProp(parms, col1, col2, col1.Namespace, col2.Namespace, "Namespace");
            CompareProp(parms, col1, col2, col1.Ordinal, col2.Ordinal, "Ordinal");
            CompareProp(parms, col1, col2, col1.Prefix, col2.Prefix, "Prefix");
            CompareProp(parms, col1, col2, col1.ReadOnly, col2.ReadOnly, "ReadOnly");
            CompareProp(parms, col1, col2, col1.Unique, col2.Unique, "Unique");
        }

        private void CompareProp<T>(CompareParms parms, DataColumn col1, DataColumn col2, T prop1, T prop2, string propName)
        {
            if (parms.Result.ExceededDifferences)
                return;

            string currentBreadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, propName);

            CompareParms childParms = new CompareParms();
            childParms.Result = parms.Result;
            childParms.Config = parms.Config;
            childParms.BreadCrumb = currentBreadCrumb;
            childParms.ParentObject1 = col1;
            childParms.ParentObject2 = col2;
            childParms.Object1 = prop1;
            childParms.Object2 = prop2;

            RootComparer.Compare(childParms);
        }
    }
}