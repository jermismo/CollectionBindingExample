using System.Windows;

namespace TestListBinding
{
    public class MainModel : DependencyObject
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(nameof(Name), typeof(string), typeof(MainModel));
        public static readonly DependencyProperty Value1Property = DependencyProperty.Register(nameof(Value1), typeof(int), typeof(MainModel));
        public static readonly DependencyProperty Value2Property = DependencyProperty.Register(nameof(Value2), typeof(int), typeof(MainModel));
        public static readonly DependencyProperty Value3Property = DependencyProperty.Register(nameof(Value3), typeof(int), typeof(MainModel));
        public static readonly DependencyProperty Value4Property = DependencyProperty.Register(nameof(Value4), typeof(int), typeof(MainModel));
        public static readonly DependencyProperty Value5Property = DependencyProperty.Register(nameof(Value5), typeof(int), typeof(MainModel));
        public static readonly DependencyProperty Value6Property = DependencyProperty.Register(nameof(Value6), typeof(int), typeof(MainModel));
        public static readonly DependencyProperty Value7Property = DependencyProperty.Register(nameof(Value7), typeof(int), typeof(MainModel));
        public static readonly DependencyProperty Value8Property = DependencyProperty.Register(nameof(Value8), typeof(int), typeof(MainModel));
        public static readonly DependencyProperty Value9Property = DependencyProperty.Register(nameof(Value9), typeof(int), typeof(MainModel));

        public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }
        public int Value1 { get => (int)GetValue(Value1Property); set => SetValue(Value1Property, value); }
        public int Value2 { get => (int)GetValue(Value2Property); set => SetValue(Value2Property, value); }
        public int Value3 { get => (int)GetValue(Value3Property); set => SetValue(Value3Property, value); }
        public int Value4 { get => (int)GetValue(Value4Property); set => SetValue(Value4Property, value); }
        public int Value5 { get => (int)GetValue(Value5Property); set => SetValue(Value5Property, value); }
        public int Value6 { get => (int)GetValue(Value6Property); set => SetValue(Value6Property, value); }
        public int Value7 { get => (int)GetValue(Value7Property); set => SetValue(Value7Property, value); }
        public int Value8 { get => (int)GetValue(Value8Property); set => SetValue(Value8Property, value); }
        public int Value9 { get => (int)GetValue(Value9Property); set => SetValue(Value9Property, value); }

        public override string ToString()
        {
            return Name;
        }
    }
}
