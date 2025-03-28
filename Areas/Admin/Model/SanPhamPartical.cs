using System;


namespace LTW.Models
{
    //Nếu để private, class này chỉ có thể sử dụng trong cùng file đó, điều này không phù hợp vì SanPham cần được dùng ở nhiều nơi (VD: Controller, View).
    //Partical định nghĩa class SanPham thành nhiều file khác nhau  
    public partial class SanPham : ICloneable // Thêm interface ICloneable
    {
        public object Clone()
        {
            return new SanPham //Tạo đối tượng mới,sao chép toàn bộ giá trị thuộc tính từ object gốc hiện tại (this).
            {
                TenSP = this.TenSP,
                Hinh = this.Hinh,
                GiaSP = this.GiaSP,
                GiaVon = this.GiaVon,
                SoLuongTon = this.SoLuongTon,
                MoTa = this.MoTa,
                MaLoai = this.MaLoai,
                MaNCC = this.MaNCC,
                TrangThai = this.TrangThai
            };
        }
    }
}   