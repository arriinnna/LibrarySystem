   M S B A M L   `   `   `  ����  WpfApp1    WpfApp1.MainAppWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$-  'Библиотечная система��5   	   $	��650��6&   $	��820��63    � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   ���	#FF3F51B5 �� Padding$ 10q�65   ._�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� -   5         ��Name$ cbSearchField��5      $	��150��62   $��5q�6>   .@�5      �� $��По названию��6#    �� 
IsSelected$
 True��69   �� 5      $��По автору��6#   �� 5      $��По жанру��6#   5      �� 5      -   5         ��Name$ txtSearchQuery��5      $	��300��62   $��5q�6>   $��5q�6I   �� 5      -   5        $��
Найти��5      $��5q�6>   $
��10,3q�6I   
����������	#FF3F51B5$
��Bold"�5   C   �� 5      -   5        $��Очистить��5      $��5q�6@   $
��10,3q�6K   
������ 
�������
�������5    E   �� 5"      -   5        $!��Личный кабинет��5"      $��Right=�5#      $��20,0,0,0q�65   $
��10,3q�6G   
����������	#FF3F51B5$
��Bold"�5$   C   5%      5&      &   System.Windows.Controls.DataGrid  5)   
   -   5          Name$ 	booksGrid��5)       $��1��6   $��10q�63   	   AutoGenerateColumns$	 False��6?   
   
IsReadOnly$

 True��5*         Columns 5+      0  *System.Windows.Controls.DataGridTextColumn  5,      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 61   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6<   �� *Id+  5-      $ Название��6%   $ * 67    6A   �� *Title+  5.      $ 
Автор��6%   $	 180 64    6@   �� *Author+  5/      $ Жанр��6%   $	 120 63    6?   �� *Genre+  50      $ Доступно��6%   $ 90 67    6B   �� *AvailableCount+52      �� 55   
   $��2��6   $ 
Horizontal=�6"   $��10q�6;   $��Bottom=�6G   .+�56      �� $(��"Дней бронирования:��6   $��Center=�62   $��0,0,5,0q�6M   �� 57      -   5        $ DaysTextBox��57      $	��100��6+   $��0,0,10,0q�67   �� 58      -   5        $ ��Забронировать��58      ���	#FF3F51B5
�������$
��Bold"�59   ?   $
��15,5q�5:      5;      5<      5=      