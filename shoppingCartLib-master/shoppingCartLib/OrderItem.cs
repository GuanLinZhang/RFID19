using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace shoppingCartLib
{
    /// <summary>
    /// This is the OrderItem Model for the control;
    /// Author: Tao Liu
    /// Date: 21/08/2016
    /// </summary>
    public class OrderItem : Comparer<OrderItem>, INotifyPropertyChanged
    {

        #region Fields

        private int _quantity;
        private double _unitPrice;
        private double _taxPercentage;
        private double _discountPercentage;
        private bool _isDiscountApplied;

        // Reference: How to: Implement Property Change Notification, https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string PicUrl { get; set; }
        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                _quantity = value;

                // Reference: How to: Implement Property Change Notification, https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx

                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Quantity");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("TotalPrice");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("TaxPrice");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DiscountedTotalPrice");
            }
        }

        public double UnitPrice
        {
            get
            {
                return _unitPrice;
            }

            protected set
            {
                _unitPrice = value >= 0 ? value : 0;
            }
        }

        public double TotalPrice => UnitPrice * Quantity;

        public double TaxPrice => TaxPercentage * TotalPrice * 0.01;

        public double TaxPercentage
        {
            get
            {
                return _taxPercentage;
            }
            protected set
            {
                if (value >= 0 && value <= 100)
                {
                    _taxPercentage = value;
                }
                else
                {
                    _taxPercentage = 100;
                }
            }
        }

        public double DiscountPercentage
        {
            get
            {
                return _discountPercentage;
            }
            set
            {
                if (value >= 0 && value < 100)
                {
                    _discountPercentage = value;
                }
                else
                {
                    _discountPercentage = 100;
                }

                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DiscountPercentage");
            }
        }

        public double DiscountedTotalPrice => TotalPrice * (DiscountPercentage * 0.01);

        public bool IsDiscountApplied
        {
            get
            {
                return _isDiscountApplied;
            }
            set
            {
                _isDiscountApplied = value;

                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("IsDiscountApplied");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DiscountedTotalPrice");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">OrderItem ID</param>
        /// <param name="name">OrderItem name</param>
        /// <param name="uniPrice">OrderItem unit price</param>
        /// <param name="taxPercentage">OrderItem tax percentage</param>
        /// <param name="category">OrderItem category</param>
        /// <param name="picUrl">Picture Url</param>
        public OrderItem(string id, string name, double uniPrice, double taxPercentage, string category, string picUrl)
        {
            this.Id = id;
            this.Name = name;
            this.UnitPrice = uniPrice;
            this.TaxPercentage = taxPercentage;
            this.Category = category;
            this.PicUrl = picUrl;
            this.Quantity = 0;
            this._isDiscountApplied = false;
            this.DiscountPercentage = 0;
        }

        /// <summary>
        /// Override the ToString() method;
        /// To generate an item string description
        /// </summary>
        /// <returns>An string description of the item</returns>
        public override string ToString()
        {
            string unitPriceAndQuantity = UnitPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")) + "*" + Quantity;
            string discount = IsDiscountApplied ? ("Dis.:" + DiscountPercentage + "%\t-" + DiscountedTotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ"))) : null;
            string totalPrice = TotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ"));
            return Name + "\n" + unitPriceAndQuantity + "\t" + totalPrice + "\t" + discount;
        }

        /// <summary>
        /// Overide the Compare Method;
        /// To compare the items by item ID;
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(OrderItem x, OrderItem y)
        {
            return x.Id.CompareTo(y.Id);
        }

        #endregion

        #region Events

        /// <summary>
        /// Create the OnPropertyChanged method to raise the event
        /// Reference: How to: Implement Property Change Notification, https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}

