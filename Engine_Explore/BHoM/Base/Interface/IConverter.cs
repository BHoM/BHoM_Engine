using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Base
{
    public interface IConverter<T>
    {
        object Read(T data);
        T Write(object data);
    }
}
