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

        #region 显示课程信息
        private void CourseInfoViewBtnClick(object sender, RoutedEventArgs e)
        {
            this.ClearWindow();
            this.CourseInfoView.Visibility = Visibility.Visible;

            using (var context = new EvalonEntities())
            {
                var courses = (from c in context.课程信息表 select c).ToList();
                var vcourses = courses.Select(course => new Course(course)).ToList();
                this.CourseGrid.ItemsSource = vcourses;
            }
        }
        #endregion


        #region 搜索显示课程信息
        private void SearchCourseByCourseIdBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var course =
                    (from c in context.课程信息表 where c.课程号 == this.SearchCourseByCourseIdTextBox.Text.Trim() select c)
                        .FirstOrDefault();
                if (course != null)
                {
                    this.C1CourseIdTextBox.Text = course.课程号;
                    this.C1CourseNameTextBox.Text = course.课程名称;
                    if (course.学分 != null)
                    {
                        this.C1CourseCreditNumericBox.Value = (int)course.学分;
                    }
                    if (course.学时 != null)
                    {
                        this.C1CourseTimeNumericBox.Value = (int)course.学时;
                    }
                    this.C1CourseTakingTimeTextBox.Text = course.上课时间;
                    this.C1CourseTakingPlaceTextBox.Text = course.上课地点;
                    if (course.预定人数 != null)
                    {
                        this.C1CourseReservationNumberNumericBox.Value = (int)course.预定人数;
                    }
                    if (course.已选人数 != null)
                    {
                        this.C1CourseTakenNumberNumericBox.Value = (int)course.已选人数;
                    }
                }
                else
                {
                    MessageBox.Show("无该课程信息,请添加");
                }
            }
        }
        #endregion


        #region 搜索修改课程信息
        private void SearchCourseChangeConfirmBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var course =
                    (from c in context.课程信息表 where c.课程号 == this.C1CourseIdTextBox.Text.Trim() select c).FirstOrDefault();
                if (course == null)
                {
                    if (MessageBox.Show("不存在该课程的信息,要添加新的课程吗?", "确认信息", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }

                    var newcourse = new 课程信息表
                    {
                        课程号 = this.C1CourseIdTextBox.Text.Trim(),
                        课程名称 = this.C1CourseNameTextBox.Text.Trim(),
                        学分 = (int?)this.C1CourseCreditNumericBox.Value,
                        学时 = (int?)this.C1CourseTimeNumericBox.Value,
                        上课时间 = this.C1CourseTakingTimeTextBox.Text.Trim(),
                        上课地点 = this.C1CourseTakingPlaceTextBox.Text.Trim(),
                        预定人数 =(int?) this.C1CourseReservationNumberNumericBox.Value,
                        已选人数 =(int?) this.C1CourseTakenNumberNumericBox.Value
                    };
                    context.课程信息表.Add(newcourse);
                    context.SaveChanges();
                    MessageBox.Show("新课程添加成功");
                }
                else
                {
                    course.课程名称 = this.C1CourseNameTextBox.Text.Trim();
                    course.学分 = (int?)this.C1CourseCreditNumericBox.Value;
                    course.学时 = (int?)this.C1CourseTimeNumericBox.Value;
                    course.上课时间 = this.C1CourseTakingTimeTextBox.Text.Trim();
                    course.上课地点 = this.C1CourseTakingPlaceTextBox.Text.Trim();
                    course.预定人数 = (int?)this.C1CourseReservationNumberNumericBox.Value;
                    course.已选人数 = (int?)this.C1CourseTakenNumberNumericBox.Value;

                    context.SaveChanges();
                    MessageBox.Show("修改成功");
                }
            }

            this.CourseInfoViewBtnClick(sender, e);
        }
        #endregion


        #region 搜索删除课程信息
        private void SearchCourseConfirmDeleteBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                if (MessageBox.Show("要删除该课程吗?", "确认信息", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var course =
                        (from c in context.课程信息表 where c.课程号 == this.C1CourseIdTextBox.Text.Trim() select c)
                            .FirstOrDefault();
                    context.课程信息表.Remove(course);
                    context.SaveChanges();
                }
            }

            this.CourseInfoViewBtnClick(sender, e);
        }
        #endregion



        #region 打开Excel文件
        private void CourseFileOpenBtnClick(object sender, RoutedEventArgs e)
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
            this.CourseFileNameTextBox.Text = filename;

            var courses = new List<课程信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
                    {
                        courses.Add(
                            new 课程信息表
                                {
                                    课程号 = dt.Rows[i][0].ToString(),
                                    课程名称 = dt.Rows[i][1].ToString(),
                                    学分 = int.Parse(dt.Rows[i][2].ToString()),
                                    学时 = int.Parse(dt.Rows[i][3].ToString()),
                                    上课地点 = dt.Rows[i][4].ToString(),
                                    上课时间 = dt.Rows[i][5].ToString(),
                                    预定人数 = int.Parse(dt.Rows[i][6].ToString()),
                                    已选人数 = int.Parse(dt.Rows[i][7].ToString())
                                });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            var vcourses = (from c in courses select new Course(c)).ToList();

            this.NewCourseGrid.ItemsSource = vcourses;

        }
        #endregion
        

        #region 确认从Excel中添加
        private void CourseConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewCourseGrid.ItemsSource;
                var courseids = (from c in context.课程信息表 select c.课程号).ToList<string>();
                foreach (var course in items.OfType<Course>().Select(s => new 课程信息表
                {
                    课程号 = s.课程号,
                    课程名称 = s.课程名称,
                    学分 = s.学分,
                    学时 = s.学时,
                    上课时间 = s.上课时间,
                    上课地点 = s.上课地点,
                    预定人数 = s.预定人数,
                    已选人数 = s.已选人数
                }))
                {
                    if (courseids.Contains(course.课程号))
                    {
                        MessageBox.Show(string.Format("存在与原来的学号冲突 {0}", course.课程号));
                    }
                    else
                    {
                        context.课程信息表.Add(course);
                        context.SaveChanges();
                    }
                }
                MessageBox.Show("执行完毕");
            }

            this.CourseInfoViewBtnClick(sender, e);

        }
        #endregion
    }
}
