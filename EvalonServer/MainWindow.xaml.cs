using System.Linq;
using System.Windows;
using EvalonServer.Window;

namespace EvalonServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }



        #region 关闭窗口
        private void QuitBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion


        #region 登陆
        private void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context=new EvalonEntities())
            {
                var login = (context.登录信息表.Where(
                    u =>
                    (u.用户名 == this.UsernameTextBox.Text.Trim() && u.密码 == this.PwdTextBox.Password.Trim()
                     && u.用户类型 == "管理员"))).FirstOrDefault();
                if (login == null)
                {
                    MessageBox.Show("请输入正确的用户名或密码");
                }
                else
                {
                    MessageBox.Show("登陆成功");
                    var adminWindow=new AdminWindow();
                    this.Close();
                    adminWindow.Show();
                }
            }
        }
        #endregion


    }
}
