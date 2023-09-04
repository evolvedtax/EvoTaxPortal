using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;

namespace EvolvedTax1099
{
    public static class HtmlHelpers
    {
        private const string IdsToReuseKey = "__htmlPrefixScopeExtensions_IdsToReuse_";
        public static string IsSelected(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = html.ViewContext.RouteData.Values["action"] as string;
            string currentController = html.ViewContext.RouteData.Values["controller"] as string;

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }
        public static IDisposable BeginCollectionItem(this IHtmlHelper html, string collectionName, string guid = "")
        {
            return BeginCollectionItem(html, collectionName, guid, html.ViewContext.Writer);
        }
        public static IDisposable BeginCollectionItem(this IHtmlHelper html, string collectionName, string guid, TextWriter writer)
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            if (html.ViewData["ContainerPrefix"] != null)
                collectionName = string.Concat(html.ViewData["ContainerPrefix"], ".", collectionName);
            var idsToReuse = GetIdsToReuse(html.ViewContext.HttpContext, collectionName);
            var itemIndex = idsToReuse.Count > 0 ? idsToReuse.Dequeue() : guid;
            string htmlFieldPrefix = $"{collectionName}[{itemIndex}]";
            html.ViewData["ContainerPrefix"] = htmlFieldPrefix;
            /* 
             * html.Name(); has been removed
             * because of incorrect naming of collection items
             * e.g.
             * let collectionName = "Collection"
             * the first item's name was Collection[0].Collection[<GUID>]
             * instead of Collection[<GUID>]
             */
            string indexInputName = $"{collectionName}.index";
            // autocomplete="off" is needed to work around a very annoying Chrome behaviour
            // whereby it reuses old values after the user clicks "Back", which causes the
            // xyz.index and xyz[...] values to get out of sync.
            writer.WriteLine($@"<input type=""hidden"" name=""{indexInputName}"" autocomplete=""off"" value=""{html.Encode(itemIndex)}"" />");
            return BeginHtmlFieldPrefixScope(html, htmlFieldPrefix);
        }
        public static IDisposable BeginHtmlFieldPrefixScope(this IHtmlHelper html, string htmlFieldPrefix)
        {
            return new HtmlFieldPrefixScope(html.ViewData.TemplateInfo, htmlFieldPrefix);
        }
        private static Queue<string> GetIdsToReuse(HttpContext httpContext, string collectionName)
        {
            // We need to use the same sequence of IDs following a server-side validation failure,
            // otherwise the framework won't render the validation error messages next to each item.
            var key = IdsToReuseKey + collectionName;
            var queue = (Queue<string>)httpContext.Items[key];
            if (queue == null)
            {
                httpContext.Items[key] = queue = new Queue<string>();
                if (httpContext.Request.Method == "POST" && httpContext.Request.HasFormContentType)
                {
                    StringValues previouslyUsedIds = httpContext.Request.Form[collectionName + ".index"];
                    if (!string.IsNullOrEmpty(previouslyUsedIds))
                        foreach (var previouslyUsedId in previouslyUsedIds)
                            queue.Enqueue(previouslyUsedId);
                }
            }
            return queue;
        }
        internal class HtmlFieldPrefixScope : IDisposable
        {
            internal readonly TemplateInfo TemplateInfo;
            internal readonly string PreviousHtmlFieldPrefix;
            public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
            {
                TemplateInfo = templateInfo;
                PreviousHtmlFieldPrefix = TemplateInfo.HtmlFieldPrefix;
                TemplateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            }
            public void Dispose()
            {
                TemplateInfo.HtmlFieldPrefix = PreviousHtmlFieldPrefix;
            }
        }
    }
}
