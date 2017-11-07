using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streaming.Crystal.Connection.Model
{
    public class DifusionCredential
    {
        private string _user;
        private string _pass;
        private string _appName;

        public DifusionCredential(string userName, string password, string _appName)
        {
            _user = userName;
            _pass = password;
            this._appName = _appName;
        }

        public string User
        {
            get { return _user; }
        }

        public string Pass
        {
            get { return _pass; }
        }

        public string AppName
        {
            get { return _appName; }
        }
    }
}
