using LaptopSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LaptopSystem.Model;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace LaptopSystem.Web.Controllers
{
    public class LaptopsController : BaseController
    {
        const int PageSize = 5;

        private IQueryable<LaptopViewModel> GetAllLaptops()
        {
            var data = this.Data.Laptops
                                .All()
                                .Select(x => new LaptopViewModel
                                        {
                                            Id = x.Id,
                                            ImageUrl = x.ImageStringUrl,
                                            Manufacturer = x.Manufactorer.Name,
                                            Model = x.Model,
                                            Price = x.Price
                                        })
                                .OrderBy(x => x.Id);

            return data;
        }


        public ActionResult KendoList()
        {


            return View();
        }

        public ActionResult Search(SubmitSearchModel submitModel)
        {
            var result = this.Data.Laptops.All();

            if (!string.IsNullOrEmpty(submitModel.ModelSearch))
            {
                result = result.Where(x => x.Model.ToLower().Contains(submitModel.ModelSearch.ToLower()));
            }

            if (submitModel.ManufacturerSearch != "All")
            {
                result = result.Where(x => x.Manufactorer.Name == submitModel.ManufacturerSearch);   
            }

            if (submitModel.PriceSearch != 0)
            {
                result = result.Where(x => x.Price <= submitModel.PriceSearch);
            }

            var finalResult = result.Select(x => new LaptopViewModel
            {
                Id = x.Id,
                ImageUrl = x.ImageStringUrl,
                Manufacturer = x.Manufactorer.Name,
                Model = x.Model,
                Price = x.Price
            });

            return View(finalResult);
        }

        public JsonResult GetLaptops([DataSourceRequest]DataSourceRequest request)
        {
            return Json(this.GetAllLaptops().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLaptopModelData(string text)
        {
            var result = this.Data.Laptops
                .All()
                .Where(x => x.Model.ToLower().Contains(text.ToLower()))
                .Select(x => new 
                {
                    Model = x.Model
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetManufacturerData()
        {
            var result = this.Data.Laptops
                .All()
                .Select(x => new
                {
                    ManufacturerName = x.Manufactorer.Name
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        public ActionResult List(int? Id)
        {
            int pageNumber = Id.GetValueOrDefault(1);

            var viewModel = GetAllLaptops().Skip((pageNumber - 1) * PageSize).Take(PageSize);

            ViewBag.Pages = Math.Ceiling((double)GetAllLaptops().Count() / PageSize);

            return View(viewModel);
        }

        //[Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult PostComment(SubmitCommentModel commentModel)
        {
            if (ModelState.IsValid)
            {

                var username = this.User.Identity.GetUserName();
                var userId = this.User.Identity.GetUserId();

                this.Data.Comments.Add(new Comment()
                {
                    AuthorId = userId,
                    Content = commentModel.Comment,
                    LaptopId = commentModel.LaptopId
                });

                this.Data.SaveChanges();

                var viewModel = new CommentViewModel()
                                {
                                    Author = username,
                                    Content = commentModel.Comment
                                };

                return PartialView("_CommentPartial", viewModel);
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest,ModelState.Values.First().ToString());
        }

        public ActionResult Details(int id)
        {
            var userId = User.Identity.GetUserId();
            var viewModel = this.Data.Laptops.All().Where(x => x.Id == id)
                .Select(x => new LaptopDetailsViewModel
                {
                    Id = x.Id,
                    ManufacturerName = x.Manufactorer.Name,
                    Model = x.Model,
                    MonitorSize = x.MonitorSize,
                    RamMemorySize = x.RamMemorySize,
                    Weight = x.Weight,
                    Price = x.Price,
                    ImageStringUrl = x.ImageStringUrl,
                    Description = x.Description,
                    AdditionalParts = x.AdditionalParts,
                    VotesCount = x.Votes.Count,
                    UserCanVote = !x.Votes.Any(p => p.VotedById == userId),
                    Comments = x.Comments.Select(y => new CommentViewModel
                    {
                        Author = y.Author.UserName,
                        Content = y.Content
                    })
                }).FirstOrDefault();

            return View(viewModel);
        }

        public ActionResult Vote(int id)
        {
            var userId = User.Identity.GetUserId();
            var canVote = !this.Data.Votes.All().Any(x => x.LaptopId == id && x.VotedById == userId);

            if (canVote)
            {
                this.Data.Laptops.GetById(id).Votes.Add(new Vote
                {
                    VotedById = userId,
                    LaptopId = id
                });

                this.Data.SaveChanges();
            }

            var votesCount = this.Data.Laptops.GetById(id).Votes.Count().ToString();

            return Content(votesCount);
        }
    }
}