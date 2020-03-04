using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicShop.ViewModels
{
    public class RoleViewModel
    {
        public string RoleId { get; set; }
        public string CurrentRole { get; set; }
        public string CurrentRoleText { get; set; }
        public IEnumerable<SelectListItem> Name { get; set; }
        public IEnumerable<UserViewModel> Users { get; set; }
    }

    public class UserViewModel
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
    }
    public class UserListViewModel
    {
        public string UserId { get; set; }
        public IEnumerable<SelectListItem> RoleName { get; set; }
        public string UserName { get; set; }

    }
}