using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Mappers
{
    /// <summary>
    ///     Helper class for <see cref="PaymentStatuses" /> per NHibernate mapping requirement
    /// </summary>
    public class PaymentStatusesMap : ClassMapping<PaymentStatuses>
    {
        public PaymentStatusesMap()
        {
            Table("PaymentStatuses");
            Lazy(true);
            Id(x => x.PaymentStatusId, map => map.Generator(Generators.Identity));
            Property(x => x.StatusDescription, map => map.Length(50));
            Property(x => x.StatusValue, map => { map.NotNullable(true); map.Length(50); });
        }
    }
}
