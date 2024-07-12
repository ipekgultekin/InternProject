using System;
using System.Collections.Generic;
using System.IO;
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
            var info = stokdata.KULLANICI.FirstOrDefault(x => x.KUL_USERNAME == fc.KUL_USERNAME && x.KUL_SIFRE == fc.KUL_SIFRE);

            if (info != null)
            {
                FormsAuthentication.SetAuthCookie(info.KUL_USERNAME, false);
                Session["ID"] = info.KUL_ID.ToString();
                Session["KullaniciTipi"] = info.KUL_TIP;
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
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }
            var model = new KULLANICI();

            return View(model);
        }

        [HttpPost]
        public ActionResult KullaniciEkle(KULLANICI kullanici)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
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
                TempData["SuccessMessage"] = "Kullanıcı başarıyla eklendi.";

                return RedirectToAction("KullaniciListele");
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
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }

            var kullanici = stokdata.KULLANICI.Find(id);
            if (kullanici == null)
            {
                return HttpNotFound();
            }

            stokdata.KULLANICI.Remove(kullanici);
            stokdata.SaveChanges();
            TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            return RedirectToAction("KullaniciListele");
        }

        [HttpGet]
        public ActionResult KullaniciGuncelle(int id)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
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

        [HttpPost]
        public ActionResult KullaniciGuncelle(KULLANICI kullanici)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }
            if (ModelState.IsValid)
            {
                var updateKullanici = stokdata.KULLANICI.Find(kullanici.KUL_ID);

                if (updateKullanici != null)
                {
                    updateKullanici.KUL_USERNAME = kullanici.KUL_USERNAME;
                    updateKullanici.KUL_AD = kullanici.KUL_AD;
                    updateKullanici.KUL_SOYAD = kullanici.KUL_SOYAD;
                    updateKullanici.KUL_SIFRE = kullanici.KUL_SIFRE;
                    updateKullanici.KUL_TIP = kullanici.KUL_TIP;
                    updateKullanici.STATU = kullanici.STATU;

                    stokdata.SaveChanges();
                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
                    return RedirectToAction("KullaniciListele");
                }
            }

            return View(kullanici);
        }


        [HttpGet]
        public ActionResult DepoEkle()
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }
            var depoModel = new DEPO();

            return View(depoModel);
        }

        [HttpPost]
        public ActionResult DepoEkle(DEPO depo)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }

            if (ModelState.IsValid)
            {
                var depoModel = new DEPO()
                {
                  
                    DEPO_ADI = depo.DEPO_ADI,
                    STATU = depo.STATU,
                    OLUSTURAN_KULLANICI = Convert.ToInt32(Session["ID"]),
                    OLUSTURMA_TARIHI = DateTime.Now,
                    GUNCELLEME_TARIHI = DateTime.Now,
                    GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["ID"]),
                };

                stokdata.DEPO.Add(depoModel);
                stokdata.SaveChanges();
                TempData["SuccessMessage"] = "Depo başarıyla eklendi.";

                return RedirectToAction("DepoListele");
            }

            return View(depo);
        }


        [HttpGet]
        public ActionResult DepoListele(string searchString = null)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }

            var depolar = from d in stokdata.DEPO select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                depolar = depolar.Where(s => s.DEPO_ADI.Contains(searchString) ||
                                             s.STATU.ToString().Contains(searchString));
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(depolar.ToList());
        }

        [HttpGet]
        public ActionResult DepoSil(int id)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Giris");
            }

            var depo = stokdata.DEPO.Find(id);
            if (depo != null)
            {
                stokdata.DEPO.Remove(depo);
                stokdata.SaveChanges();
                TempData["SuccessMessage"] = "Depo başarıyla silindi.";
            }

            return RedirectToAction("DepoListele");
        }

        [HttpGet]
        public ActionResult DepoGuncelle(int id)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }

            var depo = stokdata.DEPO.Find(id);
            if (depo == null)
            {
                return HttpNotFound();
            }

            return View(depo);
        }

        [HttpPost]
        public ActionResult DepoGuncelle(DEPO depo)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }
            if (ModelState.IsValid)
            {
                var updateDepo = stokdata.DEPO.Find(depo.DEPO_ID);

                if (updateDepo != null)
                {
                    updateDepo.DEPO_ADI = depo.DEPO_ADI;
                    updateDepo.STATU = depo.STATU;

                    stokdata.SaveChanges();
                    TempData["SuccessMessage"] = "Depo başarıyla güncellendi.";
                    return RedirectToAction("DepoListele");
                }
            }

            return View(depo);
        }


        [HttpGet]
        public ActionResult AltDepoEkle()
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }
            var altdepoModel = new ALT_DEPO();

            return View(altdepoModel);
        }

        [HttpPost]
        public ActionResult AltDepoEkle(ALT_DEPO depo)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }

            if (ModelState.IsValid)
            {
                var depoModel = new ALT_DEPO()
                {
                    ALT_DEPO_ADI = depo.ALT_DEPO_ADI,
                    STATU = depo.STATU,
                    OLUSTURAN_KULLANICI = Convert.ToInt32(Session["ID"]),
                    OLUSTURMA_TARIHI = DateTime.Now,
                    GUNCELLEME_TARIHI = DateTime.Now,
                    GUNCELLEYEN_KULLANICI = Convert.ToInt32(Session["ID"]),
                };

                stokdata.ALT_DEPO.Add(depoModel);
                stokdata.SaveChanges();
                TempData["SuccessMessage"] = "Alt depo başarıyla eklendi.";

                return RedirectToAction("AltDepoListele");
            }

            return View(depo);
        }


        [HttpGet]
        public ActionResult AltDepoListele(string searchString = null)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }

            var depolar = from d in stokdata.ALT_DEPO select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                depolar = depolar.Where(s => s.ALT_DEPO_ADI.Contains(searchString) ||
                                             s.STATU.ToString().Contains(searchString));
            }

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(depolar.ToList());
        }

        [HttpGet]
        public ActionResult AltDepoSil(int id)
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Giris");
            }

            var depo = stokdata.ALT_DEPO.Find(id);
            if (depo != null)
            {
                stokdata.ALT_DEPO.Remove(depo);
                stokdata.SaveChanges();
                TempData["SuccessMessage"] = "Depo başarıyla silindi.";
            }

            return RedirectToAction("AltDepoListele");
        }

        [HttpGet]
        public ActionResult AltDepoGuncelle(int id)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }

            var depo = stokdata.ALT_DEPO.Find(id);
            if (depo == null)
            {
                return HttpNotFound();
            }

            return View(depo);
        }

        [HttpPost]
        public ActionResult AltDepoGuncelle(ALT_DEPO depo)
        {
            if (Session["ID"] == null || (int)Session["KullaniciTipi"] != 1)
            {
                return RedirectToAction("Giris");
            }
            if (ModelState.IsValid)
            {
                var updateDepo = stokdata.ALT_DEPO.Find(depo.ALT_DEPO_ID);

                if (updateDepo != null)
                {
                    updateDepo.ALT_DEPO_ADI = depo.ALT_DEPO_ADI;
                    updateDepo.STATU = depo.STATU;

                    stokdata.SaveChanges();
                    TempData["SuccessMessage"] = "Depo başarıyla güncellendi.";
                    return RedirectToAction("AltDepoListele");
                }
            }

            return View(depo);
        }


        public ActionResult Cikis()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Giris");
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
