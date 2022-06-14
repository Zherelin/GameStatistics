using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;

namespace DateBase_10
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            using (scheduleEntities db = new scheduleEntities())
            {
                var departments = db.Departments;
                foreach(var dep in departments)
                {
                    box_dep.Items.Add(dep.Name);
                }

                var teachers = db.Teachers;
                foreach (var t in teachers)
                {
                    box_teacher.Items.Add(t.Surname);
                }

                box_condition.Items.Add("Основная");
                box_condition.Items.Add("По совместительству");
            }
        }

        private void btn_addDep_Click(object sender, RoutedEventArgs e)
        {
            using (scheduleEntities db = new scheduleEntities())
            {
                Departments department = new Departments
                {
                    Name = dep_name.Text,
                    Abbreviation = dep_abbrev.Text,
                    Address = dep_address.Text,
                    Phone = dep_phone.Text,
                    Website = dep_website.Text
                };

                db.Departments.Add(department);
                dep_info.Content = $"Кафедра '{department.Abbreviation}' добавлена!";
                db.SaveChanges();
            }

            dep_name.Text = "";
            dep_abbrev.Text = "";
            dep_address.Text = "";
            dep_phone.Text = "";
            dep_website.Text = "";
        }

        private void btn_addPost_Click(object sender, RoutedEventArgs e)
        {
            using (scheduleEntities db = new scheduleEntities())
            {
                Posts post = new Posts
                {
                    Name = post_name.Text,
                };

                db.Posts.Add(post);
                post_info.Content = $"Должность '{post.Name}' добавлена!";
                db.SaveChanges();
            }

            post_name.Text = "";
        }

        private void btn_addStaff_Click(object sender, RoutedEventArgs e)
        {
            using (scheduleEntities db = new scheduleEntities())
            {
                Staffs staff = new Staffs
                {
                    DepartmentId = int.Parse(staff_depID.Text),
                    PostId = int.Parse(staff_postID.Text),
                    TeacherId = int.Parse(staff_teacherID.Text),
                    Rate = int.Parse(staff_rate.Text),
                    Condition = staff_condition.Text
                };

                db.Staffs.Add(staff);
                staff_info.Content = "Данные успешно добавлены!";
                db.SaveChanges();
            }

            staff_depID.Text = "";
            staff_postID.Text = "";
            staff_teacherID.Text = "";
            staff_rate.Text = "";
            staff_condition.Text = "";
        }

        private void btn_addTeacher_Click(object sender, RoutedEventArgs e)
        {
            using (scheduleEntities db = new scheduleEntities())
            {
                Teachers teacher = new Teachers
                {
                    Surname = teach_surname.Text,
                    Name = teach_name.Text,
                    Patronymic = teach_patron.Text,
                    Datebirth = DateTime.Parse(teach_date.Text),
                    Phone = teach_phone.Text,
                    Degree = teach_degree.Text,
                    Title = teach_title.Text,
                    Address = teach_address.Text
                };

                db.Teachers.Add(teacher);
                teach_info.Content = $"Преподаватель '{teacher.Surname}' добавлен!";
                db.SaveChanges();
            }

            teach_surname.Text = "";
            teach_name.Text = "";
            teach_patron.Text = "";
            teach_date.Text = "";
            teach_phone.Text = "";
            teach_degree.Text = "";
            teach_title.Text = "";
            teach_address.Text = "";
        }

        private void btn_showDep_Click(object sender, RoutedEventArgs e)
        {
            if(box_dep.SelectedItem != null)
            {
                string message = "Кол-во сотрудников кафедры: ";

                using (scheduleEntities db = new scheduleEntities())
                {
                    // Кол-во сотрудников
                    var countTeachers = db.Staffs.Where(s => s.Departments.Name == box_dep.SelectedItem.ToString())
                        .Include(s => s.Teachers).Count();
                    message += $"{countTeachers}\n";

                    // Должность - Ставка
                    var staffs = db.Staffs.Where(s => s.Departments.Name == box_dep.SelectedItem.ToString())
                        .Include(s => s.Posts);
                    foreach (var s in staffs)
                    {
                        message += $"Должность: {s.Posts.Name} - Ставка: {s.Rate}\n";
                    }
                }

                MessageBox.Show(message, box_dep.SelectedItem.ToString());
            }
            else
            {
                MessageBox.Show("Элемент не выбран!", "Ошибка");
            }
        }

        private void btn_showTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (box_teacher.SelectedItem != null)
            {
                string message = "";

                using (scheduleEntities db = new scheduleEntities())
                {
                    var staff = db.Staffs.Where(s => s.Teachers.Surname == box_teacher.SelectedItem.ToString())
                        .Include(s => s.Departments)
                        .Include(s => s.Posts)
                        .Include(s => s.Teachers)
                        .FirstOrDefault();

                    message += $"ФИО: {staff.Teachers.Surname} {staff.Teachers.Name} {staff.Teachers.Patronymic}\n";
                    message += $"Должность: {staff.Posts.Name}\n";
                    message += $"Условие работы: {staff.Condition}\n";
                    message += $"Кафедра: {staff.Departments.Name}";
                }

                MessageBox.Show(message, "Информация о преподавателе");
            }
            else
            {
                MessageBox.Show("Элемент не выбран!", "Ошибка");
            }
        }

        private void btn_showCondition_Click(object sender, RoutedEventArgs e)
        {
            if (box_condition.SelectedItem != null)
            {
                string message = $"Сотрудников работающих '{box_condition.SelectedItem}' - ";

                using (scheduleEntities db = new scheduleEntities())
                {
                    var countWorkers = db.Staffs.Where(s => s.Condition == box_condition.SelectedItem.ToString()).Count();
                    message += countWorkers;
                }

                MessageBox.Show(message, "О Условиях");
            }
            else
            {
                MessageBox.Show("Элемент не выбран!", "Ошибка");
            }
        }
    }
}
