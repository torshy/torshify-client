using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;
using Torshify.Client.Modules.Core.Views.Album.Tabs;

namespace Torshify.Client.Modules.Core.Views.Album
{
    public class AlbumViewModel : TabViewModel<IAlbum>, IPartImportsSatisfiedNotification
    {
        #region Fields

        [ImportMany]
        private IEnumerable<Lazy<ITab<IAlbum>>> _tabImports = null;

        #endregion Fields

        #region Constructors

        public AlbumViewModel(CompositionContainer mefContainer)
        {
            AddTab(ServiceLocator.Current.TryResolve<AlbumTabItemView>());

            mefContainer.SatisfyImportsOnce(this);
        }

        #endregion Constructors

        #region Methods

        protected override IAlbum GetModel(NavigationContext navigationContext)
        {
            return navigationContext.Tag as IAlbum;
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var tabImport in _tabImports)
            {
                AddTab(tabImport.Value);
            }
        }

        #endregion Methods
    }
}