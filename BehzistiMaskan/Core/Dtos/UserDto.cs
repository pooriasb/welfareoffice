using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Dtos
{
    public class UserDto
    {

        public string Id { get; set; }

        public string UserName { get; set; }

        public UserInfo UserInfo { get; set; }

        public List<UserRoleDto> UserRoles { get; set; }

    }
}