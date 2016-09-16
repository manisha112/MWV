using System.Web.Mvc;
using System.Web.Routing;

namespace IdentitySample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            //added by Rajni
            //routes.MapRoute(
            //    name: "forMasterDetailDropDown",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Truck_dispatchViewModel", action = "Index", id = UrlParameter.Optional }
            //);


            //added by Rajni
            routes.MapRoute("searchCustomer",
                            "Home/searchCustomer/{agentid}/{searchStr}",
                            new { controller = "Home", action = "searchCustomer", id = @"\d+", searchStr = @"\d+" },
                            new[] { "MWV.Controllers" });

            routes.MapRoute("forProductCode",
                           "Order_product/GetProductCode/{bf_code}/{gsm_code}",
                           new { controller = "Order_product", action = "GetProductCode", bf_code = @"\d+", gsm_code = @"\d+" },
                           new[] { "MWV.Controllers" });

            //routes.MapRoute("forProductCodeAndPrice",
            //               "Order_product/GetProductCodeAndPrice/{bf_code}/{gsm_code}",
            //               new { controller = "Order_product", action = "GetProductCodeAndPrice", bf_code = @"\d+", gsm_code = @"\d+" },
            //               new[] { "MWV.Controllers" });



            routes.MapRoute("forProductPrice",
                           "Order_product/GetPrice/{customer_id}/{product_code}/{shade_code}",
                           new { controller = "Order_product", action = "GetPrice", customer_id = @"\d+", product_code = @"\d+", shade_code = @"\d+" },
                           new[] { "MWV.Controllers" });

            //routes.MapRoute("forOrderinSession",
            //               "Order/addOrderinSession/{customer_id}/{requested_delivery_date}",
            //               new { controller = "Order", action = "addOrderinSession", customer_id = @"\d+", requested_delivery_date = @"\d+" },
            //               new[] { "MWV.Controllers" });
            //added by Rajni

           // routes.MapRoute(
           //   name: "SaveOrder",
           //   url: "{ProductionPlanner}/{SubmitOrderToMill}/{id}",
           //   defaults: new { controller = "ProductionPlanner", action = "SubmitOrderToMill", id = UrlParameter.Optional },
           //namespaces: new[] { "TemplateSite.Mvc.Controllers" }
           //).DataTokens["UseNamespaceFallback"] = false;

      //      routes.MapRoute(
      //    name: "RentalProperty",
      //    url: "ProductionPlanner/{SubmitOrderToMill}/{string}",
      //    defaults: new
      //    {
      //        controller = "ProductionPlanner",
      //        action = "SubmitOrderToMill",
      //    }
      //);
 
        }
    }
}