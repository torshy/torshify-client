using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure.Models
{
    public class CommandBar : NotificationObject, ICommandBar
    {
        private ObservableCollection<CommandModel> _items;

        #region Constructors

        public CommandBar()
        {
            _items = new ObservableCollection<CommandModel>();
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<CommandModel> ChildMenuItems
        {
            get { return _items; }
        }

        #endregion Properties

        #region Public Methods

        public ICommandBar AddCommand(string displayName, ICommand command)
        {
            return AddCommand(displayName, command, null);
        }

        public ICommandBar AddCommand(string displayName, ICommand command, object commandParameter)
        {
            CommandModel commandMenuItem = new CommandModel();
            commandMenuItem.Content = displayName;
            commandMenuItem.Command = command;
            commandMenuItem.CommandParameter = commandParameter;
            _items.Add(commandMenuItem);
            return this;
        }

        public ICommandBar AddCommand(CommandModel commandPresentation)
        {
            _items.Add(commandPresentation);
            return this;
        }

        public ICommandBar AddSeparator()
        {
            return AddSeparator(null);
        }

        public ICommandBar AddSeparator(string displayName)
        {
            SeparatorCommandModel separator = new SeparatorCommandModel();
            separator.Content = displayName;
            _items.Add(separator);
            return this;
        }

        public ICommandBar AddSubmenu(string displayName)
        {
            SubmenuCommandModel submenuCommandHoster = new SubmenuCommandModel(displayName);
            submenuCommandHoster.Content = displayName;
            _items.Add(submenuCommandHoster);
            return submenuCommandHoster;
        }

        #endregion Public Methods

        #region Nested Types

        public class SeparatorCommandModel : CommandModel
        {
        }

        public class SubmenuCommandModel : CommandModel, ICommandBar
        {
            #region Fields

            private CommandBar _commandBar;

            #endregion Fields

            #region Constructors

            public SubmenuCommandModel(string displayName)
            {
                Content = displayName;
                _commandBar = new CommandBar();
            }

            #endregion Constructors

            #region Properties

            public IEnumerable<CommandModel> ChildMenuItems
            {
                get { return _commandBar.ChildMenuItems; }
            }

            #endregion Properties

            #region Public Methods

            public ICommandBar AddCommand(string displayName, ICommand menuItemCommand)
            {
                return _commandBar.AddCommand(displayName, menuItemCommand);
            }

            public ICommandBar AddCommand(string displayName, ICommand command, object commandParameter)
            {
                return _commandBar.AddCommand(displayName, command, commandParameter);
            }

            public ICommandBar AddCommand(CommandModel commandPresentation)
            {
                return _commandBar.AddCommand(commandPresentation);
            }

            public ICommandBar AddSeparator()
            {
                return _commandBar.AddSeparator();
            }

            public ICommandBar AddSeparator(string displayName)
            {
                return _commandBar.AddSeparator(displayName);
            }

            public ICommandBar AddSubmenu(string displayName)
            {
                return _commandBar.AddSubmenu(displayName);
            }

            #endregion Public Methods
        }

        #endregion Nested Types
    }
}