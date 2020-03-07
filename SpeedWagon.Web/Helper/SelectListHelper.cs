using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Models;
using System.Collections.Generic;
using System.Linq;

namespace SpeedWagon.Web.Helper
{
    public class SelectListHelper
    {
        public static IList<SelectListItem> GetSelectList(IEnumerable<SpeedWagonContent> items, bool includeEmpty = false)
        {
            IList<SelectListItem> selectList = new List<SelectListItem>();

            if(includeEmpty)
            {
                selectList.Add(new SelectListItem(string.Empty, string.Empty));
            }

            foreach(SpeedWagonContent item in items.OrderBy(x => x.Name))
            {
                selectList.Add(new SelectListItem(item.Name, item.Name));
            }

            return selectList;
        } 

    }
}
