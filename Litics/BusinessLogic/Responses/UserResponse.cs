using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.BusinessLogic.Responses
{
    public class UserResponse
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string Email { get; set; }
        public bool Locked { get; set; }
    }
}
