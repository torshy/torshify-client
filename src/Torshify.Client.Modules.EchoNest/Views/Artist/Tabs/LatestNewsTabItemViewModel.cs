using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

using EchoNest;
using EchoNest.Artist;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.EchoNest.Views.Artist.Tabs
{
    [Export]
    public class LatestNewsTabItemViewModel : NotificationObject, ITabViewModel<IArtist>
    {
        #region Fields

        private List<NewsItem> _news;

        #endregion Fields

        #region Constructors

        public LatestNewsTabItemViewModel()
        {
            OpenBrowserWithUrlCommand = new AutomaticCommand<NewsItem>(ExecuteOpenBrowserWithUrl, CanExecuteOpenBrowserWithUrl);
        }

        #endregion Constructors

        #region Properties

        public string Header
        {
            get { return "News"; }
        }

        public List<NewsItem> News
        {
            get
            {
                return _news;
            }
            set
            {
                _news = value;
                RaisePropertyChanged("News");
            }
        }

        public AutomaticCommand<NewsItem> OpenBrowserWithUrlCommand
        {
            get;
            private set;
        }

        public Visibility Visibility
        {
            get { return Visibility.Visible; }
        }

        #endregion Properties

        #region Methods

        public void Deinitialize(NavigationContext navContext)
        {
        }

        public void Initialize(NavigationContext navContext)
        {
        }

        public void SetModel(IArtist model)
        {
            Task.Factory.StartNew(() => FindNewsForArtist(model));
        }

        public void NavigatedTo()
        {
        }

        public void NavigatedFrom()
        {
        }

        private bool CanExecuteOpenBrowserWithUrl(NewsItem arg)
        {
            return arg != null && !string.IsNullOrEmpty(arg.Url);
        }

        private void ExecuteOpenBrowserWithUrl(NewsItem item)
        {
            Process.Start(item.Url);
        }

        private void FindNewsForArtist(IArtist artist)
        {
            using (EchoNestSession session = new EchoNestSession(EchoNestModule.ApiKey))
            {
                var response = session.Query<News>().Execute(artist.Name);

                if (response.Status.Code == ResponseCode.Success)
                {
                    News = response.News;

                    foreach (var newsItem in News)
                    {
                        newsItem.Name = HttpUtility.HtmlDecode(newsItem.Name);
                        newsItem.Summary = HttpUtility.HtmlDecode(newsItem.Summary);
                    }
                }
            }
        }

        #endregion Methods
    }
}