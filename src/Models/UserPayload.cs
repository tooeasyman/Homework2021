using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rickie.Homework.ShowcaseApp.Queries;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents result of <see cref="GetAllUsersQuery" />
    /// </summary>
    public class UserPayload
    {
        public virtual Guid UserId { get; set; }
        public virtual string UserName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }
}
