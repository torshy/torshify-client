using System;

using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Client.Infrastructure.Models
{
    public class HeaderedNavigationItem : NavigationItem
    {
        #region Constructors

        public HeaderedNavigationItem(Uri navigationUrl, object header)
            : base(navigationUrl)
        {
            Header = header;
        }

        #endregion Constructors

        #region Properties

        public object Header
        {
            get; private set;
        }

        #endregion Properties
    }

    public class NavigationItem : NotificationObject
    {
        #region Fields

        private bool _isSelected;

        #endregion Fields

        #region Constructors

        public NavigationItem(Uri navigationUrl)
        {
            NavigationUrl = navigationUrl;
            RegionName = CoreRegionNames.MainMusicRegion;
        }

        #endregion Constructors

        #region Properties

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        public Uri NavigationUrl
        {
            get; set;
        }

        public string RegionName
        {
            get; set;
        }

        #endregion Properties
    }
}