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

        public Visibility IsUpdatingStudent
        {
            get { return GetValue(() => IsUpdatingStudent); }
            set { SetValue(() => IsUpdatingStudent, value); }
        }

        public Visibility IsAddingStudent
        {
            get { return GetValue(() => IsAddingStudent); }
            set { SetValue(() => IsAddingStudent, value); }
        }

        public ObservableCollection<Student> Students
        {
            get { return GetValue(() => Students); }
            set { SetValue(() => Students, value); }
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


        public RelayCommand AddCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand UpdateCommand { get; set; }

        public RelayCommand DeleteCommand { get; set; }

        public RelayCommand ClearCommand { get; set; }

        public RelayCommand SaveDataCommand { get; set; }

        public static int Errors { get; set; }

        public static int ErrorsUpd { get; set; }

        public static StudentViewModel SharedViewModel()
        {

            return studentViewModel ?? (studentViewModel = new StudentViewModel());
        }

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

            NewStudent = new Student();
            DeleteCommand = new RelayCommand(Delete);
            AddCommand = new RelayCommand(Add);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            SaveCommand = new RelayCommand(Save, CanSave);
            ClearCommand = new RelayCommand(Clear);
            SaveDataCommand = new RelayCommand(SaveData);

            IsAddingStudent = Visibility.Collapsed;
            IsUpdatingStudent = Visibility.Collapsed;
        }


        public void Update(object parameter)
        {
            IsUpdatingStudent = Visibility.Visible;
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
            IsAddingStudent = Visibility.Visible;
            NewStudent = new Student();
        }

        public void Save(object parameter)
        {
            Students.Add(NewStudent);
            NewStudent = new Student();
            IsAddingStudent = Visibility.Collapsed;
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

        public void Clear(object parameter)
        {
            NewStudent = new Student();
        }

        public void SaveData(object parameter)
        {
            SerializeStudentsListAsync();
        }

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
            if (deserialize) MessageBox.Show("All data is readed!");
        }

        private void Deserialize()
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
        }


        private async Task<bool> LoadFromFile()
        {
            
            return await Task.Run(()=>
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
    }

}
