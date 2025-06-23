using DTOSPA;
using System.Linq;
using System.Collections.Generic;
using DALSPA;
using System;

namespace BLLSPA
{
    public class SpaBLL
    {
        private List<KhachHang> danhSach;

        public SpaBLL()
        {
            danhSach = new List<KhachHang>();
        }

        public List<KhachHang> DanhSach => danhSach;

       
        public void NapDanhSach(List<KhachHang> ds)
        {
            danhSach = ds;
        }

      
        public void Them(KhachHang kh)
        {
            danhSach.Add(kh);
        }

        
        public List<KhachHang> LayTatCa()
        {
            return danhSach;
        }

        public List<KhachHang> TimTheoTenKhach(string ten)
        {
            return danhSach.Where(kh => kh.TenKhachHang.ToLower().Contains(ten.ToLower())).ToList();
        }

       
        public KhachHang TimTheoMa(string ma)
        {
            return danhSach.FirstOrDefault(kh => kh.MaKhachHang == ma);
        }


        private SpaDAL dal = new SpaDAL();
        private string filePath = "DanhSachKhachHang.xml";

        // Hàm ghi danh sách vào file XML
        public void LuuDuLieu()
        {
            dal.GhiVaoFileXML(danhSach, filePath);
        }


        public void CapNhatPhiLamDep()
        {
            foreach (var kh in danhSach)
            {
                if (kh is DVLamDep lamDep)
                {
                    lamDep.Makeup = (int)(lamDep.Makeup * 1.03);
                    lamDep.Nail = (int)(lamDep.Nail * 1.03);
                    lamDep.LamToc = (int)(lamDep.LamToc * 1.03);
                }
            }
        }
      
        public List<KhachHang> LocDichVuTren500()
        {
            return danhSach.Where(kh => kh.ThanhTien() > 500000).ToList();
        }
     
        public List<KhachHang> LayDichVuLamDep()
        {
            return danhSach.Where(kh => kh is DVLamDep).ToList();
        }
      
        public List<KhachHang> KhachNhieuDichVuHon3()
        {
            return danhSach.Where(kh =>
            {
                if (kh is DVLamDep a)
                    return (a.Makeup + a.Nail + a.LamToc) > 3;
                if (kh is DVChamSocBody b) 
                    return (b.GiacHoi + b.Yoga + b.XongHoi) > 3;
                if (kh is DVDuongSinh c) 
                    return(c.XoaBop + c.ChamCuu + c.CaoGio) > 3;
                return false;
            }).ToList();
        }

        public List<KhachHang> LayDichVuDuongSinh()
        {
            return danhSach.Where(kh => kh is DVDuongSinh).ToList();
        }

        public List<KhachHang> LayDichVuChamSocBody()
        {
            return danhSach.Where(kh => kh is DVChamSocBody).ToList();
        }

        public bool ThemDichVuChoKhach(string maKH, string tenDV, int soLan)
        {
            var kh = TimTheoMa(maKH);
            if (kh == null) return false;

            tenDV = tenDV.Trim().ToLower();

            if (kh is DVLamDep ld)
            {
                if (tenDV == "makeup") ld.Makeup += soLan;
                else if (tenDV == "nail") ld.Nail += soLan;
                else if (tenDV == "làm tóc" || tenDV == "lam toc") ld.LamToc += soLan;
                else return false;
            }
            else if (kh is DVDuongSinh ds)
            {
                if (tenDV == "xoa bóp" || tenDV == "xoa bop") ds.XoaBop += soLan;
                else if (tenDV == "châm cứu" || tenDV == "cham cuu") ds.ChamCuu += soLan;
                else if (tenDV == "cạo gió" || tenDV == "cao gio") ds.CaoGio += soLan;
                else return false;
            }
            else if (kh is DVChamSocBody body)
            {
                if (tenDV == "giác hơi" || tenDV == "giac hoi") body.GiacHoi += soLan;
                else if (tenDV == "yoga") body.Yoga += soLan;
                else if (tenDV == "xông hơi" || tenDV == "xong hoi") body.XongHoi += soLan;
                else return false;
            }

            return true;
        }


        public string XacDinhNhomDichVu(string tenDV)
        {
            tenDV = tenDV.ToLower();

            if (tenDV == "makeup" || tenDV == "nail" || tenDV == "làm tóc" || tenDV == "lam toc")
                return "Làm Đẹp";
            else if (tenDV == "xoa bóp" || tenDV == "xoa bop" || tenDV == "châm cứu" || tenDV == "cham cuu" || tenDV == "cạo gió" || tenDV == "cao gio")
                return "Dưỡng Sinh";
            else if (tenDV == "giác hơi" || tenDV == "giac hoi" || tenDV == "yoga" || tenDV == "xông hơi" || tenDV == "xong hoi")
                return "Chăm Sóc Body";
            else
                return "Không xác định";
        }

        public void InDanhSachDichVu()
        {
            Console.WriteLine("== DANH SÁCH CÁC DỊCH VỤ TRONG TỪNG LOẠI ==");

            Dictionary<string, int> lamDep = new Dictionary<string, int>
    {
        { "Makeup", 100000 },
        { "Nail", 150000 },
        { "Làm Tóc", 200000 }
    };

            Dictionary<string, int> duongSinh = new Dictionary<string, int>
    {
        { "Xoa Bóp", 120000 },
        { "Châm Cứu", 180000 },
        { "Cạo Gió", 100000 }
    };

            Dictionary<string, int> chamSocBody = new Dictionary<string, int>
    {
        { "Giác Hơi", 180000 },
        { "Yoga", 200000 },
        { "Xông Hơi", 150000 }
    };

            void InDichVu(string tieuDe, Dictionary<string, int> dvDict)
            {
                Console.WriteLine($"[{tieuDe}]");
                foreach (var dv in dvDict)
                    Console.WriteLine($"  - {dv.Key} ({dv.Value:N0} VND)");
                Console.WriteLine();
            }

            InDichVu("Làm Đẹp", lamDep);
            InDichVu("Dưỡng Sinh", duongSinh);
            InDichVu("Chăm Sóc Body", chamSocBody);
        }
        public void LoadDuLieu()
        {
            danhSach = dal.DocTuFileXML(filePath);
        }
    }
}