namespace EvalonServer.Window
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using EvalonServer.Lib;

    using Microsoft.Win32;

    public partial class AdminWindow
    {

        #region 显示教师的信息
        private void TeacherInfoBtnclick(object sender, RoutedEventArgs e)
        {
            this.ClearWindow();
            this.TeacherInfoView.Visibility = Visibility.Visible;
            using (var context = new EvalonEntities())
            {
                var teachers = (from t in context.教师信息表 select t).ToList();
                var vteachers = teachers.Select(teacher => new Teacher(teacher)).ToList();
                this.TeacherGrid.ItemsSource = vteachers;
            }
            var sextype = new List<string> { "男", "女" };
            var titletype = new List<string> {"助教","讲师","副教授","教授"};
            this.C1TeacherSexualityComboBox.ItemsSource = sextype;
            this.C1TeacherTitleComboBox.ItemsSource = titletype;
        }
        #endregion


        #region 使用工号来搜索教师的信息
        private void SearchTeacherConfirmBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var teacher =
                    (from t in context.教师信息表 where t.工号 == C1SearchByTeacherIdTextBox.Text.Trim() select t)
                        .FirstOrDefault();
                if (teacher != null)
                {
                    this.C1TeacherIdTextBox.Text = teacher.工号;
                    this.C1TeacherNameTextBox.Text = teacher.姓名;
                    this.C1TeacherSexualityComboBox.Text = teacher.性别;
                    this.C1TeacherTitleComboBox.Text = teacher.职称;
                }
                else
                {
                    MessageBox.Show("不存在该教师的信息");
                }
            }
        }
        #endregion
        

        #region 修改搜索出的教师的信息
        private void SearchTeacherConfirmChangeBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var teacher =
                    (from t in context.教师信息表 where t.工号 == this.C1TeacherIdTextBox.Text.Trim() select t).FirstOrDefault(
                        );
                if (teacher == null)
                {
                    if (MessageBox.Show("不存在该教师的信息,要添加新的教师吗?", "确认信息", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }

                    var newteacher = new 教师信息表
                                         {
                                             工号 = this.C1TeacherIdTextBox.Text.Trim(),
                                             姓名 = this.C1TeacherNameTextBox.Text.Trim(),
                                             性别 = this.C1TeacherSexualityComboBox.Text.Trim(),
                                             职称 = this.C1TeacherTitleComboBox.Text.Trim()
                                         };
                    if (Teacher.TeacherCheck(newteacher))
                    {
                        context.教师信息表.Add(newteacher);
                        context.SaveChanges();
                        MessageBox.Show("添加成功");
                    }
                    else
                    {
                        MessageBox.Show("无法保存,参数不规范");
                    }
                }
                else
                {
                    teacher.姓名 = this.C1TeacherNameTextBox.Text.Trim();
                    teacher.性别 = this.C1TeacherSexualityComboBox.Text.Trim();
                    teacher.职称 = this.C1TeacherTitleComboBox.Text.Trim();
                    if (Teacher.TeacherCheck(teacher))
                    {
                        context.SaveChanges();
                        MessageBox.Show("修改成功");
                    }
                    else
                    {
                        MessageBox.Show("参数不规范,无法保存");
                    }
                }
            }

            this.TeacherInfoBtnclick(sender, e);
        }
        #endregion


        #region 删除搜索出来的教师
        private void SearchTeacherConfirmDeleteBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var teacher =
                    (from t in context.教师信息表 where t.工号 == this.C1TeacherIdTextBox.Text.Trim() select t).FirstOrDefault(
                        );
                context.教师信息表.Remove(teacher);
                context.SaveChanges();
                MessageBox.Show("删除成功");
            }

            this.TeacherInfoBtnclick(sender, e);
        }
        #endregion


        #region  打开Excel文件 
        private void TeacherFileOpenBtnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel 工作薄 .xls|*.xls";

            var result = dlg.ShowDialog();

            if (result != true)
            {
                return;
            }

            var filename = dlg.FileName;
            this.TeacherFileNameTextBox.Text = filename;

            var teachers = new List<教师信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
                    {
                        teachers.Add(new 教师信息表
                        {
                            工号 = dt.Rows[i][0].ToString(),
                            姓名 = dt.Rows[i][1].ToString(),
                            性别 = dt.Rows[i][2].ToString(),
                            职称 = dt.Rows[i][3].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            var vteachers = (from t in teachers select new Teacher(t)).ToList();

            this.NewTeacherGrid.ItemsSource = vteachers;
        }
        #endregion


        #region 确认从Excel文件中添加
        private void TeacherConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewTeacherGrid.ItemsSource;
                var teacherids = (from t in context.教师信息表 select t.工号).ToList<string>();
                foreach (var teacher in items.OfType<Teacher>().Select(s => new 教师信息表
                                                                       {
                                                                           工号 = s.工号,
                                                                           姓名 = s.姓名,
                                                                           性别 = s.性别,
                                                                           职称 = s.职称
                                                                       }))
                {
                    if (teacherids.Contains(teacher.工号))
                    {
                        MessageBox.Show(string.Format("存在与原来的学号冲突 {0}",teacher.工号));
                    }
                    else
                    {
                        if (Teacher.TeacherCheck(teacher))
                        {
                            context.教师信息表.Add(teacher);
                            context.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show(string.Format("参数不规范，无法保存{0}",teacher.工号));
                        }
                    }
                }
                MessageBox.Show("执行完毕");
            }

            this.TeacherInfoBtnclick(sender, e);

        }
        #endregion 

    }
}
