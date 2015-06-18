namespace EvalonServer.Lib
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Documents;

    #region 登录类型的另一个定义
    /// <summary>
    /// The login.
    /// </summary>
    public class Login
    {
        public string 用户名 { get; set; }

        public string 密码 { get; set; }

        public string 用户类型 { get; set; }

        public Login(登录信息表 login)
        {
            this.用户名 = login.用户名;
            this.密码 = login.密码;
            this.用户类型 = login.用户类型;
        }

        public bool Check()
        {
            var logintype = new List<string> { "学生", "教师", "管理员" };
            return logintype.Contains(this.用户类型);
        }

        public static bool LoginCheck(登录信息表 login)
        {
            var logintype = new List<string> { "学生", "教师", "管理员" };
            return logintype.Contains(login.用户类型);
        }


        public static bool LoginCheck(Login login)
        {
            var logintype = new List<string> { "学生", "教师", "管理员" };
            return logintype.Contains(login.用户类型);
        }
    }
    #endregion

    #region 学生类型的另一个定义

    public class Student
    {
        public string 学号 { get; set; }

        public string 姓名 { get; set; }

        public string 性别 { get; set; }

        public int? 年龄 { get; set; }

        public int? 系号 { get; set; }

        public string 籍贯 { get; set; }

        public string 民族 { get; set; }



        public Student(学生信息表 student)
        {
            this.学号 = student.学号;
            this.姓名 = student.姓名;
            this.性别 = student.性别;
            this.年龄 = student.年龄;
            this.系号 = student.系号;
            this.籍贯 = student.籍贯;
            this.民族 = student.民族;
        }


        public static bool StudentCheck(学生信息表 student)
        {
            using (var context = new EvalonEntities())
            {
                var sex = new[] { "男", "女" };
                var departmentids = (from d in context.院系信息表 select d.系号).ToList();
                return student.系号 != null && departmentids.Contains((int)student.系号) && (student.年龄 != null)
                       && (student.年龄 > 0) && sex.Contains(student.性别);
            }
        }
    }

    #endregion

    #region 教师类型的另一个定义

    public class Teacher
    {
        public string 工号 { set; get; }

        public string 姓名 { set; get; }

        public string 性别 { get; set; }

        public string 职称 { set; get; }

        public Teacher(教师信息表 teacher)
        {
            this.工号 = teacher.工号;
            this.姓名 = teacher.姓名;
            this.性别 = teacher.性别;
            this.职称 = teacher.职称;
        }

        public static bool TeacherCheck(教师信息表 teacher)
        {
            var sextype = new List<string> { "男", "女" };
            return sextype.Contains(teacher.性别);
        }
    }


    #endregion

    #region 课程类型的另一个定义

    public class Course
    {
        public string 课程号 { get; set; }
        public string 课程名称 { get; set; }
        public int? 学分 { get; set; }
        public int? 学时 { get; set; }
        public string 上课地点 { get; set; }
        public string 上课时间 { get; set; }
        public int? 预定人数 { get; set; }
        public int? 已选人数 { get; set; }

        public Course(课程信息表 course)
        {
            this.课程号 = course.课程号;
            this.课程名称 = course.课程名称;
            this.学分 = course.学分;
            this.学时 = course.学时;
            this.上课地点 = course.上课地点;
            this.上课时间 = course.上课时间;
            this.预定人数 = course.预定人数;
            this.已选人数 = course.已选人数;
        }
        
    }

    #endregion

    #region 院系类型的另一个定义

    public class College
    {
        public int 系号 { get; set; }
        public string 院名 { get; set; }
        public string 系名 { get; set; }

        public College(院系信息表 college)
        {
            this.系号 = college.系号;
            this.系名 = college.系名;
            this.院名 = college.院名;
        }

    }
    #endregion

    #region 培养计划类型的另一个定义

    public class TrainingPlan
    {
        public string 课程号 { get; set; }
        public int? 学期 { get; set; }
        public string 课程名 { get; set; }
        public string 院名 { get; set; }
        public string 系名 { get; set; }
        public int? 学分 { get; set; }

        public TrainingPlan(培养计划表 trainingplan)
        {
            this.课程号 = trainingplan.课程号;
            this.学期 = trainingplan.学期;
            this.课程名 = trainingplan.课程信息表.课程名称;
            this.院名 = trainingplan.院系信息表.院名;
            this.系名 = trainingplan.院系信息表.系名;
            this.学分 = trainingplan.课程信息表.学分;
        }

        public static bool TrainingPlanCheck(培养计划表 trainingplan)
        {
            using (var context = new EvalonEntities())
            {
                var courses = (from c in context.课程信息表 select c.课程号).ToList();
                var departments = (from d in context.院系信息表 select d.系号).ToList();
                return trainingplan.系号 != null
                       && (courses.Contains(trainingplan.课程号) && departments.Contains((int)trainingplan.系号))
                       && 1 <= trainingplan.学期 && trainingplan.学期 <= 8;
            }
        }


    }


    #endregion

    #region 任课信息类型的另一个定义

    public class TeachingCourse
    {
        public string 工号 { get; set; }
        public string 教师姓名 { get; set; }
        public string 课程名 { get; set; }
        public string 课程号 { get; set; }

        public TeachingCourse(任课信息表 teachingcourse)
        {
            this.工号 = teachingcourse.工号;
            this.课程号 = teachingcourse.课程号;
            this.教师姓名 = teachingcourse.教师信息表.姓名;
            this.课程名 = teachingcourse.课程信息表.课程名称;
        }

        public static bool TeachngCourseCheck(任课信息表 teachingcourse)
        {
            using (var context = new EvalonEntities())
            {
                var courseids = (from c in context.课程信息表 select c.课程号).ToList();
                var teacherids = (from t in context.教师信息表 select t.工号).ToList();
                return courseids.Contains(teachingcourse.课程号) && teacherids.Contains(teachingcourse.工号);
            }
        }
    }
    #endregion 

    #region 考试信息类型的另一个定义

    public class Test
    {
        public string 课程号 { get; set; }
        public string 课程名称 { get; set; }
        public string 考试地点 { get; set; }
        public string 考试时间 { get; set; }

        public Test(考试信息表 test)
        {
            this.课程号 = test.课程号;
            this.课程名称 = test.课程信息表.课程名称;
            this.考试地点 = test.考试地点;
            this.考试时间 = test.考试时间;
        }

        public static bool TestCheck(考试信息表 test)
        {
            using (var context = new EvalonEntities())
            {
                var courseids = (context.课程信息表.Select(c => c.课程号)).ToList();
                return courseids.Contains(test.课程号);
            }
        }
    }

    #endregion
 
    #region 选课类型的另一个定义

    public class ChooseCourse
    {
        public static bool ChooseCourseCheck(选课信息表 choosecourse)
        {
            using (var context = new EvalonEntities())
            {
                var studentids = (context.学生信息表.Select(s => s.学号)).ToList();
                var courseids = (context.课程信息表.Select(c => c.课程号)).ToList();
                return studentids.Contains(choosecourse.学号) && courseids.Contains(choosecourse.课程号);

            }
        }
    }
    #endregion 

}