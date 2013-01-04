using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TwoWayDelegateCommand<ExecuteObject, UndoObject> : TwoWayCommand
    {
        private ExecuteObject executeObject;
        private UndoObject undoObject;
        private Action<ExecuteObject> executeFunc;
        private Action<UndoObject> undoFunc;

        public TwoWayDelegateCommand(Action<ExecuteObject> executeFunc, ExecuteObject executeObject, Action<UndoObject> undoFunc, UndoObject undoObject)
        {
            this.executeFunc = executeFunc;
            this.executeObject = executeObject;
            this.undoFunc = undoFunc;
            this.undoObject = undoObject;
        }

        public void execute()
        {
            executeFunc(executeObject);
        }

        public void undo()
        {
            undoFunc(undoObject);
        }
    }

    public class TwoWayDelegateCommand<T> : TwoWayCommand
    {
        private T arg;
        private Action<T> executeFunc;
        private Action<T> undoFunc;

        public TwoWayDelegateCommand(Action<T> executeFunc, Action<T> undoFunc, T arg)
        {
            this.executeFunc = executeFunc;
            this.undoFunc = undoFunc;
            this.arg = arg;
        }

        public void execute()
        {
            executeFunc(arg);
        }

        public void undo()
        {
            undoFunc(arg);
        }
    }

    public class TwoWayDelegateCommand : TwoWayCommand
    {
        private Action executeFunc;
        private Action undoFunc;

        public TwoWayDelegateCommand(Action executeFunc, Action undoFunc)
        {
            this.executeFunc = executeFunc;
            this.undoFunc = undoFunc;
        }

        public void execute()
        {
            executeFunc();
        }

        public void undo()
        {
            undoFunc();
        }
    }
}
