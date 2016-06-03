using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Structural;
using BHoM.Structural.Results.Bars;
using BHoM.Structural.Results.Nodes;
using BHoM.Structural.Loads;
using BHoM.Global;


namespace BHoM_Engine.FormFinding
{
    public class Structure
    {
        public List<Bar> Bars = new List<Bar>();
        public List<Node> Nodes = new List<Node>();
        public List<Node> LockedNodes = new List<Node>();

        public NodalResultCollection NodalResultCollection = new NodalResultCollection();
        public BarForceCollection BarForceCollection = new BarForceCollection();

        // public ObjectManager<string, FormFinding.BarForce> barForceCollection = new ObjectManager<string, FormFinding.BarForce>("TimeStepKey2", FilterOption.UserData);
        // public ObjectManager<string, FormFinding.NodalResult> nodalResultCollection = new ObjectManager<string, FormFinding.NodalResult>("Nodes2", FilterOption.UserData);

        public Dictionary<string, NodalResult> nodalResultCollection = new Dictionary<string, NodalResult>();
        public Dictionary<string, BarForce> barForceCollection = new Dictionary<string, BarForce>();

        public Loadcase loadcase = new Loadcase(1, "Loadcase", LoadNature.Dead, 0);

        public double dt = 0.05;
        public double t = 0;
        public double c = 0.95;
        public double nodeTol = 0.1;
        public double treshold;

        public Structure(List<Line> lines)
        {
            foreach (Line ln in lines)
            {
                Bar newBar = new Bar(new Node(ln.StartPoint.X, ln.StartPoint.Y, ln.StartPoint.Z), new Node(ln.EndPoint.X, ln.EndPoint.Y, ln.EndPoint.Z));
                newBar.Name = this.Bars.Count.ToString();

                if (NodeExists(ln.StartPoint) != null)
                {
                    newBar.StartNode = NodeExists(ln.StartPoint);
                }
                else
                {
                    Node newNode = new Node(ln.StartPoint.X, ln.StartPoint.Y, ln.StartPoint.Z);
                    newNode.Name = this.Nodes.Count.ToString();
                    newBar.StartNode = newNode;
                    this.Nodes.Add(newNode);
                }

                if (NodeExists(ln.EndPoint) != null)
                {
                    newBar.EndNode = NodeExists(ln.EndPoint);
                }
                else
                {
                    Node newNode = new Node(ln.EndPoint.X, ln.EndPoint.Y, ln.EndPoint.Z);
                    newNode.Name = this.Nodes.Count.ToString();
                    newBar.EndNode = newNode;
                    this.Nodes.Add(newNode);
                }

                this.Bars.Add(newBar);

            }
        }

        public Node NodeExists(Point pt)
        {
            Node existingNode = null;
            foreach (Node structureNode in this.Nodes)
            {
                double d = structureNode.Point.DistanceTo(pt);
                if (d < this.nodeTol)
                    existingNode = structureNode;
            }

            return existingNode;
        }

        public void SetStartVelocity()
        {
            foreach (Node node in this.Nodes)
            {
                FormFinding.NodalResult nodeResult = new FormFinding.NodalResult(Int32.Parse(node.Name));
                nodeResult.Velocity = new Vector(0, 0, 0);

                nodalResultCollection.Add(node.Name + ":" + t.ToString(), nodeResult);
            }
        }

        public void CalcSlackLength(List<double> lengthMultiplier)
        {
            for (int i = 0; i < this.Bars.Count; i++)
                if (lengthMultiplier.Count > 1)
                    this.Bars[i].CustomData.Add("SlackLength", (this.Bars[i].Length * lengthMultiplier[i]));
                else
                    this.Bars[i].CustomData.Add("SlackLength", (this.Bars[i].Length * lengthMultiplier[0]));
        }

        public void SetBarStiffness(List<double> barStiffnesses)
        {
            for (int i = 0; i < this.Bars.Count; i++)
                if (barStiffnesses.Count > 1)
                    this.Bars[i].CustomData.Add("Stiffness", barStiffnesses[i]);
                else
                    this.Bars[i].CustomData.Add("Stiffness", barStiffnesses[0]);
        }

        public void SetNodeMass(List<double> nodeMass)
        {
            for (int i = 0; i < this.Nodes.Count; i++)
                if (nodeMass.Count > 1)
                    this.Nodes[i].CustomData.Add("Mass", nodeMass[i]);
                else
                    this.Nodes[i].CustomData.Add("Mass", nodeMass[0]);
        }


