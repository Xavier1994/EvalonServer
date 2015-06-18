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

        #region 初始化选课窗口
        private void ChoodeCourseInfoViewBtnClick(object sender, RoutedEventArgs e)
        {
            this.ClearWindow();
            this.ChooseCourseInfoView.Visibility = Visibility.Visible;

        }
        #endregion 


        #region 使用学号来搜索课程
        private void SearchChooseCourseStudentByCourseIdBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var students =
                    (from c in context.选课信息表
                     where c.课程号 == this.SearchChooseCourseStudentByCourseIdTextBox.Text.Trim()
                     select c.学生信息表).ToList();
                var vstudents = (from s in students select new Student(s)).ToList();
                this.SearchChooseCourseStudentByCourseIdGrid.ItemsSource = vstudents;
            }
        }
        #endregion


        #region 使用课程号来搜索上课的学生
        private void SearchChoosedCourseByStudentIdBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var courses =
                    (from c in context.选课信息表
                     where c.学号 == this.SearchChoosedCourseByStudentIdTextBox.Text.Trim()
                     select c.课程信息表).ToList();
                var vcourses = (from c in courses select new Course(c)).ToList();
                this.SearchChoosedCourseByStudentIdGrid.ItemsSource = vcourses;
            }

        }
        #endregion 



        #region  搜索添加
        private void SearchChooseCourseConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var choosecourse = (from c in context.选课信息表
                                    where
                                        c.学号 == this.SearchChooseCourseEditByStudentIdTextBox.Text.Trim()
                                        && c.课程号 == this.SearchChooseCourseEditByCourseIdTextBox.Text.Trim()
                                    select c).FirstOrDefault();
                if (choosecourse == null)
                {
                    var newchoosecourse = new 选课信息表
                                              {
                                                  学号 = this.SearchChooseCourseEditByStudentIdTextBox.Text.Trim(),
                                                  课程号 = this.SearchChooseCourseEditByCourseIdTextBox.Text.Trim()
                                              };
                    if (ChooseCourse.ChooseCourseCheck(newchoosecourse))
                    {
                        context.选课信息表.Add(newchoosecourse);
                        context.SaveChanges();
                        MessageBox.Show("添加成功");
                    }
                    else
                    {
                        MessageBox.Show("参数不规范");
                    }
                }
                else
                {
                    MessageBox.Show("已存在，不能添加");
                }
            }
        }
        #endregion 


        #region 搜索删除
        private void SearchChooseCourseConfirmDeleteBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var choosecourse = (from c in context.选课信息表
                                    where
                                        c.学号 == this.SearchChooseCourseEditByStudentIdTextBox.Text.Trim()
                                        && c.课程号 == this.SearchChooseCourseEditByCourseIdTextBox.Text.Trim()
                                    select c).FirstOrDefault();
                if (choosecourse == null)
                {
                    MessageBox.Show("不存在该选课信息，所以无法删除");
                }
                else
                {
                    context.选课信息表.Remove(choosecourse);
                    var course = (from c in context.课程信息表 where c.课程号 == choosecourse.课程号 select c).FirstOrDefault();
                    if (course != null)
                    {
                        course.已选人数 -= 1;
                    }
                    context.SaveChanges();
                    MessageBox.Show("删除成功");
                }
            }

        }
        #endregion 


        

        #region 打开Excel文件

        private void ChooseCourseFileOpenBtnClick(object sender, RoutedEventArgs e)
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
            this.ChooseCourseFileNameTextBox.Text = filename;

            var choosecourses = new List<选课信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
                    {
                        choosecourses.Add(new 选课信息表
                        {
                            学号 = dt.Rows[i][0].ToString(),
                            课程号 = dt.Rows[i][1].ToString(),
                            课程成绩 = int.Parse(dt.Rows[i][2].ToString())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            this.NewChooseCourseGrid.ItemsSource = choosecourses;

        }

        #endregion 



        #region  确认从Excel中添加
        private void ChooseCourseConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewChooseCourseGrid.ItemsSource;
                var studentsids = (from s in context.学生信息表 select s.学号).ToList();
                var courseids = (from s in context.课程信息表 select s.课程号).ToList();

                foreach (var choosecourse in items)
                {
                    var tp = choosecourse as 选课信息表;
                    if (tp != null && studentsids.Contains(tp.学号))
                    {
                        var mycourseids = (from c in context.选课信息表 where c.学号 == tp.学号 select c.课程号).ToList() ;
                        if (mycourseids.Contains(tp.课程号))
                        {
                            MessageBox.Show("该选课信息已存在");
                            return;
                        }
                        if (ChooseCourse.ChooseCourseCheck(choosecourse as 选课信息表))
                        {
                            context.选课信息表.Add(choosecourse as 选课信息表);
                            var course =
                                (from c in context.课程信息表 where c.课程号 == (choosecourse as 选课信息表).课程号 select c)
                                    .FirstOrDefault();
                            if (course != null)
                            {
                                course.已选人数 += 1;
                            }
                            context.SaveChanges();
                            MessageBox.Show("成功保存");
                        }
                        else
                        {
                            MessageBox.Show("参数不规范");
                        }
                    }
                    else
                    {
                        if (ChooseCourse.ChooseCourseCheck(choosecourse as 选课信息表))
                        {
                            context.选课信息表.Add(choosecourse as 选课信息表);
                            context.SaveChanges();
                            MessageBox.Show("成功保存");
                        }
                        else
                        {
                            MessageBox.Show("参数不规范");
                        }
                    }
                }
                MessageBox.Show("执行完毕");
            }

            this.ChoodeCourseInfoViewBtnClick(sender, e);
        }
        #endregion 

    }
}
