using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTW.Controllers.ThanhToan
{
    public interface ICommand //thao tác đặt hàng và thanh toán sẽ là 1 command riêng   
    {
        void Execute(); //đại diện cho một hành động cụ thể mà lệnh (command) thực hiện.
    }
}


