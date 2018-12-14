using ASP.NET_MVC_5_Datatables.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace ASP.NET_MVC_5_Datatables.Controllers
{
    public class DemoController : Controller
    {
        
        // GET: Demo
        public ActionResult ShowGrid()
        {
            return View();
        }

        public ActionResult LoadData()
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
                        customerData = customerData.Where(m => m.CompanyName == searchValue);
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


    }
}