        public void FindLockedNodes(List<Point> listLocked)
        {
            foreach (Node node in this.Nodes)
                node.CustomData.Add("isLocked", false);

            foreach (Node node in this.Nodes)
                foreach (Point lockedPt in listLocked)
                    if (node.Point.DistanceTo(lockedPt) < this.nodeTol)
                        node.CustomData["isLocked"] = true;
        }

        public void CalcBarForce()
        {
            foreach (Bar bar in this.Bars)
            {
                FormFinding.BarForce barForce = new FormFinding.BarForce(Int32.Parse(bar.Name), 0.5, this.loadcase, new Plane(bar.StartNode.Point, Vector.CrossProduct(new Vector(bar.EndNode.X - bar.StartNode.X, bar.EndNode.Y - bar.StartNode.Y, bar.EndNode.Z - bar.StartNode.Z), new Vector(0, 0, 1))));
                Vector unitVec = new Vector((bar.EndNode.Point.X - bar.StartNode.Point.X) / bar.Length, (bar.EndNode.Point.Y - bar.StartNode.Point.Y) / bar.Length, (bar.EndNode.Point.Z - bar.StartNode.Point.Z) / bar.Length);

                double dl = bar.Length - (double)bar.CustomData["SlackLength"];

                barForce.FX = unitVec.X * dl * (double)bar.CustomData["Stiffness"];
                barForce.FY = unitVec.Y * dl * (double)bar.CustomData["Stiffness"];
                barForce.FZ = unitVec.Z * dl * (double)bar.CustomData["Stiffness"];

                barForceCollection.Add(bar.Name + ":" + t.ToString(), barForce);


                //this.BarForceCollection.Add(barForce, this.t);
            }
        }

        public void CalcNodeForce(List<double> gravity)
        {
            foreach (Node node in this.Nodes)
            {
                FormFinding.NodalResult nodeResult = new FormFinding.NodalResult(Int32.Parse(node.Name));

                nodeResult.Force = new Vector(0, 0, 0);

                if (gravity.Count > 1)
                    nodeResult.Force = nodeResult.Force + new Vector(0, 0, (double)node.CustomData["Mass"] * gravity[Int32.Parse(node.Name)]);
                else
                    nodeResult.Force = nodeResult.Force + new BHoM.Geometry.Vector(0, 0, (double)node.CustomData["Mass"] * gravity[0]);

                nodalResultCollection.Add(node.Name + ":" + t.ToString(), nodeResult);
                //  this.NodalResultCollection.Add(nodeResult, this.t);
            }

            foreach (Bar bar in this.Bars)
            {
                FormFinding.BarForce barForce = barForceCollection[bar.Name + ":" + t.ToString()];
                nodalResultCollection[bar.StartNode.Name + ":" + t.ToString()].Force = nodalResultCollection[bar.StartNode.Name + ":" + t.ToString()].Force + new Vector(barForce.FX, barForce.FY, barForce.FZ);
                nodalResultCollection[bar.EndNode.Name + ":" + t.ToString()].Force = nodalResultCollection[bar.EndNode.Name + ":" + t.ToString()].Force + new Vector(-barForce.FX, -barForce.FY, -barForce.FZ);
            }
        }

        public void SetLockedToZero()
        {
            foreach (Node node in this.Nodes)
                if ((bool)node.CustomData["isLocked"])
                    nodalResultCollection[node.Name + ":" + t.ToString()].Force = new Vector(0, 0, 0);
        }

        public void CalcAcceleration()
        {
            foreach (Node node in this.Nodes)
                nodalResultCollection[node.Name + ":" + t.ToString()].Acceleration = nodalResultCollection[node.Name + ":" + t.ToString()].Force / (double)node.CustomData["Mass"];
        }

        public void CalcVelocity()
        {
            foreach (Node node in this.Nodes)
                nodalResultCollection[node.Name + ":" + t.ToString()].Velocity = c * nodalResultCollection[node.Name + ":" + (t - dt).ToString()].Velocity + nodalResultCollection[node.Name + ":" + t.ToString()].Acceleration * (dt);
        }
        public void CalcTranslation()
        {
            foreach (Node node in this.Nodes)
                nodalResultCollection[node.Name + ":" + t.ToString()].Translation = nodalResultCollection[node.Name + ":" + t.ToString()].Velocity * dt;
        }

        public void UpdateGeometry()
        {
            foreach (Node node in this.Nodes)
                node.Point += nodalResultCollection[node.Name + ":" + t.ToString()].Translation;

            foreach (Bar bar in Bars) //Make bar update line and length
            {
                Node newStartNode = bar.StartNode;
                bar.StartNode = newStartNode;
                Node newEndNode = bar.EndNode;
                bar.EndNode = newEndNode;
            }

        }

