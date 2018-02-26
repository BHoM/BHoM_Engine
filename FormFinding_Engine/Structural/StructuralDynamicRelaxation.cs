using System;
using System.Collections.Generic;
using FormFinding_Engine.Base;
using BH.oM.DataStructure;
using BH.oM.Geometry;
using BH.Engine.DataStructure;

namespace FormFinding_Engine.Structural
{
    public class StructuralDynamicRelaxation
    {
        private double m_mergeTolerance = 0.001;


        private PointMatrix<int> m_positions;
        private RelaxSystem m_engine;
        private IRelaxCalculator m_calculator;
        

        public StructuralDynamicRelaxation()
        {
            m_calculator = new StructuralRelaxCalculator();
            m_positions = new PointMatrix<int> { CellSize = 0.1 };
            m_engine = new RelaxSystem(m_calculator);
        }


        public StructuralDynamicRelaxation(double dt, double threshold, double damping, double maxiterations)
        {
            m_calculator = new StructuralRelaxCalculator(dt, threshold, damping, maxiterations);
            m_positions = new PointMatrix<int> { CellSize = 0.1 };
            m_engine = new RelaxSystem(m_calculator);
        }

        public void Run()
        {
            m_engine.Run();
        }

        /***********************************************************/
        /************* Add Methods *********************************/
        /***********************************************************/

        private void AddPositionItem(IRelaxPosition item)
        {
            int maxIndex = m_engine.Nodes.Count;
            List<int> indices = new List<int>();

            for (int i = 0; i < item.Positions.Count; i++)
            {
                Point p = item.Positions[i];
                LocalData<int> val = m_positions.ClosestData(p, m_mergeTolerance);

                if (val.Position == null)
                {
                    m_positions.Add(p, maxIndex);
                    indices.Add( maxIndex);
                    RelaxNode node = new RelaxNode(m_calculator, 3);
                    node.Data[NodeProps.POS] = new double[] { p.X, p.Y, p.Z };
                    m_engine.Nodes.Add(node);
                    maxIndex++;
                }
                else
                {
                    indices.Add(val.Data);
                }

            }
            item.NodeIndices = indices;
        }

        /***********************************************************/

        public void AddGoals(IEnumerable<IRelaxPositionGoal> goals)
        {
            foreach (IRelaxPositionGoal goal in goals)
            {
                AddPositionItem(goal);
                m_engine.AddGoal(goal);
            }
        }


        /***********************************************************/

        public void AddBCs(IEnumerable<IRelaxPositionBC> bcs)
        {
            foreach (IRelaxPositionBC bc in bcs)
            {
                AddPositionItem(bc);
                m_engine.BoundaryConditions.Add(bc);
            }
        }

        /***********************************************************/

        public void AddMasses(IEnumerable<IRelaxMass> masses)
        {
            foreach (IRelaxMass mass in masses)
            {
                AddPositionItem(mass);
                mass.ApplyMass(m_engine.Nodes);
            }
        }

        /***********************************************************/
        /************* Get Geometry   ******************************/
        /***********************************************************/

        public List<Point> GetPoints()
        {
            List<Point> pts = new List<Point>();

            foreach (RelaxNode n in m_engine.Nodes)
            {
                double[] newPos = n.NewPosition();
                pts.Add(new Point { X = newPos[0], Y = newPos[1], Z = newPos[2] });
            }

            return pts;
        }

        public int Iterations()
        {
            return m_engine.Iterations;
        }


        public Action<List<RelaxNode>> ResultCallback
        {
            get { return m_engine.ResultCallback; }
            set { m_engine.ResultCallback = value; }
        }


        public List<IRelaxGoal> Goals
        {
            get { return m_engine.Goals; }
        }

        /***********************************************************/
        /***************** Get Forces ******************************/
        /***********************************************************/



    }
}
