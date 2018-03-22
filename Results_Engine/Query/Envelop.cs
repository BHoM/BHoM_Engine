//using System;
//using System.Reflection;
//using System.Collections.Generic;
//using System.Collections;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BH.oM.Structural.Results;
//using BH.oM.Common;
//using BH.Engine.Results.Objects.Envelopes;


//namespace BH.Engine.Results
//{
//    public static partial class Query
//    {
//        /***************************************************/
//        /**** Public Methods                            ****/
//        /***************************************************/

//        public static BarForce Envelop(IEnumerable<BarForce> forces, IEnvelope envelop, bool idFromFirst = false, bool caseFromFirst = false)
//        {
//            return new BarForce()
//            {
//                ObjectId = idFromFirst ? forces.First().ObjectId : "",
//                Case = caseFromFirst ? forces.First().Case : "",
//                FX = envelop.Apply(forces, (x => x.FX)),
//                FY = envelop.Apply(forces, (x => x.FX)),
//                FZ = envelop.Apply(forces, (x => x.FX)),
//                MX = envelop.Apply(forces, (x => x.FX)),
//                MY = envelop.Apply(forces, (x => x.FX)),
//                MZ = envelop.Apply(forces, (x => x.FX))
//            };
//        }

//        /***************************************************/

//        public static NodeReaction Envelop(IEnumerable<NodeReaction> forces, IEnvelope envelop, bool idFromFirst = false, bool caseFromFirst = false)
//        {

//            return new NodeReaction()
//            {
//                ObjectId = idFromFirst ? forces.First().ObjectId : "",
//                Case = caseFromFirst ? forces.First().Case : "",
//                FX = envelop.Apply(forces, (x => x.FX)),
//                FY = envelop.Apply(forces, (x => x.FX)),
//                FZ = envelop.Apply(forces, (x => x.FX)),
//                MX = envelop.Apply(forces, (x => x.FX)),
//                MY = envelop.Apply(forces, (x => x.FX)),
//                MZ = envelop.Apply(forces, (x => x.FX))
//            };
//        }

//        /***************************************************/

//        public static GlobalReactions Envelop(IEnumerable<GlobalReactions> forces, IEnvelope envelop, bool idFromFirst = false, bool caseFromFirst = false)
//        {
//            return new GlobalReactions()
//            {
//                ObjectId = idFromFirst ? forces.First().ObjectId : "",
//                Case = caseFromFirst ? forces.First().Case : "",
//                FX = envelop.Apply(forces, (x => x.FX)),
//                FY = envelop.Apply(forces, (x => x.FX)),
//                FZ = envelop.Apply(forces, (x => x.FX)),
//                MX = envelop.Apply(forces, (x => x.FX)),
//                MY = envelop.Apply(forces, (x => x.FX)),
//                MZ = envelop.Apply(forces, (x => x.FX))
//            };
//        }

//        /***************************************************/

//        public static NodeDisplacement Envelop(IEnumerable<NodeDisplacement> forces, IEnvelope envelop, bool idFromFirst = false, bool caseFromFirst = false)
//        {
//            return new NodeDisplacement()
//            {
//                ObjectId = idFromFirst ? forces.First().ObjectId : "",
//                Case = caseFromFirst ? forces.First().Case : "",
//                UX = envelop.Apply(forces, (x => x.UX)),
//                UY = envelop.Apply(forces, (x => x.UX)),
//                UZ = envelop.Apply(forces, (x => x.UX)),
//                RX = envelop.Apply(forces, (x => x.RX)),
//                RY = envelop.Apply(forces, (x => x.RX)),
//                RZ = envelop.Apply(forces, (x => x.RX))
//            };
//        }



//        /***************************************************/
//        /**** Public Methods - Interface                ****/
//        /***************************************************/

