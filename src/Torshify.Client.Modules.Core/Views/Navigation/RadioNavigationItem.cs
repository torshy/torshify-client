using System;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class RadioNavigationItem : INavigationItem
    {
        private readonly IRegionManager _regionManager;

        public RadioNavigationItem(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void NavigateTo()
        {

        }

        public DataTemplate DataTemplate
        {
            get { return null; }
        }

        public bool IsMe(IRegionNavigationJournalEntry entry)
        {
            return false;
        }
    }
}