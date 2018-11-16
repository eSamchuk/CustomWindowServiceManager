using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WindowsServiceManager.Models
{
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action _execute;
        readonly Predicate<object> _canExecute;

        #endregion

        #region Constructors
        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameters)
        {
            return _canExecute == null ? true : _canExecute(parameters);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameters)
        {
            _execute();
        }

        #endregion 
    }
}
