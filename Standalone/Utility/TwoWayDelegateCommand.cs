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
        private Action<ExecuteObject, UndoObject> poppedFrontFunc;
        private Action<ExecuteObject, UndoObject> clearedFunc;
        private Action<ExecuteObject, UndoObject> trimmedFunc;

        public TwoWayDelegateCommand(Action<ExecuteObject> executeFunc, ExecuteObject executeObject, Action<UndoObject> undoFunc, UndoObject undoObject, Action<ExecuteObject, UndoObject> poppedFrontFunc = null, Action<ExecuteObject, UndoObject> trimmedFunc = null, Action<ExecuteObject, UndoObject> clearedFunc = null)
        {
            this.executeFunc = executeFunc;
            this.executeObject = executeObject;
            this.undoFunc = undoFunc;
            this.undoObject = undoObject;
            this.poppedFrontFunc = poppedFrontFunc;
            this.trimmedFunc = trimmedFunc;
            this.clearedFunc = clearedFunc;
        }

        public void execute()
        {
            executeFunc(executeObject);
        }

        public void undo()
        {
            undoFunc(undoObject);
        }

        public void poppedFront()
        {
            if (poppedFrontFunc != null)
            {
                poppedFrontFunc.Invoke(executeObject, undoObject);
            }
        }

        public void cleared()
        {
            if (clearedFunc != null)
            {
                clearedFunc.Invoke(executeObject, undoObject);
            }
        }

        public void trimmed()
        {
            if (trimmedFunc != null)
            {
                trimmedFunc.Invoke(executeObject, undoObject);
            }
        }
    }

    public class TwoWayDelegateCommand<T> : TwoWayCommand
    {
        private T arg;
        private Action<T> executeFunc;
        private Action<T> undoFunc;
        private Action<T> poppedFrontFunc;
        private Action<T> clearedFunc;
        private Action<T> trimmedFunc;

        public TwoWayDelegateCommand(Action<T> executeFunc, Action<T> undoFunc, T arg, Action<T> poppedFrontFunc = null, Action<T> trimmedFunc = null, Action<T> clearedFunc = null)
        {
            this.executeFunc = executeFunc;
            this.undoFunc = undoFunc;
            this.arg = arg;
            this.poppedFrontFunc = poppedFrontFunc;
            this.trimmedFunc = trimmedFunc;
            this.clearedFunc = clearedFunc;
        }

        public void execute()
        {
            executeFunc(arg);
        }

        public void undo()
        {
            undoFunc(arg);
        }

        public void poppedFront()
        {
            if (poppedFrontFunc != null)
            {
                poppedFrontFunc.Invoke(arg);
            }
        }

        public void cleared()
        {
            if (clearedFunc != null)
            {
                clearedFunc.Invoke(arg);
            }
        }

        public void trimmed()
        {
            if (trimmedFunc != null)
            {
                trimmedFunc.Invoke(arg);
            }
        }
    }

    public class TwoWayDelegateCommand : TwoWayCommand
    {
        private Action executeFunc;
        private Action undoFunc;
        private Action poppedFrontFunc;
        private Action clearedFunc;
        private Action trimmedFunc;

        public TwoWayDelegateCommand(Action executeFunc, Action undoFunc, Action poppedFrontFunc = null, Action trimmedFunc = null, Action clearedFunc = null)
        {
            this.executeFunc = executeFunc;
            this.undoFunc = undoFunc;
            this.poppedFrontFunc = poppedFrontFunc;
            this.trimmedFunc = trimmedFunc;
            this.clearedFunc = clearedFunc;
        }

        public void execute()
        {
            executeFunc();
        }

        public void undo()
        {
            undoFunc();
        }

        public void poppedFront()
        {
            if (poppedFrontFunc != null)
            {
                poppedFrontFunc.Invoke();
            }
        }

        public void cleared()
        {
            if (clearedFunc != null)
            {
                clearedFunc.Invoke();
            }
        }

        public void trimmed()
        {
            if (trimmedFunc != null)
            {
                trimmedFunc.Invoke();
            }
        }
    }
}
