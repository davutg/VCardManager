   M S B A M L   `   `   `  ����K  GVCF_VCardManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:VCF_VCardManagerVCF_VCardManager  5   	   !    VCF_VCardManager.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:VCF_VCardManager     5   	     � Title$  VCard Contact Helper��5   	   $	��350��6&   $	��525��63   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6   .+�5      � $��5q�6    � ColumnDefinitions 5      �� 5      $
��Auto �6'   �� 5      $��* �6'   �� 5      $
��Auto �6'   .+�5      �� -   5        $
��Load��5      $��5q�6(   6J   �� 5      $��1��6   $��5q�6*   ��65   �� *
FileName+6Q   �� 5      -   5        $��2��5      $
��Save��6)   $��4.6,5,4.8,0q�68   $��19��6M   $	��Top=�6Y   6q   5      5      � 5   
   $��1��6   .+�5      �� $��Center=�60   ��6   �� *Status+60   5      � 5   
   $��2��6   .+�5      &   System.Windows.Controls.DataGrid  -   5           ColumnHeaderHeight$ 38Q�5   8      AutoGenerateColumns .  ��6   �� *
Contacts+   Columns 5       4  .System.Windows.Controls.DataGridCheckBoxColumn  5!      ,  &System.Windows.Controls.DataGridColumn   Header$ 	Required?��6-   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6@   �� *
IsRequired+ �� Mode$ TwoWay=�6m   0  *System.Windows.Controls.DataGridTextColumn  5"      $ ItemNo��6)   	   
IsReadOnly	 .  69   �� *ItemNo+$ OneWay=�6t     5#      
   
FontWeight$

 Bold"�6)   $
 Name��6;    6I   �� *ContactName+$ TwoWay=�6w     5$      $ Phone��6)    68   �� *Phone+$ TwoWay=�6`   5&      5'      5(      5)      