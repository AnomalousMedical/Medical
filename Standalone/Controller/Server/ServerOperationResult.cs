using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// A simple class with a success/fail status and a message. Used by the server to return results.
    /// </summary>
    public class ServerOperationResult : Saveable
    {
        /// <summary>
        /// Use this type finder to prevent servers from returning whatever random types they want
        /// we only want this class if we are making a request that expects this as a result.
        /// </summary>
        public static TypeFinder TypeFinder { get; private set; }

        static ServerOperationResult()
        {
            TypeFinder = new ServerOperationTypeFinder();
        }

        public ServerOperationResult()
        {

        }

        public bool Success { get; set; }

        public String Message { get; set; }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Success", Success);
            info.AddValue("Message", Message);
        }

        protected ServerOperationResult(LoadInfo info)
        {
            Success = info.GetBoolean("Success");
            Message = info.GetString("Message");
        }

        class ServerOperationTypeFinder : TypeFinder
        {
            private static readonly char[] SPLIT = { ',' };

            public Type findType(string assemblyQualifiedName)
            {
                String typeName = assemblyQualifiedName.Split(SPLIT)[0];
                if(typeName == "Medical.ServerOperationResult")
                {
                    return typeof(ServerOperationResult);
                }
                return null;
            }
        }
    }
}
