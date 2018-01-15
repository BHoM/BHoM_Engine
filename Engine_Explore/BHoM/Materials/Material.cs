using Engine_Explore.BHoM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Materials
{
    /// <summary>
    /// Material class for use in all other object classes and namespaces
    /// </summary>
    public class Material : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public MaterialType Type { get; set; }

        public double YoungsModulus { get; set; }

        public double PoissonsRatio { get; set; }

        public double ShearModulus { get; set; }

        public double DryDensity { get; set; }

        public double CoeffThermalExpansion { get; set; }

        public double DampingRatio { get; set; }

        public double Density { get; set; }

        public double CompressiveYieldStrength { get; set; }

        public double TensileYieldStrength { get; set; }

        public double StainAtYield { get; set; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/
        

        public Material(string name = "")
        {
            Name = name;
        }

        /***************************************************/

        public Material(string name, MaterialType type, double E, double v, double tC, double G, double density)
        {
            Name = name;
            Type = type;
            YoungsModulus = E;
            PoissonsRatio = v;
            CoeffThermalExpansion = tC;
            ShearModulus = G;
            Density = density;
        }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/
    }
}
