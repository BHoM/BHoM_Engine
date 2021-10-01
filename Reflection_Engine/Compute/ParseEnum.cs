using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Reflection
{
    public static partial class Compute
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        public static T ParseEnum<T>(string value)
        {
            return (T)ParseEnum(typeof(T), value);
        }

        /*******************************************/

        public static object ParseEnum(Type enumType, string value)
        {
            if (Enum.IsDefined(enumType, value))
                return Enum.Parse(enumType, value);
            else
            {
                return Enum.GetValues(enumType).OfType<Enum>()
                    .FirstOrDefault(x => {
                        FieldInfo fi = enumType.GetField(x.ToString());
                        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

                        return attributes != null && attributes.Count() > 0 && attributes.First().Description == value;
                    });
            }
        }

        /*******************************************/
    }
}
