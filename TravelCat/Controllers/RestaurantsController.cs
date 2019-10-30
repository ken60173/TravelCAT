﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelCat.Models;
using TravelCat.ViewModels;

namespace TravelCat.Controllers
{
    public class RestaurantsController : Controller
    {
        dbTravelCat db = new dbTravelCat();

        // GET: Hotels
        public ActionResult Index()
        {
            return View(db.restaurants.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(restaurant restaurant, HttpPostedFileBase tourism_photo)
        {
            string fileName = "";
            if (tourism_photo != null)
            {
                if (tourism_photo.ContentLength > 0)
                {
                    fileName = System.IO.Path.GetFileName(tourism_photo.FileName);
                    tourism_photo.SaveAs(Server.MapPath("~/images/restaurant/" + fileName));
                }
            }

            tourism_photo tp = new tourism_photo();
            tp.tourism_photo1 = fileName;
            tp.tourism_id = restaurant.restaurant_id;

            db.restaurants.Add(restaurant);
            db.tourism_photo.Add(tp);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        public ActionResult Delete(string Id)
        {
            var product = db.restaurants.Where(m => m.restaurant_id == Id).FirstOrDefault();
            db.restaurants.Remove(product);
            db.SaveChanges();

            var photos = db.tourism_photo.Where(m => m.tourism_id == Id).FirstOrDefault();
            string fileName = photos.tourism_photo1;
            if (fileName != "")
            {
                System.IO.File.Delete(Server.MapPath("~/images/restaurant/" + fileName));
            }
            db.tourism_photo.Remove(photos);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Edit(string id)
        {
            RestaurantPhotoViewModel model = new RestaurantPhotoViewModel()
            {
                restaurant = db.restaurants.Where(m => m.restaurant_id == id).FirstOrDefault(),
                restaurant_photos = db.tourism_photo.Where(m => m.tourism_id == id).FirstOrDefault()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(string id, RestaurantPhotoViewModel restaurantPhotoViewModel, HttpPostedFileBase tourism_photo, String oldImg)
        {

            db.Entry(restaurantPhotoViewModel).State = EntityState.Modified;
            db.SaveChanges();

            string fileName = "";
            if (tourism_photo != null)
            {
                if (tourism_photo.ContentLength > 0)
                {
                    System.IO.File.Delete(Server.MapPath("~/images/restaurant/" + oldImg));
                    fileName = System.IO.Path.GetFileName(tourism_photo.FileName);      //取得檔案的檔名(主檔名+副檔名)
                    tourism_photo.SaveAs(Server.MapPath("~/images/restaurant/" + fileName));      //將檔案存到該資料夾
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