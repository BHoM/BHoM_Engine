//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BH.Engine.Results.Objects.Envelopes
//{
//    public class MaxAbsoluteEnvelope : IEnvelope
//    {
//        public double Apply<T>(IEnumerable<T> source, Func<T, double> selector)
//        {
//            if (source == null) throw new ArgumentNullException("source");
//            bool first = true;
//            double absMax = 0;
//            double signedAbsMax = 0;
//            foreach (var item in source)
//            {
//                if (first)
//                {
//                    signedAbsMax = selector(item);
//                    absMax = Math.Abs(signedAbsMax);

//                    first = false;
//                }
//                else
//                {
//                    double currentVal = selector(item);
//                    double currentAbs = Math.Abs(currentVal);
//                    if (currentAbs > absMax)
//                    {
//                        absMax = currentAbs;
//                        signedAbsMax = currentVal;
//                    }
//                }
//            }
//            if (first) throw new InvalidOperationException("Sequence is empty.");
//            return signedAbsMax;
//        }
//    }
//}
