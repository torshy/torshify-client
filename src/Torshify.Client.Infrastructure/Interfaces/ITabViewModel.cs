using System.Windows;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface ITabViewModel<TModel>
    {
        string Header
        {
            get;
        }

        Visibility Visibility
        {
            get;
        }

        void Initialize(NavigationContext navContext);

        void Deinitialize(NavigationContext navContext);

        void SetModel(TModel model);

        void NavigatedTo();

        void NavigatedFrom();
    }
}