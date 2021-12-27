using OrchardCore.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Dto
{
    public class GetAllUserInput : PagedReqest
    {
        public string SearchText
        {
            get;
            set;
        }


        public UsersOrder Order
        {
            get;
            set;
        }
        public UsersFilter Filter
        {
            get;
            set;
        }


        public string SelectedRole
        {
            get;
            set;
        } 

    }
}
