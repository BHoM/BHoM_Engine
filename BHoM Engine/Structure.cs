using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM;
using BHoM.Structural.Results.Bars;
using BHoM.Structural.Results.Nodes;

namespace BHoM_Engine
{
    class Structure
    {
        NodalResultCollection nodalResultCollection = new NodalResultCollection();

        public void AddBarForcesToNodes(double timeStep)
        {
            BHoM.Global.Project p = new BHoM.Global.Project();
            BHoM.Structural.LoadcaseFactory loadCaseFactory = new BHoM.Structural.LoadcaseFactory(p);
            BHoM.Structural.Loads.Loadcase barForceLoads = loadCaseFactory.Create(0, "barForceLoads");

            int loadCaseNumber = barForceLoads.Number;

            foreach (BHoM.Structural.Bar bar in _edges)
            {   
                BarForceCollection barForceCollection = new BarForceCollection();
                BHoM.Geometry.Vector barForce = new BHoM.Geometry.Vector(barForceCollection.TryGetBarForce(loadCaseNumber, bar.Number, 0, timeStep).FX, barForceCollection.TryGetBarForce(loadCaseNumber, bar.Number, 0, timeStep).FY, barForceCollection.TryGetBarForce(loadCaseNumber, bar.Number, 0, timeStep).FZ);

                NodalResult startNodeResult = new NodalResult(bar.StartNode.Number);
                startNodeResult.Force += -1*barForce;
                
                NodalResult endNodeResult = new NodalResult(bar.EndNode.Number);
                endNodeResult.Force += barForce;
            }
        }

        public void AddGravitationalForceToNodes(double g, double timeStep)
        {
   
            //Flipped the direction of the weight, therefor the -1.
            foreach (BHoM.Structural.Node node in _vertices)
            {
                NodalResult nodalResult = nodalResultCollection.TryGetNodalResult(node.Number, timeStep);
                nodalResult.Force+= new BHoM.Geometry.Vector(0.0, 0.0, node.Mass * -1.0 * g);
            }
        }

        public void CalcNodalForces(double timeStep)
        {
            double g = 9.82;
            AddBarForcesToNodes(timeStep);
            AddGravitationalForceToNodes(g, timeStep);
        }

        public void ResetNodalForces(double timeStep)
        {
            foreach (BHoM.Structural.Node node in _vertices)
            {
                NodalResult nodalResult = nodalResultCollection.TryGetNodalResult(node.Number, timeStep);
                nodalResult.Force = new BHoM.Geometry.Vector(0.0, 0.0, 0.0);
            }
        }

        public void CalculatePanelForces()
        {
            foreach (PanelForm panel in _faces) //  BHoM.Structural.Panel?
                panel.CalculatePanelForces();
        }

        public void ResetInitialLength()
        {
            foreach (BHoM.Structural.Bar bar in _edges)
                bar.SetInitLengthToCurrentLength();
        }

        public void CalculateBarForces()
        {
            foreach (BHoM.Structural.Bar bar in _edges)
            {
                //bar.CalcLength();
                bar.CalcForce();
            }
        }

