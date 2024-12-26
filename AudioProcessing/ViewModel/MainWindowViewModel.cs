using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Xaml.Behaviors;
using TagLib.Mpeg;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace AudioProcessing.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {

        [ObservableProperty]
        public ObservableCollection<AudioFile> audioFiles;

        [ObservableProperty]
        public ObservableCollection<AudioFile> selectedAudioFiles;

        [ObservableProperty]
        public AudioFile selectedAudioFile;

        [ObservableProperty]
        public double progressValue = 0;
        [ObservableProperty]
        public string progressTextBlockValue;

        [ObservableProperty]
        public Visibility progressTextBlockVisibility = Visibility.Hidden;

        private List<string> tempFolderPaths = new List<string>();

        private string tempPropertyValue;

        public RelayCommand OpenFolderCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand<ListView> SelectAllCommand { get; }
        public RelayCommand<TextBlock> ApplyToAllSelectedCommand { get; }
        public RelayCommand<ListView> SelectNothingCommand { get; }
        public RelayCommand<IList> ListViewSelectionChangedCommand { get; }
        public RelayCommand<CheckBox> ColumnCheckBoxClickCommand { get; }
        public RelayCommand<CheckBox> CheckBoxClickCommand { get; }
        public RelayCommand<TextBox> TextChangedCommand { get; }
        public RelayCommand<DataGrid> OtherPropertiesComboBoxSelectedCommand { get; }

        public MainWindowViewModel()
        {
            OpenFolderCommand = new RelayCommand(BtnOpenFolder);
            OpenFileCommand = new RelayCommand(BtnOpenFile);
            SelectAllCommand = new RelayCommand<ListView>(SelectAll);
            ApplyToAllSelectedCommand = new RelayCommand<TextBlock>(ApplyToAllSelectedAsync);
            SelectNothingCommand = new RelayCommand<ListView>(SelectNothing);
            ListViewSelectionChangedCommand = new RelayCommand<IList>(ListViewSelectionChanged);
            ColumnCheckBoxClickCommand = new RelayCommand<CheckBox>(ColumnCheckBoxClick);
            CheckBoxClickCommand = new RelayCommand<CheckBox>(CheckBoxClick);
            TextChangedCommand = new RelayCommand<TextBox>(TextChanged);
            OtherPropertiesComboBoxSelectedCommand = new RelayCommand<DataGrid>(OtherPropertiesComboBoxSelected);


            SelectedAudioFiles = new ObservableCollection<AudioFile>();
            AudioFiles = new ObservableCollection<AudioFile>();
        }

        private void OtherPropertiesComboBoxSelected(DataGrid dataGrid)
        {
            foreach (var item in dataGrid.Items)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    if (presenter != null)
                    {
                        DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(0) as DataGridCell;
                        if (cell != null)
                        {
                            CheckBox checkBox = cell.Content as CheckBox;
                            if (checkBox != null)
                            {

                            }
                        }

                        // 获取第四列的 ComboBox
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(3) as DataGridCell;
                        if (cell != null)
                        {
                            ComboBox fourthColumnComboBox = cell.Content as ComboBox;
                            if (fourthColumnComboBox != null)
                            {

                            }
                        }
                    }
                }
            }
        }

        private T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject visual = VisualTreeHelper.GetChild(parent, i);
                child = visual as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(visual);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }


        private void BtnOpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav;*.flac|All Files|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {

                tempFolderPaths.Clear();
                tempFolderPaths = openFileDialog.FileNames.ToList();
                AudioFiles.Clear();
                LoadAudioFiles(tempFolderPaths);
                //OpenFile(tempFolderPaths);
            }

        }

        private void BtnOpenFolder()
        {
            // 使用 CommonOpenFileDialog 选择文件夹
            using (var folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.IsFolderPicker = true;
                if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {

                    var audioFiles = Directory.GetFiles(folderDialog.FileName, "*.*", SearchOption.AllDirectories)
                                           .Where(file => file.EndsWith(".mp3") || file.EndsWith(".wav") || file.EndsWith(".flac")).ToList();


                    tempFolderPaths.Clear();
                    tempFolderPaths = audioFiles;
                    AudioFiles.Clear();
                    LoadAudioFiles(audioFiles);
                    //OpenFolder(tempFolderPaths);
                }
            }

        }

        private void TextChanged(TextBox box)
        {
            tempPropertyValue = box.Text;
        }

        private void CheckBoxClick(CheckBox box)
        {
            var audioProperty = box.DataContext as AudioProperty;
            if (box.IsChecked != true)
            {
                foreach (var selectedAudioFile in SelectedAudioFiles)
                {
                    if (selectedAudioFile.Properties.Any(p => p.PropertyName == audioProperty.PropertyName))
                    {
                        selectedAudioFile.Properties.First(p => p.PropertyName == audioProperty.PropertyName).IsSelected = false;
                    }
                }
            }
            else
            {
                foreach (var item in SelectedAudioFiles)
                {

                    if (item.Properties.Any(p => p.PropertyName == audioProperty.PropertyName))
                    {
                        item.Properties.First(p => p.PropertyName == audioProperty.PropertyName).IsSelected = true;
                    }
                }

            }
        }

        private void ColumnCheckBoxClick(CheckBox box)
        {
            if (box.IsChecked != true)
            {
                foreach (var item in SelectedAudioFile.Properties)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in SelectedAudioFile.Properties)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void SelectNothing(ListView listView)
        {
            // 获取所有项的容器（ListViewItem）
            for (int i = 0; i < listView.Items.Count; i++)
            {
                ListViewItem listViewItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(i);
                if (listViewItem != null)
                {
                    // 你可以在这里对 listViewItem 进行操作，例如选择它
                    listViewItem.IsSelected = false;
                }
            }
        }

        private void ListViewSelectionChanged(IList list)
        {
            SelectedAudioFiles.Clear();
            foreach (var item in list)
            {
                var audioFile = item as AudioFile;
                if (audioFile != null && !SelectedAudioFiles.Contains(audioFile))
                {
                    audioFile.IsSelected = true;
                    SelectedAudioFiles.Add(audioFile);
                }
            }
        }

        private void SelectAll(ListView listView)
        {
            // 获取所有项的容器（ListViewItem）
            for (int i = 0; i < listView.Items.Count; i++)
            {
                ListViewItem listViewItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(i);
                if (listViewItem != null)
                {
                    // 你可以在这里对 listViewItem 进行操作，例如选择它
                    listViewItem.IsSelected = true;
                }
            }
        }

        private async void ApplyToAllSelectedAsync(TextBlock textBlock)
        {
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            ProgressTextBlockVisibility = Visibility.Visible;
            //});

            ProgressValue = 0;
            double totalFiles = SelectedAudioFiles.Count;

            await Task.Run(async () =>
            {
                double processedFiles = 0;
                foreach (var file in SelectedAudioFiles)
                {
                    try
                    {
                        using (var tagFile = TagLib.File.Create(file.FilePath))
                        {
                            // 获取当前属性
                            var currentProperties = file.Properties.ToDictionary(prop => prop.PropertyName, prop => prop.PropertyValue);

                            // 更新属性
                            foreach (var property in file.Properties)
                            {
                                if (property.IsReadOnly || property.IsSelected != true) // 检查属性是否为只读
                                {
                                    continue; // 跳过只读属性
                                }
                                tempPropertyValue = SelectedAudioFile.Properties.Where(p => p.PropertyName == property.PropertyName).FirstOrDefault().PropertyValue;
                                string tempValue = file.Properties.Where(p => p.SelectedOtherPropertyName == property.SelectedOtherPropertyName).FirstOrDefault().PropertyValue;
                                if (tempValue != "无")
                                {
                                    property.PropertyValue = tempValue;

                                    // 更新 TagLib 文件的属性
                                    SetPropertyValue(tagFile, property, tempValue);
                                }
                                else if (!string.IsNullOrEmpty(tempPropertyValue))
                                {
                                    SetPropertyValue(tagFile, property, tempPropertyValue);
                                }
                            }

                            // 保存修改后的属性到文件中
                            tagFile.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        // 处理异常
                        file.Properties.Add(new AudioProperty { PropertyName = "错误", PropertyValue = ex.Message, IsReadOnly = true });
                    }

                    processedFiles++;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProgressValue = Math.Round((processedFiles / (double)totalFiles) * 100, 2);
                        ProgressTextBlockValue = ProgressValue + "%";
                    });

                }
            });

            ReFreshData();

            MessageBox.Show("更新完成");
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            ProgressTextBlockVisibility = Visibility.Hidden;
            ProgressValue = 0;
            //});
        }

        private void ReFreshData()
        {
            AudioFiles.Clear();
            LoadAudioFiles(tempFolderPaths);
            SelectedAudioFile = AudioFiles.FirstOrDefault();
        }

        //在修改文件属性后，刷星数据源

        private void SetPropertyValue(TagLib.File tagFile, AudioProperty property, string selectedPropertyValue)
        {
            switch (property.PropertyName)
            {
                case "艺术家":
                    tagFile.Tag.Performers = new[] { selectedPropertyValue };
                    break;
                case "专辑":
                    tagFile.Tag.Album = selectedPropertyValue;
                    break;
                case "曲目编号":
                    if (int.TryParse(selectedPropertyValue, out int track))
                    {
                        tagFile.Tag.Track = (uint)track;
                    }
                    break;
                case "发行日期":
                    if (int.TryParse(selectedPropertyValue, out int year))
                    {
                        tagFile.Tag.Year = (uint)year;
                    }
                    break;
                case "流派":
                    tagFile.Tag.Genres = new[] { selectedPropertyValue };
                    break;
                case "版权信息":
                    tagFile.Tag.Copyright = selectedPropertyValue;
                    break;
                case "歌词":
                    tagFile.Tag.Lyrics = selectedPropertyValue;
                    break;
                case "评论":
                    tagFile.Tag.Comment = selectedPropertyValue;
                    break;
                case "标题":
                    tagFile.Tag.Title = selectedPropertyValue;
                    break;
                    // 其他属性可以类似地处理
            }
        }

        private void LoadAudioFiles(List<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                try
                {
                    using (var tagFile = TagLib.File.Create(filePath))
                    {
                        var properties = LoadAudioProperties(filePath);

                        // 创建 AudioFile 对象并添加到 AudioFiles 列表
                        var audioFile = new AudioFile
                        {
                            FileName = Path.GetFileName(filePath),
                            FilePath = filePath,
                            Properties = properties
                        };

                        AudioFiles.Add(audioFile);
                    }
                }
                catch (Exception ex)
                {
                    // 处理异常
                    Console.WriteLine($"无法加载文件 {filePath}: {ex.Message}");
                }
            }
        }

        private ObservableCollection<AudioProperty> LoadAudioProperties(string fileName)
        {
            var properties = new ObservableCollection<AudioProperty>();

            try
            {
                using (var tagFile = TagLib.File.Create(fileName))
                {
                    // 添加其他属性
                    properties.Add(new AudioProperty { PropertyName = "名称", PropertyValue = Path.GetFileNameWithoutExtension(fileName) });
                    properties.Add(new AudioProperty { PropertyName = "时长", PropertyValue = tagFile.Properties.Duration.ToString(@"mm\:ss"), IsReadOnly = true });
                    properties.Add(new AudioProperty { PropertyName = "比特率", PropertyValue = tagFile.Properties.AudioBitrate.ToString() + " kbps", IsReadOnly = true });
                    properties.Add(new AudioProperty { PropertyName = "采样率", PropertyValue = tagFile.Properties.AudioSampleRate.ToString() + " Hz", IsReadOnly = true });
                    properties.Add(new AudioProperty { PropertyName = "通道数", PropertyValue = tagFile.Properties.AudioChannels.ToString(), IsReadOnly = true });
                    properties.Add(new AudioProperty { PropertyName = "编码格式", PropertyValue = tagFile.MimeType, IsReadOnly = true }); // 使用 MimeType 获取编码格式
                    properties.Add(new AudioProperty { PropertyName = "艺术家", PropertyValue = string.Join(", ", tagFile.Tag.Performers) });
                    properties.Add(new AudioProperty { PropertyName = "专辑", PropertyValue = tagFile.Tag.Album });
                    properties.Add(new AudioProperty { PropertyName = "曲目编号", PropertyValue = tagFile.Tag.Track.ToString() });
                    properties.Add(new AudioProperty { PropertyName = "发行日期", PropertyValue = tagFile.Tag.Year.ToString() });
                    properties.Add(new AudioProperty { PropertyName = "流派", PropertyValue = string.Join(", ", tagFile.Tag.Genres) });
                    properties.Add(new AudioProperty { PropertyName = "版权信息", PropertyValue = tagFile.Tag.Copyright });
                    properties.Add(new AudioProperty { PropertyName = "歌词", PropertyValue = tagFile.Tag.Lyrics });
                    properties.Add(new AudioProperty { PropertyName = "评论", PropertyValue = string.Join(", ", tagFile.Tag.Comment) });
                    properties.Add(new AudioProperty { PropertyName = "标题", PropertyValue = tagFile.Tag.Title });
                    // 填充 OtherProperties
                    foreach (var property in properties)
                    {

                        property.OtherProperties = properties.Where(p => p.PropertyName != property.PropertyName).Select(p => p.PropertyName).ToList();
                        property.OtherProperties.Insert(0, "无");

                    }
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"无法加载文件 {fileName}: {ex.Message}");
            }

            return properties;
        }

    }

    public partial class AudioFile : ObservableObject
    {

        [ObservableProperty]
        public string? fileName;


        [ObservableProperty]
        public ObservableCollection<AudioProperty> properties = new ObservableCollection<AudioProperty>();


        [ObservableProperty]
        public bool isSelected;

        [ObservableProperty]
        public string? filePath;

    }

    public partial class AudioProperty : ObservableObject
    {
        [ObservableProperty]
        public bool isSelected = false;

        [ObservableProperty]
        public string? propertyName;

        [ObservableProperty]
        public string? propertyValue;

        [ObservableProperty]
        public bool isReadOnly;

        [ObservableProperty]
        public List<string> otherProperties = new List<string>();

        [ObservableProperty]
        public string? selectedOtherPropertyName;


    }

}
