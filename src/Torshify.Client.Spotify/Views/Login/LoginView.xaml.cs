using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Torshify.Client.Spotify.Views.Login
{
    public partial class LoginView : UserControl
    {
        #region Constructors

        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public LoginViewModel Model
        {
            get { return DataContext as LoginViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void OnForgotPasswordLinkClicked(object sender, RoutedEventArgs e)
        {
            Process.Start(Model.ForgotPasswordUrl.OriginalString);
        }

        #endregion Methods
    }
}