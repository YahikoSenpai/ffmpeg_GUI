using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoConverter.Localization
{
    /// <summary>
    /// Provides localization services for the VideoConverter application.
    /// Supports Hungarian, English, and German languages.
    /// 
    /// This singleton service manages:
    /// - Current language selection
    /// - All translatable strings across the application
    /// - Change notifications to update UI when language changes
    /// 
    /// USAGE:
    /// LocalizationService.Instance.CurrentLanguage = LanguageType.English;
    /// string buttonText = LocalizationService.Instance.GetString("AddFiles");
    /// </summary>
    public class LocalizationService : INotifyPropertyChanged
    {
        /// <summary>
        /// Singleton instance of the LocalizationService.
        /// Access through LocalizationService.Instance
        /// </summary>
        public static LocalizationService Instance { get; } = new LocalizationService();

        /// <summary>
        /// Enumeration of supported languages in the application.
        /// </summary>
        public enum LanguageType
        {
            /// <summary>Hungarian language</summary>
            Hungarian = 0,
            /// <summary>English language</summary>
            English = 1,
            /// <summary>German language</summary>
            German = 2
        }

        private LanguageType _currentLanguage = LanguageType.Hungarian;

        /// <summary>
        /// Gets or sets the current language.
        /// Setting this property triggers PropertyChanged notification to update all UI bindings.
        /// </summary>
        public LanguageType CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AddFiles));
                    OnPropertyChanged(nameof(SelectOutput));
                    OnPropertyChanged(nameof(Format));
                    OnPropertyChanged(nameof(StartConversion));
                    OnPropertyChanged(nameof(NoFilesSelected));
                    OnPropertyChanged(nameof(NoOutputFolderSelected));
                    OnPropertyChanged(nameof(ConversionStarted));
                    OnPropertyChanged(nameof(ConversionCompleted));
                    OnPropertyChanged(nameof(Converting));
                    OnPropertyChanged(nameof(Completed));
                    OnPropertyChanged(nameof(LanguageLabel));
                    OnPropertyChanged(nameof(CompletionAlertTitle));
                    OnPropertyChanged(nameof(CompletionAlertMessage));
                    OnPropertyChanged(nameof(UnknownFormat));
                }
            }
        }

        /// <summary>
        /// Supported language names for display in the language selector combo box.
        /// </summary>
        public ObservableCollection<string> SupportedLanguages { get; } = new()
        {
            "Hungarian",
            "English",
            "German"
        };

        /// <summary>
        /// Gets the localized string for "Add Files" button text.
        /// </summary>
        public string AddFiles => GetString("AddFiles");

        /// <summary>
        /// Gets the localized string for "Select Output Folder" button text.
        /// </summary>
        public string SelectOutput => GetString("SelectOutput");

        /// <summary>
        /// Gets the localized string for "Format" label text.
        /// </summary>
        public string Format => GetString("Format");

        /// <summary>
        /// Gets the localized string for "Start Conversion" button text.
        /// </summary>
        public string StartConversion => GetString("StartConversion");

        /// <summary>
        /// Gets the localized string for "No files selected" error message.
        /// </summary>
        public string NoFilesSelected => GetString("NoFilesSelected");

        /// <summary>
        /// Gets the localized string for "No output folder selected" error message.
        /// </summary>
        public string NoOutputFolderSelected => GetString("NoOutputFolderSelected");

        /// <summary>
        /// Gets the localized string for "Conversion started" message.
        /// </summary>
        public string ConversionStarted => GetString("ConversionStarted");

        /// <summary>
        /// Gets the localized string for "Conversion completed" message.
        /// </summary>
        public string ConversionCompleted => GetString("ConversionCompleted");

        /// <summary>
        /// Gets the localized string for "Converting:" message.
        /// </summary>
        public string Converting => GetString("Converting");

        /// <summary>
        /// Gets the localized string for "Completed:" message.
        /// </summary>
        public string Completed => GetString("Completed");

        /// <summary>
        /// Gets the localized string for "Language" label.
        /// </summary>
        public string LanguageLabel => GetString("Language");

        /// <summary>
        /// Gets the localized string for completion alert title/caption.
        /// </summary>
        public string CompletionAlertTitle => GetString("CompletionAlertTitle");

        /// <summary>
        /// Gets the localized string for completion alert message.
        /// </summary>
        public string CompletionAlertMessage => GetString("CompletionAlertMessage");

        /// <summary>
        /// Gets the localized string for unknown format error.
        /// </summary>
        public string UnknownFormat => GetString("UnknownFormat");

        /// <summary>
        /// Retrieves the localized string for the given key based on the current language.
        /// 
        /// This method is the core localization engine. It looks up the key in language-specific
        /// string dictionaries and returns the appropriate translation.
        /// </summary>
        /// <param name="key">The string key to translate</param>
        /// <returns>The translated string for the current language, or the key itself if not found</returns>
        public string GetString(string key)
        {
            var translations = _currentLanguage switch
            {
                LanguageType.Hungarian => HungarianStrings,
                LanguageType.English => EnglishStrings,
                LanguageType.German => GermanStrings,
                _ => HungarianStrings
            };

            return translations.TryGetValue(key, out var value) ? value : key;
        }

        /// <summary>
        /// Dictionary containing all Hungarian language strings.
        /// Key: String identifier
        /// Value: Hungarian translation
        /// </summary>
        private static readonly Dictionary<string, string> HungarianStrings = new()
        {
            { "AddFiles", "Fájlok hozzáadása" },
            { "SelectOutput", "Kimeneti mappa" },
            { "Format", "Formátum:" },
            { "StartConversion", "Konvertálás indítása" },
            { "NoFilesSelected", "Nincs kiválasztott fájl." },
            { "NoOutputFolderSelected", "Nincs kimeneti mappa kiválasztva." },
            { "ConversionStarted", "Konvertálás indítása..." },
            { "ConversionCompleted", "Minden konverzió befejezõdött." },
            { "Converting", "Konvertálás:" },
            { "Completed", "Kész:" },
            { "Language", "Nyelv:" },
            { "CompletionAlertTitle", "Konvertálás kész" },
            { "CompletionAlertMessage", "A videó konvertálása sikeresen befejezõdött!" },
            { "UnknownFormat", "Ismeretlen formátum." }
        };

        /// <summary>
        /// Dictionary containing all English language strings.
        /// Key: String identifier
        /// Value: English translation
        /// </summary>
        private static readonly Dictionary<string, string> EnglishStrings = new()
        {
            { "AddFiles", "Add Files" },
            { "SelectOutput", "Output Folder" },
            { "Format", "Format:" },
            { "StartConversion", "Start Conversion" },
            { "NoFilesSelected", "No files selected." },
            { "NoOutputFolderSelected", "No output folder selected." },
            { "ConversionStarted", "Starting conversion..." },
            { "ConversionCompleted", "All conversions completed." },
            { "Converting", "Converting:" },
            { "Completed", "Completed:" },
            { "Language", "Language:" },
            { "CompletionAlertTitle", "Conversion Complete" },
            { "CompletionAlertMessage", "Video conversion has finished successfully!" },
            { "UnknownFormat", "Unknown format." }
        };

        /// <summary>
        /// Dictionary containing all German language strings.
        /// Key: String identifier
        /// Value: German translation
        /// </summary>
        private static readonly Dictionary<string, string> GermanStrings = new()
        {
            { "AddFiles", "Dateien hinzufügen" },
            { "SelectOutput", "Ausgabeordner" },
            { "Format", "Format:" },
            { "StartConversion", "Konvertierung starten" },
            { "NoFilesSelected", "Keine Dateien ausgewählt." },
            { "NoOutputFolderSelected", "Kein Ausgabeordner ausgewählt." },
            { "ConversionStarted", "Konvertierung wird gestartet..." },
            { "ConversionCompleted", "Alle Konvertierungen abgeschlossen." },
            { "Converting", "Konvertierung:" },
            { "Completed", "Abgeschlossen:" },
            { "Language", "Sprache:" },
            { "CompletionAlertTitle", "Konvertierung abgeschlossen" },
            { "CompletionAlertMessage", "Die Videokonvertierung wurde erfolgreich abgeschlossen!" },
            { "UnknownFormat", "Unbekanntes Format." }
        };

        /// <summary>
        /// Raises the PropertyChanged event to notify UI bindings of property changes.
        /// Used by the INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Protected method to raise PropertyChanged events safely.
        /// Uses CallerMemberName to automatically capture the calling property name.
        /// </summary>
        /// <param name="propertyName">Auto-captured name of the calling property</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
