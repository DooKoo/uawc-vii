using Microsoft.AspNet.Mvc;
using System;


namespace uwcua
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Result(string url)
        {
            try
            {
                ViewData["FilePath"] = (new SiteMapBuilder(url)).FilePath;
                return View();
            }
            catch(UriFormatException e)
            {
                return Error("Incorrect url:\"" + e.Message + "\" Try again please :(");
            }

            catch(System.Net.WebException e)
            {
                return Error(e.Message);
            }
            
            catch(Exception e)
            {
                return Error(e.Message);
            }

        }

        public IActionResult Error(string message)
        {
            if (message == null)
                message = "Error 404. Page not found :(";

            ViewData["message"] = message;
            return View("Error");
        }

        public string Xmls()
        {
            string path = "./" + Request.Path.ToString();
            if (System.IO.File.Exists(path))
            {
                string allText = System.IO.File.ReadAllText(path);
                System.IO.File.Delete(path);
                return allText;
            }
            else
                return null;
        }
    }
}