using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIK.ServiceOrderMES.Util
{
    public static class Formatting
    {
        public static bool IsDate(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                DateTime dt;
                return (DateTime.TryParse(input, out dt));
            }
            else
            {
                return false;
            }
        }
        public static bool CanCovertTo(string testString, string testType)
        {
            Type type = Type.GetType(testType, null, null);
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.IsValid(testString);
        }
    }
}
