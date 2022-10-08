using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TestListBinding
{
    public static class Extensions
    {

        public static void SetPropertyValue(this object obj, string propertyPath, object? value)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));

            var props = propertyPath.Split('.');

            object? target = obj;
            PropertyInfo? info;

            for (int i = 0; i < props.Length - 1; i++)
            {
                var prop = props[i];
                var type = target.GetType();
                info = type.GetProperty(prop);

                if (info is not null)
                {
                    target = info.GetValue(target);
                    if (target is null)
                    {
                        throw new InvalidOperationException($"Object {prop} in path {propertyPath} was null.");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Property '{prop}' was not found for path {propertyPath}.");
                }
            }
            if (target is not null)
            {
                info = target.GetType().GetProperty(props[^1]);
                if (info is null)
                {
                    throw new InvalidOperationException($"Property '{props[^1]}' was not found for path {propertyPath}.");
                }
                info.SetValue(target, value);
            }
        }

        public static object? GetPropertyValue(this object obj, string propertyPath)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));

            var props = propertyPath.Split('.');
            object? value = obj;
            for(int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var type = value.GetType();
                var propInfo = type.GetProperty(prop);
                if (propInfo is not null)
                {
                    var temp = propInfo.GetValue(value);
                    if (temp is null)
                    {
                        value = null;
                        break;
                    }
                    else
                    {
                        value = temp;
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Property '{prop}' was not found for path {propertyPath}.");
                }
            }
            return value;
        }

        public static void AddPropertyChanged(this DependencyProperty property, DependencyObject obj, EventHandler eventHandler)
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty(property, obj.GetType());
            descriptor.AddValueChanged(obj, eventHandler);
        }

    }
}