        public void CalcKineticEnergy()
        {
            foreach (Node node in this.Nodes)
                node.CustomData["KineticEnergy"] = Math.Pow(nodalResultCollection[node.Name + ":" + t.ToString()].Velocity.Length, 2) * (double)node.CustomData["Mass"] / 2.0;
        }

        public bool HasConverged()
        {
            bool hasConverged = true;
            foreach (Node node in this.Nodes)
                if ((double)node.CustomData["KineticEnergy"] > this.treshold)
                    hasConverged = false;

            return hasConverged;
        }

        public int IndexOfLargest(List<double> values)
        {
            int maxIndex = -1;
            double maxValue = 0;
            int index = 0;

            foreach (double val in values)
            {
                if (val > maxValue)
                {
                    maxValue = val;
                    maxIndex = index;
                }
                index++;
            }

            return maxIndex;
        }

        public int IndexOfSmallest(List<double> values)
        {
            int minIndex = -1;
            double minValue = 10000000;
            int index = 0;

            foreach (double val in values)
            {
                if (val < minValue)
                {
                    minValue = val;
                    minIndex = index;
                }
                index++;
            }

            return minIndex;
        }






        /** Find the node with the smallest mass
        *
        */
        public Node FindLightestNode()
        {
            double smallestMass = 1000000000;
            int index = 0;
            foreach (Node node in Nodes)
            {
                if ((double)node.CustomData["Mass"] < smallestMass)
                {
                    smallestMass = (double)node.CustomData["Mass"];
                    index = Nodes.IndexOf(node);
                }
            }
            return Nodes[index];
        }

        /** Find the bar with the greatest stiffness
        *
        */

        public Bar FindStiffestBar()
        {
            if (Bars.Count == 0) return null;

            double stiffest = 0;
            double barStiffness;
            int index = 0;
            foreach (BHoM.Structural.Bar bar in Bars)
            {
                barStiffness = (double)bar.CustomData["Stiffness"] * (double)bar.CustomData["StiffnessGlobalScaleFactor"] * (double)bar.CustomData["StiffnessCustomScaleFactor"];
                if (barStiffness > stiffest)
                {
                    stiffest = barStiffness;
                    index = Bars.IndexOf(bar);
                }
            }
            return Bars[index];
        }



        /** Loops throught all mNodes and calculates the weight of all bars connected to the node.
        *
        */
        public void SetNodalMassesPerUnitLength(double massPerUnitLength)
        {
            foreach (BHoM.Structural.Node node in Nodes)
            {
                node.CustomData.Add("SumLength", 0);
            }

                foreach (Bar bar in Bars)
            {
                bar.StartNode.CustomData["SumLength"] = (double)bar.StartNode.CustomData["SumLength"] + massPerUnitLength * bar.Length / 2;
                bar.EndNode.CustomData["SumLength"] = (double)bar.StartNode.CustomData["SumLength"] + massPerUnitLength * bar.Length / 2;
            }
        }

        /// <summary>
        ///  Calculate the total average length of bars in the model
        /// </summary>
        /// <returns></returns>
        public double CalculateMeanBarLength()
        {
            double avrgLength = 0;
            foreach (Bar bar in Bars)
            {
                avrgLength += bar.Length;
            }
            avrgLength /= Bars.Count;
            return avrgLength;
        }

        public double CalculateMeanVelocity()
        {
            double mAvrgVelocity;
            // double[] mAvrgVelocities = new double[3];
            Vector mAvrgVelocities = new Vector();

            mAvrgVelocity = 0;

            foreach (BHoM.Structural.Node node in Nodes)
            {
                //mAvrgVelocities[0] += node.Velocity[0];
                //mAvrgVelocities[1] += node.Velocity[1];
                //mAvrgVelocities[2] += node.Velocity[2];
                //mAvrgVelocity += Math.Sqrt(node.Velocity[0] * node.Velocity[0] + node.Velocity[1] * node.Velocity[1] + node.Velocity[2] * node.Velocity[2]);
                mAvrgVelocities = mAvrgVelocities + nodalResultCollection[node.Name + ":" + t.ToString()].Velocity;
                mAvrgVelocity += nodalResultCollection[node.Name + ":" + t.ToString()].Velocity.Length;

            }
            //mAvrgVelocities[0] = mAvrgVelocities[0] / _vertices.Count;
            //mAvrgVelocities[1] = mAvrgVelocities[1] / _vertices.Count;
            //mAvrgVelocities[2] = mAvrgVelocities[2] / _vertices.Count;
            //why? does it do anything?
            mAvrgVelocities = mAvrgVelocities / Nodes.Count;

            return mAvrgVelocity / Nodes.Count;

        }




    }
}

