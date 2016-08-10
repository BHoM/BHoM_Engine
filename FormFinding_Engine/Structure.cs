using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Structural.Elements;
using BHoM.Structural.Results;
using BHoM.Structural.Properties;
using BHoM.Structural.Loads;


namespace FormFinding_Engine
{
    public class Structure
    {
        public List<Bar> Bars = new List<Bar>();
        public List<Node> Nodes = new List<Node>();

        public Dictionary<string, NodalResult> nodalResultCollection;
        public Dictionary<string, BarForce> barForceCollection;

        public Dictionary<string, double> kineticEnergy;

        public Loadcase loadcase;

        public double t = 0;
        public double dt;
        public double c;

        public double nodeTol = 0.1;
        public double treshold;

        public double safeTimeStep;
        public double maxTimeStep;

        public Structure(List<Bar> bars, List<Node> lockedNodes)
        {
            //Fix - Creating new bars and nodes to not change position of input BHoM objects.
            //Fix - Forcing node names to not mix up when some nodes are added as restrained and some from bars.

            int nodeID = 1;
            foreach (Node node in lockedNodes)
            {
                Node newNode = new Node(node.Point);
                newNode.Name = nodeID.ToString();
                newNode.CustomData.Add("Mass", 1.0);

                newNode.SetConstraint(new NodeConstraint("Pin", new bool[6] { true, true, true, false, false, false }, new double[6] { -1, -1, -1, 0, 0, 0 }));
                Nodes.Add(newNode);
                nodeID++;
            }

            foreach (Bar bar in bars)
            {
                Node startNode;
                Node endNode;

                if (NodeExists(bar.StartNode) != null)
                    startNode = NodeExists(bar.StartNode);
                else
                {
                    startNode = new Node(bar.StartNode.Point, nodeID.ToString());
                    nodeID++;
                    Nodes.Add(startNode);
                }

                if (NodeExists(bar.EndNode) != null)
                    endNode = NodeExists(bar.EndNode);
                else
                {
                    endNode = new Node(bar.EndNode.Point, nodeID.ToString());
                    nodeID++;
                    Nodes.Add(endNode);
                }

                Bar newBar = new Bar(startNode,endNode);
                newBar.CustomData = bar.CustomData;
                newBar.Material = bar.Material;
                newBar.Name = bar.Name;
                newBar.OrientationAngle = bar.OrientationAngle;
                newBar.Release = bar.Release;
                newBar.SectionProperty = bar.SectionProperty;
                newBar.Spring = bar.Spring;
                
                Bars.Add(newBar);
            }




            for (int i = 0; i < Bars.Count; i++)
            {
                Bars[i].Name = i.ToString();
                Bars[i].CustomData.Clear();

                if (NodeExists(Bars[i].StartNode) != null)
                    Bars[i].StartNode = NodeExists(Bars[i].StartNode);
                else
                {
                    Bars[i].StartNode.Name = Nodes.Count.ToString();
                    Nodes.Add(Bars[i].StartNode);
                }

                if (NodeExists(Bars[i].EndNode) != null)
                    Bars[i].EndNode = NodeExists(Bars[i].EndNode);
                else
                {
                    Bars[i].EndNode.Name = Nodes.Count.ToString();
                    Nodes.Add(Bars[i].EndNode);
                }
            }

            loadcase = new Loadcase(1, "Loadcase", LoadNature.Dead, 0);
            nodalResultCollection = new Dictionary<string, NodalResult>();
            barForceCollection = new Dictionary<string, BarForce>();
            kineticEnergy = new Dictionary<string, double>();

    }

        private Node NodeExists(Node node)
        {
            Node existingNode = null;
            foreach (Node structureNode in this.Nodes)
            {
                double d = structureNode.Point.DistanceTo(node.Point);
                if (d < this.nodeTol)
                    existingNode = structureNode;
            }
            return existingNode;
        }

        public void SetBarData(List<double> areas, List<double> prestresses)
        {
            foreach (Bar bar in Bars)
                bar.CustomData.Clear();

            for (int i = 0; i < Bars.Count; i++)
            {
                Bars[i].CustomData.Add("StartLength", Bars[i].Length);

                Bars[i].CustomData.Add("Area", areas[i]);

                Bars[i].CustomData.Add("Prestress", prestresses[i]);
            }
        }

