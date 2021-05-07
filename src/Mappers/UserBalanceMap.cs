using System;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Mappers
{
    /// <summary>
    ///     Helper class for <see cref="UserBalance" /> per NHibernate mapping requirement
    /// </summary>
    public class UserBalanceMap:ClassMapping<UserBalance>
    {
        public UserBalanceMap()
        {
            Table("UserBalances");
            Lazy(true);
            Id(x => x.UserBalanceId, map =>
            {
                map.Generator(Generators.Guid);
                map.Column("UserBalanceId");
                map.UnsavedValue(Guid.Empty);
            });
            Property(x => x.UserId, map => { map.NotNullable(true); map.Type(NHibernateUtil.Guid); });
            Property(x => x.Balance, map => { map.NotNullable(true); map.Type(NHibernateUtil.Decimal); });
        }
    }
}
