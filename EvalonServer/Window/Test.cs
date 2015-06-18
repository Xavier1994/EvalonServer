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
        private void TestInfoViewBtnClick(object sender, RoutedEventArgs e)
        {
            this.ClearWindow();
            this.TestInfoView.Visibility = Visibility.Visible;
            using (var context = new EvalonEntities())
            {
                var tests = (from t in context.考试信息表 select t).ToList();
                var vtests = (from t in tests select new Test(t)).ToList();
                this.TestGrid.ItemsSource = vtests;
            }
        }


        private void SearchTestByCourseIdBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var tests =
                    (from t in context.考试信息表 where t.课程号 == this.SearchTestByCourseIdTextBox.Text.Trim() select t)
                        .ToList();
                var vtests = (from t in tests select new Test(t)).ToList();
                this.SearchTestByCourseIdGrid.ItemsSource = vtests;

            }

        }


        private void SearchTestedStudentBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var test =
                    (from t in context.考试信息表 where t.课程号 == this.SearchTestedStudentByCourseIdTestBox.Text.Trim() && t.考试地点 == this.SearchTestedStudentByTestingPlaceTextBox.Text.Trim() && t.考试地点==this.SearchTestedStudentByTestingTimeTestBox.Text.Trim() select t).FirstOrDefault();
                if (test == null)
                {
                    MessageBox.Show("不存在该场考试");
                }
                else
                {
                    var students = (from s in context.选课信息表 where s.课程号 == test.课程号 select s.学生信息表).ToList();
                    var vstudents = (from s in students select new Student(s)).ToList();
                    this.SearchTestedStudentGrid.ItemsSource = vstudents;
                }
            }

        }



        #region 确认从Excel中添加
        private void TestConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewTestGrid.ItemsSource;
                var courseids = (from s in context.考试信息表 select s.课程号).ToList();

                foreach (var test in items)
                {
                    var tp = test as 考试信息表;
                    if (tp != null && courseids.Contains(tp.课程号))
                    {
                        var tests = (from t in context.考试信息表 where t.课程号 == tp.课程号 select t).ToList();
                        if (tests.Any(tt => tt.考试地点 != null && (tt.考试时间 == tp.考试时间 && tt.考试地点 == tt.考试地点)))
                        {
                            MessageBox.Show("该任课信息已存在");
                            return;
                        }

                        if (Test.TestCheck(test as 考试信息表))
                        {
                            context.考试信息表.Add(test as 考试信息表);
                            context.SaveChanges();
                            MessageBox.Show("成功保存");
                        }
                        else
                        {
                            MessageBox.Show("参数不规范，无法保存{0}");
                        }
                    }
                    else
                    {
                        if (Test.TestCheck((test as 考试信息表)))
                        {
                            context.考试信息表.Add(test as 考试信息表);
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

            this.TestInfoViewBtnClick(sender, e);
        }

        #endregion 




        #region 打开Excel文件
        private void TestFileOpenBtnClick(object sender, RoutedEventArgs e)
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
            this.TestFileNameTextBox.Text = filename;

            var tests = new List<考试信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
                    {
                        tests.Add(new 考试信息表
                        {
                            课程号 = dt.Rows[i][0].ToString(),
                            考试时间 = dt.Rows[i][1].ToString(),
                            考试地点 = dt.Rows[i][1].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            this.NewTestGrid.ItemsSource = tests;

        }
        #endregion 

    }
}
