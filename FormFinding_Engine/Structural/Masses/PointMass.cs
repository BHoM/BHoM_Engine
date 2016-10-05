using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using FormFinding_Engine.Base;

namespace FormFinding_Engine.Structural
{
    public class PointMass : IRelaxMass
    {
        public PointMass()
        { }

       private double m_mass;

        public PointMass(Point point, double mass)
        {
            Positions = new List<Point> { point };
            m_mass = mass;
        }
        

        public List<int> NodeIndices
        {
            get; set;
        }

        public List<Point> Positions
        {
            get; private set;
        }
        

        public void ApplyMass(List<RelaxNode> nodeData)
        {
            double mass = nodeData[NodeIndices[0]].Mass() + m_mass;

            nodeData[NodeIndices[0]].SetMass(mass);
        }
    }
}
