   M S B A M L   `   `   `  ����G  CEvalonServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:EvalonServerEvalonServer  5   	       EvalonServer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:EvalonServer     5   	     � Title$  管理员登陆窗口��5   	   $	��300��6   $	��400��6&   .��5	      � .+�5
   
   �� $��	用户名��6   $
��Left=�6   $��	80,65,0,0q�69   $	��Top=�6L   �� 5   
   $��密码��6   $
��Left=�6   $��
80,129,0,0q�68   $	��Top=�6L   �� 5   
   -   5         ��Name$ UsernameTextBox��5      $
��Left=�6+   $��21��6F   $��
174,65,0,0q�6R    �� TextWrapping$
 Wrap=�6f   $	��Top=�6z   $	��141��6�    J� RenderTransformOrigin$ 0.5,0.5&�6�   �� 5   
   -   5         ��Name$ LoginBtn��5      $��登陆��69   $
��Left=�6F   $��102,204,0,0q�6a   $	��Top=�6v   $��75��6�   �� 5   
   -   5        $ QuitBtn��5      $��退出��67   $
��Left=�6D   $��220,204,0,0q�6_   $	��Top=�6t   $��75��6�   G� 5   
   -   5         G�Name$ 
PwdTextBox��5      $
��Left=�6+   $��174,133,0,0q�6F   $	��Top=�6[   $	��141��6s   5      5      