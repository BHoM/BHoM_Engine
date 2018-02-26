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
