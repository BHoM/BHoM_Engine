using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormFinding_Engine.Base
{
    public interface IRelaxItem
    {
        List<int> NodeIndices { get; set; }
    }
}
