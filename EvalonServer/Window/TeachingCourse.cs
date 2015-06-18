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
        #region 初始化任课窗口
        private void TeachingCourseBtnClick(object sender, RoutedEventArgs e)
        {
            this.ClearWindow();
            this.TeachingCourseInfoView.Visibility = Visibility.Visible;
            using (var context = new EvalonEntities())
            {
                var teachingcourses = (from t in context.任课信息表 select t).ToList();
                var vteachingcourses = (from t in teachingcourses select new TeachingCourse(t)).ToList();
                this.TeachingCourseGrid.ItemsSource = vteachingcourses;
            }
        }
        #endregion 

        #region 使用教师工号搜索显示窗口
        private void SearchTeachingCourseByTeacherIdBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var teachingcourses =
                    (from t in context.任课信息表
                     where t.工号 == this.SearchTeachingCourseByTeacherIdTextBox.Text.Trim()
                     select t).ToList();
                var vteachingcourses = (from t in teachingcourses select new TeachingCourse(t)).ToList();
                this.TeachingCourseSearchByTeacherIdGrid.ItemsSource = vteachingcourses;
            }
        }
        #endregion 

        #region 使用课程号号搜索显示窗口
        private void SearchTeachingCourseByCourseIdBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var teachingcourses =
                    (from t in context.任课信息表
                     where t.课程号 == this.SearchTeachingCourseByCourseIdTextBox.Text.Trim()
                     select t).ToList();
                var vteachingcourses = (from t in teachingcourses select new TeachingCourse(t)).ToList();
                this.TeachingCourseSearchByCourseIdGrid.ItemsSource = vteachingcourses;
            }
        }
        #endregion

        #region 添加任课信息
        private void SearchTeachingCourseConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var teachingcourse = (from t in context.任课信息表
                                      where
                                          t.课程号 == this.C1SearchTeachingCourseCourseIdTextBox.Text.Trim()
                                          && t.工号 == this.C1SearchTeachingCourseTeacherIdTextBox.Text.Trim()
                                      select t).FirstOrDefault();
                if (teachingcourse != null)
                {
                    MessageBox.Show("已存在该任课项目，不能重复添加");
                    return;
                }
                var newteachingcourse = new 任课信息表
                                            {
                                                课程号 = this.C1SearchTeachingCourseCourseIdTextBox.Text.Trim(),
                                                工号 = this.C1SearchTeachingCourseTeacherIdTextBox.Text.Trim()
                                            };
                if (TeachingCourse.TeachngCourseCheck(newteachingcourse))
                {
                    context.任课信息表.Add(newteachingcourse);
                    context.SaveChanges();
                    MessageBox.Show("保存成功");
                }
                else
                {
                    MessageBox.Show("不存在该课程号或工号");
                }
            }
        }
        #endregion

        #region 删除任课信息
        private void SearchTeachingCourseConfirmDeleteBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var teachingcourse = (from t in context.任课信息表
                                      where
                                          t.课程号 == this.C1SearchTeachingCourseCourseIdTextBox.Text.Trim()
                                          && t.工号 == this.C1SearchTeachingCourseTeacherIdTextBox.Text.Trim()
                                      select t).FirstOrDefault();
                if (teachingcourse == null)
                {
                    MessageBox.Show("不存在该任课信息，所以无法删除");
                }
                else
                {
                    context.任课信息表.Remove(teachingcourse);
                    context.SaveChanges();
                    MessageBox.Show("删除成功");
                }               
            }
        }
        #endregion 

        #region 确认从Excel中添加
        private void TeachingCourseConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewTeachingCourseGrid.ItemsSource;
                var teacherids = (from s in context.任课信息表 select s.工号).ToList();
                var courseids = (from s in context.培养计划表 select s.课程号).ToList();

                foreach (var teachingcourse in items)
                {
                    var tp = teachingcourse as 任课信息表;
                    if (tp != null && teacherids.Contains(tp.工号) && courseids.Contains(tp.课程号))
                    {
                        MessageBox.Show("该任课信息已存在");
                    }
                    else
                    {
                        if (TeachingCourse.TeachngCourseCheck((teachingcourse as 任课信息表)))
                        {
                            context.任课信息表.Add(teachingcourse as 任课信息表);
                            context.SaveChanges();
                            MessageBox.Show("成功保存");
                        }
                        else
                        {
                            MessageBox.Show("参数不规范，无法保存{0}");
                        }
                    }
                }
                MessageBox.Show("执行完毕");
            }

            this.TeachingCourseBtnClick(sender, e);

        }
        #endregion 

        #region 打开Excel表格
        private void TeachingCourseFileOpenBtnClick(object sender, RoutedEventArgs e)
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
            this.TeachingCourseFileNameTextBox.Text = filename;

            var teachingcourses = new List<任课信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
                    {
                        teachingcourses.Add(new 任课信息表
                        {
                            工号 = dt.Rows[i][0].ToString(),
                            课程号 = dt.Rows[i][1].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            this.NewTeachingCourseGrid.ItemsSource = teachingcourses;
        }
        #endregion 



    }
}
