using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ch9
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage2 : ContentPage
    {
        public MainPage2ViewModel ViewModel
        {
            get => BindingContext as MainPage2ViewModel;
            set => BindingContext = value;
        }

        public MainPage2()
        {
            InitializeComponent();
        }
    }

    // returns whether or not the text length is smaller than the integer parameter
    public class TextLengthIsGreaterThanIntegerValueToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string txt = value as string;
            if (txt == null)
                return true;
            
            return txt.Length < (int)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {throw new NotImplementedException();}
    }
}