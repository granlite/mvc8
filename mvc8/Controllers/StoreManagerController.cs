using muscshop.Context;
using muscshop.filters;
using muscshop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace muscshop.Controllers
{
    [Authorize(Roles = "Manager")]
    public class StoreManagerController : Controller
    {
        StoreContext _storeContext = new StoreContext();
        public ActionResult Index()
        {
            var albums = _storeContext.Albums;
            return View(albums);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var albums = _storeContext.Albums.Where(x => x.AlbumId == id).FirstOrDefault();
            if (albums == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtistId = new SelectList(_storeContext.Artists, "ArtistId", "Name", albums.ArtistId);
            ViewBag.GenreId = new SelectList(_storeContext.Genres, "GenreId", "Name", albums.GenreId);
            return View(albums);
        }
        [HttpPost]
        public ActionResult Edit(Album updateAlbum)
        {
            if (ModelState.IsValid)
            {
                _storeContext.Entry(updateAlbum).State = EntityState.Modified;
                _storeContext.SaveChanges();
                return RedirectToAction("index");
            }
            else
            {
                return Edit(updateAlbum.AlbumId);
            }
        }
        public ActionResult Create()
        {
            ViewBag.ArtistId = new SelectList(_storeContext.Artists, "ArtistId", "Name", 1);
            ViewBag.GenreId = new SelectList(_storeContext.Genres, "GenreId", "Name", 1);
            return View();
        }
        [HttpPost]
        public ActionResult Create(Album newalbum)
        {
            if (!ModelState.IsValid)
            {
                List<KeyValuePair<string, ModelState>> errorList = ModelState.Where(x => x.Value.Errors.Count > 0).ToList();
                ViewBag.ErrorList = errorList;
                return Create();
            }
            _storeContext.Albums.Add(newalbum);
            _storeContext.SaveChanges();
            return RedirectToAction("index");
        }
        public ActionResult DeleteAlb(int id)
        {
            var deletealbum = _storeContext.Albums.Find(id); ;
            _storeContext.Albums.Remove(deletealbum);
            _storeContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Search(string par)
        {
            var albums = _storeContext.Albums.Where(x => x.Title.ToLower().Contains(par.ToLower()));
            return View("index", albums);
        }
    }
}