using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8PuzzelGame
{
    // integer with INotifyPropertyChanged
    public class MyInt : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseEventHandler(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                RaiseEventHandler("Value");
            }
        }

        private int value;
    }

    // string with INotifyPropertyChanged
    public class MyString : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseEventHandler(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                RaiseEventHandler("Value");
            }
        }

        private string value;
    }
}
