using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomalous.Medical.StoreManager.Models
{
    public class ResponseModel : Saveable
    {
        public ResponseModel()
        {

        }

        public bool Success { get; set; }

        public String Message { get; set; }

        public ResponseModel(LoadInfo info)
        {
            Success = info.GetBoolean("Success");
            Message = info.GetString("Message");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Success", Success);
            info.AddValue("Message", Message);
        }
    }
}
