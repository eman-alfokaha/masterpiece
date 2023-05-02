using onlinemarketing.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace onlinemarketing.Controllers
{
    public class UserController : Controller

    {
        onlinemarketingEntities db = new onlinemarketingEntities();

        // GET: User
        public ActionResult Index(int? page)
        {

            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.cateogorys.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<cateogory> cate = list.ToPagedList(pageindex, pagesize);
            return View(cate);


        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(tbl_user us, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);

            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not be uploaded";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = us.u_name;
                u.u_email = us.u_email;
                u.u_password = us.u_password;
                u.u_contact = us.u_contact;
                u.u_image = path;
                db.tbl_user.Add(u);
                db.SaveChanges();

                return RedirectToAction("Login");

            }
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(tbl_user svm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_email == svm.u_email && x.u_password == svm.u_password).SingleOrDefault();

            if (ad != null)
            {
                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("Add_Post");
            }
            else
            {
                ViewBag.error = "Invalid User Name or Password";
            }
            return View();
        }

        [HttpGet]
        public ActionResult CreateAdd()
        {
            List<cateogory> li = db.cateogorys.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateAdd(product p, HttpPostedFileBase imgfile)
        {
            List<cateogory> li = db.cateogorys.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");

            string path = uploadimage(imgfile);

            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not be uploaded";
            }
            else
            {

                product pr = new product();
                pr.pro_name = p.pro_name;
                pr.pro_price = p.pro_price;
                pr.pro_image = path;
                pr.cat_id_fk = p.cat_id_fk;
                pr.pro_desc = p.pro_desc;
                p.pro_user_id_fk = Convert.ToInt32(Session["u_id"].ToString());
                db.products.Add(pr);
                db.SaveChanges();

                Response.Redirect("Index");
            }
            return View();

        }
        
        public ActionResult Add_Post()
        { return View(); }




        //image upload user
        public string uploadimage(HttpPostedFileBase file)
            {
                Random r = new Random();
                string path = "-1";
                int random = r.Next();
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName);

                    if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                    {

                        try
                        {
                            path = Path.Combine(Server.MapPath("~/Content/upload/"), random + Path.GetFileName(file.FileName));

                            file.SaveAs(path);
                            path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);


                        }
                        catch (Exception ex)
                        {
                            path = "-1";
                        }

                    }
                    else
                    {
                        Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....');</script>");
                    }
                }
                else
                {
                    Response.Write("<script>alert('Please select a file');</script>");

                    path = "-1";

                }
                return path;
            }



            //End image user.................................
        }
    }
