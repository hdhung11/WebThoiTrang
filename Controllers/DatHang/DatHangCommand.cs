using LTW.Models; // Import namespace chứa các model của ứng dụng
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTW.Controllers.ThanhToan
{
    // Lớp DatHangCommand thực hiện đặt hàng theo Command Pattern
    public class DatHangCommand : ICommand
    {
        MyDataDataContext _context = new MyDataDataContext(); // Kết nối với database
        private readonly KhachHang _khachHang; // Thông tin khách hàng
        private readonly List<CartItem> _gioHang; // Danh sách sản phẩm trong giỏ hàng
        private readonly FormCollection _form; // Form dữ liệu gửi từ client

        // Constructor nhận dữ liệu cần thiết để xử lý đơn hàng
        public DatHangCommand(MyDataDataContext data, KhachHang khachHang, List<CartItem> gioHang, FormCollection form)
        {
            _context = data; // Khởi tạo database context
            _khachHang = khachHang; // Lưu thông tin khách hàng
            _gioHang = gioHang; // Lưu danh sách sản phẩm trong giỏ hàng
            _form = form; // Lưu dữ liệu từ form
        }

        // Thực thi đặt hàng
        public void Execute()
        {
            // Kiểm tra khách hàng đã đăng nhập chưa
            if (_khachHang == null)
                throw new Exception("Bạn chưa đăng nhập!");

            // Kiểm tra giỏ hàng có sản phẩm không
            if (!_gioHang.Any())
                throw new Exception("Giỏ hàng trống!");

            // Lấy ngày giao hàng dự kiến từ form, kiểm tra hợp lệ
            var ngayGiaoHangDuKien = DateTime.Parse(_form["NgayGiao"]);
            if (ngayGiaoHangDuKien < DateTime.Now)
                throw new Exception("Ngày giao hàng không hợp lệ!");

            // Lấy trạng thái thanh toán từ form
            int status = int.Parse(_form["TrangThai"]);

            // Tính toán tổng tiền hàng và các chi phí khác
            decimal tongTienHang = _gioHang.Sum(i => (decimal)i.dThanhtien); // Tổng tiền sản phẩm
            decimal phiVanChuyen = 25000; // Phí vận chuyển cố định
            decimal thueVAT = 10; // Thuế VAT 10%
            decimal thanhTienTruocVAT = tongTienHang + phiVanChuyen; // Tổng tiền trước VAT
            decimal tienThueVAT = thanhTienTruocVAT * (thueVAT / 100); // Tiền thuế VAT
            decimal thanhTienSauVAT = thanhTienTruocVAT + tienThueVAT; // Tổng tiền sau VAT

            // Tạo đơn hàng mới
            DonHang dh = new DonHang
            {
                MaKH = _khachHang.MaKH, // Mã khách hàng
                NgayDatHang = DateTime.Now, // Ngày đặt hàng
                NgayGiaoHangDuKien = ngayGiaoHangDuKien, // Ngày giao hàng dự kiến
                TrangThai = "chuagiao", // Trạng thái đơn hàng mặc định là chưa giao
                TrangThaiThanhToan = (status == 1) ? "chuathanhtoan" : "dathanhtoan", // Xác định trạng thái thanh toán
                MaTT = status, // Mã trạng thái thanh toán
                TongTienDonHang = tongTienHang, // Tổng tiền đơn hàng chưa tính thuế, phí
                GhiChu = _form["GhiChu"], // Lưu ghi chú từ form
                PhiVanChuyen = phiVanChuyen, // Phí vận chuyển
                ThueVAT = thueVAT, // Thuế VAT
                TienThueVAT = tienThueVAT, // Tiền thuế VAT
                ThanhTienTruocVAT = thanhTienTruocVAT, // Tổng tiền trước VAT
                ThanhTienSauVAT = thanhTienSauVAT, // Tổng tiền sau VAT
                NgayTao = DateTime.Now // Ngày tạo đơn hàng
            };

            // Thêm đơn hàng vào database
            _context.DonHangs.InsertOnSubmit(dh);
            _context.SubmitChanges(); // Lưu thay đổi vào DB

            // Duyệt qua từng sản phẩm trong giỏ hàng và thêm vào chi tiết đơn hàng
            foreach (var item in _gioHang)
            {
                var sp = _context.SanPhams.SingleOrDefault(n => n.MaSP == item.MaSP);
                if (sp == null || sp.SoLuongTon < item.isoluong)
                    throw new Exception($"Sản phẩm {sp.TenSP} không đủ số lượng trong kho");

                // Tạo chi tiết đơn hàng
                ChiTietDonHang ctdh = new ChiTietDonHang

                {
                    MaDH = dh.MaDH, // Mã đơn hàng vừa tạo
                    MaSP = item.MaSP, // Mã sản phẩm
                    MaTT = status, // Trạng thái thanh toán
                    SoLuongMua = item.isoluong, // Số lượng mua
                    TongTien = (decimal)item.dThanhtien // Tổng tiền sản phẩm
                };

                // Thêm chi tiết đơn hàng vào database
                _context.ChiTietDonHangs.InsertOnSubmit(ctdh);

                // Giảm số lượng tồn kho sau khi đặt hàng
                sp.SoLuongTon -= item.isoluong;
            }

                // Tìm giỏ hàng của khách hàng hiện tại
            var dbCart = _context.GioHangs.FirstOrDefault(g => g.MaKH == _khachHang.MaKH && g.TrangThai == true);
            if (dbCart != null)
            {
                // Xóa toàn bộ chi tiết giỏ hàng
                var chiTietGioHang = _context.ChiTietGioHangs.Where(ct => ct.MaGioHang == dbCart.MaGioHang);
                _context.ChiTietGioHangs.DeleteAllOnSubmit(chiTietGioHang);

                // Cập nhật trạng thái giỏ hàng thành "đã đặt hàng" (không còn hoạt động)
                dbCart.TrangThai = false;
            }

            // Lưu thay đổi vào database
            _context.SubmitChanges();
        }
    }
}