        /// <summary>
        /// Calculates the nodal acceleration in each node
        /// </summary>
        /// <param name="IsPointConstraintActive"></param>
        public void CalculateNodalAccelerations(bool IsPointConstraintActive, double timeStep)
        {
            BHoM.Geometry.Vector force = new BHoM.Geometry.Vector();
            BHoM.Geometry.Vector acceleration = new BHoM.Geometry.Vector();

            //double[] f = new double[3];
            //double[] a = new double[3];
            double m;
            // same????!!!! as below
            if (IsPointConstraintActive)
                foreach (BHoM.Structural.Node node in _vertices)
                {
                    if (node.Status != "fixed")
                    {
                        force = nodalResultCollection.TryGetNodalResult(node.Number, timeStep).Force; // need to add all loadcases together. loadcombinations?
                        m = node.Mass;
                        acceleration = force / m;
                        //a[0] = f[0] / m;
                        //a[1] = f[1] / m;
                        //a[2] = f[2] / m;
                        nodalResultCollection.TryGetNodalResult(node.Number, timeStep).Acceleration = acceleration;
                    }
                }
        }
        /// <summary>
        /// Calculates the nodal velocity in each node
        /// </summary>
        /// <param name="zeta"></param>
        /// <param name="deltaT"></param>
        public void CalculateNodalVelocities(double zeta, double deltaT, double timeStep)
        {
            BHoM.Geometry.Vector velocity0 = new BHoM.Geometry.Vector();
            BHoM.Geometry.Vector velocity1 = new BHoM.Geometry.Vector();
            BHoM.Geometry.Vector acceleration = new BHoM.Geometry.Vector();

            //double[] a = new double[3];
            //double[] v1 = new double[3];
            //double[] v0 = new double[3];

            ////TODO: what was this for?
            ////CalcAverageVelocity();

            foreach (BHoM.Structural.Node node in _vertices)
            {
                ////if (node.Status == "fixed") continue; if we want to disable point constraint...
                acceleration = nodalResultCollection.TryGetNodalResult(node.Number, timeStep).Acceleration;
                velocity0 = nodalResultCollection.TryGetNodalResult(node.Number, timeStep).Velocity;
                velocity1 = zeta * velocity0 + acceleration * deltaT;
                //v1[0] = zeta * v0[0] + a[0] * deltaT;
                //v1[1] = zeta * v0[1] + a[1] * deltaT;
                //v1[2] = zeta * v0[2] + a[2] * deltaT;
                nodalResultCollection.TryGetNodalResult(node.Number, timeStep).Velocity = velocity1;
            }
        }


        /// <summary>
        /// Calculate the nodal coordinates for each node
        /// </summary>
        /// <param name="deltaT"></param>
        /// <param name="bStabilityControl"></param>
        /// <param name="bPointConst"></param>
        /// <param name="maxMoveDist"></param>
        public void CalculateNodalCoordinates(double deltaT, bool bStabilityControl, bool bPointConst, double maxMoveDist, double timeStep)
        {         
            BHoM.Geometry.Vector velocity = new BHoM.Geometry.Vector();
            BHoM.Geometry.Point pos0 = new BHoM.Geometry.Point();
            BHoM.Geometry.Point pos1 = new BHoM.Geometry.Point();
            //double[] v = new double[3];
            //double[] x1 = new double[3];
            //double[] x0 = new double[3];
            double moveDist;
            double scaleMovement;
            string statusCheck = "";
            int maxMoveCount = 0;

            if (bPointConst) { statusCheck = "fixed"; }
            else if (!bPointConst) { statusCheck = ""; }

            foreach (BHoM.Structural.Node node in _vertices)
            {
                if (node.Status != statusCheck)
                {
                    velocity = nodalResultCollection.TryGetNodalResult(node.Number, timeStep).Velocity;
                    pos0 = node.Point;
                    pos1 = pos0 + velocity * deltaT;
                    //v = node.Velocity;
                    //x0 = node.GetCoord();

                    //x1[0] = x0[0] + v[0] * deltaT;
                    //x1[1] = x0[1] + v[1] * deltaT;
                    //x1[2] = x0[2] + v[2] * deltaT;

                    if (bStabilityControl)
                    {
                        //distance between two points
                        //moveDist = Utilities.MyDistance(x0, x1);
                        moveDist = pos0.DistanceTo(pos1);

                        //If the point moves more than mMaxMoveDist the movement will be reduced
                        //maxMoveDist *= 1000; //this line added as a temp fix and might cause instability; can be removed
                        if (moveDist > maxMoveDist)
                        {
                            scaleMovement = moveDist / maxMoveDist;
                            pos1 = pos0 + velocity * (deltaT / scaleMovement);
                            //x1[0] = x0[0] + (v[0] * deltaT) / scaleMovement;
                            //x1[1] = x0[1] + (v[1] * deltaT) / scaleMovement;
                            //x1[2] = x0[2] + (v[2] * deltaT) / scaleMovement;

                            maxMoveCount++;
                            //RhUtil.RhinoApp().Print("Maximum movement distance exceeded, nr: " + maxMoveCount + "\n");
                        }
                    }
                    nodalResultCollection.TryGetNodalResult(node.Number, timeStep).Translation = new BHoM.Geometry.Vector(pos1.X-pos0.X,pos1.Y-pos0.Y,pos1.Z-pos0.Z);
                    //node.SetCoord(x1[0], x1[1], x1[2]);
                }
            }
        }


