using ASP.NET_MVC_5_Datatables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.Entity;

namespace ASP.NET_MVC_5_Datatables.Controllers
{
    public class DemoController : Controller
    {
        
        // GET: Demo
        public ActionResult ShowGrid()
        {
            return View();
        }

        public JsonResult LoadData()
        {
            try
            {
                //Creating instance of DatabaseContext class  
                using (DatabaseContext _context = new DatabaseContext())
                {
                    var draw = Request.Form.GetValues("draw").FirstOrDefault();
                    var start = Request.Form.GetValues("start").FirstOrDefault();
                    var length = Request.Form.GetValues("length").FirstOrDefault();
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                    //Paging Size (10,20,50,100)    
                    int pageSize = length != null ? Convert.ToInt32(length) : 0;
                    int skip = start != null ? Convert.ToInt32(start) : 0;
                    int recordsTotal = 0;
                    
                    // Getting all Customer data    
                    var customerData = (from tempcustomer in _context.Customers
                                        select tempcustomer);

                    //Sorting    
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        customerData = customerData.OrderBy(sortColumn + " " + sortColumnDir);
                        
                    }
                    //Search    
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        customerData = customerData.Where(m => m.CompanyName.Contains(searchValue));//staviti prvo branch za obican search
                    }

                    //total number of rows count     
                    recordsTotal = customerData.Count();
                    //Paging     
                    var data = customerData.Skip(skip).Take(pageSize).ToList();

                    //Returning Json Data    
                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet]
        public ActionResult Edit(string ID)
        {
            try
            {
                using (DatabaseContext _context = new DatabaseContext())
                {
                    var Customer = (from customer in _context.Customers
                                    where customer.CustomerID == ID
                                    select customer).FirstOrDefault();

                    return View(Customer);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Edit(Customers customers)
        {
            using (DatabaseContext _context = new DatabaseContext())
            {
                if (ModelState.IsValid)
                {

                    _context.Entry(customers).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("ShowGrid");
                }
                return View(customers);
            }
        }

        [HttpPost]
        public JsonResult DeleteCustomer(string ID)
        {
            using (DatabaseContext _context = new DatabaseContext())
            {
                var customer = _context.Customers.Find(ID);
                if (ID == null)
                    return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
                _context.Customers.Remove(customer);
                _context.SaveChanges();

                return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
        }


    }
}