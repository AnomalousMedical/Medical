using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Medical.GUI;

namespace Medical
{
    public class DataDrivenNavigationManager
    {
        private static DataDrivenNavigationManager instance;

        public static DataDrivenNavigationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataDrivenNavigationManager();
                }
                return instance;
            }
        }

        private Stack<DataDrivenNavigationState> stateStack = new Stack<DataDrivenNavigationState>();

        public DataDrivenNavigationManager()
        {
            
        }

        public void pushNavigationState(DataDrivenNavigationState state)
        {
            stateStack.Push(state);
        }

        public DataDrivenNavigationState popNavigationState()
        {
            return stateStack.Pop();
        }

        public DataDrivenNavigationState Current
        {
            get
            {
                return stateStack.Peek();
            }
        }

        public int Count
        {
            get
            {
                return stateStack.Count;
            }
        }
    }
}
