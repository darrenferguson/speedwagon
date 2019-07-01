using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Models;
using System.Collections.Generic;

namespace SpeedWagon.Web.Helper
{
    public class SelectListHelper
    {
        public static IList<SelectListItem> GetSelectList(IEnumerable<SpeedWagonContent> items)
        {
            IList<SelectListItem> selectList = new List<SelectListItem>();

            foreach(SpeedWagonContent item in items)
            {
                selectList.Add(new SelectListItem(item.Name, item.Name));
            }

            return selectList;
        } 

    }
}
