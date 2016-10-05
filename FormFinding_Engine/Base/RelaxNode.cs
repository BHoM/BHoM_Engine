using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormFinding_Engine.Base
{
    public class RelaxNode
    {
        public Dictionary<string, double[]> Data { get; set; }

        public RelaxNode()
        {
            Data = new Dictionary<string, double[]>();
        }

        public RelaxNode(IRelaxCalculator calc, int dimensions)
        {
            Data = calc.InitiateDictionary(dimensions);
        }

    }
}
