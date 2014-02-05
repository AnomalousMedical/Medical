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
        private Funcs funcs;
        public class Funcs
        {
            public Action<ExecuteObject> ExecuteFunc { get; set; }
            public Action<UndoObject> UndoFunc { get; set; }
            public Action<ExecuteObject, UndoObject> PoppedFrontFunc { get; set; }
            public Action<ExecuteObject, UndoObject> ClearedFunc { get; set; }
            public Action<ExecuteObject, UndoObject> TrimmedFunc { get; set; }
            public Action<ExecuteObject, UndoObject> RemovedFunc { get; set; }
        }

        public TwoWayDelegateCommand(ExecuteObject executeObject, UndoObject undoObject, Funcs funcs)
        {
            this.executeObject = executeObject;
            this.undoObject = undoObject;
            this.funcs = funcs;
        }

        public void execute()
        {
            funcs.ExecuteFunc(executeObject);
        }

        public void undo()
        {
            funcs.UndoFunc(undoObject);
        }

        public void poppedFront()
        {
            if (funcs.PoppedFrontFunc != null)
            {
                funcs.PoppedFrontFunc.Invoke(executeObject, undoObject);
            }
        }

        public void cleared()
        {
            if (funcs.ClearedFunc != null)
            {
                funcs.ClearedFunc.Invoke(executeObject, undoObject);
            }
        }

        public void trimmed()
        {
            if (funcs.TrimmedFunc != null)
            {
                funcs.TrimmedFunc.Invoke(executeObject, undoObject);
            }
        }

        public void removed()
        {
            if (funcs.RemovedFunc != null)
            {
                funcs.RemovedFunc.Invoke(executeObject, undoObject);
            }
        }
    }

    public class TwoWayDelegateCommand<T> : TwoWayCommand
    {
        private T arg;
        private Funcs funcs;
        public class Funcs
        {
            public Action<T> ExecuteFunc { get; set; }
            public Action<T> UndoFunc { get; set; }
            public Action<T> PoppedFrontFunc { get; set; }
            public Action<T> ClearedFunc { get; set; }
            public Action<T> TrimmedFunc { get; set; }
            public Action<T> RemovedFunc { get; set; }
        }

        public TwoWayDelegateCommand(T arg, Funcs funcs)
        {
            this.arg = arg;
            this.funcs = funcs;
        }

        public void execute()
        {
            funcs.ExecuteFunc(arg);
        }

        public void undo()
        {
            funcs.UndoFunc(arg);
        }

        public void poppedFront()
        {
            if (funcs.PoppedFrontFunc != null)
            {
                funcs.PoppedFrontFunc.Invoke(arg);
            }
        }

        public void cleared()
        {
            if (funcs.ClearedFunc != null)
            {
                funcs.ClearedFunc.Invoke(arg);
            }
        }

        public void trimmed()
        {
            if (funcs.TrimmedFunc != null)
            {
                funcs.TrimmedFunc.Invoke(arg);
            }
        }

        public void removed()
        {
            if (funcs.RemovedFunc != null)
            {
                funcs.RemovedFunc.Invoke(arg);
            }
        }
    }

    public class TwoWayDelegateCommand : TwoWayCommand
    {
        private Funcs funcs;
        public class Funcs
        {
            public Action ExecuteFunc { get; set; }
            public Action UndoFunc { get; set; }
            public Action PoppedFrontFunc { get; set; }
            public Action ClearedFunc { get; set; }
            public Action TrimmedFunc { get; set; }
            public Action RemovedFunc { get; set; }
        }

        public TwoWayDelegateCommand(Funcs funcs)
        {
            this.funcs = funcs;
        }

        public void execute()
        {
            funcs.ExecuteFunc();
        }

        public void undo()
        {
            funcs.UndoFunc();
        }

        public void poppedFront()
        {
            if (funcs.PoppedFrontFunc != null)
            {
                funcs.PoppedFrontFunc.Invoke();
            }
        }

        public void cleared()
        {
            if (funcs.ClearedFunc != null)
            {
                funcs.ClearedFunc.Invoke();
            }
        }

        public void trimmed()
        {
            if (funcs.TrimmedFunc != null)
            {
                funcs.TrimmedFunc.Invoke();
            }
        }

        public void removed()
        {
            if (funcs.RemovedFunc != null)
            {
                funcs.RemovedFunc.Invoke();
            }
        }
    }
}
