using System.Collections.Generic;
using Kendo.Mvc.UI;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoleManage
{
    public class TreeViewItemJsonViewModel
    {
        public TreeViewItemJsonViewModel()
        {
            items = new List<TreeViewItemJsonViewModel>();
        }
        public string id { get; set; }
        public string text { get; set; }
        public List<TreeViewItemJsonViewModel> items { get; set; }
        public bool @checked { get; set; }
        public bool expanded { get; set; }
        public bool hasChildren { get; set; }
    }
    public static class TreeViewItemViewModelExtension
    {
        public static TreeViewItemJsonViewModel ToJsonViewModel(this TreeViewItemModel item)
        {
            var result = new TreeViewItemJsonViewModel
            {
                id = item.Id,
                text = item.Text,
                @checked = item.Checked,
                expanded = item.Expanded,
                hasChildren = item.HasChildren
            };
            foreach(var child in item.Items)
            {
                result.items.Add(child.ToJsonViewModel());
            }
            return result;
        }
    }
}