using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using CMS.Core;
using CMS.DocumentEngine;

using Kentico.Content.Web.Mvc;

using MEDIOClinic.Models;
using MEDIOClinic.Models.Menu;

namespace MEDIOClinic.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly IPageRetriever pageRetriever;
        private readonly IPageUrlRetriever urlRetriever;

        public NavigationMenuViewComponent()
        {
            // Initializes instances of required services
            // NOTE: This method of instantiating services is not recommended for
            // real-world projects and is only used for the sake of brevity in this tutorial.
            // Instead, we recommend configuring a dependency injection container to resolve
            // object dependencies (e.g., Autofac). See the Xperience documentation for details.
            pageRetriever = Service.Resolve<IPageRetriever>();
            urlRetriever = Service.Resolve<IPageUrlRetriever>();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieves a collection of page objects with data for the menu (pages of all page types)
            IEnumerable<TreeNode> menuItems = await pageRetriever.RetrieveAsync<TreeNode>(query => query
                                                    // Selects pages that have the 'Show in menu" flag enabled
                                                    .MenuItems()
                                                    // Only loads pages from the first level of the site's content tree
                                                    .NestingLevel(1)
                                                    // Filters the query to only retrieve required columns
                                                    .Columns("DocumentName", "NodeID", "NodeSiteID")
                                                    // Uses the menu item order from the content tree
                                                    .OrderByAscending("NodeOrder"));

            // Creates a collection of view models based on the menu item data
            IEnumerable<MenuItemViewModel> model = menuItems.Select(item => new MenuItemViewModel()
            {
                // Gets the name of the page as the menu item caption text
                MenuItemText = item.DocumentName,
                // Retrieves the URL for the page (as a relative virtual path)
                MenuItemRelativeUrl = urlRetriever.Retrieve(item).RelativePath
            });

            return View(model);
        }
    }
}
