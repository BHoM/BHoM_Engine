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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Database
{
    public interface IDataAdapter
    {
        List<string> Names();
        List<string> TableNames();
        List<string> ColumnNames();
        List<IDataRow> Query(string query);

        string TableName { get; set; }

        IDataRow GetDataRow(string column, string matches);
        IDataRow GetDataRow(string[] columns, string[] matches, bool AND = false);

        List<string> GetDataColumn(string columnName);

        T GetDataRow<T>(string column, string matches) where T : IDataRow;
        List<IDataRow> GetDataSet(string column, string matches);
        List<string> GetDataColumn(string columnName, string column, string matches);
    }
}
