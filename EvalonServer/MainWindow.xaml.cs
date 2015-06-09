using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EvalonServer.Window;

namespace EvalonServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        #region 关闭窗口
        private void QuitBtnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion


        #region 登陆
        private void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context=new EvalonEntities())
            {
                var login = (from u in context.登录信息表
                    where (u.用户名 == UsernameTextBox.Text.Trim()
                           && u.密码 == PwdTextBox.Password.Trim() && u.用户类型 == "管理员")
                    select u).FirstOrDefault();
                if (login == null)
                {
                    MessageBox.Show("请输入正确的用户名或密码");
                }
                else
                {
                    MessageBox.Show("登陆成功");
                    var adminWindow=new AdminWindow();
                    Close();
                    adminWindow.Show();
                }
            }
        }
        #endregion


    }
}
