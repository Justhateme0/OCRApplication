   M S B A M L   `   `   `  ����  OCRApplication    OCRApplication.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   _ [MaterialDesignThemes.Wpf, Version=4.9.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$  OCR Application��5   	   $	��700��6!   $
��1200��6.      MaterialDesignBody#���   5	   	     MaterialDesignPaper#���  5
   	   $��Regular"�5   	   $��13(�5   	   ��5   	   :  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension   � 	Resources 5        	BoolToVis��%   &	       �� 5   
   6'   5      .��5      � $��16q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6"    �� Orientation$ 
Horizontal=�62   $��0,0,0,16q�6K   .+�5      �� -   5         ��Name$ BtnSelectImage��5        MaterialDesignRaisedButton#��[ 5       $��0,0,8,0q�5"      .��5#      �� $ 
Horizontal=�6   .+�5$      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ 	ImagePlus=�6.   $��0,0,8,0q�6?   �� 5%      $+��%Выбрать изображение��6    5&      5'      �� 5)      -   5        $ BtnScreenshot��5)         MaterialDesignOutlinedButton#��[ 5*      $��0,0,8,0q�5,      .��5-      �� $ 
Horizontal=�6   .+�5.        $ Camera=�6.   $��0,0,8,0q�6<   �� 5/      $��Скриншот��6    50      51      52      �� 55   
   $��0��6   $��1��6"   $ 
Horizontal=�62   $��	16,0,0,16q�6K   .+�56      �� -   5         ��Name$ LanguageComboBox��56       " MaterialDesignOutlinedComboBox#��[ 57      )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Язык��58      $	��150��59      $��0,0,8,0q�5:      .@�5;      �� $��Русский��6   �� 5<      $��English��6   5=      �� 5?      -   5        $ BtnRecognize��5?       $  MaterialDesignRaisedAccentButton#��[ 5@      .��5B      �� $ 
Horizontal=�6   .+�5C        $ TextRecognition=�6.   $��0,0,8,0q�6E   �� 5D      $%��Распознать текст��6    5E      5F      5G      �� 5J   
   $��1��6   $��0��6     MaterialDesignDivider#���  5K      $��1q�5L      $��0,0,8,0q�5M      ._�5N      � .+�5O      �� -   5        	 ��Name$	 ImageDisplay��5O      $��Uniform=�5P      
 J� 	AllowDrop$

 True��5Q      5S      �� 5U      -   5         ��Name$ DropHintText��5[      $���Перетащите изображение сюда или нажмите кнопку 'Выбрать изображение'��5U      $��Center=�5V      $��Center=�5W       �� TextAlignment$ Center=�5X      $
��Wrap=�5Y        MaterialDesignBodyLight#���  5Z      5[      5\      5]      � 5`   
   $��1��6   $��1��6   $��8,0,0,0q�6,    5a      �� 5b      $��* �6    �� 5c      $
��Auto �6    .+�5f      �� -   5         ��Name$ OutputTextBox��5f       !	 MaterialDesignOutlinedTextBox#��[	 5g      $) #Распознанный текст��5h       �� TextWrapping$
 Wrap=�5i       �� AcceptsReturn$
 True��5j      " �� VerticalScrollBarVisibility$
 Auto=�5k      $��0,0,0,16q�5l      �� 5n      $��1��6   $ 
Horizontal=�6&   $��Right=�6?   .+�5o      �� -   5        $ BtnCopy��5o      #��[ 5p      $��0,0,8,0q�5r      .��5s      �� $ 
Horizontal=�6!   .+�5t        $ ContentCopy=�62   $��0,0,8,0q�6E   �� 5u      $��Копировать��6$   5v      5w      �� 5y      -	   5        $ BtnClear��5y      #��[ 5z      $��0,0,8,0q�5|      .��5}      �� $ 
Horizontal=�6!   .+�5~        $ Delete=�62   $��0,0,8,0q�6@   �� 5      $��Очистить��6$   5�      5�      �� 5�      -
   5        $ BtnSave��5�      #��[ 5�      .��5�      �� $ 
Horizontal=�6!   .+�5�        $ ContentSave=�62   $��0,0,8,0q�6E   �� 5�      $��Сохранить��6$   5�      5�      5�      5�      � 5�   
   $��0��6   $��2��6   $��0��6-   $��2��6=   ���	#80000000y�5�      �� *IsProcessing+ �� 	Converter# [ .+�5�      �  %
 !MaterialDesignCircularProgressBar#��[
 6    � Value$ 0Q�5�       � IsIndeterminate$
 True��5�      $��48��5�      $��48��5�      5�      5�      5�      