using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Rickie.Homework.ShowcaseApp.Models;

namespace Rickie.Homework.ShowcaseApp.Mappers
{
    /// <summary>
    ///     Helper class for <see cref="UserMap" /> per NHibernate mapping requirement
    /// </summary>
    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Table("Users");
            Lazy(true);
            Id(x => x.UserId, map =>
            {
                map.Generator(Generators.Guid);
                map.Column("UserId");
                map.UnsavedValue(Guid.Empty);
            });
            Property(x => x.UserName, map =>
            {
                map.NotNullable(true);
                map.Length(50);
            });
            Property(x => x.FirstName, map =>
            {
                map.NotNullable(true);
                map.Length(50);
            });
            Property(x => x.LastName, map => map.Length(50));
            Property(x => x.PasswordHash, map => map.NotNullable(true));
            Property(x => x.PasswordSalt, map => map.NotNullable(true));

            Bag(x => x.UserTokens, colmap =>
            {
                colmap.Key(x => x.Column("UserId"));
                colmap.Inverse(true);
                colmap.Cascade(Cascade.Persist);
            }, map => { map.OneToMany(a => a.Class(typeof(UserToken))); });
            Bag(x => x.Payments, colmap =>
            {
                colmap.Key(x => x.Column("UserId"));
                colmap.Inverse(true);
                colmap.Cascade(Cascade.Persist);
            }, map => { map.OneToMany(a => a.Class(typeof(Payment))); });
        }
    }
}