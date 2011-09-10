using System;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Client.Modules.EchoNest.Views.Similar
{
    public class SimilarArtistModel : NotificationObject, IEquatable<SimilarArtistModel>
    {
        #region Properties

        public string Name
        {
            get; set;
        }

        public string ImageUrl
        {
            get; set;
        }

        public double Hotttnesss
        {
            get; set;
        }
        
        public double Familiarity
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public bool Equals(SimilarArtistModel other)
        {
            return other.Name.Equals(Name);
        }

        public override bool Equals(object obj)
        {
            SimilarArtistModel other = obj as SimilarArtistModel;

            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion Methods
    }
}