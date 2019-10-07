using System;
using System.Text;

namespace shoppingCartLib
{
    /// <summary>
    /// This is the Coupon Model for the control;
    /// Author: Tao Liu
    /// Date: 21/08/2016
    /// </summary>
    public class Coupon
    {
        #region Fields

        private double _discountPercentage;
        private bool _isValid;

        #endregion

        #region Properties

        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ApplicableCategory { get; set; }
        public string ApplicableItemCode { get; set; }
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
            }
        }

        public bool IsValid
        {
            get
            {
                // Get the current date
                DateTime dateToday = DateTime.Now;

                // Check if current date is between start date and end date
                _isValid = (dateToday.Ticks > StartDate.Ticks) && (dateToday.Ticks < EndDate.Ticks);

                return _isValid;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Coupon code</param>
        /// <param name="startDate">Coupon start date</param>
        /// <param name="endDate">Coupon end date</param>
        /// <param name="disCount">Coupon discount percentage</param>
        /// <param name="applicableCategory">Applicable category</param>
        /// <param name="applicableItemCode">Applicable item</param>
        public Coupon(string code, DateTime startDate, DateTime endDate, double disCount, string applicableCategory, string applicableItemCode)
        {
            this.Code = code;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.DiscountPercentage = disCount;
            this.ApplicableCategory = applicableCategory;
            this.ApplicableItemCode = applicableItemCode;
        }

        /// <summary>
        /// Override the ToString() method;
        /// To generate a message shows the details of this coupon,
        /// including if it's valid during this period
        /// </summary>
        /// <returns>A string description of a coupon object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Discount: " + DiscountPercentage + "%");
            sb.AppendLine("Coupon Code: " + Code);
            sb.AppendLine("Start Date: " + StartDate);
            sb.AppendLine("End Date: " + EndDate);

            return sb.ToString();
        }
        #endregion
    }
}
