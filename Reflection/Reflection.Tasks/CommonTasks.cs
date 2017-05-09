using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reflection.Tasks
{
    public static class CommonTasks
    {

        /// <summary>
        /// Returns the lists of public and obsolete classes for specified assembly.
        /// Please take attention: classes (not interfaces, not structs)
        /// </summary>
        /// <param name="assemblyName">name of assembly</param>
        /// <returns>List of public but obsolete classes</returns>
        public static IEnumerable<string> GetPublicObsoleteClasses(string assemblyName)
        {
            var assembly=Assembly.Load(assemblyName);

            return assembly.GetTypes().Where(type => type.IsClass && type.IsPublic && type.IsDefined(typeof(ObsoleteAttribute), false))
                .Select(x => x.Name);
        }

        /// <summary>
        /// Returns the value for required property path
        /// </summary>
        /// <example>
        ///  1) 
        ///  string value = instance.GetPropertyValue("Property1")
        ///  The result should be equal to invoking statically
        ///  string value = instance.Property1;
        ///  2) 
        ///  string name = instance.GetPropertyValue("Property1.Property2.FirstName")
        ///  The result should be equal to invoking statically
        ///  string name = instance.Property1.Property2.FirstName;
        /// </example>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="obj">source object to get property from</param>
        /// <param name="propertyPath">dot-separated property path</param>
        /// <returns>property value of obj for required propertyPath</returns>
        public static T GetPropertyValue<T>(this object obj, string propertyPath)
        {
          var paths = propertyPath.Split('.');
 
           foreach(var path in paths)
          {
              obj = obj.GetSinglePropertyVal(path);
          }
          return (T)obj;
        }

        private static object GetSinglePropertyVal(this object obj,string singlePath)
        {
            var property = obj.GetType().GetProperty(singlePath);
            return property.GetValue(obj, null);
        }


        /// <summary>
        /// Assign the value to the required property path
        /// </summary>
        /// <example>
        ///  1)
        ///  instance.SetPropertyValue("Property1", value);
        ///  The result should be equal to invoking statically
        ///  instance.Property1 = value;
        ///  2)
        ///  instance.SetPropertyValue("Property1.Property2.FirstName", value);
        ///  The result should be equal to invoking statically
        ///  instance.Property1.Property2.FirstName = value;
        /// </example>
        /// <param name="obj">source object to set property to</param>
        /// <param name="propertyPath">dot-separated property path</param>
        /// <param name="value">assigned value</param>
        public static void SetPropertyValue(this object obj, string propertyPath, object value)
        {
            var paths=propertyPath.Split('.');

            for(int i=0;i<paths.Length-1;i++)
            {
                obj = obj.GetSinglePropertyVal(paths[i]);
            }
            var property = obj.GetType().BaseType.GetProperty(paths.Last());
            property.SetValue(obj, value, null);
        }
    }
}
