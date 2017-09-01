using AreaUploader.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.IO;
using static AreaUploader.Models.AreaPath;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace AreaUploader.Controllers
{
    public class HomeController : Controller
    {
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
        private string logPath;
        private string logFileName;

        string accname = string.Empty;
        string PAT = string.Empty;
        string URL = string.Empty;
        string projectName = "";


        string _credentials;
        Configuration _configuration;
        string errlist = string.Empty;
        string errors = string.Empty;
        public ActionResult Index(AreaUpload.Authentication model, string id)
        {
            if (Session["Username"] != null && Session["Password"] != null)
            {
                return View(model);
            }
            return RedirectToAction("Login", "Access");
        }
        [HttpPost]
        public ActionResult Index(AreaUpload.Authentication model)
        {

            bool isvalid = ValidateDetails(model);
            if (isvalid)
            {
                return RedirectToAction("CopyArea", new
                {
                    pat = model.pat,
                    accname = model.accname,
                });
            }
            return RedirectToAction("Index", new
            {
                pat = model.pat,
                accname = model.accname,
                Message = "Invalid PAT or Account Name",
                errors = errors.TrimEnd('*')
            });
        }

        public ActionResult CopyArea(AreaUpload.Authentication mod)
        {
            try
            {
                if (Session["Username"] != null && Session["Password"] != null)
                {

                    AreaUpload.ProjectCount modal = new AreaUpload.ProjectCount();
                    if (mod.accname != null && mod.pat != null)
                    {

                        modal = GetprojectList(mod);
                        List<SelectListItem> projects = new List<SelectListItem>();

                        foreach (var project in modal.value)
                        {
                            projects.Add(new SelectListItem { Text = project.name, Value = project.id });
                        }
                        modal.ProjectList = new SelectList(projects);
                        modal.AccName = mod.accname;
                        modal.PAT = mod.pat;                        
                        modal.accURL = mod.accURL;
                        modal.SuccessMsg = mod.SuccessMsg;
                        if (mod.ErrList!=null)
                        {
                            modal.ErrList = "One or more areas in the list provided are already present. Please verify your file and try again.";
                        }
                        return View(modal);
                    }
                    return RedirectToAction("Login", "Access");

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                errors = errors + message + '*';
            }
            return RedirectToAction("Index", new
            {
                pat = mod.pat,
                accname = mod.accname,
                Message = "Invalid PAT or Account Name",
                errors = errors
            });
        }

        public bool ValidateDetails(AreaUpload.Authentication auth)
        {
            AreaUpload.ProjectCount load = new AreaUpload.ProjectCount();
            accname = auth.accname;
            PAT = auth.pat;
            string URL = "https://" + accname + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", auth.pat)));
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync("/_apis/projects?stateFilter=All&api-version=1.0").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        load = JsonConvert.DeserializeObject<AreaUpload.ProjectCount>(res);
                        return true;
                    }
                    //  return Json(load, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                logPath = System.Web.HttpContext.Current.Server.MapPath("~/ApiLog/AreaUploaderErrors");
                logFileName = logPath + "\\ERROR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + "Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);

                string message = ex.Message.ToString();
                errors = errors + message + '*';
            }
            return false;

        }

        public AreaUpload.ProjectCount GetprojectList(AreaUpload.Authentication auth)
        {
            AreaUpload.ProjectCount load = new AreaUpload.ProjectCount();
            accname = auth.accname;
            PAT = auth.pat;
            URL = "https://" + accname + ".visualstudio.com/DefaultCollection/";
            string _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", auth.pat)));
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(URL);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("appication/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);
                    HttpResponseMessage response = client.GetAsync("/_apis/projects?stateFilter=All&api-version=1.0").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        load = JsonConvert.DeserializeObject<AreaUpload.ProjectCount>(res);
                    }
                }
            }
            catch (Exception ex)
            {
                logPath = System.Web.HttpContext.Current.Server.MapPath("~/ApiLog/AreaUploaderErrors");
                logFileName = logPath + "\\ERROR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + "Error has been occured :" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                watch.Stop();

                string message = ex.Message.ToString();
                errors = errors + message + '*';
            }
            return load;

        }
        public string file(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Files"),
                Path.GetFileName(file.FileName));
                file.SaveAs(path);
                return path;
            }
            else
            {
                return null;
            }
        }
        Stopwatch watch = new Stopwatch();

        public ActionResult Import(AreaUpload.ProjectCount choose)
        {
            watch.Start();
            projectName = choose.SelectedID;
            string path = file(choose.filechoosen);
            if (path != null)
            {
                logPath = System.Web.HttpContext.Current.Server.MapPath("~/ApiLog");
                logFileName = logPath + "\\AreaUploader_TestData" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + Environment.NewLine);

                _credentials = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", choose.PAT)));
                _configuration = new Configuration() { UriString = "https://" + choose.AccName + ".visualstudio.com", PersonalAccessToken = choose.PAT };

                AreaPaths ap = new AreaPaths();
                ap.Suites = new List<Suite>();

                string[] data = System.IO.File.ReadAllLines(path);

                foreach (string row in data)
                {
                    string suiteName = string.Empty;
                    string familtyName = string.Empty;
                    string productName = string.Empty;
                    string areaName = string.Empty;

                    TextFieldParser parser = new TextFieldParser(new StringReader(row));
                    parser.HasFieldsEnclosedInQuotes = true;
                    parser.SetDelimiters(",");
                    string[] values = new string[] { };

                    while (!parser.EndOfData)
                    {
                        values = parser.ReadFields();
                    }

                    parser.Close();
                    if (values.Length == 2)
                    {
                        suiteName = values[0];
                        familtyName = values[1];
                    }
                    else if (values.Length == 3)
                    {
                        suiteName = values[0];
                        familtyName = values[1];
                        productName = values[2];
                    }
                    else if (values.Length == 4)
                    {
                        suiteName = values[0];
                        familtyName = values[1];
                        productName = values[2];
                        areaName = values[3];
                    }
                    //string suiteName = values[0];
                    //string familtyName = values[1];
                    //string productName = values[2];
                    //string areaName = values[3];
                    //string subAreaName = values[4];

                    //SubArea subarea = new SubArea();
                    //subarea.name = subAreaName;

                    Areas areas = new Areas();
                    areas.name = areaName;

                    Products product = new Products();
                    product.name = productName;

                    Family family = new Family();
                    family.name = familtyName;

                    Suite suit = new Suite();
                    suit.name = suiteName;

                    var fSuite = ap.Suites.FirstOrDefault(i => i.name == suiteName);
                    if (fSuite == null)
                    {
                        ap.Suites.Add(suit);
                        suit.Families = new List<Family>();
                        suit.Families.Add(family);
                        family.Products = new List<Products>();
                        family.Products.Add(product);
                        product.Areas = new List<Areas>();
                        product.Areas.Add(areas);
                        areas.Subareas = new List<SubArea>();
                        //areas.Subareas.Add(subarea);
                    }
                    else
                    {
                        var fFamilty = fSuite.Families.FirstOrDefault(i => i.name == familtyName);
                        if (fFamilty == null)
                        {
                            fSuite.Families.Add(family);
                            family.Products = new List<Products>();
                            family.Products.Add(product);
                            product.Areas = new List<Areas>();
                            product.Areas.Add(areas);
                            areas.Subareas = new List<SubArea>();
                            //areas.Subareas.Add(subarea);
                        }
                        else
                        {
                            var fProduct = fFamilty.Products.FirstOrDefault(i => i.name == productName);
                            if (fProduct == null)
                            {
                                fFamilty.Products.Add(product);
                                product.Areas = new List<Areas>();
                                product.Areas.Add(areas);
                                areas.Subareas = new List<SubArea>();
                                //areas.Subareas.Add(subarea);
                            }
                            else
                            {
                                var fArea = fProduct.Areas.FirstOrDefault(i => i.name == areaName);
                                if (fArea == null)
                                {
                                    fProduct.Areas.Add(areas);
                                    areas.Subareas = new List<SubArea>();
                                    //areas.Subareas.Add(subarea);
                                }
                                else
                                {
                                    if (fArea.Subareas == null)
                                        fArea.Subareas = new List<SubArea>();

                                    //fArea.Subareas.Add(subarea);
                                }
                            }
                        }
                    }
                }
                foreach (var suite in ap.Suites)
                {
                    string suiteName = suite.name.Replace("\"", string.Empty).Replace("&", "and").Trim();
                    suite.id = CreateAreaPath(projectName, suiteName);

                    foreach (var family in suite.Families)
                    {
                        string familyName = family.name.Replace("\"", string.Empty).Replace("&", "and").Replace("/", "-").Trim();
                        //if (suite.id > 0)
                        {
                            family.id = CreateAreaPath(projectName, familyName);
                            MoveArePath(projectName, family.id, suiteName);
                        }

                        foreach (var product in family.Products)
                        {

                            string productName = product.name.Replace("\"", string.Empty).Replace("&", "and").Replace("/", "-").Trim();

                            product.id = CreateAreaPath(projectName, productName);
                            MoveArePath(projectName, product.id, suiteName + "\\" + familyName);


                            foreach (var area in product.Areas)
                            {

                                string areaName = area.name.Replace("\"", string.Empty).Replace("&", "and").Replace("/", "-").Trim();

                                area.id = CreateAreaPath(projectName, areaName);
                                MoveArePath(projectName, area.id, suiteName + "\\" + familyName + "\\" + productName);

                                foreach (var subArea in area.Subareas)
                                {
                                    string subAreaName = subArea.name.Replace("\"", string.Empty).Replace("&", "and").Replace("/", "-").Trim();

                                    subArea.id = CreateAreaPath(projectName, subAreaName);
                                    MoveArePath(projectName, subArea.id, suiteName + "\\" + familyName + "\\" + productName + "\\" + areaName);
                                }
                            }
                        }

                    }
                }
            }
            AreaUpload.ProjectCount er = new AreaUpload.ProjectCount();

            if (System.IO.File.Exists(path))
            {
                // Use a try block to catch IOExceptions, to 
                // handle the case of the file already being 
                // opened by another process. 
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (System.IO.IOException e)
                {
                    logPath = System.Web.HttpContext.Current.Server.MapPath("~/ApiLog/AreaUploaderErrors");
                    logFileName = logPath + "\\ERROR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + "Error has been occured :" + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                    watch.Stop();
                    er.errmsg = e.Message;
                }
            }
            //return RedirectToAction("CopyArea","Home");
            errlist = errlist.TrimEnd('*');
            AreaUpload.Authentication returnmod = new AreaUpload.Authentication();
            if (errlist.Length > 0)
            {
                returnmod.SuccessMsg = "Area paths uploaded successfully with errors..";
            }
            else
            {
                returnmod.SuccessMsg = "Area paths uploaded successfully..";
            }
            returnmod.ErrList = errlist;
            returnmod.pat = choose.PAT;
            returnmod.accname = choose.AccName;
            returnmod.accURL = _configuration.UriString + "/" + choose.SelectedID + "/_admin/_work?_a=areas";
            return RedirectToAction("CopyArea", returnmod);
            //return "00";
        }

        public void GetListOfAreaPaths(string project)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                HttpResponseMessage response = client.GetAsync(project + "/_apis/wit/classificationNodes/areas?$depth=2&api-version=1.0").Result;

                if (response.IsSuccessStatusCode)
                {
                    var results = response.Content.ReadAsStringAsync().Result;
                }
            }
        }


        public int CreateAreaPath(string project, string name)
        {
            if (string.IsNullOrEmpty(name)) return 0;

            CreateUpdateNodeViewModel.Node node = new CreateUpdateNodeViewModel.Node();
            node.name = name;

            GetNodeResponse.Node viewModel = new GetNodeResponse.Node();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);


                // serialize the fields array into a json string          
                var postValue = new StringContent(JsonConvert.SerializeObject(node), Encoding.UTF8, "application/json");
                var method = new HttpMethod("POST");

                // send the request
                string url = _configuration.UriString + "/DefaultCollection/" + project + "/_apis/wit/classificationNodes/areas?api-version=1.0";
                var request = new HttpRequestMessage(method, url) { Content = postValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetNodeResponse.Node>().Result;
                    viewModel.Message = "success";
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    //GetListOfAreaPaths(project);
                    viewModel.Message = msg.ToString();
                    //int id = 0;
                    //if (viewModel.Message.IndexOf("VS402371") > 0)
                    //{
                    //    id = responseForInvalidStatusCode.Id;
                    //}


                    logPath = System.Web.HttpContext.Current.Server.MapPath("~/ApiLog/AreaUploaderErrors");
                    logFileName = logPath + "\\ERROR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + "Error has been occured :" + Environment.NewLine + viewModel.Message + Environment.NewLine);

                    JObject jItems = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    string message = jItems["message"].ToString();
                    errlist = errlist + message + '*';

                }
                viewModel.HttpStatusCode = response.StatusCode;
                return viewModel.id;

            }
        }

        public int MoveArePath(string project, int childId, string parentName)
        {
            if (childId == 0) return 0;

            CreateUpdateNodeViewModel.Node node = new CreateUpdateNodeViewModel.Node();
            node.id = childId;
            GetNodeResponse.Node viewModel = new GetNodeResponse.Node();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_configuration.UriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _credentials);

                // serialize the fields array into a json string          
                var postValue = new StringContent(JsonConvert.SerializeObject(node), Encoding.UTF8, "application/json");
                var method = new HttpMethod("POST");

                // send the request
                string url = _configuration.UriString + "/DefaultCollection/" + project + "/_apis/wit/classificationNodes/areas/" + parentName + "?api-version=1.0";
                var request = new HttpRequestMessage(method, url) { Content = postValue };
                var response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    viewModel = response.Content.ReadAsAsync<GetNodeResponse.Node>().Result;
                    viewModel.Message = "success";
                }
                else
                {
                    dynamic responseForInvalidStatusCode = response.Content.ReadAsAsync<dynamic>();
                    Newtonsoft.Json.Linq.JContainer msg = responseForInvalidStatusCode.Result;
                    viewModel.Message = msg.ToString();
                    //int id = 0;
                    //if (viewModel.Message.IndexOf("VS402371") > 0)
                    //{
                    //    id = responseForInvalidStatusCode.Id;

                    //}

                    logPath = System.Web.HttpContext.Current.Server.MapPath("~/ApiLog/AreaUploaderErrors");
                    logFileName = logPath + "\\ERROR_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    LogData("Vsts call has been made at:" + DateTime.Now + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Time Taken: " + watch.Elapsed.TotalSeconds.ToString() + Environment.NewLine + "Error has been occured :" + Environment.NewLine + viewModel.Message + Environment.NewLine);
                    // return id;
                    //errlist.Add(responseForInvalidStatusCode.Result.message);
                    JObject jItems = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    string message = jItems["message"].ToString();
                    errlist = errlist + message + '*';

                }
                viewModel.HttpStatusCode = response.StatusCode;
                // errlist.Add(JsonConvert.DeserializeObject<string>(viewModel.Message));
                return viewModel.id;

            }

        }

        private void LogData(string message)
        {
            //File.Create(logFileName);
            System.IO.File.AppendAllText(logFileName, message);
        }

    }
}