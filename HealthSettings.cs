using System.Collections.Generic;
using CookieUtils;
using UnityEditor;
using UnityEngine;

namespace Cookie.HealthSystem
{
    [SettingsObject(
        "HealthSettings",
        "Health settings",
        "Cookie Utils/Health settings",
        "Health",
        "Mask",
        "Wall"
    )]
    public class HealthSettings : SettingsObject<HealthSettings>
    {
        public List<string> masks = new();
        public LayerMask wallMasks;

#if UNITY_EDITOR
        [SettingsProvider]
        private static SettingsProvider ProvideSettings() => GetSettings();
#endif
    }
}
