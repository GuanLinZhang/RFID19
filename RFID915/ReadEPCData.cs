using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFID915
{
    class ReadEPCData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields
        private long id;
        private string epcstring;
        //private int times;
        #endregion

        #region Properties
        public long Id { get => id; set => id = value; }
        public string EPCstring { get => epcstring; set => epcstring = value; }
        //public int Times { get => times; set => times = value; }
        #endregion

        #region Methods
        public ReadEPCData(long id, string epcstring)
        {
            this.id = id;
            this.epcstring = epcstring;
            
        }
        public ReadEPCData() { }
        #endregion

        #region Events
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }
        #endregion
    }
}
