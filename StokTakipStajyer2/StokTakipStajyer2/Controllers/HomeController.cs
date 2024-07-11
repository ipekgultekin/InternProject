using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using StokTakipStajyer2.Models;

namespace StokTakipStajyer2.Controllers
{
   
    public class HomeController : Controller
    {
        StokTakipDBEntities stokdata = new StokTakipDBEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Admin()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Giris()
        {
            return View();

        }


        [HttpPost]
        public ActionResult Giris(KULLANICI fc)
        {
            var info = stokdata.KULLANICI.FirstOrDefault(x=> x.KUL_USERNAME == fc.KUL_USERNAME && x.KUL_SIFRE == fc.KUL_SIFRE);
            
            if(info != null)
            {
                FormsAuthentication.SetAuthCookie(info.KUL_USERNAME, false);
                Session["ID"] = info.KUL_ID.ToString();
                ViewBag.ErrorMessage = "Kullanıcı girişi başarılı.";

                return RedirectToAction("KullaniciEkle");

            }

            else
            {
                ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre.";
                return View();
            }
     

        }

        [HttpGet]
        public ActionResult KullaniciEkle()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Giris");
            }
            var model = new KULLANICI();

            return View(model);
        }

        [HttpPost]
        public ActionResult KullaniciEkle(KULLANICI kullanici)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Giris");
            }

            if (ModelState.IsValid)
            {
                var model = new KULLANICI()
                {
                    KUL_USERNAME = kullanici.KUL_USERNAME,
                    KUL_AD = kullanici.KUL_AD,
                    KUL_SOYAD = kullanici.KUL_SOYAD,
                    KUL_SIFRE = kullanici.KUL_SIFRE,
                    KUL_TIP = kullanici.KUL_TIP,
                    STATU = kullanici.STATU,

                    OLUSTURAN_KULLANICI = Convert.ToInt32(Session["ID"]),
                    OLUSTURMA_TARIHI = DateTime.Now,
                    GUNCELLEME_TARIHI = DateTime.Now,
                    GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["ID"]),

                };

                stokdata.KULLANICI.Add(model);
                stokdata.SaveChanges();
                ViewBag.Message = "Kullanıcı başarıyla eklendi.";

                return RedirectToAction("Index");
            }

            return View(kullanici);
        }



        [HttpGet]
        public ActionResult KullaniciListele(string searchString = null)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Giris");
            }

            var kullanicilar = from k in stokdata.KULLANICI select k;

            if (!String.IsNullOrEmpty(searchString))
            {
                kullanicilar = kullanicilar.Where(s => s.KUL_USERNAME.Contains(searchString) ||
                                                       s.KUL_AD.Contains(searchString) ||
                                                       s.KUL_SOYAD.Contains(searchString) ||
                                                       s.KUL_ID.ToString().Contains(searchString) ||
                                                       s.KUL_TIP.ToString().Contains(searchString) ||
                                                       (s.STATU == true ? "true" : "false").Contains(searchString));
            }

            return View(kullanicilar.ToList());
        }

        [HttpGet]
        public ActionResult KullaniciSil(int id)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Giris");
            }

            var kullanici = stokdata.KULLANICI.Find(id);
            if (kullanici == null)
            {
                return HttpNotFound();
            }

            return View(kullanici);
        }

        [HttpPost, ActionName("KullaniciSil")]
        public ActionResult KullaniciSilConfirmed(int id)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Giris");
            }

            var kullanici = stokdata.KULLANICI.Find(id);
            if (kullanici != null)
            {
                stokdata.KULLANICI.Remove(kullanici);
                stokdata.SaveChanges();
                ViewBag.Message = "Kullanıcı başarıyla silindi.";
            }

            return RedirectToAction("KullaniciListele");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}