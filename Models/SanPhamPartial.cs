using System;

namespace LTW.Models
{
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
