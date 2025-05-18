using System;
using System.Windows.Input;

namespace SimonSays.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action? _execute; // Paraméter nélküli függvényekhez
        private readonly Action<object?>? _executeWithParam; // Paraméteres függvényekhez
        private readonly Func<object?, bool>? _canExecute;

        // Paraméter nélküli konstruktor
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute != null ? _ => canExecute() : (Func<object?, bool>?)null;
        }

        // Paraméteres konstruktor
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _executeWithParam = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter)
        {
            if (_execute != null)
                _execute();
            else if (_executeWithParam != null)
                _executeWithParam(parameter);
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
