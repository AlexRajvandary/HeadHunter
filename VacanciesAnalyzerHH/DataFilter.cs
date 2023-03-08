using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VacanciesAnalyzerHH
{
    public class DataFilter : INotifyPropertyChanged
    {
        private string adress;
        private string companyName;
        private string contacts;
        private string name;
        private string published;
        private double? salaryFrom;
        private double? salaryTo;
        private string schedule;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Adress
        {
            get => adress;
            set
            {
                if (adress != value)
                {
                    adress = value;
                    OnProperyChanged();
                }
            }
        }

        public string CompanyName
        {
            get => companyName;
            set
            {
                if (companyName != value)
                {
                    companyName = value;
                    OnProperyChanged();
                }
            }
        }

        public string Contacts
        {
            get => contacts;
            set
            {
                if (contacts != value)
                {
                    contacts = value;
                    OnProperyChanged();
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnProperyChanged();
                }
            }
        }

        public string Published
        {
            get => published;
            set
            {
                if (published != value)
                {
                    published = value;
                    OnProperyChanged();
                }
            }
        }

        public double? SalaryFrom
        {
            get => salaryFrom;
            set
            {
                if (salaryFrom != value)
                {
                    salaryFrom = value;
                    OnProperyChanged();
                }
            }
        }

        public double? SalaryTo
        {
            get => salaryTo;
            set
            {
                if (salaryTo != value)
                {
                    salaryTo = value;
                    OnProperyChanged();
                }
            }
        }

        public string Schedule
        {
            get => schedule;
            set
            {
                if (schedule != value)
                {
                    schedule = value;
                    OnProperyChanged();
                }
            }
        }

        public void OnProperyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
