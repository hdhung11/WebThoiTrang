using LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTW.Builders
{
    public class SanPhamBuilder
    {
        private SanPham sanPham;

        public SanPhamBuilder()
        {
            sanPham = new SanPham();
        }

        public SanPhamBuilder SetTenSP(string tenSP)
        {
            sanPham.TenSP = tenSP;
            return this;
        }

        public SanPhamBuilder SetHinh(string hinh)
        {
            sanPham.Hinh = hinh;
            return this;
        }

        public SanPhamBuilder SetGiaSP(int giaSP)
        {
            sanPham.GiaSP = giaSP;
            return this;
        }
        public SanPhamBuilder SetGiaVon(int giaVon)
        {
            sanPham.GiaVon = giaVon;
            return this;
        }
        public SanPhamBuilder SetSoLuongTon(int soLuongTon)
        {
            sanPham.SoLuongTon = soLuongTon;
            return this;
        }

        public SanPhamBuilder SetMoTa(string moTa)
        {
            sanPham.MoTa = moTa;
            return this;
        }

        public SanPhamBuilder SetMaLoai(int maLoai)
        {
            sanPham.MaLoai = maLoai;
            return this;
        }

        public SanPhamBuilder SetNCC(int ncc)
        {
            sanPham.MaNCC = ncc;
            return this;
        }
   

        public SanPham Build()
        {
            return sanPham;
        }
    }
}
