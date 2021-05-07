using System;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Mappers
{
    /// <summary>
    ///     Helper class for <see cref="Payment" /> per NHibernate mapping requirement
    /// </summary>
    public class PaymentMap : ClassMapping<Payment>
    {
        public PaymentMap()
        {
            Table("Payments");
            Lazy(true);
            Id(x => x.PaymentId, map =>
            {
                map.Generator(Generators.Guid);
                map.Column("PaymentId");
                map.UnsavedValue(Guid.Empty);
            });
            Property(x => x.UserId, map =>
            {
                map.NotNullable(true);
                map.Type(NHibernateUtil.Guid);
            });
            Property(x => x.Amount, map =>
            {
                map.NotNullable(true);
                map.Type(NHibernateUtil.Decimal);
            });
            Property(x => x.PayTo, map =>
            {
                map.NotNullable(true);
                map.Type(NHibernateUtil.Guid);
            });
            ManyToOne(x => x.Status, map =>
            {
                map.Column("PaymentStatusId");
                map.Cascade(Cascade.All);
                map.Fetch(FetchKind.Select);
            });
            Property(x => x.ClosedReason, map => map.Length(50));
            Property(x => x.Date, map => map.NotNullable(true));
        }
    }
}