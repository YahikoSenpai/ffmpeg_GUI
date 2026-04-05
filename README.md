# VideoConverter

A simple, user-friendly video conversion application built with .NET 9 and WPF. Convert videos between multiple formats (AVI, MP4, WebM) with real-time logging and multi-language support.

## ?? Features

- **Multiple Output Formats**: Convert to AVI, MP4, or WebM
- **Batch Processing**: Convert multiple videos at once
- **Multi-Language Support**: Hungarian, English, and German interfaces
- **Drag & Drop**: Convenient file selection
- **Real-Time Logging**: Monitor conversion progress with detailed FFmpeg output
- **Language Switching**: Change UI language on-the-fly
- **Completion Alerts**: Get notified when conversions finish
- **Self-Contained**: Single executable file - no dependencies except FFmpeg

## ?? Requirements

- **Windows 10/11** (x64)
- **RAM**: 4 GB minimum (8 GB recommended)
- **Disk Space**: 2 GB free space
- **FFmpeg**: Must be installed separately and added to system PATH

## ?? Installation

### Option 1: Download Pre-Built Executable
1. Download `VideoConverter.exe` from the [Releases](https://github.com/yourusername/VideoConverter/releases) page
2. Place it anywhere on your computer
3. No installation needed - just run it!

### Option 2: Build from Source
```bash
# Clone the repository
git clone https://github.com/yourusername/VideoConverter.git
cd VideoConverter

# Build the release executable
dotnet publish -c Release -o publish

# The executable will be at: publish\VideoConverter.exe
```

## ?? FFmpeg Installation

FFmpeg is required for video conversion. Choose one option:

### Windows
**Using Chocolatey** (recommended):
```bash
choco install ffmpeg
```

**Manual Installation**:
1. Download FFmpeg from [ffmpeg.org](https://ffmpeg.org/download.html)
2. Extract to a folder (e.g., `C:\ffmpeg`)
3. Add the folder to your system PATH:
   - Right-click "This PC" ? Properties ? Advanced system settings
   - Click "Environment Variables"
   - Under System variables, click "Path" ? "Edit"
   - Click "New" and add your FFmpeg path
   - Click OK and restart your computer

**Verify Installation**:
```bash
ffmpeg -version
```

## ?? Usage

1. **Launch** `VideoConverter.exe`
2. **Add Files**: Click "Add Files" or drag & drop videos
3. **Select Output Folder**: Choose where to save converted videos
4. **Choose Format**: Select AVI, MP4, or WebM
5. **Select Language**: Pick your preferred language (Hungarian, English, German)
6. **Start Conversion**: Click "Start Conversion"
7. **Monitor Progress**: Watch the log for real-time conversion details

### Supported Input Formats
- MP4
- WebM

### Output Formats & Settings

| Format | Video Codec | Audio Codec | Quality |
|--------|------------|------------|---------|
| **AVI** | MPEG-4 Part 2 (Xvid) | MP3 | High (qscale:3) |
| **MP4** | H.264 | AAC (192k) | Medium (CRF 23) |
| **WebM** | VP9 | Opus | Medium (CRF 32) |

## ?? Features in Detail

### Multi-Language Support
The application includes complete localization for:
- ???? **Hungarian** (Magyarország)
- ???? **English**
- ???? **German** (Deutsch)

Change languages instantly from the language selector in the top-right corner.

### Batch Conversion
Convert multiple videos sequentially. The application will:
1. Process each file with your selected settings
2. Log progress for each file
3. Show completion alert when all files are done

### Real-Time Logging
View detailed FFmpeg output including:
- Frame information
- Bitrate
- FPS (frames per second)
- Processing time
- Completion status

## ??? Technical Details

### Built With
- **.NET 9.0** - Latest .NET framework
- **WPF** - Windows Presentation Foundation for UI
- **WindowsAPICodePack** - For native Windows dialogs
- **FFmpeg** - For video encoding

### Architecture
- **Single File Deployment**: VideoConverter.exe includes everything
- **Self-Contained Runtime**: No external .NET installation needed
- **x64 Architecture**: Windows 64-bit systems

### Code Organization
```
VideoConverter/
??? MainWindow.xaml          # UI Layout
??? MainWindow.xaml.cs       # UI Logic & Conversion Logic
??? App.xaml                 # Application configuration
??? App.xaml.cs              # Application startup
??? Localization/
?   ??? LocalizationService.cs    # Multi-language support
??? VideoConverter.csproj    # Project configuration
```

## ?? Building from Source

### Prerequisites
- Visual Studio 2022 or later (with .NET 9 SDK)
- Or: .NET 9 SDK installed

### Build Commands

**Development Build**:
```bash
dotnet build
```

**Release Build (Single .exe)**:
```bash
dotnet publish -c Release -o publish
```

This creates `VideoConverter.exe` in the `publish` folder (~170 MB, self-contained).

## ?? Localization

The application supports three languages with complete UI localization:

### Adding New Languages

1. Edit `Localization/LocalizationService.cs`
2. Add new enum value to `LanguageType`
3. Add property for new language translations
4. Add translations dictionary:
```csharp
private static readonly Dictionary<string, string> NewLanguageStrings = new()
{
    { "AddFiles", "Translation..." },
    { "SelectOutput", "Translation..." },
    // ... add all other strings
};
```
5. Update `GetString()` method and `CurrentLanguage` property
6. Rebuild application

## ?? Troubleshooting

### "Cannot find ffmpeg.exe"
**Solution**: Ensure FFmpeg is installed and in your system PATH
```bash
# Verify FFmpeg is in PATH
ffmpeg -version
```

### Application won't start
**Solution**: Ensure you have Windows 10 or later (x64)

### Conversion fails
**Solution**: 
- Verify input video file is valid
- Check FFmpeg installation
- Try with a different video format
- Ensure output folder has write permissions

### "No files selected" error
**Solution**: Add at least one video file before starting conversion

### "No output folder selected" error
**Solution**: Click "Output Folder" and select a destination folder

## ?? License

This project is licensed under the MIT License - see the LICENSE file for details.

## ?? Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Guidelines
- Follow C# naming conventions (PascalCase for public members)
- Add XML documentation comments for public methods
- Test with multiple video formats
- Ensure all three languages work correctly

## ?? Support

If you encounter issues:

1. Check the troubleshooting section above
2. Verify FFmpeg is installed correctly
3. Check the conversion log for error messages
4. Open an issue on GitHub with:
   - Error message
   - Video file format/size
   - Windows version
   - FFmpeg version

## ?? Video Encoding Parameters

### AVI Format
```
-c:v libxvid -qscale:v 3 -c:a mp3
```
- High quality Xvid video codec
- MP3 audio codec
- Quality level 3 (highest)

### MP4 Format
```
-c:v libx264 -preset medium -crf 23 -c:a aac -b:a 192k
```
- H.264 video codec
- Medium encoding speed/quality balance
- Constant Rate Factor 23
- AAC audio at 192 kbps

### WebM Format
```
-c:v libvpx-vp9 -b:v 0 -crf 32 -c:a libopus
```
- VP9 video codec (modern, efficient)
- Constant quality mode
- Opus audio codec (high quality)

## ?? Performance

- **Conversion Speed**: Depends on video format, resolution, and CPU
  - Fast (30 seconds): Short HD videos on fast CPU
  - Normal (1-2x realtime): Typical conversions
  - Slow (0.1-0.5x realtime): 4K or very high quality settings

- **Memory Usage**: ~200-500 MB during conversion
- **Disk Space**: Output file size varies by format and resolution

## ?? Security

- No external network connections
- No telemetry or data collection
- All processing done locally
- Open source - code is fully auditable

## ?? Version History

### 1.0.0 (Current)
- Initial release
- Multi-language support (Hungarian, English, German)
- Batch video conversion
- Multiple output formats (AVI, MP4, WebM)
- Real-time logging
- Completion alerts

## ?? Acknowledgments

- **FFmpeg**: Video encoding engine
- **Windows API Code Pack**: Native Windows dialog integration
- **.NET Team**: Excellent framework

## ?? Additional Resources

- [FFmpeg Documentation](https://ffmpeg.org/documentation.html)
- [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/)

---

**Made with ?? using .NET 9 and WPF**

For questions or suggestions, please open an issue on GitHub!
