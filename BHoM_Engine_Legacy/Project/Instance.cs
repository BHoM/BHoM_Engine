using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using BH.oM.Base;
using BH.oM.Structural;
using BH.oM.Structural.Elements;
using BH.oM.Database;

namespace BH.oM.Project
{
    public struct Task
    {
        public Task(BHoMObject bhomObject, string property, string value)
        {
            BhomObject = bhomObject;
            Property = property;
            Value = value;
        }

        public BHoMObject BhomObject;
        public string Property;
        public string Value;
    }

    /// <summary>
    /// A global project class that encapsulates all objects (all disciplines) of a BHoM project
    /// </summary>
    
    public class Instance
    {
        private Dictionary<Guid, BH.oM.Base.BHoMObject> m_Objects;
        private Queue<Task> m_TaskQueue;
        private Dictionary<DatabaseType, IDataAdapter> m_Databases;

        private static readonly Lazy<Instance> m_Instance = new Lazy<Instance>(() => new Instance());
        
        /// <summary>
        /// All object currently in the model
        /// </summary>
        public IEnumerable<BHoMObject> Objects
        {
            get
            {
                return m_Objects.Values;
            }
        }

        /// <summary>Structure name</summary>
        public string Name { get; set; }

        /// <summary>Tolerance of structure for node merge etc</summary>
        public double Tolerance { get; private set; }

        /// <summary>
        /// Project Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Active project
        /// </summary>
        public static Instance Active
        {
            get
            {
                return m_Instance.Value;
            }
        }

        public Config Config { get; set; }

        /// <summary>
        /// Constructs an empty instance
        /// </summary>
        public Instance()
        {
            m_Objects = new Dictionary<Guid, BHoMObject>();
            m_TaskQueue = new Queue<Task>();
            m_Databases = new Dictionary<DatabaseType, IDataAdapter>();
            Config = new Config();
        }

        /// <summary>Returns a BHoM by unique identifier</summary>
        public BH.oM.Base.BHoMObject GetObject(Guid id)
        {
            BH.oM.Base.BHoMObject result = null;
            m_Objects.TryGetValue(id, out result);
            return result;
        }

        /// <summary>Returns a BHoM by unique identifier</summary>
        public BH.oM.Base.BHoMObject GetObject(string id)
        {
            BH.oM.Base.BHoMObject result = null;
            Guid guid = new Guid();
            if (Guid.TryParse(id, out guid))
                m_Objects.TryGetValue(guid, out result);
            return result;
        }
         
        /// <summary>
        /// Adds a BHoM Object to the project
        /// </summary>
        /// <param name="value"></param>
        public void AddObject(BH.oM.Base.BHoMObject value)
        {
            if (m_Objects.ContainsKey(value.BHoM_Guid))
                return;

            m_Objects.Add(value.BHoM_Guid, value);
        }

        /// <summary>
        /// Adds a BHoM Object to the project
        /// </summary>
        /// <param name="values"></param>
        public void AddObjects(IEnumerable<BH.oM.Base.BHoMObject> values)
        {
           foreach (BHoMObject b in values)
            {
                AddObject(b);
            }
        }

        /// <summary>
        /// Removes an object from the project
        /// </summary>
        /// <param name="guid"></param>
        public void RemoveObject(Guid guid)
        {
            m_Objects.Remove(guid);
        }

        /// <summary>
        /// Removes all objects from the project
        /// </summary>
        public void Clear()
        {
            m_Objects.Clear();
        }

        internal void AddTask(Task task)
        {
            m_TaskQueue.Enqueue(task);
        }

        public void RunTasks()
        {
            while(m_TaskQueue.Count > 0)
            {
                Task t = m_TaskQueue.Dequeue();
                //BHoMJSON.ReadProperty(t.BhomObject, t.Property, t.Value, this);               
            }
        }

        public IEnumerable<string> GetTaskValues()
        {
            return m_TaskQueue.Select(x => x.Value);
        }

        public IDataAdapter GetDatabase<T>(DatabaseType dbType) where T : IDataRow
        {
            IDataAdapter result = null;
            if (!m_Databases.TryGetValue(dbType, out result))
            {
                //result = new JsonFileDB<T>(dbType);
                //m_Databases.Add(dbType, result);
            }
            return result;
        }
    }
}
