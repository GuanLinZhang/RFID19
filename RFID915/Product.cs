using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;

namespace RFID915
{
    class Product : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields
        private int id;
        private String productID;
        private String name;
        private String type;
        private double unitPrice;
        private int store;
        private byte[] image;
        #endregion

        #region Properties
        //public int Id
        //{
        //    get => id;
        //    set => id = value;
        //}
        public string ProductID
        {
            get => productID;

            set
            {
                if(value.Length <= 10)
                {
                    this.productID = value;

                }
                else
                {
                    this.productID = value.Substring(0, 10);
                }

                OnPropertyChanged("ProductID");
            }
        }
        public string Name
        {
            get => name;

            set
            {
                if(this.name != value)
                {
                    this.name = value;
                    
                }
                OnPropertyChanged("Name");
            }
        }
        public string Type
        {
            get => type;

            set
            {
                if(this.type != value)
                {
                    this.type = value;
                    
                }
                OnPropertyChanged("Type");
            }

        }
        public double UnitPrice
        {
            get => unitPrice;

            set
            {
                if(value > 0.0 && value < 9999.0)
                {
                    this.unitPrice = value;

                }
                else
                {
                    this.unitPrice = 0;
                }
                OnPropertyChanged("UnitPrice");
            }
        }
        public int Store
        {
            get => store;

            set
            {
                if(value > 0 && value < 9999)
                {
                    this.store = value;
                    
                }
                else
                {
                    this.store = 0;
                }
                OnPropertyChanged("Store");
            }

        }
        public byte[] Image
        {
            get => image;

            set
            {
                this.image = value;
                
                OnPropertyChanged("Image");
            }
        }


        #endregion

        #region Methods
        public Product(int id, string productID, string name, string type, double unitPrice, int store, byte[] image)
        {
            this.id = id;
            this.productID = productID;
            this.name = name;
            this.type = type;
            this.unitPrice = unitPrice;
            this.store = store;
            this.image = image;
        }
        public Product() { }

        public Product(DataRow datarow)
        {
            this.id = (int)datarow["id"];
            this.productID = (string)datarow["productID"];
            this.name = (string)datarow["name"];
            this.type = (string)datarow["type"];
            this.unitPrice = System.Convert.ToDouble(datarow["unitPrice"]);
            this.store = System.Convert.ToInt32(datarow["store"]);
            this.image = (byte[])datarow["image"];

            //trigger it
            OnPropertyChanged("ProductID");
            OnPropertyChanged("Name");
            OnPropertyChanged("Type");
            OnPropertyChanged("UnitPrice");
            OnPropertyChanged("Store");
            OnPropertyChanged("Image");

        }

        private byte[] image2ByteArray() {
            //dummy
            return new byte[1] {0};
        }


        #endregion

        #region EVENTS

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion


    }
}
