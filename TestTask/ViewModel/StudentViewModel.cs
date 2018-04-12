using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TestTask.Notification;
using TestTask.Model;
using TestTask.Commands;
using System.Windows;
using System.Xml.Linq;

namespace TestTask.ViewModel
{
    public class StudentViewModel : PropertyChangedNotification
    {
        private static StudentViewModel studentViewModel;
        private static string Filename = "Resources/Students.xml";

        #region Properties
        public static int Errors { get; set; }

        public static int ErrorsUpd { get; set; }

        public Visibility UpdatingStudent
        {
            get { return GetValue(() => UpdatingStudent); }
            set { SetValue(() => UpdatingStudent, value); }
        }

        public Visibility AddingStudent
        {
            get { return GetValue(() => AddingStudent); }
            set { SetValue(() => AddingStudent, value); }
        }

        public Visibility StudentsList
        {
            get { return GetValue(() => StudentsList); }
            set { SetValue(() => StudentsList, value); }
        }

        public Visibility EmptyStudentsList
        {
            get { return GetValue(() => EmptyStudentsList); }
            set { SetValue(() => EmptyStudentsList, value); }
        }

        public Visibility EditingStudent
        {
            get { return GetValue(() => EditingStudent); }
            set { SetValue(() => EditingStudent, value); }
        }

        public ObservableCollection<Student> Students
        {
            get { return GetValue(() => Students); }
            set
            {
                SetValue(() => Students, value);
            }
        }

        public Student NewStudent
        {
            get { return GetValue(() => NewStudent); }
            set { SetValue(() => NewStudent, value); }
        }

        public Student SelectedStudent
        {
            get { return GetValue(() => SelectedStudent); }
            set { SetValue(() => SelectedStudent, value); }
        }

        public static StudentViewModel SharedViewModel()
        {

            return studentViewModel ?? (studentViewModel = new StudentViewModel());
        }
        
        #endregion


        #region Constructors
        private StudentViewModel()
        {
            Students = new ObservableCollection<Student>();
            if (File.Exists(Filename))
            {
                //Deserialize();
                DeserializeStudentsListAsync();
            }
            else
            {
                XDocument xd = new XDocument();
                xd.Save(Filename);
            }

            Students.CollectionChanged += Students_CollectionChanged;

            NewStudent = new Student();
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
            SaveDataCommand = new RelayCommand(SaveData);

            EditingStudent = Visibility.Collapsed;
            //AddingStudent = Visibility.Collapsed;
            //UpdatingStudent = Visibility.Collapsed;
        }

        #endregion


        #region Commands
        public RelayCommand AddCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand UpdateCommand { get; set; }

        public RelayCommand DeleteCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public RelayCommand SaveDataCommand { get; set; }
        #endregion


        #region Methods
        private void Students_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsStudentsListEmpty())
            {
                StudentsList = Visibility.Collapsed;
                EmptyStudentsList = Visibility.Visible;
            }
            else
            {
                StudentsList = Visibility.Visible;
                EmptyStudentsList = Visibility.Collapsed;
            }
        }

        private bool IsStudentsListEmpty() => Students.Count == 0 ? true : false;

        public void Update(object parameter)
        {
            NewStudent = new Student
            {
                Id = SelectedStudent.Id,
                FirstName = SelectedStudent.FirstName,
                LastName = SelectedStudent.LastName,
                Age = SelectedStudent.Age,
                Gender = SelectedStudent.Gender
            };
            EditingStudent = Visibility.Visible;
        }

        public void Delete(object parameter)
        {
            if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                System.Collections.IList items = (System.Collections.IList)parameter;
                List<Student> studs = new List<Student>(items.Cast<Student>());
                foreach (Student std in studs)
                {
                    Students.Remove(std);
                }
                SelectedStudent = null;
            }
        }

        

        public void Add(object parameter)
        {
            EditingStudent = Visibility.Visible;
            NewStudent = new Student();
            if (Students.Count != 0)
                NewStudent.Id = Students.Last().Id + 1;
            else NewStudent.Id = 0;
        }

        public void Save(object parameter)
        {
            if (Students.Where(x => x.Id == NewStudent.Id).Count() == 0)
            {
                Students.Add(NewStudent);
                NewStudent = new Student();
                EditingStudent = Visibility.Collapsed;
            }
            else
            {

                SelectedStudent.Id = NewStudent.Id;
                SelectedStudent.FirstName = NewStudent.FirstName;
                SelectedStudent.LastName = NewStudent.LastName;
                SelectedStudent.Age = NewStudent.Age;
                SelectedStudent.Gender = NewStudent.Gender;
                SelectedStudent = null;
                EditingStudent = Visibility.Collapsed;
            }
        }


        public bool CanSave(object parameter)
        {
            if (Errors == 0)
                return true;
            else
                return false;
        }

        public bool CanUpdate(object parameter)
        {
            return SelectedStudent == null ? false : true;
        }

        public bool CanDelete(object parameter)
        {
            return SelectedStudent == null ? false : true;
        }

        public void Cancel(object parameter)
        {
            NewStudent = new Student();
            EditingStudent = Visibility.Collapsed;
        }

        public void SaveData(object parameter)
        {
            SerializeStudentsListAsync();
        }

        #region Serialization and deserialization
        private async void SerializeStudentsListAsync()
        {
            bool serialize = await WriteToFile();
            if (serialize) MessageBox.Show("All data is writed!");
        }

        private async Task<bool> WriteToFile()
        {
            return await Task.Run(() =>
            {
                try
                {
                    XDocument xdoc = XDocument.Load(Filename);
                    xdoc.Element("Students").RemoveAll();
                    XElement _Students = xdoc.Element("Students");
                    int _id = 0;
                    foreach (Student std in Students)
                    {
                        _Students.Add(
                            new XElement("Student",
                                new XAttribute("Id", _id.ToString()),
                                new XElement("Firstname", std.FirstName),
                                new XElement("Last", std.LastName),
                                new XElement("Age", std.Age.ToString()),
                                new XElement("Gender", ((int)std.Gender).ToString())
                            ));
                    }
                    xdoc.Save(Filename);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Writing faliled!\n" + e.Message);
                    return false;
                }

            });
        }

        private async void DeserializeStudentsListAsync()
        {
            bool deserialize = await LoadFromFile();
        }

        private async Task<bool> LoadFromFile()
        {

            return await Task.Run(() =>
            {
                try
                {
                    XDocument xdoc = XDocument.Load(Filename);
                    foreach (XElement student in xdoc.Element("Students").Elements("Student"))
                    {
                        Students.Add(
                            new Student
                            {
                                FirstName = student.Element("FirstName").Value,
                                LastName = student.Element("Last").Value,
                                Age = Int32.Parse(student.Element("Age").Value),
                                Gender = student.Element("Gender").Value == "0" ? Gender.Male : Gender.Female
                            });
                    }
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Reading faliled!\n" + e.Message);
                    return false;
                }
            }
            );
        }
        #endregion

        #endregion


    }

}
