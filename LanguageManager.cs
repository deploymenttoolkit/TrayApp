using AutoMapper;
using DeploymentToolkit.Modals;
using DeploymentToolkit.Util;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace DeploymentToolkit.TrayApp
{
	internal class LanguageManager
    {
        internal static Language Language;

        private string _languageName;

        private readonly string _twoLetterISOLanguageName;
        private readonly string _uiTwoLetterISOLanguageName;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _languagesMainPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets\\Languages");

        /// <summary>
        /// Key: Registry Path
        /// Value: Registry Value
        /// </summary>
        private readonly Dictionary<string, string> _registryLanguagePaths = new Dictionary<string, string>()
        {
            {
                @"HKLM\SOFTWARE\Policies\Microsoft\MUI\Settings",
                @"PreferredUILanguages"
            },
            {
                @"HKCU\Software\Policies\Microsoft\Windows\Control Panel\Desktop",
                @"PreferredUILanguages"
            },
            {
                @"HKCU\Control Panel\Desktop",
                @"PreferredUILanguages"
            },
            {
                @"HKCU\Control Panel\Desktop\MuiCached",
                @"MachinePreferredUILanguages"
            },
            {
                @"HKCU\Control Panel\International",
                @"LocaleName"
            }
        };

        internal LanguageManager(string forceLanguage = null)
        {
            _logger.Trace("Initializing LanguageManager ...");

            _logger.Trace("Getting culture...");
            var culture = CultureInfo.CurrentCulture;
            var uiCulture = CultureInfo.CurrentUICulture;
            _logger.Trace($"Current culture is {culture.Name}");

            _logger.Trace("Setting variables...");

            _twoLetterISOLanguageName = culture.TwoLetterISOLanguageName;
            _uiTwoLetterISOLanguageName = uiCulture.TwoLetterISOLanguageName;

            _logger.Trace("Reading prefered language from registry...");
            GetLanguageFromRegistry();

			if(!string.IsNullOrEmpty(forceLanguage))
			{
				_logger.Debug($"Preferred language was {_languageName} but is now overwritten to {forceLanguage}");
				_languageName = forceLanguage;
			}

            _logger.Trace($"Trying to use {_languageName} as language...");
            try
            {
                var language = CultureInfo.GetCultureInfo(_languageName);
                _languageName = language.TwoLetterISOLanguageName.ToUpper();

                if (_languageName == "ZH")
                {
                    if (language.EnglishName.Contains("Simplified"))
                    {
                        _languageName = "ZH-Hans";
                        _logger.Trace("Detected Chinese Simplified");
                    }
                    else if (language.EnglishName.Contains("Traditional"))
                    {
                        _languageName = "ZH-Hant";
                        _logger.Trace("Detected Chinese Traditional");
                    }
                }
                else if (_languageName == "PT")
                {
                    if (language.ThreeLetterWindowsLanguageName == "PTB")
                    {
                        _languageName = "PT-BR";
                        _logger.Trace("Detected Brazilian Portuguese");
                    }
                }
            }
            catch (CultureNotFoundException)
            {
                _logger.Warn($"Failed to find language for {_languageName}. Using EN");
                _languageName = "EN";
            }

            var filePath = Path.Combine(_languagesMainPath, $"{_languageName}.xml");
            _logger.Trace($"Searching for language ({filePath})");
            if (File.Exists(filePath))
            {
                LoadLanguageStringsFromXml(filePath);
            }
            else
            {
                _logger.Warn($"Failed to find language {_languageName}. Reverting to EN");

                _languageName = "EN";
                filePath = Path.Combine(_languagesMainPath, $"EN.xml");
                if (!File.Exists(filePath))
                    throw new FileNotFoundException(filePath);
                else
                    LoadLanguageStringsFromXml(filePath);
            }

            _logger.Trace("LanguageManager initialized");
        }

        private void LoadLanguageStringsFromXml(string filePath)
        {
            try
            {
                Language = Xml.ReadXml<Language>(filePath);

				var psadtPath = Path.Combine(Path.GetDirectoryName(filePath), $"PSADT\\UI_Messages_{_languageName}.xml");
				if(File.Exists(psadtPath))
				{
					_logger.Debug($"Loading PSADT Language file from '{psadtPath}'");

					var psadtLanguage = Xml.ReadXml<Language>(psadtPath);

					var mapper = new MapperConfiguration(cfg =>
					{
						cfg.CreateMap<Language, Language>().ForAllMembers(o => o.Condition((source, destination, value) => value != null));
					}).CreateMapper();

					Language = mapper.Map(psadtLanguage, Language);
				}

				var upgradePath = Path.Combine(Path.GetDirectoryName(filePath), $"Upgrade\\{_languageName}.rtf");
				if(!File.Exists(upgradePath))
				{
					upgradePath = Path.Combine(Path.GetDirectoryName(filePath), $"Upgrade\\EN.rtf");
					if(!File.Exists(upgradePath))
					{
						_logger.Warn("Found no language and no fallback language windows upgrade RTF file. This can be ignored if DTK is not used for Windows Upgrades");
					}
					else
					{
						Language.PathToUpgradeRtf = upgradePath;
					}
				}
				else
				{
					Language.PathToUpgradeRtf = upgradePath;
				}
				_logger.Trace($"PathToUpgradeRtf: {upgradePath}");

				_logger.Info($"Successfully loaded language files for {_languageName}");
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to read language file");
            }
        }

        private void GetLanguageFromRegistry()
        {
            foreach (var registryPair in _registryLanguagePaths)
            {
                try
                {
                    var registryRoot = Registry.CurrentUser;
                    if (registryPair.Key.StartsWith("HKLM"))
                        registryRoot = Registry.LocalMachine;

                    var path = registryPair.Key.Substring(5, registryPair.Key.Length - 5);
                    var value = registryPair.Value;

                    var registryKey = registryRoot.OpenSubKey(path);
                    if (registryKey != null)
                    {
                        var language = (string[])registryKey.GetValue(value, new string[0]);
                        if (language.Length > 0)
                        {
                            _languageName = language[0];
                            _logger.Trace($"Using {_languageName} as UI language");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, $"Failed to read from registry ({registryPair.Key}/{registryPair.Value})");
                }
            }

            var xpLanguageRoot = Registry.CurrentUser.OpenSubKey(@"Control Panel\International");
            if (xpLanguageRoot != null)
            {
                var locale = (string)xpLanguageRoot.GetValue("Locale", string.Empty);
                if (!string.IsNullOrEmpty(locale))
                {
                    var localeNumber = Convert.ToInt32($"0x{locale}", 16);
                    try
                    {
                        var language = CultureInfo.GetCultureInfo(localeNumber);
                        _languageName = language.Name;
                        return;
                    }
                    catch (CultureNotFoundException)
                    {
                        _logger.Warn($"Failed to find culture {locale}->{localeNumber}");
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        _logger.Warn($"Failed to find culture {locale}->{localeNumber}");
                    }
                }
            }

            _logger.Warn($"Failed to find any suitable UI language. Falling back to {_twoLetterISOLanguageName}");
            _languageName = _twoLetterISOLanguageName;
        }
    }
}
