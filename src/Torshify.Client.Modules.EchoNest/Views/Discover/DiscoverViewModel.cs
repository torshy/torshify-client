using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Modules.EchoNest.Models;

namespace Torshify.Client.Modules.EchoNest.Views.Discover
{
    public class DiscoverViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private ICollectionView _moods;
        private ICollectionView _styles;

        #endregion Fields

        #region Properties

        public ICollectionView Moods
        {
            get { return _moods; }
            private set
            {
                _moods = value;
                RaisePropertyChanged("Moods");
            }
        }

        public ICollectionView Styles
        {
            get { return _styles; }
            private set
            {
                _styles = value;
                RaisePropertyChanged("Styles");
            }
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            Task.Factory.StartNew(() =>
            {
                using (var session = new EchoNestSession(EchoNestModule.ApiKey))
                {
                    var styles = session.Query<ListTerms>().Execute(ListTermsType.Style);
                    var moods = session.Query<ListTerms>().Execute(ListTermsType.Mood);

                    if (styles.Status.Code == ResponseCode.Success)
                    {
                        Styles =
                            new ListCollectionView(styles.Terms.Select(s => new TermModel{Name = s.Name}).ToArray());
                    }

                    if (moods.Status.Code == ResponseCode.Success)
                    {
                        Moods =
                            new ListCollectionView(moods.Terms.Select(s => new TermModel{Name = s.Name}).ToArray());
                    }
                }
            });
        }

        #endregion Methods
    }
}