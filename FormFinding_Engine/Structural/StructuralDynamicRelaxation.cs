using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Generic;
using FormFinding_Engine.Base;

namespace FormFinding_Engine.Structural
{
    public class StructuralDynamicRelaxation
    {
        private double m_mergeTolerance = 0.001;


        private PointMatrix<int> m_positions;
        private RelaxSystem engine;
        private IRelaxCalculator calculator;
        

        public StructuralDynamicRelaxation()
        {
            calculator = new StructuralRelaxCalculator();
            m_positions = new PointMatrix<int>(0.1);
            engine = new RelaxSystem(calculator);
        }


        public StructuralDynamicRelaxation(double dt, double threshold, double damping, double maxiterations)
        {
            calculator = new StructuralRelaxCalculator(dt, threshold, damping, maxiterations);
            m_positions = new PointMatrix<int>(0.1);
            engine = new RelaxSystem(calculator);
        }

        public void Run()
        {
            engine.Run();
        }

        /***********************************************************/
        /************* Add Methods *********************************/
        /***********************************************************/

        private void AddPositionItem(IRelaxPosition item)
        {
            int maxIndex = engine.Nodes.Count;
            List<int> indices = new List<int>();

            for (int i = 0; i < item.Positions.Count; i++)
            {
                Point p = item.Positions[i];
                PointMatrix<int>.CompositeValue val = m_positions.GetClosestPoint(p, m_mergeTolerance);

                if (val.Point == null)
                {
                    m_positions.AddPoint(p, maxIndex);
                    indices.Add( maxIndex);
                    RelaxNode node = new RelaxNode(calculator, 3);
                    node.Data[NodeProps.POS] = new double[] { p.X, p.Y, p.Z };
                    engine.Nodes.Add(node);
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
                engine.AddGoal(goal);
            }
        }


        /***********************************************************/

        public void AddBCs(IEnumerable<IRelaxPositionBC> bcs)
        {
            foreach (IRelaxPositionBC bc in bcs)
            {
                AddPositionItem(bc);
                engine.BoundaryConditions.Add(bc);
            }
        }

        /***********************************************************/

        public void AddMasses(IEnumerable<IRelaxMass> masses)
        {
            foreach (IRelaxMass mass in masses)
            {
                AddPositionItem(mass);
                mass.ApplyMass(engine.Nodes);
            }
        }

        /***********************************************************/
        /************* Get Geometry   ******************************/
        /***********************************************************/

        public List<Point> GetPoints()
        {
            List<Point> pts = new List<Point>();

            foreach (RelaxNode n in engine.Nodes)
            {
                double[] newPos = n.NewPosition();
                pts.Add(new Point(newPos[0], newPos[1], newPos[2]));
            }

            return pts;
        }

        public int Iterations()
        {
            return engine.Iterations;
        }


        public Action<List<RelaxNode>> ResultCallback
        {
            get { return engine.ResultCallback; }
            set { engine.ResultCallback = value; }
        }



        /***********************************************************/
        /***************** Get Forces ******************************/
        /***********************************************************/



    }
}
