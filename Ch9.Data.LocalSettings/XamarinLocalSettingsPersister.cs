using System.Collections.Generic;
using System.Threading.Tasks;
using Ch9.Data.Contracts;
using Xamarin.Forms;

namespace Ch9.Data.LocalSettings
{
    public class XamarinLocalSettingsPersister : IPersistLocalSettings
    {
        public IDictionary<string, object> PropertiesDictionary => Application.Current.Properties;
        public async Task SavePropertiesAsync()
        {
            await Application.Current.SavePropertiesAsync();
        }
    }
}
