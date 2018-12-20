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

using BH.oM.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHM = BH.oM.Materials;

namespace BH.oM.Structural.Materials
{
    public static class Create
    {
        //public static BHM.Material FromDatabase(string name)
        //{
        //    IDataAdapter database = Instance.Active.GetDatabase<MaterialRow>(DatabaseType.Material);
        //    database.TableName = Instance.Active.Config.MaterialDatabase;
        //    MaterialRow data = (MaterialRow)database.GetDataRow("Name", name);
        //    if (data != null)
        //    {
        //        return FromDataRow(data);
        //    }
        //    return null;
        //}

        public static BHM.Material Default(MaterialType type)
        {
            return new Material(type.ToString(), type, 0, 0, 0, 0, 0);
        }

        //private static BHM.Material FromDataRow(MaterialRow data)
        //{
        //    MaterialType type = (MaterialType)Enum.Parse(typeof(MaterialType), data.Type.ToString(), true);
        //    string name = data.Name.Trim();
        //    double e = data.YoungsModulus;
        //    double v = data.PoissonsRatio;
        //    double tC = data.CoefOfThermalExpansion;
        //    double density = data.Mass;
        //    double g = e / (2 * (1 + v));

        //    BHM.Material m = new BHM.Material(name, type, e, v, tC, g, density);

        //    switch (type)
        //    {
        //        case MaterialType.Concrete:
        //            m.CompressiveYieldStrength = data.CompressiveStrength;
        //            break;
        //        case MaterialType.Steel:
        //            m.TensileYieldStrength = data.MinimumYieldStress;
        //            m.CompressiveYieldStrength = m.TensileYieldStrength;
        //            break;
        //        case MaterialType.Rebar:
        //            m.TensileYieldStrength = data.EffectiveTensileStress;
        //            m.CompressiveYieldStrength = data.CompressiveStrength;
        //            break;
        //    }
        //    return m;
        //}
    }
}
