using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Tesseract;
using System.Drawing;
using System.Windows.Forms;
using SDImageFormat = System.Drawing.Imaging.ImageFormat;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace OCRApplication;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private bool _isProcessing;
    private string? _currentImagePath;
    private readonly string _tessdataPath;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            _isProcessing = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsProcessing)));
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        
        // Initialize Tesseract data path - check multiple locations
        var possiblePaths = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"),
            Path.Combine(Directory.GetCurrentDirectory(), "tessdata"),
            Path.Combine(Environment.CurrentDirectory, "tessdata"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "tessdata")
        };

        _tessdataPath = possiblePaths.FirstOrDefault(Directory.Exists) 
            ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

        EnsureTessdataExists();
        LanguageComboBox.SelectedIndex = 0;
    }

    private void EnsureTessdataExists()
    {
        try
        {
            if (!Directory.Exists(_tessdataPath))
            {
                Directory.CreateDirectory(_tessdataPath);
            }

            // Check if language files exist
            var rusFile = Path.Combine(_tessdataPath, "rus.traineddata");
            var engFile = Path.Combine(_tessdataPath, "eng.traineddata");

            if (!File.Exists(rusFile) || !File.Exists(engFile))
            {
                var message = $"Языковые файлы не найдены в директории:\n{_tessdataPath}\n\n" +
                             "Пожалуйста, убедитесь что файлы rus.traineddata и eng.traineddata находятся в этой папке.\n\n" +
                             "Вы можете скачать их по следующим ссылкам:\n" +
                             "rus.traineddata: https://github.com/tesseract-ocr/tessdata/raw/main/rus.traineddata\n" +
                             "eng.traineddata: https://github.com/tesseract-ocr/tessdata/raw/main/eng.traineddata";

                MessageBox.Show(
                    message,
                    "Отсутствуют языковые файлы",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка при проверке директории tessdata:\n{ex.Message}",
                "Ошибка Tessdata",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    private async void BtnSelectImage_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp, *.tiff)|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|All files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            await LoadImage(openFileDialog.FileName);
        }
    }

    private async void BtnScreenshot_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
        await Task.Delay(500); // Give time for window to minimize

        try
        {
            using (var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                }

                var tempPath = Path.Combine(Path.GetTempPath(), $"screenshot_{DateTime.Now:yyyyMMddHHmmss}.png");
                bitmap.Save(tempPath, SDImageFormat.Png);
                await LoadImage(tempPath);
            }
        }
        finally
        {
            this.WindowState = WindowState.Normal;
        }
    }

    private async Task LoadImage(string imagePath)
    {
        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            ImageDisplay.Source = bitmap;
            _currentImagePath = imagePath;
            DropHintText.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnRecognize_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentImagePath))
        {
            MessageBox.Show("Please select an image first.", "No Image", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsProcessing = true;
        try
        {
            var language = LanguageComboBox.SelectedIndex == 0 ? "rus" : "eng";
            var result = await RecognizeText(_currentImagePath, language);
            OutputTextBox.Text = result;
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error during recognition:\n\n{ex.Message}";
            if (ex is TesseractException)
            {
                errorMessage += $"\n\nTessdata path: {_tessdataPath}";
                var files = Directory.Exists(_tessdataPath) 
                    ? string.Join("\n", Directory.GetFiles(_tessdataPath, "*.traineddata"))
                    : "Directory not found";
                errorMessage += $"\n\nAvailable language files:\n{files}";
            }
            MessageBox.Show(errorMessage, "Recognition Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task<string> RecognizeText(string imagePath, string language)
    {
        // Verify tessdata directory
        if (!Directory.Exists(_tessdataPath))
        {
            throw new DirectoryNotFoundException($"Директория tessdata не найдена: {_tessdataPath}");
        }

        // Verify language file
        var languageFile = Path.Combine(_tessdataPath, $"{language}.traineddata");
        if (!File.Exists(languageFile))
        {
            throw new FileNotFoundException(
                $"Языковой файл не найден: {languageFile}\n" +
                "Пожалуйста, скачайте необходимый файл и поместите его в папку tessdata."
            );
        }

        return await Task.Run(() =>
        {
            try
            {
                using var engine = new TesseractEngine(_tessdataPath, language, EngineMode.Default);
                using var img = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(img);
                var text = page.GetText();
                
                // Если текст пустой, возможно проблема с распознаванием
                if (string.IsNullOrWhiteSpace(text))
                {
                    return "Текст не распознан. Пожалуйста, убедитесь что:\n" +
                           "1. Изображение содержит четкий текст\n" +
                           "2. Выбран правильный язык распознавания\n" +
                           "3. Изображение хорошего качества";
                }
                
                return text;
            }
            catch (TesseractException tex)
            {
                throw new Exception(
                    $"Ошибка Tesseract: {tex.Message}\n" +
                    $"Путь к tessdata: {_tessdataPath}\n" +
                    "Убедитесь, что языковые файлы корректно установлены.", 
                    tex
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка обработки OCR: {ex.Message}", ex);
            }
        });
    }

    private void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(OutputTextBox.Text))
        {
            System.Windows.Clipboard.SetText(OutputTextBox.Text);
            MessageBox.Show("Text copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        OutputTextBox.Clear();
        ImageDisplay.Source = null;
        _currentImagePath = null;
        DropHintText.Visibility = Visibility.Visible;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(OutputTextBox.Text))
        {
            MessageBox.Show("No text to save.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = "txt"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                File.WriteAllText(saveFileDialog.FileName, OutputTextBox.Text);
                MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ImageDisplay_Drop(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (files?.Length > 0)
            {
                LoadImage(files[0]);
            }
        }
    }

    private void ImageDisplay_DragEnter(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
        {
            e.Effects = System.Windows.DragDropEffects.Copy;
        }
        else
        {
            e.Effects = System.Windows.DragDropEffects.None;
        }
        e.Handled = true;
    }
}