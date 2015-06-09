using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EvalonServer.Lib;
using Microsoft.Win32;

namespace EvalonServer.Window
{
    /// <summary>
    /// AdminWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdminWindow : System.Windows.Window
    {

        #region 构造函数
        public AdminWindow()
        {
            InitializeComponent();
        }
        #endregion


        #region 返回主页
        private void ReturnBtnClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion


        #region 修改密码窗口
        private void ChangePwdBtnClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion


        #region 刷新窗口
        private void RefleshBtnClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion


        #region 关闭窗口
        private void QuitBtnClick(object sender, RoutedEventArgs e)
        {
            Close();

        }
        #endregion


        #region 清理窗口
        private void ClearWindow()
        {
            LoginInfoView.Visibility = Visibility.Hidden;
        }   
        #endregion


        #region 登录信息模块

        #region 显示登录账户的信息
        private void LoginInfoBtnClick(object sender, RoutedEventArgs e)
        {
            ClearWindow();
            LoginInfoView.Visibility=Visibility.Visible;
            SearchUserTextBox.Text=string.Empty;
            SearchUserNameTextBox.Text = string.Empty;
            SearchUserPwdTextBox.Text = string.Empty;
            SearchUserTypeTextBox.Text = string.Empty;

            using (var context = new EvalonEntities())
            {
                var logins = (from l in context.登录信息表 select l).ToList();
                LoginGrid.ItemsSource = logins;
            }
        }
       #endregion

        #region 确认修改登录信息表
        private void ConfirmChangeLoginInfoBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var loginslength = (from l in context.登录信息表 select l).Count();
                var usernames = (from l in context.登录信息表 select l.用户名).ToList<string>();
                var logins = (from l in context.登录信息表 select l).ToList<登录信息表>();
                var items = LoginGrid.ItemsSource;

                int i = 0;
                foreach (var item in items)
                {
                    if (i <= loginslength)return;
                    if (usernames.Contains(((item as 登录信息表).用户名)))
                    {
                        MessageBox.Show("用户名相同,不能添加");
                    }
                    else
                    {
                        context.登录信息表.Add((item as 登录信息表));
                        context.SaveChanges();
                        MessageBox.Show("添加成功");
                    }
                    i++;
                }
            }
            LoginInfoBtnClick(sender, e);
        }
        #endregion


        #region 打开Excel文件
        private void FileOpenBtnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".xls",
                Filter = "Excel 工作薄 .xls|*.xls"
            };

            var result = dlg.ShowDialog();

            if (result != true) return;
            var filename = dlg.FileName;
            FileNameTextBox.Text = filename;

            var logins = new List<登录信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if(dt==null)return;
                    for (int i = 0; i < dt.Rows.Count; ++i)
                    {
                        logins.Add(new 登录信息表
                        {
                            用户名 = dt.Rows[i][0].ToString(),
                            密码 = dt.Rows[i][1].ToString(),
                            用户类型 = dt.Rows[i][2].ToString()
                        });
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            NewLoginGrid.ItemsSource = logins;
        }
        #endregion


        #region 确认从Excel文件里面添加 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = NewLoginGrid.ItemsSource;
                var usernames = (from l in context.登录信息表 select l.用户名).ToList<string>();
                foreach (var item in items)
                {
                    if (usernames.Contains((item as 登录信息表).用户名))
                    {
                        MessageBox.Show("存在与原来的用户名冲突");
                    }
                    else
                    {
                        context.登录信息表.Add((item as 登录信息表));
                        context.SaveChanges();
                    }
                }
                MessageBox.Show("执行成功");
            }
            LoginInfoBtnClick(sender, e);
        }
        #endregion


        #region 使用用户名搜索登录用户
        private void ConfirmSearchUserBtnClick(object sender, RoutedEventArgs e)
        {
            var username = SearchUserTextBox.Text.Trim();
            using (var context = new EvalonEntities())
            {
                var user = (from l in context.登录信息表 where l.用户名 == username select l).FirstOrDefault();
                if (user == null)
                {
                    MessageBox.Show("不存在该用户");
                }
                else
                {
                    SearchUserNameTextBox.Text = user.用户名;
                    SearchUserPwdTextBox.Text = user.密码;
                    SearchUserTypeTextBox.Text = user.用户类型;

                }
            }
        }
        #endregion

        #region 搜索确认修改登录用户
        private void ConfirmChangeFromSearchClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var user =
                    (from l in context.登录信息表 where l.用户名 == SearchUserNameTextBox.Text.Trim() select l).FirstOrDefault();
                if (user == null)
                {
                    if (MessageBox.Show("不存在该用户,要添加新用户吗?", "确认信息", MessageBoxButton.OKCancel) ==
                        MessageBoxResult.OK)
                    {
                        context.登录信息表.Add(new 登录信息表
                        {
                            用户名 = SearchUserNameTextBox.Text.Trim(),
                            密码 = SearchUserPwdTextBox.Text.Trim(),
                            用户类型 = SearchUserTypeTextBox.Text.Trim()
                        }
                            );
                        context.SaveChanges();
                        MessageBox.Show("新用户添加成功");
                    }
                }
                else
                {
                    user.密码 = SearchUserPwdTextBox.Text.Trim();
                    user.用户类型 = SearchUserTypeTextBox.Text.Trim();
                    context.SaveChanges();
                    MessageBox.Show("修改成功");

                }
            }
            LoginInfoBtnClick(sender, e);
        }



        #endregion


        #region 搜索删除登录用户
        private void DeleteUserFromSearchBoxClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var user =
                    (from l in context.登录信息表 where l.用户名 == SearchUserNameTextBox.Text.Trim() select l).FirstOrDefault();
                context.登录信息表.Remove(user);
                context.SaveChanges();
                MessageBox.Show("删除成功");
            }
            LoginInfoBtnClick(sender,e);
        }
        #endregion

        #endregion
    }
}
