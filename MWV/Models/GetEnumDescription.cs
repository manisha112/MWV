using System;
using System.Collections.Generic;
//using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Web;
using System.ComponentModel;

namespace MWV.Models
{
    public class GetEnumDescription
    {
        
            public static T GetValueFromDescription<T>(string description)
            {
                var type = typeof(T);
                if (!type.IsEnum) throw new InvalidOperationException();
                foreach (var field in type.GetFields())
                {
                    var attribute = Attribute.GetCustomAttribute(field,
                        typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attribute != null)
                    {
                        //if (attribute.Description == description)
                        if (attribute.Match(description))
                            return (T)field.GetValue(null);
                    }
                    else
                    {
                        if (field.Name == description)
                            return (T)field.GetValue(null);
                    }
                }
                //throw new ArgumentException("Not found.", "description");
                return default(T);
            }

            public static string GetDescription(object enumValue, string defDesc)
            {
                FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

                if (null != fi)
                {
                    object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                        return ((DescriptionAttribute)attrs[0]).Description;
                }

                return defDesc;
            }
        
    }
}