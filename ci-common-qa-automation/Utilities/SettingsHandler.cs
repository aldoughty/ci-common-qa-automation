using Microsoft.Extensions.Configuration;

namespace ci_common_qa_automation.Utilities
{
    public static class SettingsHandler
    {
        /// <summary>
        /// Get the setting requested from the config root and return it
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="config"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetSettingString(string settingName, IConfiguration config, string? defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(settingName))
                throw new ArgumentNullException(nameof(settingName));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(defaultValue))
            {
                return config.GetValue<string>(settingName);
            }
            else
            {
                string settingValue = config.GetValue<string>(settingName);
                return string.IsNullOrWhiteSpace(settingValue) ? defaultValue : settingValue;
            }
        }
    }
}
