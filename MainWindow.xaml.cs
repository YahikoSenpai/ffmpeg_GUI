using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using VideoConverter.Localization;

namespace VideoConverter
{
    /// <summary>
    /// MainWindow is the primary user interface for the FFmpeg Video Converter application.
    /// It provides functionality to select input video files, choose an output folder, 
    /// select a target video format, and initiate batch video conversion operations.
    /// 
    /// The application supports conversion to AVI, MP4, and WebM formats with format-specific
    /// encoding parameters optimized for quality and file size.
    /// 
    /// Features:
    /// - File selection via dialog or drag-and-drop
    /// - Output folder selection
    /// - Format selection (AVI, MP4, WebM)
    /// - Video duration querying using ffprobe
    /// - Batch conversion using ffmpeg
    /// - Real-time logging of conversion progress
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Stores the selected output directory path where converted videos will be saved.
        /// Empty string indicates no output folder has been selected.
        /// </summary>
        private string outputFolder = "";

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// This is called by the XAML framework during window creation.
        /// Calls InitializeComponent() to load and initialize XAML-defined controls.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the language selection ComboBox change event.
        /// 
        /// When the user selects a different language from the LanguageBox ComboBox,
        /// this event handler updates the LocalizationService with the new language selection.
        /// The LocalizationService then raises PropertyChanged notifications that automatically
        /// update all UI text bindings throughout the application.
        /// 
        /// Language Mapping:
        /// - Index 0: Hungarian (default)
        /// - Index 1: English
        /// - Index 2: German
        /// </summary>
        /// <param name="sender">The ComboBox control that triggered this event</param>
        /// <param name="e">Selection changed event arguments</param>
        private void LanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageBox.SelectedIndex >= 0)
            {
                LocalizationService.Instance.CurrentLanguage = (LocalizationService.LanguageType)LanguageBox.SelectedIndex;
            }
        }

        /// <summary>
        /// Handles the "Add Files" button click event.
        /// Opens an OpenFileDialog allowing users to select one or multiple video files
        /// (.mp4 or .webm format) to be converted.
        /// 
        /// When files are selected, the FileList ListBox is cleared and repopulated with
        /// the full file paths of the selected files.

        /// </summary>
        /// <param name="sender">The button control that triggered this event</param>
        /// <param name="e">Routed event arguments</param>
        private void AddFiles_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "Video files|*.mp4;*.webm";

            if (dlg.ShowDialog() == true)
            {
                FileList.Items.Clear();

                foreach (var f in dlg.FileNames)
                    FileList.Items.Add(f);

            }
        }

        /// <summary>
        /// Handles the "Select Output" button click event.
        /// Opens a CommonOpenFileDialog in folder picker mode, allowing users to select
        /// the destination folder where converted videos will be saved.
        /// 
        /// Updates the OutputFolderText TextBlock and the internal outputFolder variable
        /// with the selected path.
        /// </summary>
        /// <param name="sender">The button control that triggered this event</param>
        /// <param name="e">Routed event arguments</param>
        private void SelectOutput_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                outputFolder = dlg.FileName;
                OutputFolderText.Text = outputFolder;
            }
        }

        /// <summary>
        /// Handles the "Start Conversion" button click event.
        /// Validates that both video files and an output folder are selected, then iterates
        /// through each file in FileList and performs the following for each:
        /// 1. Retrieves the selected format from FormatBox ComboBox
        /// 2. Constructs the output file path using the input filename and selected format
        /// 3. Gets the video duration using ffprobe
        /// 4. Executes FFmpeg conversion with format-specific parameters
        /// 5. Logs the conversion progress and completion
        /// 
        /// This method runs asynchronously to prevent UI freezing during video processing.
        /// Displays message boxes for validation errors (missing files or output folder).
        /// </summary>
        /// <param name="sender">The button control that triggered this event</param>
        /// <param name="e">Routed event arguments</param>
        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            if (FileList.Items.Count == 0)
            {
                MessageBox.Show(LocalizationService.Instance.NoFilesSelected);
                return;
            }

            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                MessageBox.Show(LocalizationService.Instance.NoOutputFolderSelected);
                return;
            }

            Log(LocalizationService.Instance.ConversionStarted);

            foreach (string input in FileList.Items)
            {
                string selectedFormat = ((ComboBoxItem)FormatBox.SelectedItem).Content.ToString();

                string output = Path.Combine(
                    outputFolder,
                    Path.GetFileNameWithoutExtension(input) + "." + selectedFormat
                );

                Log($"{LocalizationService.Instance.Converting} {input}");

                double duration = await GetDuration(input);
                await RunFfmpeg(input, output, selectedFormat, duration);

                Log($"{LocalizationService.Instance.Completed} {output}");
            }

            Log(LocalizationService.Instance.ConversionCompleted);

            /// <summary>
            /// Display a localized completion alert to notify the user that all conversions finished successfully.
            /// The message box uses the current language setting from LocalizationService.
            /// </summary>
            MessageBox.Show(
                LocalizationService.Instance.CompletionAlertMessage,
                LocalizationService.Instance.CompletionAlertTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// Asynchronously retrieves the duration of a video file using ffprobe.exe.
        /// 
        /// Executes ffprobe with specific arguments to extract only the duration value
        /// in seconds from the video file. The process runs without a window and captures
        /// standard output for parsing.
        /// 
        /// The ffprobe command used:
        /// ffprobe -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1
        /// 
        /// This returns only the numeric duration value (e.g., "123.45" for 123.45 seconds).
        /// </summary>
        /// <param name="input">Full file path to the video file</param>
        /// <returns>
        /// The duration of the video in seconds as a double value.
        /// Returns 0 if the duration cannot be determined or parsing fails.
        /// </returns>
        private async Task<double> GetDuration(string input)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "ffprobe.exe",
                Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{input}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var proc = Process.Start(psi);
            string output = await proc.StandardOutput.ReadToEndAsync();
            proc.WaitForExit();

            if (double.TryParse(output, out double duration))
                return duration;

            return 0;
        }

        /// <summary>
        /// Asynchronously executes FFmpeg to convert a video file to the specified format.
        /// 
        /// This method:
        /// 1. Builds format-specific encoding arguments using BuildArguments()
        /// 2. Creates and starts an ffmpeg process with redirected error output
        /// 3. Reads error stream output line-by-line (FFmpeg writes progress to stderr)
        /// 4. Extracts progress information from FFmpeg output (time= timestamps)
        /// 5. Updates the progress bar based on current time vs. video duration
        /// 6. Logs each line of output for user feedback
        /// 7. Waits asynchronously for the process to complete
        /// 
        /// The process runs without displaying a window and does not use the shell.
        /// Progress information is extracted from FFmpeg messages that contain "time=HH:MM:SS.mm"
        /// and converted to a percentage for the progress bar.
        /// </summary>
        /// <param name="input">Full file path to the input video file</param>
        /// <param name="output">Full file path where the converted video will be saved</param>
        /// <param name="format">Target format: "avi", "mp4", or "webm"</param>
        /// <param name="duration">Duration of the video in seconds (used for progress calculation)</param>
        /// <returns>A Task that completes when the FFmpeg process finishes</returns>
        private async Task RunFfmpeg(string input, string output, string format, double duration)
        {
            string args = BuildArguments(input, output, format);

            var psi = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var proc = Process.Start(psi);

            while (!proc.StandardError.EndOfStream)
            {
                string line = await proc.StandardError.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    Log(line);
                }
            }

            await proc.WaitForExitAsync();
        }

        /// <summary>
        /// Builds the FFmpeg command-line arguments appropriate for the specified video format.
        /// 
        /// Each format uses specific video and audio codec parameters optimized for the target use case:
        /// 
        /// AVI (MPEG-4 Part 2):
        ///   - Video codec: libxvid with quality scale 3 (highest quality, largest file)
        ///   - Audio codec: MP3
        ///   
        /// MP4 (H.264):
        ///   - Video codec: libx264 with preset "medium" (balance of quality/speed)
        ///   - Quality: CRF 23 (Constant Rate Factor, default quality)
        ///   - Audio codec: AAC at 192 kbps
        ///   
        /// WebM (VP9):
        ///   - Video codec: libvpx-vp9 with constant quality mode (CRF 32)
        ///   - Variable bitrate (b:v 0 = auto bitrate)
        ///   - Audio codec: Opus (modern, efficient codec)
        /// 
        /// All output paths are quoted to handle spaces and special characters.
        /// </summary>
        /// <param name="input">Full file path to the input video file</param>
        /// <param name="output">Full file path where the converted video will be saved</param>
        /// <param name="format">Target format: "avi", "mp4", or "webm"</param>
        /// <returns>Complete FFmpeg command-line argument string</returns>
        /// <exception cref="Exception">Thrown if an unknown format is specified</exception>
        private string BuildArguments(string input, string output, string format)
        {
            switch (format)
            {
                case "avi":
                    return $"-i \"{input}\" -c:v libxvid -qscale:v 3 -c:a mp3 \"{output}\"";

                case "mp4":
                    return $"-i \"{input}\" -c:v libx264 -preset medium -crf 23 -c:a aac -b:a 192k \"{output}\"";

                case "webm":
                    return $"-i \"{input}\" -c:v libvpx-vp9 -b:v 0 -crf 32 -c:a libopus \"{output}\"";

                default:
                    throw new Exception(LocalizationService.Instance.UnknownFormat);
            }
        }

        /// <summary>
        /// Appends a message to the log display with a timestamp.
        /// 
        /// This method uses Dispatcher.Invoke() to ensure thread-safe UI updates since
        /// FFmpeg processes may write output from background threads. The log message is
        /// appended to the LogBox TextBox, and the view is scrolled to the end to show
        /// the most recent message.
        /// </summary>
        /// <param name="msg">The message text to append to the log</param>
        private void Log(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                LogBox.AppendText(msg + Environment.NewLine);
                LogBox.ScrollToEnd();
            });
        }

        /// <summary>
        /// Handles the DragOver event for file drag-and-drop operations.
        /// 
        /// Sets the drag effect to Copy to indicate that files can be dropped into
        /// the FileList ListBox. This visual feedback (usually a "+" cursor) informs users
        /// that dropping files is a valid operation.
        /// </summary>
        /// <param name="sender">The ListBox control that triggered this event</param>
        /// <param name="e">Drag event arguments containing the drag operation information</param>
        private void FileList_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        /// <summary>
        /// Handles the Drop event for file drag-and-drop operations.
        /// 
        /// When files are dropped onto the FileList ListBox:
        /// 1. Checks if the dropped data contains file paths
        /// 2. Clears the existing FileList
        /// 3. Filters for only valid video formats (.mp4 or .webm)
        /// 4. Adds the filtered files to the FileList for conversion
        /// 
        /// This prevents invalid file types from being added to the conversion queue.
        /// </summary>
        /// <param name="sender">The ListBox control that triggered this event</param>
        /// <param name="e">Drag event arguments containing the file paths from the drop operation</param>
        private void FileList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                FileList.Items.Clear();

                foreach (var f in files)
                {
                    if (f.EndsWith(".mp4") || f.EndsWith(".webm"))
                        FileList.Items.Add(f);
                }

            }
        }
    }
}
