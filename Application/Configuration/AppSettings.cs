using System.Collections.Generic;

namespace Application.Configuration
{
    public class AppSettings
    {
        private readonly Dictionary<string, string> _settings;

        public AppSettings(Dictionary<string, string> settings)
        {
            _settings = settings;
        }

        public string this[string key]
        {
            get
            {
                if (_settings.TryGetValue(key, out var value))
                {
                    return value;
                }
                return null;
            }
        }
    }
}
