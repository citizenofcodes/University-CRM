using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace University_CRM.Infrastructure.Commands
{
    public class Command : ICommand
    {
        public bool IsParametrized { get; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private readonly Action actionNoParam;
        private readonly Func<bool> actionCanExecuteNoParam;

        private readonly Action<object> actionParam;
        private readonly Func<object, bool> actionCanExecuteParam;

        public Command(Action<object> actionParam, Func<object, bool> actionCanExecuteParam = null)
        {
            this.actionParam = actionParam;
            this.actionCanExecuteParam = actionCanExecuteParam;
            IsParametrized = true;
        }

        public Command(Action actionNoParam, Func<bool> actionCanExecuteNoParam = null)
        {
            this.actionNoParam = actionNoParam;
            this.actionCanExecuteNoParam = actionCanExecuteNoParam;
        }



        public bool CanExecute(object parameter)
        {
            return actionCanExecuteParam?.Invoke(parameter) ?? actionCanExecuteNoParam?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            if (IsParametrized)
            {
                actionParam(parameter);
            }
            else
            {
                actionNoParam();
            }
        }
    }
}
