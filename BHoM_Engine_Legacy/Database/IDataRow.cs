﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Database
{
    public interface IDataRow 
    {
        string Name { get; set; }

        string Type { get; set; }
    }
}
