using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Base
{
    /// <summary>
    /// BHoM object abstract class, all methods and attributes applicable to all structural objects with
    /// BHoM implemented
    /// </summary>
    public class BHoMObject : IObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Guid BHoM_Guid { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = "";

        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public BHoMObject() { }


        /***************************************************/
        /**** Local Methods                             ****/
        /***************************************************/

        /// <summary>
        /// Looks for the key in the custom data dictionary
        /// </summary>
        /// <param name="key">Data key</param>
        /// <returns>The corresponding object if the key exists, null otherwise</returns>
        public object this[string key]
        {
            get
            {
                object value = null;
                CustomData.TryGetValue(key, out value);
                return value;
            }
        }

        /***************************************************/

        /// <summary>
        /// Create a shallow copy of the object
        /// </summary>
        /// <param name="newGuid">Defines if the clone needs a new Guid</param>
        /// <returns>The clone </returns>
        public BHoMObject ShallowClone(bool newGuid = false)
        {
            BHoMObject obj = (BHoMObject)this.MemberwiseClone();
            if (newGuid)
                obj.BHoM_Guid = Guid.NewGuid();
            return obj;
        }

    }
}
