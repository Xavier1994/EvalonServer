// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StudentWindow.cs" company="">
//   
// </copyright>
// <summary>
//   The admin wind.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EvalonServer.Window
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;

    using EvalonServer.Lib;

    using Microsoft.Win32;

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public partial class AdminWindow
    {
        #region 初始化学生显示窗口 
        private void StudentInfoBtnClick(object sender, RoutedEventArgs e)
        {
            ClearWindow();
            StudentInfoView.Visibility = Visibility.Visible;
            using (var context = new EvalonEntities())
            {
                var students = (from s in context.学生信息表 select s).ToList();
                var viewstudents = students.Select(student => new Student(student)).ToList();
                StudentGrid.ItemsSource = viewstudents;
            }

            // 初始化性别选项栏
            var dropDownListComedy = new List<string>
                                                  {
                                                     "男",
                                                     "女"
                                                  };
            this.C1StudentSexualityComboBox.ItemsSource = dropDownListComedy;
        }
        #endregion

        #region 学号搜索显示
        private void SearchStudentConfirmBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var student =
                    (from s in context.学生信息表 where s.学号 == this.SearchStudentByStudentId.Text.Trim() select s)
                        .FirstOrDefault();
                if (student == null)
                {
                    return;
                }
                this.C1StudentNameTextBox.Text = student.姓名;
                this.C1StudentSexualityComboBox.Text = student.性别;
                this.C1studentIdTextBox.Text = student.学号;
                if (student.年龄 != null)
                {
                    this.C1StudentAgeTextBox.Value = (int)student.年龄;
                }

                if (student.系号 != null)
                {
                    this.C1StudentDepartmentIdTextBox.Value = (int)student.系号;
                }

                this.C1StudentNationTextbox.Text = student.民族;
                this.C1StudentPlaceTextBox.Text = student.籍贯;
            }
        }
        #endregion

        #region 确认按照学号来修改学生
        private void SearchStudentConfirmChangeBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var student =
                    (from l in context.学生信息表 where l.学号 == C1studentIdTextBox.Text.Trim() select l).FirstOrDefault();
                if (student == null)
                {
                    if (MessageBox.Show("不存在该学生的信息,要添加新的学生吗?", "确认信息", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }

                    var newstudent = new 学生信息表
                                         {
                                             学号 = this.C1studentIdTextBox.Text.Trim(),
                                             姓名 = this.C1StudentNameTextBox.Text.Trim(),
                                             性别 = this.C1StudentSexualityComboBox.Text.Trim(),
                                             年龄 = (int)this.C1StudentAgeTextBox.Value
                                         };
                    if (Student.StudentCheck(newstudent))
                    {
                        context.学生信息表.Add(newstudent);
                        context.SaveChanges();
                        MessageBox.Show("新学生添加成功");
                    }
                    else
                    {
                        MessageBox.Show("参数不规范，无法保存");
                    }
                }
                else
                {
                    student.姓名 = this.C1StudentNameTextBox.Text.Trim();
                    student.性别 = this.C1StudentSexualityComboBox.Text.Trim();
                    student.年龄 = (int)this.C1StudentAgeTextBox.Value;
                    student.系号 = (int)this.C1StudentDepartmentIdTextBox.Value;
                    student.民族 = this.C1StudentNationTextbox.Text.Trim();
                    student.籍贯 = this.C1StudentPlaceTextBox.Text.Trim();
                    if (Student.StudentCheck(student))
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

            StudentInfoBtnClick(sender, e);
        }
        #endregion

        #region 确认按照学号来删除学生信息
        private void SearchStudentDeleteStudentBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                if (MessageBox.Show("要删除该学生吗?", "确认信息", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    var student =
                        (from s in context.学生信息表 where s.学号 == this.C1studentIdTextBox.Text.Trim() select s)
                            .FirstOrDefault();
                    context.学生信息表.Remove(student);
                    context.SaveChanges();
                }
            }

            StudentInfoBtnClick(sender, e);
        }
        #endregion


        #region 打开Excel文件
        private void StudentFileOpenBtnClick(object sender, RoutedEventArgs e)
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
            this.StudentFileNameTextBox.Text = filename;

            var  students = new List<学生信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
                    {
                        students.Add(new 学生信息表
                        {
                            学号 = dt.Rows[i][0].ToString(),
                            姓名 = dt.Rows[i][1].ToString(),
                            性别 = dt.Rows[i][2].ToString(),
                            年龄 = int.Parse(dt.Rows[i][3].ToString()),
                            系号 = int.Parse(dt.Rows[i][4].ToString()),
                            籍贯 = dt.Rows[i][5].ToString(),
                            民族 = dt.Rows[i][6].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            var vstudents = (from s in students select new Student(s)).ToList();

            this.NewStudentGrid.ItemsSource = vstudents;

        }
        #endregion


        #region  确认从Excel文件中添加
        private void NewStudentAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewStudentGrid.ItemsSource;
                var studentids = (from s in context.学生信息表 select s.学号).ToList<string>();
                foreach (var student in items.OfType<Student>().Select(s => new 学生信息表
                                                                       {
                                                                           学号 = s.学号,
                                                                           姓名 = s.姓名,
                                                                           性别 = s.性别,
                                                                           年龄 = s.年龄,
                                                                           系号 = s.系号,
                                                                           籍贯 = s.籍贯,
                                                                           民族 = s.民族
                                                                       }))
                {
                    if (studentids.Contains(student.学号))
                    {
                        MessageBox.Show(string.Format("存在与原来的学号冲突 {0}",student.学号));
                    }
                    else
                    {
                        if (Student.StudentCheck(student))
                        {
                            context.学生信息表.Add(student);
                            context.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show(string.Format("参数不规范，无法保存{0}",student.学号));
                        }
                    }
                }
                MessageBox.Show("执行完毕");
            }

            StudentInfoBtnClick(sender, e);

        }
        #endregion


    }
}
