using System.Threading.Tasks;
using System.Collections.Generic;

namespace Ch9.Data.Contracts
{
    public interface IPersistLocalSettings
    {
        IDictionary<string, object> PropertiesDictionary { get; }

        Task SavePropertiesAsync();
    }
}