        public void SetStiffness()
        {
            foreach (Bar bar in Bars)
                bar.CustomData.Add("Ks", (bar.Material.YoungsModulus*1000000 * (double)bar.CustomData["Area"] + (double)bar.CustomData["Prestress"]) / (double)bar.CustomData["StartLength"]);
        }

        public void SetStartVelocity()
        {
            foreach (Node node in this.Nodes)
            {
                NodalResult nodeResult = new NodalResult();
                nodeResult.Velocity = new Vector(0, 0, 0);
                nodalResultCollection.Add(node.Name + ":" + t.ToString(), nodeResult);     
            }
        }

        public void SetConnectedBars()
        {
            foreach(Node node in Nodes)
                foreach (Bar bar in Bars)
                {
                    if (bar.StartNode.Point.DistanceTo(node.Point) < nodeTol)
                        node.ConnectedBars.Add(bar);
                    if (bar.EndNode.Point.DistanceTo(node.Point) < nodeTol)
                        node.ConnectedBars.Add(bar);
                }
        }

        public void SetFictionalNodeMass()
        {
            foreach (Node node in Nodes)
            {
                double S = 0;
                double g = 1;
                foreach (Bar bar in node.ConnectedBars)
                    S += bar.Material.YoungsModulus * 1000000 * (double)bar.CustomData["Area"] / (double)bar.CustomData["StartLength"] + g * Math.Abs((double)bar.CustomData["Prestress"]) / bar.Length;

                double M = dt * dt / 2 * S;
                node.CustomData["Mass"]= M;
            }
        }

        public void SetLumpedNodeMass()
        {
            foreach (Node node in Nodes)
            {
                double lumpedMass = 0;
                foreach (Bar bar in node.ConnectedBars)
                {                  
                    lumpedMass += bar.SectionProperty.MassPerMetre * bar.Length / 2;
                }

                node.CustomData["Mass"] = lumpedMass;
            }
        }

        public void CalcBarForce()
        {
            foreach (Bar bar in this.Bars)
            {
                BarForce barForce = new BarForce();
                Vector unitVec = new Vector((bar.EndNode.X - bar.StartNode.X) / bar.Length, (bar.EndNode.Y - bar.StartNode.Y) / bar.Length, (bar.EndNode.Z - bar.StartNode.Z) / bar.Length);

                double dl = bar.Length - (double)bar.CustomData["StartLength"];

                double T = (double)bar.CustomData["Prestress"] + dl * (double)bar.CustomData["Ks"];

                bar.CustomData["T"] =  T;

                barForce.FX = unitVec.X * T;
                barForce.FY = unitVec.Y * T;
                barForce.FZ = unitVec.Z * T;

                //if(bar is Cable)
                //  if(T<0) 
                //  {
                //      barForce.FX = 0;
                //      barForce.FY = 0;
                //      barForce.FZ = 0;
                //  }

            barForceCollection.Add(bar.Name + ":" + t.ToString(), barForce);
            }
        }

        public void CalcNodeForce(double unaryNodalLoad)
        {
            foreach (Node node in Nodes)
            {
               NodalResult nodeResult = new NodalResult();

                nodeResult.Force = new Vector(0, 0, 0);

                nodeResult.Force = nodeResult.Force + new BHoM.Geometry.Vector(0, 0, unaryNodalLoad);

                nodalResultCollection.Add(node.Name + ":" + t.ToString(), nodeResult);
            }

            foreach (Bar bar in Bars)
            {
                BarForce barForce = barForceCollection[bar.Name + ":" + t.ToString()];
                nodalResultCollection[bar.StartNode.Name + ":" + t.ToString()].Force = nodalResultCollection[bar.StartNode.Name + ":" + t.ToString()].Force + new Vector(barForce.FX, barForce.FY, barForce.FZ - bar.SectionProperty.MassPerMetre * (double)bar.CustomData["StartLength"] / 2);
                nodalResultCollection[bar.EndNode.Name + ":" + t.ToString()].Force = nodalResultCollection[bar.EndNode.Name + ":" + t.ToString()].Force + new Vector(-barForce.FX, -barForce.FY, -barForce.FZ - bar.SectionProperty.MassPerMetre * (double)bar.CustomData["StartLength"] / 2);
            }
        }

