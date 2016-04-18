using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sodu.Util
{
    /// <summary>
    /// 命令类
    /// </summary>
    public class BaseCommand : ICommand
    {
        #region Members
        private Action mCommand;
        private Action<Object> mParaCommand;
        private Func<Object, Boolean> mCanExecute;

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Ctor
        public BaseCommand(Action command, Func<Object, Boolean> canExecute = null)
        {
            mCommand = command;
            mCanExecute = canExecute;
        }

        public BaseCommand(Action<Object> paraCommand, Func<Object, Boolean> canExecute = null)
        {
            mParaCommand = paraCommand;
            mCanExecute = canExecute;
        }
        #endregion

        #region Method
        public bool CanExecute(object parameter)
        {
            if (null != mCanExecute)
            {
                return mCanExecute(parameter);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            if (null != mCommand)
            {
                mCommand();
            }

            if (null != mParaCommand)
            {
                mParaCommand(parameter);
            }
        }
        #endregion
    }

}