        public void CalculateKineticEnergy(double timeStep)
        {
            //Set mKe to zero
            _Ke = 0;
            //double[] v;
            BHoM.Geometry.Vector velocity;
            double m;

            foreach (BHoM.Structural.Node node in _vertices)
            {
                //TODO: 
                if (node.Status != "BoundToCurve")
                {
                    //v = node.Velocity;
                    velocity = nodalResultCollection.TryGetNodalResult(node.Number, timeStep).Velocity;
                    m = node.Mass;
                    _Ke = Math.Pow(velocity.Length, 2) * m / 2.0;
                    //mKe += v[0] * v[0] * m / 2.0;
                    //mKe += v[1] * v[1] * m / 2.0;
                    //mKe += v[2] * v[2] * m / 2.0;
                }
            }
        }



        /** Move fixed mNodes back to their initial position.
       *
       */
        public void ResetPointBC()
        {
            foreach (BHoM.Structural.Node node in _vertices)
            {
                if (node.Status == "fixed")
                {
                    node.ResetInitialPos();
                }
            }
        }

        /** Find the node with the smallest mass
        *
        */
        public BHoM.Structural.Node FindLightestNode()
        {
            double smallestMass = 1000000000;
            int index = 0;
            foreach (BHoM.Structural.Node node in _vertices)
            {
                if (node.Mass < smallestMass)
                {
                    smallestMass = node.Mass;
                    index = _vertices.IndexOf(node);
                }
            }
            return (BHoM.Structural.Node)_vertices[index];
        }

        /** Find the bar with the greatest stiffness
        *
        */

        public BHoM.Structural.Bar FindStiffestBar()
        {
            if (_edges.Count == 0) return null;

            double stiffest = 0;
            double barStiffness;
            int index = 0;
            foreach (BHoM.Structural.Bar bar in _edges)
            {
                barStiffness = bar.Stiffness * bar.StiffnessGlobalScaleFactor * bar.StiffnessCustomScaleFactor;
                if (barStiffness > stiffest)
                {
                    stiffest = barStiffness;
                    index = _edges.IndexOf(bar);
                }
            }
            return (BHoM.Structural.Bar)_edges[index];
        }



        /** Loops throught all mNodes and calculates the weight of all bars connected to the node.
        *
        */
        public void SetNodalMassesPerUnitLength(double massPerUnitLength)
        {
            List<BHoM.Structural.Bar> tempBars = new List<BHoM.Structural.Bar>();
            double sumLength;

            foreach (BHoM.Structural.Node node in _vertices)
            {
                sumLength = 0;
                tempBars = node.GetRingEdges();

                foreach (BHoM.Structural.Bar bar in tempBars)
                {
                    sumLength += bar.Length / 2;
                }
                node.Mass = massPerUnitLength * sumLength;
            }
        }




        /// <summary>
        ///  Calculate the total average length of bars in the model
        /// </summary>
        /// <returns></returns>
        public double CalculateMeanBarLength()
        {
            double avrgLength = 0;
            foreach (BHoM.Structural.Bar bar in _edges)
            {
                avrgLength += bar.Length;
            }
            avrgLength /= _edges.Count;
            return avrgLength;
        }

        public double CalculateMeanVelocity()
        {
            double mAvrgVelocity;
            // double[] mAvrgVelocities = new double[3];
            BHoM.Geometry.Vector mAvrgVelocities = new BHoM.Geometry.Vector();

            mAvrgVelocity = 0;

            foreach (BHoM.Structural.Node node in _vertices)
            {
                //mAvrgVelocities[0] += node.Velocity[0];
                //mAvrgVelocities[1] += node.Velocity[1];
                //mAvrgVelocities[2] += node.Velocity[2];
                //mAvrgVelocity += Math.Sqrt(node.Velocity[0] * node.Velocity[0] + node.Velocity[1] * node.Velocity[1] + node.Velocity[2] * node.Velocity[2]);
                mAvrgVelocities += node.Velocity;
                mAvrgVelocity += node.Velocity.Length;

            }
            //mAvrgVelocities[0] = mAvrgVelocities[0] / _vertices.Count;
            //mAvrgVelocities[1] = mAvrgVelocities[1] / _vertices.Count;
            //mAvrgVelocities[2] = mAvrgVelocities[2] / _vertices.Count;
            //why? does it do anything?
            mAvrgVelocities = mAvrgVelocities / _vertices.Count;

            return mAvrgVelocity / _vertices.Count;

        }





    }
}