        public void CalcAcceleration()
        {
            foreach (Node node in this.Nodes)
                nodalResultCollection[node.Name + ":" + t.ToString()].Acceleration = nodalResultCollection[node.Name + ":" + t.ToString()].Force / (double)node.CustomData["Mass"];
        }

        public void CalcVelocity()
        {
            foreach (Node node in this.Nodes)
                nodalResultCollection[node.Name + ":" + t.ToString()].Velocity = c * nodalResultCollection[node.Name + ":" + (t - dt).ToString()].Velocity + nodalResultCollection[node.Name + ":" + t.ToString()].Acceleration * dt;
        }

        public void CheckConstraints()
        {
            foreach (Node node in this.Nodes)
                if (node.IsConstrained)
                {
                    if (node.Constraint.UX.Type == DOFType.Fixed)
                        nodalResultCollection[node.Name + ":" + t.ToString()].Velocity.X = 0;

                    if (node.Constraint.UY.Type == DOFType.Fixed)
                        nodalResultCollection[node.Name + ":" + t.ToString()].Velocity.Y = 0;

                    if (node.Constraint.UZ.Type == DOFType.Fixed)
                        nodalResultCollection[node.Name + ":" + t.ToString()].Velocity.Z = 0;

                    if (node.Constraint.RX.Type == DOFType.Fixed)
                        nodalResultCollection[node.Name + ":" + t.ToString()].AngularVelocity.X = 0;

                    if (node.Constraint.RY.Type == DOFType.Fixed)
                        nodalResultCollection[node.Name + ":" + t.ToString()].AngularVelocity.Y = 0;

                    if (node.Constraint.RZ.Type == DOFType.Fixed)
                        nodalResultCollection[node.Name + ":" + t.ToString()].AngularVelocity.Z = 0;
                }
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

            foreach (Bar bar in Bars) //Make BHoMbar update line and length when node points change instead?
            {
                Node newStartNode = bar.StartNode;
                bar.StartNode = newStartNode;
                Node newEndNode = bar.EndNode;
                bar.EndNode = newEndNode;
            }
        }

        public void CalcKineticEnergy()
        {
            double Ke = 0;
            foreach (Node node in Nodes)
                Ke += Math.Pow(nodalResultCollection[node.Name + ":" + t.ToString()].Velocity.Length, 2) * (double)node.CustomData["Mass"] / 2.0;

            kineticEnergy.Add(t.ToString(), Ke);
        }

        public void CheckKineticEnergyPeak()
        {
            if(kineticEnergy.ContainsKey((t- 2*dt).ToString()))
                if((double)kineticEnergy[(t- dt).ToString()] > (double)kineticEnergy[(t - 2*dt).ToString()] && (double)kineticEnergy[t.ToString()]<(double)kineticEnergy[(t- dt).ToString()])
                    Reinitialise();
        }

        public void Reinitialise()
        {
            foreach (Node node in Nodes)
            {
                nodalResultCollection[node.Name + ":" + t.ToString()].Translation = nodalResultCollection[node.Name + ":" + t.ToString()].Velocity * dt/2;
                nodalResultCollection[node.Name + ":" + t.ToString()].Velocity = new Vector(0, 0, 0);
            }
        }

        public bool HasConverged()
        {
            bool hasConverged = true;
            if ((double)kineticEnergy[t.ToString()] > this.treshold)
                hasConverged = false;

            return hasConverged;
        }

        public void CalcSafeTimeStep()
        {
            double smallestRatio = 100000000000;
            foreach (Node node in Nodes)
                foreach (Bar bar in node.ConnectedBars)
                    if ((double)node.CustomData["Mass"] / (double)bar.CustomData["Ks"] < smallestRatio)
                        smallestRatio = (double)node.CustomData["Mass"] / (double)bar.CustomData["Ks"];

            safeTimeStep = Math.Sqrt(2 * smallestRatio);
            dt = safeTimeStep;
        }

    }
}

