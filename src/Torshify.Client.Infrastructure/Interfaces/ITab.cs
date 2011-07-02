namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface ITab<TModel>
    {
        ITabViewModel<TModel> ViewModel
        {
            get;
        }
    }
}