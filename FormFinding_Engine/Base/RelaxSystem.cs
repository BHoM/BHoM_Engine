using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormFinding_Engine.Base
{
    

    public class RelaxSystem
    {
        /************************************************/
        /*********** Fields *****************************/
        /************************************************/

        private List<RelaxNode> m_nodes;
        private List<IRelaxGoal> m_goals;
        private IRelaxCalculator m_calculator;
        private List<IRelaxBC> m_BoundaryConditons;
        public Action<List<RelaxNode>> ResultCallback;

        /************************************************/
        /*********** Constructors ***********************/
        /************************************************/

        public RelaxSystem(IRelaxCalculator calculator)
        {
            m_calculator = calculator;
            m_nodes = new List<RelaxNode>();
            m_goals = new List<IRelaxGoal>();
            m_BoundaryConditons = new List<IRelaxBC>();

        }


        /************************************************/
        /*********** Relax methods **********************/
        /************************************************/

        public void Run()
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            timer.Start();

            while (Relax())
            {
                Iterations++;

                if (ResultCallback == null)
                    continue;

                if (timer.ElapsedMilliseconds == 30L)
                {
                    ResultCallback.Invoke(m_nodes);
                    timer.Restart();
                }
            }

        }

        /************************************************/

        public bool Relax()
        {
            m_calculator.SetUpIteration(m_nodes);

            CalculateForces();

            m_calculator.CalculateChangeRate(m_nodes);

            ConstrainVelocity();

            m_calculator.CalculateDisplacement(m_nodes);

            bool converged = m_calculator.CheckConvergence(m_nodes, Iterations);

            m_calculator.CalculateEnergy(m_nodes);

            return !converged;

        }

        /************************************************/

        private void CalculateForces()
        {
            foreach (IRelaxGoal goal in m_goals)
                goal.CalcForces(m_nodes);
        }

        /************************************************/

        private void ConstrainVelocity()
        {
            foreach (IRelaxBC bc in m_BoundaryConditons)
            {
                bc.ApplyConstraint(m_nodes);
            }
        }

        /************************************************/
        /*********** Getters and setters ****************/
        /************************************************/

        public List<IRelaxGoal> Goals
        {
            get { return m_goals; }
            set { m_goals = value; }
        }

        public List<IRelaxBC> BoundaryConditions
        {
            get { return m_BoundaryConditons; }
            set { m_BoundaryConditons = value; }
        }

        public List<RelaxNode> Nodes
        {
            get { return m_nodes; }
            set { m_nodes = value; }
        }

        public int Iterations
        {
            private set;
            get;
        }


        public void AddGoal(IRelaxGoal goal)
        {
            m_goals.Add(goal);
        }

        public void AddGoal(IEnumerable<IRelaxGoal> goals)
        {
            m_goals.AddRange(goals);
        }

    }
}
