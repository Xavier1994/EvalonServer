using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EvalonServer.Lib;
using Microsoft.Win32;

namespace EvalonServer.Window
{
    public partial class AdminWindow
    {
        #region 登录信息模块

        #region 显示登录账户的信息
        private void LoginInfoBtnClick(object sender, RoutedEventArgs e)
        {
            this.ClearWindow();
            this.LoginInfoView.Visibility = Visibility.Visible;
            this.SearchUserTextBox.Text = string.Empty;
            this.SearchUserNameTextBox.Text = string.Empty;
            this.SearchUserPwdTextBox.Text = string.Empty;
            this.SearchUserTypeTextBox.Text = string.Empty;

            using (var context = new EvalonEntities())
            {
                var logins = (from l in context.登录信息表 select l).ToList();
                this.LoginGrid.ItemsSource = logins;
            }
        }
        #endregion



        #region 确认修改登录信息表
        private void ConfirmChangeLoginInfoBtnClick(object sender, RoutedEventArgs e)
        {
            /*
            using (var context = new EvalonEntities())
            {
                var usernames = (from l in context.登录信息表 select l.用户名).ToList<string>();
                var items = this.LoginGrid.ItemsSource;

                var i = 0;
                foreach (var item in items)
                {
                    if (i <= (from l in context.登录信息表 select l).Count())
                    {
                        return;
                    }
                    if (usernames.Contains(item: ((item as 登录信息表)?.用户名)))
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
           this.LoginInfoBtnClick(sender, e);
             * */
        }
        #endregion


        #region 确认从Excel文件里面添加 
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewLoginGrid.ItemsSource;
                var usernames = (from l in context.登录信息表 select l.用户名).ToList<string>();
                foreach (var item in items)
                {
                    var l = item as 登录信息表;
                    if (l != null && usernames.Contains(l.用户名))
                    {
                        MessageBox.Show("存在与原来的用户名冲突");
                    }
                    else
                    {
                        if (Login.LoginCheck(item as 登录信息表))
                        {
                            context.登录信息表.Add((item as 登录信息表));
                            context.SaveChanges();
                        }
                        else
                        {
                            var 登录信息表 = item as 登录信息表;
                            if (登录信息表 != null)
                            {
                                MessageBox.Show(string.Format("参数不规范，无法保存 {0}"), 登录信息表.用户名);
                            }
                        }
                    }
                }
                MessageBox.Show("执行完毕");
            }
            this.LoginInfoBtnClick(sender, e);
             
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

            if (result != true)
            {
                return;
            }

            var filename = dlg.FileName;
            this.FileNameTextBox.Text = filename;

            var logins = new List<登录信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
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
                Console.WriteLine(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            this.NewLoginGrid.ItemsSource = logins;
        }
        #endregion

        #region 使用用户名搜索登录用户
        private void ConfirmSearchUserBtnClick(object sender, RoutedEventArgs e)
        {
            var username = this.SearchUserTextBox.Text.Trim();
            using (var context = new EvalonEntities())
            {
                var user = (from l in context.登录信息表 where l.用户名 == username select l).FirstOrDefault();
                if (user == null)
                {
                    MessageBox.Show("不存在该用户");
                }
                else
                {
                    this.SearchUserNameTextBox.Text = user.用户名;
                    this.SearchUserPwdTextBox.Text = user.密码;
                    this.SearchUserTypeTextBox.Text = user.用户类型;

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
                    (from l in context.登录信息表 where l.用户名 == this.SearchUserNameTextBox.Text.Trim() select l).FirstOrDefault();
                if (user == null)
                {
                    if (MessageBox.Show("不存在该用户,要添加新用户吗?", "确认信息", MessageBoxButton.OKCancel) ==
                        MessageBoxResult.OK)
                    {
                        var newlogin=new 登录信息表()
                                         {
                                             用户名 = this.SearchUserNameTextBox.Text.Trim(),
                                             密码 = this.SearchUserPwdTextBox.Text.Trim(),
                                             用户类型 = this.SearchUserTypeTextBox.Text.Trim()
                        };
                        if (Login.LoginCheck(newlogin))
                        {
                            context.登录信息表.Add(newlogin);
                            context.SaveChanges();
                            MessageBox.Show("新用户添加成功");
                        }
                        else
                        {
                            MessageBox.Show("参数不规范，无法保存");
                        }
                    }
                }
                else
                {
                    user.密码 = this.SearchUserPwdTextBox.Text.Trim();
                    user.用户类型 = this.SearchUserTypeTextBox.Text.Trim();
                    if (Login.LoginCheck(user))
                    {
                        context.SaveChanges();
                        MessageBox.Show("修改成功");
                    }
                    else
                    {
                        MessageBox.Show("参数不规范，无法保存");
                    }
                }
            }
            this.LoginInfoBtnClick(sender, e);
        }
        #endregion

        #region 搜索删除登录用户
        private void DeleteUserFromSearchBoxClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var user =
                    (from l in context.登录信息表 where l.用户名 == this.SearchUserNameTextBox.Text.Trim() select l).FirstOrDefault();
                context.登录信息表.Remove(user);
                context.SaveChanges();
                MessageBox.Show("删除成功");
            }
            this.LoginInfoBtnClick(sender, e);
        }
        #endregion

        #endregion
    }
}
