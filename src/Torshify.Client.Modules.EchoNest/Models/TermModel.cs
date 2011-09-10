using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Client.Modules.EchoNest.Models
{
    public class TermModel : NotificationObject
    {
        #region Fields

        private string _name;
        private double _boost;
        private bool _require;
        private bool _ban;

        #endregion Fields

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public double Boost
        {
            get { return _boost; }
            set
            {
                if (_boost != value)
                {
                    _boost = value;
                    RaisePropertyChanged("Boost");
                }
            }
        }

        public bool Require
        {
            get { return _require; }
            set
            {
                if (_require != value)
                {
                    _require = value;
                    RaisePropertyChanged("Require");
                }
            }
        }

        public bool Ban
        {
            get { return _ban; }
            set
            {
                if (_ban != value)
                {
                    _ban = value;
                    RaisePropertyChanged("Ban");
                }
            }
        }

        #endregion Properties
    }
}