using System;

namespace Paya.Automation.Editor.Data
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using Paya.Automation.Editor.Properties;

    internal static class TypeHelper
    {
        #region Constants

        // Fields
        internal const char IndexParameterSeparator = ',';
        internal const char LeftIndexerToken = '[';
        internal const char PropertyNameSeparator = '.';
        internal const char RightIndexerToken = ']';

        #endregion

        #region Methods

        // Methods
        internal static Type GetCustomOrCLRType(this object instance)
        {
            var provider = instance as ICustomTypeProvider;
            if (provider == null)
            {
                return instance.GetType();
            }
            return (provider.GetCustomType() ?? instance.GetType());
        }

        internal static Type GetNestedPropertyType(this Type parentType, string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                return parentType;
            }
            object item = null;
            Exception exception;
            var info = parentType.GetNestedProperty(propertyPath, out exception, ref item);
            return info != null ? info.PropertyType : null;
        }

        internal static object GetNestedPropertyValue(object item, string propertyPath, Type propertyType, out Exception exception)
        {
            exception = null;
            if (item == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(propertyPath))
            {
                return item;
            }
            var obj2 = item;
            Type customOrCLRType = item.GetCustomOrCLRType();
            if (customOrCLRType != null)
            {
                PropertyInfo info = customOrCLRType.GetNestedProperty(propertyPath, out exception, ref obj2);
                if ((info != null) && (info.PropertyType != propertyType))
                {
                    return null;
                }
            }
            return obj2;
        }

        internal static Type GetNonNullableType(this Type type)
        {
            return !type.IsNullableType() ? type : type.GetGenericArguments()[0];
        }

        internal static string GetTypeName(this Type type)
        {
            var nonNullableType = type.GetNonNullableType();
            string name = nonNullableType.Name;
            if (type != nonNullableType)
            {
                name = name + '?';
            }
            return name;
        }

        internal static bool IsNullableType(this Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof (Nullable<>)));
        }

        private static string GetDefaultMemberName(this Type type)
        {
            var customAttributes = type.GetCustomAttributes(typeof (DefaultMemberAttribute), true);
            if ((customAttributes.Length == 1))
            {
                var attribute = customAttributes[0] as DefaultMemberAttribute;
                if (attribute != null) return attribute.MemberName;
            }
            return null;
        }

        private static PropertyInfo GetNestedProperty(this Type parentType, string propertyPath, out Exception exception, ref object item)
        {
            exception = null;
            var nonNullableType = parentType;
            PropertyInfo info = null;
            var list = SplitPropertyPath(propertyPath);
            foreach (string t in list)
            {
                object[] index;
                info = nonNullableType.GetPropertyOrIndexer(t, out index);
                if (info == null)
                {
                    item = null;
                    return null;
                }
                if (!info.CanRead)
                {
                    exception = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, PagedCollectionViewResources.PropertyNotReadable, t, nonNullableType.GetTypeName()));
                    item = null;
                    return null;
                }
                if (item != null)
                {
                    item = info.GetValue(item, index);
                }
                nonNullableType = info.PropertyType.GetNonNullableType();
            }
            return info;
        }

        private static PropertyInfo GetPropertyOrIndexer(this Type type, string propertyPath, out object[] index)
        {
            propertyPath = propertyPath ?? string.Empty;
            index = null;
            if (string.IsNullOrEmpty(propertyPath) || (propertyPath[0] != '['))
            {
                return type.GetProperty(propertyPath);
            }
            if ((propertyPath.Length < 3) || (propertyPath[propertyPath.Length - 1] != ']'))
            {
                return null;
            }
            var str = propertyPath.Substring(1, propertyPath.Length - 2);
            if (str.IndexOf(',') != -1)
            {
                return null;
            }
            string defaultMemberName = type.GetDefaultMemberName();
            if (string.IsNullOrEmpty(defaultMemberName))
            {
                return null;
            }
            PropertyInfo info = null;
            foreach (var info2 in type.GetProperties())
            {
                if (string.Equals(info2.Name, defaultMemberName))
                {
                    var indexParameters = info2.GetIndexParameters();
                    if ((indexParameters.Length == 1))
                    {
                        if (indexParameters[0].ParameterType == typeof (int))
                        {
                            int result;
                            if (int.TryParse(str.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out result))
                            {
                                info = info2;
                                index = new object[] {result};
                                return info;
                            }
                        }
                        if (indexParameters[0].ParameterType == typeof (string))
                        {
                            info = info2;
                            index = new object[] {str};
                        }
                    }
                }
            }
            return info;
        }

        private static List<string> SplitPropertyPath(string propertyPath)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(propertyPath))
            {
                var num = 0;
                var startIndex = 0;
                for (var i = 0; i < propertyPath.Length; i++)
                {
                    switch (propertyPath[i])
                    {
                        case '[':
                            num++;
                            break;
                        case ']':
                            num--;
                            break;
                    }
                    if ((propertyPath[i] == '.') && (num == 0))
                    {
                        list.Add(propertyPath.Substring(startIndex, i - startIndex));
                        startIndex = i + 1;
                    }
                    else if ((startIndex != i) && (propertyPath[i] == '['))
                    {
                        list.Add(propertyPath.Substring(startIndex, i - startIndex));
                        startIndex = i;
                    }
                    else if (i == (propertyPath.Length - 1))
                    {
                        list.Add(propertyPath.Substring(startIndex));
                    }
                }
            }
            return list;
        }

        #endregion
    }
}