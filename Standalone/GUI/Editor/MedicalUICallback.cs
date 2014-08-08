using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using MyGUIPlugin;
using Logging;

namespace Medical.GUI
{
    public class MedicalUICallback : EditUICallback
    {
        private Dictionary<Object, Delegate> customQueries = new Dictionary<object, Delegate>();

        public MedicalUICallback()
        {

        }

        public void getInputString(string prompt, SendResult<string> resultCallback)
        {
            InputBox.GetInput("Enter value", prompt, true, resultCallback);
        }

        public EditInterface getSelectedEditInterface()
        {
            return SelectedEditInterface;
        }

        public virtual void showBrowser<T>(Browser browser, SendResult<T> resultCallback)
        {
            BrowserWindow<T>.GetInput(browser, true, resultCallback);
        }

        public void showInputBrowser<T>(Browser browser, SendResult<T, string> resultCallback)
        {
            InputBrowserWindow<T>.GetInput(browser, true, resultCallback);
        }

        public void showFolderBrowserDialog(SendResult<string> resultCallback)
        {
            throw new NotImplementedException();
        }

        public void showOpenFileDialog(string filterString, SendResult<string> resultCallback)
        {
            throw new NotImplementedException();
        }

        public void showSaveFileDialog(string filterString, SendResult<string> resultCallback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method allows the interface to run a custom query on the
        /// UICallback. This can do anything and is not defined here.
        /// </summary>
        /// <param name="queryKey">The key for the query to run.</param>
        /// <param name="resultCallback">The callback with the results.</param>
        public void runCustomQuery<Ret>(Object queryKey, SendResult<Ret> resultCallback)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                ((Action<SendResult<Ret>>)queryDelegate).Invoke(resultCallback);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        public void runCustomQuery<Ret, Arg1>(Object queryKey, SendResult<Ret> resultCallback, Arg1 arg1)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                ((Action<SendResult<Ret>, Arg1>)queryDelegate).Invoke(resultCallback, arg1);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        public void runCustomQuery<Ret, Arg1, Arg2>(Object queryKey, SendResult<Ret> resultCallback, Arg1 arg1, Arg2 arg2)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                ((Action<SendResult<Ret>, Arg1, Arg2>)queryDelegate).Invoke(resultCallback, arg1, arg2);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        public void runCustomQuery<Ret, Arg1, Arg2, Arg3>(Object queryKey, SendResult<Ret> resultCallback, Arg1 arg1, Arg2 arg2, Arg3 arg3)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                ((Action<SendResult<Ret>, Arg1, Arg2, Arg3>)queryDelegate).Invoke(resultCallback, arg1, arg2, arg3);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        /// <summary>
        /// This method allows the interface to run a custom query on the
        /// UICallback. This can do anything and is not defined here. This is
        /// for a callback that expects no result.
        /// </summary>
        /// <param name="queryKey">The key for the query to run.</param>
        public void runOneWayCustomQuery(Object queryKey)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                ((Action)queryDelegate).Invoke();
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        public void runOneWayCustomQuery<Arg1>(Object queryKey, Arg1 arg1)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                ((Action<Arg1>)queryDelegate).Invoke(arg1);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        public void runOneWayCustomQuery<Arg1, Arg2>(Object queryKey, Arg1 arg1, Arg2 arg2)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                ((Action<Arg1, Arg2>)queryDelegate).Invoke(arg1, arg2);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        public void runOneWayCustomQuery<Arg1, Arg2, Arg3>(Object queryKey, Arg1 arg1, Arg2 arg2, Arg3 arg3)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                ((Action<Arg1, Arg2, Arg3>)queryDelegate).Invoke(arg1, arg2, arg3);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
            }
        }

        /// <summary>
        /// This method allows the interface to run a custom query on the
        /// UICallback. This can do anything and is not defined here.
        /// </summary>
        /// <param name="queryKey">The key for the query to run.</param>
        /// <param name="resultCallback">The callback with the results.</param>
        public Ret runSyncCustomQuery<Ret>(Object queryKey)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                return ((Func<Ret>)queryDelegate).Invoke();
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
                return default(Ret);
            }
        }

        public Ret runSyncCustomQuery<Ret, Arg1>(Object queryKey, Arg1 arg1)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                return ((Func<Arg1, Ret>)queryDelegate).Invoke(arg1);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
                return default(Ret);
            }
        }

        public Ret runSyncCustomQuery<Ret, Arg1, Arg2>(Object queryKey, Arg1 arg1, Arg2 arg2)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                return ((Func<Arg1, Arg2, Ret>)queryDelegate).Invoke(arg1, arg2);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
                return default(Ret);
            }
        }

        public Ret runSyncCustomQuery<Ret, Arg1, Arg2, Arg3>(Object queryKey, Arg1 arg1, Arg2 arg2, Arg3 arg3)
        {
            Delegate queryDelegate;
            if (customQueries.TryGetValue(queryKey, out queryDelegate))
            {
                return ((Func<Arg1, Arg2, Arg3, Ret>)queryDelegate).Invoke(arg1, arg2, arg3);
            }
            else
            {
                Log.Warning("Could not find custom object query {0}. No query run.", queryKey.ToString());
                return default(Ret);
            }
        }

        public void addCustomQuery<Ret>(Object queryKey, Action<SendResult<Ret>> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addCustomQuery<Ret, Arg1>(Object queryKey, Action<SendResult<Ret>, Arg1> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addCustomQuery<Ret, Arg1, Arg2>(Object queryKey, Action<SendResult<Ret>, Arg1, Arg2> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addCustomQuery<Ret, Arg1, Arg2, Arg3>(Object queryKey, Action<SendResult<Ret>, Arg1, Arg2, Arg3> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addOneWayCustomQuery(Object queryKey, Action queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addOneWayCustomQuery<Arg1>(Object queryKey, Action<Arg1> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addOneWayCustomQuery<Arg1, Arg2>(Object queryKey, Action<Arg1, Arg2> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addOneWayCustomQuery<Arg1, Arg2, Arg3>(Object queryKey, Action<Arg1, Arg2, Arg3> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addSyncCustomQuery<Ret>(Object queryKey, Func<Ret> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addSyncCustomQuery<Ret, Arg1>(Object queryKey, Func<Arg1, Ret> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addSyncCustomQuery<Ret, Arg1, Arg2>(Object queryKey, Func<Arg1, Arg2, Ret> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void addSyncCustomQuery<Ret, Arg1, Arg2, Arg3>(Object queryKey, Func<Arg1, Arg2, Arg3, Ret> queryDelegate)
        {
            customQueries.Add(queryKey, queryDelegate);
        }

        public void removeCustomQuery(Object queryKey)
        {
            customQueries.Remove(queryKey);
        }

        public bool hasCustomQuery(Object queryKey)
        {
            return customQueries.ContainsKey(queryKey);
        }

        public EditInterface SelectedEditInterface { get; set; }
    }
}
