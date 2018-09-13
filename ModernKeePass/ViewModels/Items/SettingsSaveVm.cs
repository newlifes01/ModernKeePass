using ModernKeePass.Interfaces;
using ModernKeePass.Services;

namespace ModernKeePass.ViewModels
{
    public class SettingsSaveVm
    {
        private readonly ISettingsService _settings;

        public SettingsSaveVm() : this(SettingsService.Instance)
        { }

        public SettingsSaveVm(ISettingsService settings)
        {
            _settings = settings;
        }

        public bool IsSaveSuspend
        {
            get => _settings.GetSetting("SaveSuspend", true);
            set => _settings.PutSetting("SaveSuspend", value);
        }
    }
}