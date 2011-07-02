using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Torshify.Client.Infrastructure.Input;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core
{
    public class CoreCommandsHandler : IInitializable
    {
        public void Initialize()
        {
            Application.Current.MainWindow.InputBindings.Add(
                new ExtendedMouseBinding
                    {
                        Command = CoreCommands.NavigateBackCommand,
                        Gesture = new ExtendedMouseGesture(MouseButton.XButton1)
                    });

            Application.Current.MainWindow.InputBindings.Add(
                new ExtendedMouseBinding
                {
                    Command = CoreCommands.NavigateForwardCommand,
                    Gesture = new ExtendedMouseGesture(MouseButton.XButton2)
                });
        }
    }
}