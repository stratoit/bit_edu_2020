   M S B A M L   `   `   `  ����M  IMBStore_MVVM_merge, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:MBStore_MVVMMBStore_MVVM  5   	       MBStore_MVVM.FindAddress   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:MBStore_MVVM     5   	   -   5          � WindowStyle$
  None=�5       � WindowStartupLocation$ CenterOwner=�6-    � 
ResizeMode$ NoResize=�6Q   $	��475��5   	   $	��640��6   .��5	      �  � RowDefinitions 5
   
   �� 5      $��25 �6   6*   �� 5      6   .+�5   
   � -   5        $��0��5      ���	#FF0051F0 � ColumnDefinitions 5      �� 5      $
��0.1* �6#   62   �� 5      $
��1.2* �6#   62   �� 5      $
��0.3* �6#   62   .+�5      �� $��Center=�6   
�������$��1��6F   $��우편번호 검색��6V   �� 5      -   5         ��Name$ btn_minimum��5      ���	#FF0051F0
�������$
��Bold"�6T   $��2��6f   $	��___��6v   $��0,0,65,0q�6�   ��6�   Z� �� 5      -   5        $ btn_maximum��5      ���	#FF0051F0
�������$
��Bold"�6T   $��2��6f   $	��□��6v   $��32.5,0,32.5,0q�6�   ��6�   Z� �� 5      -   5        $ 	btn_close��5      ���	#FF0051F0
�������$
��Bold"�6R   $��2��6d   $��X��6t   $��65,0,0,0q�6�   ��6�   Z� 6�   5      � 5   
   $��1��6   .+�5      (  "System.Windows.Controls.WebBrowser  -   5          Name$ wb_find��5      5      5      5      