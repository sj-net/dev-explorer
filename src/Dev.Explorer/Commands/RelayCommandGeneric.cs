namespace FileExplorer.Commands
{
    using System;
    using System.Windows.Input;

    public class RelayCommand<T> : ICommand
    {
        private Action<T> execute;
        private Func<T, bool> canExecute;
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object? parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
            {
                return this.canExecute.Invoke(default);
            }
            if (parameter == null || parameter is T)
            {
                return this.canExecute.Invoke((T)parameter);
            }

            return false;
        }

        public void Execute(object? parameter)
        {
            object val = parameter;

            if (parameter != null && parameter.GetType() != typeof(T))
            {
                if (parameter is IConvertible)
                {
                    val = Convert.ChangeType(parameter, typeof(T), null);
                }
                else if (typeof(T).IsAssignableFrom(parameter.GetType()))
                {
                    val = parameter;
                }
                else if (typeof(T) != typeof(object))
                {
                    throw new InvalidCastException($"Expected parameter of type {typeof(T).FullName}. But got {parameter.GetType().FullName}");
                }
            }

            if (CanExecute(val) && this.execute != null)
            {
                if (val == null)
                {
                    if (typeof(T).IsValueType)
                    {
                        this.execute.Invoke(default);
                    }
                    else
                    {
                        this.execute.Invoke((T)val);
                    }
                }
                else
                {
                    this.execute.Invoke((T)val);
                }
            }
        }
    }
}
