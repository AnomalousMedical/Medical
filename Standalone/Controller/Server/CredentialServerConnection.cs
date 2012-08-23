using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Medical
{
    public class CredentialServerConnection : ServerConnection
    {
        public CredentialServerConnection(String url, String user, String pass)
            :base(url)
        {
            this.User = user;
            this.Pass = pass;
        }

        public override void makeRequest(Action<HttpWebResponse> response)
        {
            addArgument("User", User);
            addArgument("Pass", Pass);
            base.makeRequest(response);
        }

        public String User { get; set; }

        public String Pass { get; set; }
    }
}
