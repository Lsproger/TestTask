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

namespace TestTask.ViewModel
{
    public class StudentViewModel : PropertyChangedNotification
    {
        private static StudentViewModel studentViewModel;
        private static string Filename = Directory.GetCurrentDirectory() + "\\Output.xml";

        public Visibility IsUpdatingStudent
        {
            get { return GetValue(()=>IsUpdatingStudent); }
            set { SetValue(()=>IsUpdatingStudent, value); }
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
            if (File.Exists(Filename))
            {
                Students = Deserialize<ObservableCollection<Student>>();
            }
            else
            {
                Students = new ObservableCollection<Student>();
                Students.Add(new Student { Id = 1, FirstName = "William", LastName = "William", Age = 23, Gender = Gender.Male });
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
            var result = Serialize<ObservableCollection<Student>>(Students);
            if (result) MessageBox.Show("Data Saved Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show("Data Not Saved", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool Serialize<T>(T value)
        {
            if (value == null)
            {
                return false;
            }
            try
            {
                XmlSerializer _xmlserializer = new XmlSerializer(typeof(T));
                Stream stream = new FileStream(Filename, FileMode.CreateNew);
                _xmlserializer.Serialize(stream, value);
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private T Deserialize<T>()
        {
            if (string.IsNullOrEmpty(Filename))
            {
                return default(T);
            }
            try
            {
                XmlSerializer _xmlSerializer = new XmlSerializer(typeof(T));
                Stream stream = new FileStream(Filename, FileMode.Open, FileAccess.Read);
                var result = (T)_xmlSerializer.Deserialize(stream);
                stream.Close();
                return result;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }

}
