﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelCat.Models;
using TravelCat.ViewModels;

namespace TravelCat.Controllers
{
    public class HotelsController : Controller
    {
        dbTravelCat db = new dbTravelCat();

        // GET: Hotels
        public ActionResult Index()
        {
            return View(db.hotels.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]    
        public ActionResult Create(hotel hotel, HttpPostedFileBase tourism_photo)     
        {
            string fileName = "";
            if (tourism_photo != null)
            {
                if (tourism_photo.ContentLength > 0)
                {
                    fileName = System.IO.Path.GetFileName(tourism_photo.FileName);      
                    tourism_photo.SaveAs(Server.MapPath("~/images/hotel/" + fileName));    
                }
            }

            tourism_photo tp = new tourism_photo();
            tp.tourism_photo1 = fileName;
            tp.tourism_id = hotel.hotel_id;

            db.hotels.Add(hotel);
            db.tourism_photo.Add(tp);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        public ActionResult Delete(string Id)
        {
            var product = db.hotels.Where(m => m.hotel_id == Id).FirstOrDefault();
            db.hotels.Remove(product);
            db.SaveChanges();

            var photos = db.tourism_photo.Where(m => m.tourism_id == Id).FirstOrDefault();
            string fileName = photos.tourism_photo1;
            if (fileName != "")
            {
                System.IO.File.Delete(Server.MapPath("~/images/hotel/" + fileName));
            }
            db.tourism_photo.Remove(photos);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Edit(string id)
        {
            HotelPhotoViewModel model = new HotelPhotoViewModel()
            {
                hotel = db.hotels.Where(m => m.hotel_id == id).FirstOrDefault(),
                hotel_photos = db.tourism_photo.Where(m => m.tourism_id == id).FirstOrDefault()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(string id, HotelPhotoViewModel hotelPhotoViewModel, HttpPostedFileBase tourism_photo, String oldImg)
        {

            db.Entry(hotelPhotoViewModel.hotel).State = EntityState.Modified;
            db.SaveChanges();

            string fileName = "";
            if (tourism_photo != null)
            {
                if (tourism_photo.ContentLength > 0)
                {
                    System.IO.File.Delete(Server.MapPath("~/images/hotel/" + oldImg));
                    fileName = System.IO.Path.GetFileName(tourism_photo.FileName);      //取得檔案的檔名(主檔名+副檔名)
                    tourism_photo.SaveAs(Server.MapPath("~/images/hotel/" + fileName));      //將檔案存到該資料夾
                }
            }
            else
            {
                fileName = oldImg;
            }
            var tp1 = db.tourism_photo.Where(m => m.tourism_id == id).FirstOrDefault();

            tp1.tourism_photo1 = fileName;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}