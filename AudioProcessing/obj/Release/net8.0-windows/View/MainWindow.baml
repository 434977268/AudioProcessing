   M S B A M L   `   `   `  ���� J  FAudioProcessing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:AudioProcessing.ViewModelAudioProcessing.ViewModel  %    AudioProcessing.View.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     =	ViewModel'clr-namespace:AudioProcessing.ViewModel     @d2http://schemas.microsoft.com/expression/blend/2008     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006    � Title$  
MainWindow��$	��800��$	��450��.���  � ColumnDefinitions �� $	��200 ��� $��* �.+�]� $
��True��.+��� $��打开文件夹��$	��Top=����� *OpenFolderCommand+�� $��打开文件��$	��Top=����� *OpenFileCommand+� $	��Top=� �� �� .+��� $��全选������ *SelectAllCommand+����  �� ElementName$ AudioFilesListView���� $��1��$��	全不选������ *SelectNothingCommand+���� $ AudioFilesListView���� $��应用到所有选中项��$	��Top=����� *ApplyToAllSelectedCommand+�� -    ��Name$ AudioFilesListView��$��Stretch=�$	��Top=�$
��Auto=� �� SelectionMode$ Extended=����� *
AudioFiles+ �� SelectedItem �� *SelectedAudioFile+���� .R��� *  $Microsoft.Xaml.Behaviors.Interaction   Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ SelectionChanged��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction  	   Command	 �� *!ListViewSelectionChangedCommand+
   CommandParameter
 �� $ AudioFilesListView�� �� Path$ SelectedItems����� .R���  �� Orientation$ 
Horizontal=�.+��� $��5,0,0,0q����� *
FileName+� $��1�� � RowDefinitions �� $��* ��� $
��Auto �.+�&   System.Windows.Controls.DataGrid  -     Name$ AudioPropertiesDataGrid��$��0��   AutoGenerateColumns$ False��   CanUserAddRows$ False��   CanUserDeleteRows$ False������ *SelectedAudioFile.Properties+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 28    CanUserResize$ False��   Header �� -    ��Name$ ColumnCheckBox��$��Center=�$��Center=�   $ Click��.   	 �� *(&DataContext.ColumnCheckBoxClickCommand+ �� RelativeSource ��  �� AncestorType
 Window
 �� $ ColumnCheckBox��   CellTemplate �� .R��� $ CheckBox��$��Center=�$��Center=� d� 	IsChecked �� *
IsSelected+ �� Mode$ TwoWay=�   $ Click��.   	 �� *" DataContext.CheckBoxClickCommand+ �� 
 Window
 �� $ CheckBox��0	  *System.Windows.Controls.DataGridTextColumn	  $ 属性名称��   
IsReadOnly$
 True��1
  +System.Windows.Controls.DataGridBoundColumn 
  Binding �� *PropertyName+  $ 	属性值�� �� .R���  ��Name$ PropertyValueTextBox�� �� 
IsReadOnly �� *
IsReadOnly+���� *PropertyValue+$ TwoWay=�  �� UpdateSourceTrigger$  PropertyChanged=�.��   $ TextChanged��.   	 �� * DataContext.TextChangedCommand+ �� 
 Window
 �� $ PropertyValueTextBox��.��  $ 其他属性�� �� .R��� ! ��Name$! OtherPropertiesComboBox��$	��200������ *OtherProperties+ �� *SelectedOtherPropertyName+$ TwoWay=�$  PropertyChanged=�   $ Selected��.   	 �� *42DataContext.OtherPropertiesComboBoxSelectedCommand+ �� 
 Window
 �� $ AudioPropertiesDataGrid��� -   " �Name$" ProgressBar��$��1��$��20��# � Maximum$	# 100Q�$ � Minimum$$ 0Q�% � Value% �� *ProgressValue+