using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Filters {
    public class CheckNotSessionAdmin : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var currentSession = HttpContext.Current.Session;

            if ( currentSession [ "admin_id" ] != null ) {
                filterContext.Result = new RedirectResult( "/admin/user" );
            }
        }
    }
}