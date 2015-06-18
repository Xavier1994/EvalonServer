using System.Windows;

namespace EvalonServer.Window
{
    /// <summary>
    /// AdminWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdminWindow : System.Windows.Window
    {

         #region 窗口功能

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
            this.Close();

        }
        #endregion


        #region 清理窗口
        private void ClearWindow()
        {
            this.LoginInfoView.Visibility = Visibility.Hidden;
            this.StudentInfoView.Visibility = Visibility.Hidden;
            this.TeacherInfoView.Visibility = Visibility.Hidden;
            this.CourseInfoView.Visibility = Visibility.Hidden;
            this.CollegeInfoView.Visibility = Visibility.Hidden;
            this.TeachingCourseInfoView.Visibility = Visibility.Hidden;
            this.TestInfoView.Visibility = Visibility.Hidden;
            this.ChooseCourseInfoView.Visibility = Visibility.Hidden;
        }

        #endregion








        #endregion
    }
}
