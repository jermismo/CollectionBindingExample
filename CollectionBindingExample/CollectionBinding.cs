using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace TestListBinding
{
    /// <summary>
    /// Exposes a Dependency Property for use in Binding in classes that don't inherit DependencyObject
    /// </summary>
    public class BindingValue : DependencyObject
    {
        public object? Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(BindingValue), new PropertyMetadata(null));
    }

    /// <summary>
    /// Binds the value of a property from a collection of items. Will return null if the
    /// items don't all share the same value. The collection property path needs to be on the DataContext
    /// for it to be found.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public class CollectionBinding : MarkupExtension
    {
        private WeakReference<IEnumerable>? _collection;
        private Binding? binding;
        private BindingExpressionBase? bindingExpression;
        private BindingValue boundValue;
        private bool areAllEqual;
        private Type? sourceType;
        private bool isReadingValue = false;

        protected IEnumerable? Collection
        {
            get
            {
                if (_collection is not null)
                {
                    if (_collection.TryGetTarget(out var obj))
                    {
                        return obj;
                    }
                }
                return null;
            }
            set
            {
                if (value is null)
                {
                    _collection = null;
                }
                else
                {
                    _collection = new WeakReference<IEnumerable>(value);
                }
            }
        }

        public BindingValue BoundValue
        {
            get => boundValue;
        }

        /// <summary>
        /// The property path to the collection on the target's DataContext.
        /// </summary>
        public string? CollectionPath { get; set; }

        /// <summary>
        /// The property path for the value to bind to on the collection items.
        /// </summary>
        public PropertyPath? ValuePath { get; set; }

        // Normal binding properties
        public IValueConverter? Converter { get; set; }
        public object? ConverterParamter { get; set; }
        public int Delay { get; set; }
        public bool ValidatesOnDataErrors { get; set; }
        public bool ValidatesOnExceptions { get; set; }       
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public CultureInfo? ConverterCulture { get; set; }
        public BindingMode? Mode { get; set; }
        public UpdateSourceTrigger? UpdateSourceTrigger { get; set; }

        public CollectionBinding()
            : base()
        {
            boundValue = new BindingValue();
            BindingValue.ValueProperty.AddPropertyChanged(boundValue, BoundValue_ValueChanged);
        }

        public CollectionBinding(string path)
            : this()
        {
            ValuePath = new PropertyPath(path);
        }

        private void BoundValue_ValueChanged(object? sender, EventArgs e)
        {
            UpdateListItems();
        }

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            var valueProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (valueProvider != null)
            {
                var bindingTarget = valueProvider.TargetObject as FrameworkElement;
                var bindingProperty = valueProvider.TargetProperty as DependencyProperty;
                if (bindingProperty == null || bindingTarget == null)
                {
                    throw new NotSupportedException(string.Format(
                        "The property '{0}' on target '{1}' is not valid for a CollectionBinding. The CollectionBinding target must be a FrameworkElement, "
                        + "and the target property must be a DependencyProperty.",
                        valueProvider.TargetProperty,
                        valueProvider.TargetObject));
                }

                if (CollectionPath is null)
                {
                    throw new InvalidOperationException("CollectionBinding expected Source to be set, but it was null.");
                }
                if (ValuePath is null)
                {
                    throw new InvalidOperationException("CollectionBinding expected a property path.");
                }

                // try to find the collection
                var context = bindingTarget.DataContext;
                if (context is null)
                {
                    // throwing an exception here would be cool... 
                    // but it makes the designer complain
                    return null;
                }

                var enumerable = context.GetPropertyValue(CollectionPath) as IEnumerable;
                if (enumerable is null)
                {
                    throw new InvalidOperationException($"CollectionBinding expected an IEnumerable on the data context at {context.GetType().Name}.{CollectionPath}.");
                }

                // get collection
                Collection = enumerable;
                if (enumerable is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged += NotifyCollection_CollectionChanged;
                }

                SetBoundValue();

                binding = new Binding();
                binding.Path = new PropertyPath(nameof(BoundValue.Value));
                binding.Source = BoundValue;
                binding.ValidatesOnDataErrors = ValidatesOnDataErrors;
                binding.ValidatesOnExceptions = ValidatesOnExceptions;
                binding.Delay = Delay;
                binding.Mode = Mode ?? BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger ?? System.Windows.Data.UpdateSourceTrigger.Default;

                binding.Converter = Converter;
                binding.ConverterCulture = ConverterCulture;
                binding.ConverterParameter = ConverterParamter;
                
                bindingExpression = BindingOperations.SetBinding(bindingTarget, bindingProperty, binding);

                return BoundValue.Value;
            }
            return null;
        }

        private void NotifyCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            SetBoundValue();
        }

        private void UpdateListItems()
        {
            if (ValuePath is null) throw new ArgumentNullException(nameof(ValuePath));
            if (isReadingValue) return; // internal update

            IEnumerable? coll = Collection;
            if (coll is null) return;

            // if the values were not all equal, then the bound value will be null
            // in this case we don't want to update the values
            if (boundValue.Value is null && !areAllEqual) return;

            object? value = boundValue.Value;

            if (value is not null)
            {
                if (sourceType != value.GetType())
                {
                    if (Converter is not null)
                    {
                        value = Converter.ConvertBack(value, bindingExpression?.TargetProperty.PropertyType, ConverterParamter, ConverterCulture);
                    }
                    else if (sourceType is not null)
                    {
                        // try to convert ourselves
                        var tc = TypeDescriptor.GetConverter(sourceType);
                        value = tc.ConvertFrom(value);
                    }
                }
            }
            else if (sourceType?.IsValueType == true)
            {
                value = Activator.CreateInstance(sourceType);
            }

            foreach(var item in coll)
            {
                item.SetPropertyValue(ValuePath.Path, value);
            }
        }

        private void SetBoundValue()
        {
            if (ValuePath is null) throw new ArgumentNullException(nameof(ValuePath));

            object? value = null;
            IEnumerable? coll = Collection;

            if (coll is null) return;

            areAllEqual = true;
            bool first = true;

            foreach (var item in coll)
            {
                var val = item.GetPropertyValue(ValuePath.Path);
                // just load the value the first time
                if (first)
                {
                    sourceType = val?.GetType();
                    value = val;
                    first = false;
                    continue;
                }
                
                if (!value?.Equals(val) == true)
                {
                    value = null;
                    areAllEqual = false;
                    break;
                }
            }

            isReadingValue = true;
            boundValue.Value = value;
            bindingExpression?.UpdateTarget();
            isReadingValue = false;
        }

    }
}
