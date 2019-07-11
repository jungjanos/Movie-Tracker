using Android.Content;
using Ch9.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(Ch9.Droid.ExtendedLabelRenderer))]
namespace Ch9.Droid
{
    public class ExtendedLabelRenderer : LabelRenderer
    {
        public ExtendedLabelRenderer(Context context) : base(context)
        {}

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var el = (Element as ExtendedLabel);

            if (el != null && el.JustifyText)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    Control.JustificationMode = Android.Text.JustificationMode.InterWord;
                }
            }
        }
    }
}