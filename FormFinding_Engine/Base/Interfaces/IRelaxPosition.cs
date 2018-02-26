using BH.oM.Geometry;
using System.Collections.Generic;

namespace FormFinding_Engine.Base
{
    public interface IRelaxPosition : IRelaxItem
    {
        List<Point> Positions { get; }
    }
}