//        public static T IEnvelop<T>(IEnumerable<T> results, IEnvelope envelop, bool idFromFirst = false, bool caseFromFirst = false) where T : IResult
//        {
//            return Envelop(results.Cast<dynamic>().ToList() as dynamic, envelop, idFromFirst, caseFromFirst);
//        }


//        ///***************************************************/
//        ///**** Reflection tests                          ****/
//        ///***************************************************/


//        ///***************************************************/
//        ///**** Private methods                           ****/
//        ///***************************************************/


//        //public static T Envelop<T>(IEnumerable<T> forces, IEnvelope envelop, bool idFromFirst = false, bool caseFromFirst = false) where T : IResult
//        //{
//        //    T first = forces.First();

//        //    if (first == null)
//        //        return default(T);

//        //    Type type = first.GetType();

//        //    List<Func<T, double>> getProps;
//        //    List<Action<T, double>> setProps;

//        //    ValueProps(first, out getProps, out setProps);

//        //    //List<PropertyInfo> properties = first.GetType().GetProperties().Where(x => x.CanRead && x.CanWrite && x.GetType() == typeof(double) && x.Name != "TimeStep").ToList();

//        //    //List<Func<T, double>> getProps = properties.Select(x => (Func<T, double>)Delegate.CreateDelegate(typeof(Func<T, double>), x.GetGetMethod())).ToList();
//        //    //List<Action<T, double>> setProps = properties.Select(x => (Action<T, double>)Delegate.CreateDelegate(typeof(Action<T, double>), x.GetSetMethod())).ToList();

//        //    T res = (T)Activator.CreateInstance(type);
//        //    if (idFromFirst)
//        //        res.ObjectId = first.ObjectId;

//        //    if (caseFromFirst)
//        //        res.Case = first.Case;

//        //    for (int i = 0; i < getProps.Count; i++)
//        //    {
//        //        setProps[i](res, envelop.Apply(forces, getProps[i]));
//        //    }

//        //    return res;
//        //}

//        //private static void ValueProps<T>(T first, out List<Func<T, double>> getProps, out List<Action<T, double>> setProps) where T : IResult
//        //{
//        //    Type type = first.GetType();
//        //    IList getList, setList;

//        //    //Try to get out the property values
//        //    if (m_getValueProps.TryGetValue(type, out getList) && m_setValueProps.TryGetValue(type, out setList))
//        //    {
//        //        getProps = getList as List<Func<T, double>>;
//        //        setProps = setList as List<Action<T, double>>;

//        //        if (getProps != null && setProps != null)
//        //            return;
//        //    }

//        //    //If they was not found:
//        //    typeof(Query).GetMethod("CreateValueProps", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(new Type[] { type }).Invoke(null, new object[] { type });

//        //    getProps = m_getValueProps[type] as List<Func<T, double>>;
//        //    setProps = m_setValueProps[type] as List<Action<T, double>>;

//        //}

//        ///***************************************************/

//        //private static void CreateValueProps<T>(Type type)
//        //{
//        //    Type doubleType = typeof(double);
//        //    List<PropertyInfo> properties = type.GetProperties().Where(x => x.CanRead && x.CanWrite && x.PropertyType == doubleType && x.Name != "TimeStep").ToList();

//        //    List<Func<T, double>>  getProps = properties.Select(x => (Func<T, double>)Delegate.CreateDelegate(typeof(Func<T, double>), x.GetGetMethod())).ToList();
//        //    List<Action<T, double>> setProps = properties.Select(x => (Action<T, double>)Delegate.CreateDelegate(typeof(Action<T, double>), x.GetSetMethod())).ToList();

//        //    m_getValueProps[type] = getProps;
//        //    m_setValueProps[type] = setProps;
//        //}

//        ///***************************************************/
//        ///**** Private fields                            ****/
//        ///***************************************************/

//        //private static Dictionary<Type, IList> m_getValueProps = new Dictionary<Type, IList>();
//        //private static Dictionary<Type, IList> m_setValueProps = new Dictionary<Type, IList>();

//    }
//}
