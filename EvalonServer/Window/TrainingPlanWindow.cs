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

        #region 初始化显示窗口
        private void TrainingPlanViewBtnClick(object sender, RoutedEventArgs e)
        {
            this.ClearWindow();
            this.TrainingPlanInfoView.Visibility = Visibility.Visible;
        }
        #endregion

        #region 使用系号来显示培养计划列表
        private void DisplayTraningPlanByDepartmentIdBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var trainingplans =
                    (from t in context.培养计划表
                     where t.系号 == this.DisplayTrainingPlanByDepartmentIdNumericBox.Value
                     select t).ToList();

                var vtrainingplan = (from t in trainingplans select new TrainingPlan(t)).ToList();
                this.TrainingPlanGrid.ItemsSource = vtrainingplan;
            }

        }
        #endregion

        #region 使用系号和课程号来搜索培养计划
        private void SearchTrainingPlanBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var trainingplan = (from t in context.培养计划表
                                    where
                                        t.系号 == this.SearchTrainingPlanByDepartmentIdNumericBox.Value
                                        && t.课程号 == this.SearchTrainingPlanByCourseIdTextBox.Text.Trim()
                                    select t).FirstOrDefault();
                if (trainingplan != null)
                {
                    this.C1SearchTrainingPlanCourseIdTextBox.Text = trainingplan.课程号;
                    if (trainingplan.学期 != null)
                    {
                        this.C1SearchTrainingPlanTermNumericBox.Value = (int)trainingplan.学期;
                    }
                    if (trainingplan.系号 != null)
                    {
                        this.C1SearchTrainingPlanDepartmentIdNumericBox.Value = (int)trainingplan.系号;
                    }
                }
                else
                {
                    MessageBox.Show("不存在该项的培养计划");
                }
            }
        }
        #endregion

        #region 删除搜索的培养计划项目
        private void SearchTrainingPlanConfirmDeleteBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var trainingplan = (from t in context.培养计划表
                                    where
                                        t.系号 == this.C1SearchTrainingPlanDepartmentIdNumericBox.Value
                                        && t.课程号 == this.C1SearchTrainingPlanCourseIdTextBox.Text.Trim()
                                    select t).FirstOrDefault();
                if (trainingplan != null)
                {
                    if (MessageBox.Show("要删除该学生吗?", "确认信息", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        context.培养计划表.Remove(trainingplan);
                        context.SaveChanges();
                        MessageBox.Show("删除成功");
                    }
                }
                else
                {
                    MessageBox.Show("不存在该课程，无法删除");
                }
            }

            this.TrainingPlanViewBtnClick(sender, e);
            this.DisplayTraningPlanByDepartmentIdBtnClick(sender, e);

        }
        #endregion

        #region 修改或增加培养计划项目
        private void SearchTrainingPlanConfirmChangeBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var trainingplan = (from t in context.培养计划表
                                    where
                                        t.系号 == this.C1SearchTrainingPlanDepartmentIdNumericBox.Value
                                        && t.课程号 == this.C1SearchTrainingPlanCourseIdTextBox.Text.Trim()
                                    select t).FirstOrDefault();
                if (trainingplan == null)
                {
                    if (MessageBox.Show("不存在该培养计划，要添加吗?", "确认信息", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        var newtraingplan = new 培养计划表
                                                {
                                                    课程号 = this.C1SearchTrainingPlanCourseIdTextBox.Text.Trim(),
                                                    学期 = (int?)this.C1SearchTrainingPlanTermNumericBox.Value,
                                                    系号 = (int?)this.C1SearchTrainingPlanDepartmentIdNumericBox.Value
                                                };
                        if (TrainingPlan.TrainingPlanCheck(newtraingplan))
                        {
                            context.培养计划表.Add(newtraingplan);
                            context.SaveChanges();
                            MessageBox.Show("添加成功");
                        }
                        else
                        {
                            MessageBox.Show("参数不规范,无法保存");
                        }
                    }
                }
                else
                {

                    trainingplan.学期 = (int?)this.C1SearchTrainingPlanTermNumericBox.Value;
                    context.SaveChanges();
                    MessageBox.Show("修改成功");

                }
            }

            this.TrainingPlanViewBtnClick(sender, e);
            this.DisplayTraningPlanByDepartmentIdBtnClick(sender, e);
        }
        #endregion

        #region 打开Excel文件
        private void TrainingPlanFileOpenButtonClick(object sender, RoutedEventArgs e)
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
            this.TrainingPlanFileNameTextBox.Text = filename;

            var trainingplans = new List<培养计划表>();
            try
            {
                using (var excelHelper = new ExcelHelper(filename))
                {
                    var dt = excelHelper.ExcelToDataTable("Sheet1", true);
                    if (dt == null) return;
                    for (var i = 0; i < dt.Rows.Count; ++i)
                    {
                        trainingplans.Add(new 培养计划表
                        {
                            系号 = int.Parse(dt.Rows[i][0].ToString()),
                            课程号 = dt.Rows[i][1].ToString(),
                            学期 = int.Parse(dt.Rows[i][2].ToString())

                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.AdminWindow_FileOpenBtnClick_Exception__ + ex.Message);
            }

            this.NewTrainingPlanGrid.ItemsSource = trainingplans;
        }
        #endregion

        #region 确认从Excel中添加
        private void TrainingPlanConfirmAddBtnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new EvalonEntities())
            {
                var items = this.NewTrainingPlanGrid.ItemsSource;
                var trainingplandepartmentids = (from s in context.培养计划表 select s.系号).ToList();
                var trainingplancourseids = (from s in context.培养计划表 select s.课程号).ToList();

                foreach (var trainingplan in items)
                {
                    var tp = trainingplan as 培养计划表;
                    if (tp != null && trainingplandepartmentids.Contains(tp.系号) && trainingplancourseids.Contains(tp.课程号))
                    {
                        MessageBox.Show("该培养计划已存在");
                    }
                    else
                    {
                        if (TrainingPlan.TrainingPlanCheck((trainingplan as 培养计划表)))
                        {
                            context.培养计划表.Add(trainingplan as 培养计划表);
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

            this.TrainingPlanViewBtnClick(sender, e);
            this.DisplayTraningPlanByDepartmentIdBtnClick(sender, e);

        }
        #endregion 

    }
}
