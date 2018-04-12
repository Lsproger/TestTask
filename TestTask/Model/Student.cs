using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask.Notification;

namespace TestTask.Model
{
    public class Student : PropertyChangedNotification
    {
        [Required(ErrorMessage = "Id is Required")]
        public int Id
        {
            get { return GetValue(() => Id); }
            set { SetValue(() => Id, value); }
        }

        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(50, ErrorMessage = "Name exceeded 50 letters")]
        public string FirstName
        {
            get { return GetValue(() => FirstName); }
            set { SetValue(() => FirstName, value); }
        }

        [Required(ErrorMessage = "Lastname is Required")]
        [MaxLength(50, ErrorMessage = "Lastname exceeded 50 letters")]
        public string LastName
        {
            get { return GetValue(() => LastName); }
            set { SetValue(() => LastName, value); }
        }


        [Range(16, 100, ErrorMessage = "Age should be between 16 to 100")]
        public int Age
        {
            get { return GetValue(() => Age); }
            set { SetValue(() => Age, value); }
        }

        [Required(ErrorMessage = "Gender is Required")]
        public Gender Gender
        {
            get { return GetValue(() => Gender); }
            set { SetValue(() => Gender, value); }
        }
    }

    public enum Gender
    {
        Male,
        Female
    }
}
