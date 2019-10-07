using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFID915
{
    class CheckOutData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields

        private int id;
        private string name;
        private double unitPrice;
        private byte[] image;
        private int count;

        public int Id
        {
            get => id;
            set
            {
                if(this.id != value)
                {
                    this.id = value;
                }
                OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get => name;
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                }
                OnPropertyChanged("Name");
            }
        }
        public double UnitPrice
        {
            get => unitPrice;
            set
            {
                if (this.unitPrice != value)
                {
                    this.unitPrice = value;
                }
                OnPropertyChanged("UnitPrice");
            }
        }
        public byte[] Image
        {
            get => image;
            set
            {
                if (this.image != value)
                {
                    this.image = value;
                }
                OnPropertyChanged("Image");
            }
        }
        public int Count
        {
            get => count;
            set
            {
                if (this.count != value)
                {
                    this.count = value;
                }
                OnPropertyChanged("Count");
            }
        }
        

        #endregion

        #region Events
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }
        #endregion
    }
}
