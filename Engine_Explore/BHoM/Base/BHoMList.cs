﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Base
{
    public class BHoMList<T> : BHoMObject
    {
        public List<T> Objects { get; set; }
    }
}
