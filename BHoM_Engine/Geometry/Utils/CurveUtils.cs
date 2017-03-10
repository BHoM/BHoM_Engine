using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BHoM.Geometry
{
    public static class CurveUtils
    {
        #region Static Functions

        public static List<Curve> Join(Group<Curve> curves)
        {
            return Join(curves.ToList());
        }
        public static List<Curve> Join(List<Curve> curves)
        {
            List<Curve> result = new List<Curve>();
            for (int i = 0; i < curves.Count; i++)
            {
                result.Add(curves[i]);
            }
            int counter = 0;
            int dimensions = result.Count > 0 ? result[0].Dimensions : 0;
            while (counter < result.Count)
            {
                double[] cps1 = result[counter].ControlPointVector;

                for (int j = counter + 1; j < result.Count; j++)
                {
                    double[] cps2 = result[j].ControlPointVector;
                    if (ArrayUtils.Equal(cps1, cps1.Length - dimensions - 1, cps2, 0, dimensions, 0.0001))
                    {
                        result[j] = result[counter].Append(result[j]);
                        result.RemoveAt(counter--);
                        break;
                    }
                    else if (ArrayUtils.Equal(cps1, 0, cps2, cps2.Length - dimensions - 1, dimensions, 0.0001))
                    {
                        result[j] = result[j].Append(result[counter]);
                        result.RemoveAt(counter--);
                        break;
                    }
                    else if (ArrayUtils.Equal(cps1, 0, cps2, 0, dimensions, 0.0001))
                    {
                        result[j] = result[counter].Flip().Append(result[j]);
                        result.RemoveAt(counter--);
                        break;
                    }
                    else if (ArrayUtils.Equal(cps1, cps1.Length - dimensions - 1, cps2, cps2.Length - dimensions - 1, dimensions, 0.0001))
                    {
                        result[j] = result[counter].Append(result[j].Flip());
                        result.RemoveAt(counter--);
                        break;
                    }
                }
                counter++;
            }
            return result;
        }

        #endregion
    }
}
