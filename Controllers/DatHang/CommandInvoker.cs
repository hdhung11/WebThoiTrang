using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTW.Controllers.ThanhToan
{
    public class CommandInvoker //controller gọi involker để thực thi command
    {
        private ICommand _command;

        public void SetCommand(ICommand command)
        {
            _command = command;
        }

        public void ExecuteCommand()
        {
            _command?.Execute(); //Kiểm tra _command có giá trị không (?.) rồi gọi Execute()
        }
    }
}