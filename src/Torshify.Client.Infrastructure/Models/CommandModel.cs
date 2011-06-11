using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Client.Infrastructure.Models
{
    public class CommandModel : NotificationObject
    {
        #region Fields

        private bool _isChecked;
        private ICommand _command;
        private object _commandParameter;
        private object _content;
        private DataTemplate _contentTemplate;
        private object _tooltip;
        private Visibility _visibility;

        #endregion Fields

        #region Properties

        public ICommand Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
                RaisePropertyChanged("Command");
            }
        }

        public object CommandParameter
        {
            get
            {
                return _commandParameter;
            }
            set
            {
                _commandParameter = value;
                RaisePropertyChanged("CommandParameter");
            }
        }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        public object Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }

        public DataTemplate ContentTemplate
        {
            get
            {
                return _contentTemplate;
            }
            set
            {
                _contentTemplate = value;
                RaisePropertyChanged("ContentTemplate");
            }
        }

        public object Tooltip
        {
            get
            {
                return _tooltip;
            }
            set
            {
                _tooltip = value;
                RaisePropertyChanged("Tooltip");
            }
        }

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                RaisePropertyChanged("Visibility");
            }
        }

        #endregion Properties
    }
}