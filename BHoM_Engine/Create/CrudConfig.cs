using BH.oM.Base;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Reflection;
using BH.oM.Base.CRUD;

namespace BH.Engine.Base
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CrudConfig GenericPushConfig(PushActionType actionType = PushActionType.Replace) 
        {
            return new CrudConfig
            {
                PushActionType = actionType
            };
        }

        /***************************************************/
    }
}
