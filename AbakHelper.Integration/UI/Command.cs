using System;
using System.Windows.Input;

namespace AbakHelperV2.Infrastructure
{
    public class Command : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action<object> executeAction, Func<object, bool> canExecuteAction = null)
        {
            _execute = executeAction;
            if (canExecuteAction == null)
                canExecuteAction = o => true;
            _canExecute = canExecuteAction;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            if (_execute != null && CanExecute(parameter))
                _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

    }
}
