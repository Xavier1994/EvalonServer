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

        #region 显示院系信息
        private void CollegeInfoViewBtnClick(object sender, RoutedEventArgs e)
        {
            this.ClearWindow();;
            this.CollegeInfoView.Visibility = Visibility.Visible;
            using (var context = new EvalonEntities())
            {
                var colleges = (from c in context.院系信息表 select c).ToList();
                var vcolleges = (from c in colleges select new College(c)).ToList();
                this.CollegeGrid.ItemsSource = vcolleges;
            }
        }
        #endregion 


        #region 搜索显示院系信息
        private void SearchCollegeByCollegeIdBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var college =
                    (from c in context.院系信息表 where c.系号 == (int?)this.SearchCollegeByCollegIdComboBox.Value select c)
                        .FirstOrDefault();
                if (college != null)
                {
                    this.C1CollegeIdComboBox.Value = college.系号;
                    this.C1CollegeNameTextBox.Text = college.系名;
                    this.C1CollegeSchoolNameTextBox.Text = college.院名;
                }
            }
        }
        #endregion 

        
        #region 使用系号来搜索修改
        private void SearchCollegeConfirmChangeBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var college =
                    (from c in context.院系信息表 where c.系号 == (int)this.C1CollegeIdComboBox.Value select c)
                        .FirstOrDefault();

                if (college == null)
                {
                    if (MessageBox.Show("不存在该系的信息,要添加新的系吗?", "确认信息", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }
                    var newcollege = new 院系信息表
                                         {
                                             系号 = (int)this.C1CollegeIdComboBox.Value,
                                             系名 = this.C1CollegeNameTextBox.Text.Trim(),
                                             院名 = this.C1CollegeSchoolNameTextBox.Text.Trim()
                                         };
                    context.院系信息表.Add(newcollege);
                    context.SaveChanges();
                    MessageBox.Show("添加成功");
                }
                else
                {
                    college.系号 = (int)this.C1CollegeIdComboBox.Value;
                    college.系名 = this.C1CollegeNameTextBox.Text.Trim();
                    college.院名 = this.C1CollegeSchoolNameTextBox.Text.Trim();

                    context.SaveChanges();
                    MessageBox.Show("修改成功");
                }

            }

            this.CollegeInfoViewBtnClick(sender, e);

        }
        #endregion 


        #region 使用系号来删除
        private void SearchCollegeConfirmDeleteBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var college =
                    (from c in context.院系信息表 where c.系号 == (int)this.C1CollegeIdComboBox.Value select c)
                        .FirstOrDefault();

                if (college == null)
                {
                    MessageBox.Show("不存在该系，所以无法删除");
                }
                else
                {
                    context.院系信息表.Remove(college);
                    context.SaveChanges();
                    MessageBox.Show("删除成功");
                }

            }

            this.CollegeInfoViewBtnClick(sender, e);

        }

        #endregion 


        #region 打开Excel文件
        private void CollegeFileOpenBtnClick(object sender, RoutedEventArgs e)
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
            this.CollegeFileNameTextBox.Text = filename;

            var colleges = new List<院系信息表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
                    {
                        colleges.Add(new 院系信息表
                        {
                            系号 = int.Parse(dt.Rows[i][0].ToString()),
                            院名= dt.Rows[i][1].ToString(),
                            系名 = dt.Rows[i][2].ToString(),
                            
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            var vcolleges = (from s in colleges select new College(s)).ToList();

            this.NewCollegeGrid.ItemsSource = vcolleges;

        }
        #endregion


        #region  确认从Excel中添加
        private void CollegeConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewCollegeGrid.ItemsSource;
                if (context.院系信息表 != null)
                {
                    var collegeids = (context.院系信息表.Select(s => s.系号)).ToList();
                    foreach (var college in items.OfType<College>().Select(s => new 院系信息表
                                                                                    {
                                                                                        系号 = s.系号,
                                                                                        院名 = s.院名,
                                                                                        系名 = s.系名
                                                                                    }))
                    {
                        if (collegeids.Contains(college.系号))
                        {
                            MessageBox.Show(string.Format("存在与原来的学号冲突 {0}", college.系号));
                        }
                        else
                        {
                            context.院系信息表.Add(college);
                            context.SaveChanges();
                        }
                    }
                }
                MessageBox.Show("执行完毕");
            }

            this.CollegeInfoViewBtnClick(sender, e);

        }
        #endregion
    }
}
