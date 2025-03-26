using LTW.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTW.Controllers.ThanhToan
{
    public class DatHangCommand : ICommand
    {
        MyDataDataContext _context = new MyDataDataContext();
        private readonly KhachHang _khachHang;
        private readonly List<CartItem> _gioHang;
        private readonly FormCollection _form;

        public DatHangCommand(MyDataDataContext data, KhachHang khachHang, List<CartItem> gioHang, FormCollection form)
        {
            _context = data;
            _khachHang = khachHang;
            _gioHang = gioHang;
            _form = form;
        }

        public void Execute()
        {
            if (_khachHang == null)
                throw new Exception("Bạn chưa đăng nhập!");

            if (!_gioHang.Any())
                throw new Exception("Giỏ hàng trống!");

            var ngayGiaoHangDuKien = DateTime.Parse(_form["NgayGiao"]);
            if (ngayGiaoHangDuKien < DateTime.Now)
                throw new Exception("Ngày giao hàng không hợp lệ!");

            int status = int.Parse(_form["TrangThai"]);
            decimal tongTienHang = _gioHang.Sum(i => (decimal)i.dThanhtien);
            decimal phiVanChuyen = 25000;
            decimal thueVAT = 10;
            decimal thanhTienTruocVAT = tongTienHang + phiVanChuyen;
            decimal tienThueVAT = thanhTienTruocVAT * (thueVAT / 100);
            decimal thanhTienSauVAT = thanhTienTruocVAT + tienThueVAT;

            DonHang dh = new DonHang
            {
                MaKH = _khachHang.MaKH,
                NgayDatHang = DateTime.Now,
                NgayGiaoHangDuKien = ngayGiaoHangDuKien,
                TrangThai = "chuagiao",
                TrangThaiThanhToan = (status == 1) ? "chuathanhtoan" : "dathanhtoan",
                MaTT = status,
                TongTienDonHang = tongTienHang,
                GhiChu = _form["GhiChu"],
                PhiVanChuyen = phiVanChuyen,
                ThueVAT = thueVAT,
                TienThueVAT = tienThueVAT,
                ThanhTienTruocVAT = thanhTienTruocVAT,
                ThanhTienSauVAT = thanhTienSauVAT,
                NgayTao = DateTime.Now
            };

            _context.DonHangs.InsertOnSubmit(dh);
            _context.SubmitChanges();

            foreach (var item in _gioHang)
            {
                var sp = _context.SanPhams.SingleOrDefault(n => n.MaSP == item.MaSP);
                if (sp == null || sp.SoLuongTon < item.isoluong)
                    throw new Exception($"Sản phẩm {sp.TenSP} không đủ số lượng trong kho");

                ChiTietDonHang ctdh = new ChiTietDonHang
                {
                    MaDH = dh.MaDH,
                    MaSP = item.MaSP,
                    MaTT = status,
                    SoLuongMua = item.isoluong,
                    TongTien = (decimal)item.dThanhtien
                };

                _context.ChiTietDonHangs.InsertOnSubmit(ctdh);
                sp.SoLuongTon -= item.isoluong;
            }

            var dbCart = _context.GioHangs.FirstOrDefault(g => g.MaKH == _khachHang.MaKH && g.TrangThai == true);
            if (dbCart != null)
            {
                var chiTietGioHang = _context.ChiTietGioHangs.Where(ct => ct.MaGioHang == dbCart.MaGioHang);
                _context.ChiTietGioHangs.DeleteAllOnSubmit(chiTietGioHang);
                dbCart.TrangThai = false;
            }

            _context.SubmitChanges();
        }
    }
 }