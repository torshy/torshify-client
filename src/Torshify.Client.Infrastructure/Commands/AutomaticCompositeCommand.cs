// <copyright file="AutomaticCompositeCommand.cs" company="Nito Programs">
//     Copyright (c) 2009 Nito Programs.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Torshify.Client.Infrastructure.Commands
{
    /// <summary>
    /// Composite command that ties in to <see cref="CommandManager.RequerySuggested"/>.
    /// </summary>
    /// <remarks>
    /// <para>A composite command is composed of its <see cref="Subcommands"/>.</para>
    /// </remarks>
    public sealed class AutomaticCompositeCommand : ICommand
    {
        private List<ICommand> _subcommands;
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticCompositeCommand"/> class with no subcommands.
        /// </summary>
        public AutomaticCompositeCommand()
        {
            _subcommands = new List<ICommand>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticCompositeCommand"/> class with the specified subcommands.
        /// </summary>
        /// <param name="subcommands">The subcommands used to initialize <see cref="Subcommands"/>.</param>
        public AutomaticCompositeCommand(IEnumerable<ICommand> subcommands)
        {
            _subcommands = new List<ICommand>(subcommands);
        }

        /// <summary>
        /// This is a weak event. Provides notification that the result of <see cref="ICommand.CanExecute"/> may be different.
        /// </summary>
        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Gets or sets a the sequence of subcommands. This sequence is re-evaluated when necessary; it is not cached.
        /// </summary>
        /// <remarks>
        /// <para>This sequence is re-evaluated each time <see cref="ICommand.CanExecute"/> or <see cref="ICommand.Execute"/> is invoked.</para>
        /// <para>A composite command cannot be executed if <see cref="Subcommands"/> contains no commands or includes a command that cannot be invoked.</para>
        /// <para>Executing a composite command executes its subcommands.</para>
        /// </remarks>
        public IEnumerable<ICommand> Subcommands { get { return _subcommands; } }

        /// <summary>
        /// Determines if this command can execute. See <see cref="Subcommands"/>.
        /// </summary>
        /// <param name="parameter">The parameter for this command.</param>
        /// <returns>Whether this command can execute.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            bool ret = false;
            foreach (ICommand command in this.Subcommands)
            {
                ret = true;
                if (!command.CanExecute(parameter))
                {
                    return false;
                }
            }

            return ret;
        }

        /// <summary>
        /// Executes this command. See <see cref="Subcommands"/>.
        /// </summary>
        /// <param name="parameter">The parameter for this command.</param>
        void ICommand.Execute(object parameter)
        {
            foreach (ICommand command in this.Subcommands)
            {
                command.Execute(parameter);
            }
        }

        public void RegisterCommand(ICommand command)
        {
            _subcommands.Add(command);
        }
    }
}
