using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Misc
{
    public class Company
    {
        #region Constructor

        public Company()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #endregion

        #region Properties

        private string _CompName;
        private string _Contact;
        private string _Password;
        private string _UserName;
        private string _Addr1;
        private string _Addr2;
        private string _Phone;
        private string _State;
        private string _Zip;
        private string _City;
        private string _EMail;

        private string _BillAddr1;
        private string _BillAddr2;
        private string _BillCity;
        private string _BillState;
        private string _BillZip;

        public string BillAddr1
        {
            get
            {
                return _BillAddr1;
            }
            set
            {
                _BillAddr1 = value;
            }
        }

        public string BillAddr2
        {
            get
            {
                return _BillAddr2;
            }
            set
            {
                _BillAddr2 = value;
            }
        }

        public string BillCity
        {
            get
            {
                return _BillCity;
            }
            set
            {
                _BillCity = value;
            }
        }

        public string BillState
        {
            get
            {
                return _BillState;
            }
            set
            {
                _BillState = value;
            }
        }

        public string BillZip
        {
            get
            {
                return _BillZip;
            }
            set
            {
                _BillZip = value;
            }
        }

        public string CompName
        {
            get
            {
                return _CompName;
            }
            set
            {
                _CompName = value;
            }
        }

        public string Contact
        {
            get
            {
                return _Contact;
            }
            set
            {
                _Contact = value;
            }
        }

        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        public string Addr1
        {
            get
            {
                return _Addr1;
            }
            set
            {
                _Addr1 = value;
            }
        }

        public string Addr2
        {
            get
            {
                return _Addr2;
            }
            set
            {
                _Addr2 = value;
            }
        }

        public string Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                _Phone = value;
            }
        }

        public string State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
            }
        }

        public string Zip
        {
            get
            {
                return _Zip;
            }
            set
            {
                _Zip = value;
            }
        }

        public string City
        {
            get
            {
                return _City;
            }
            set
            {
                _City = value;
            }
        }

        public string EMail
        {
            get
            {
                return _EMail;
            }
            set
            {
                _EMail = value;
            }
        }

        private bool _Shipper = false;
        private bool _Account = true;
        private string _AccountNbr = "";
        private int _AcctMgrID = 0;
        private bool _BillToSame = false;
        private string _Terms = "";

        #endregion

        #region Fields

        public bool Shipper
        {
            get
            {
                return _Shipper;
            }
            set
            {
                _Shipper = value;
            }
        }
        public bool Account
        {
            get
            {
                return _Account;
            }
            set
            {
                _Account = value;
            }
        }
        public bool BillToSame
        {
            get
            {
                return _BillToSame;
            }
            set
            {
                _BillToSame = value;
            }
        }
        public string AccountNbr
        {
            get
            {
                return _AccountNbr;
            }
            set
            {
                _AccountNbr = value;
            }
        }
        public string Terms
        {
            get
            {
                return _Terms;
            }
            set
            {
                _Terms = value;
            }
        }
        public int AcctMgrID
        {
            get
            {
                return _AcctMgrID;
            }
            set
            {
                _AcctMgrID = value;
            }
        }

        #endregion
    }
}