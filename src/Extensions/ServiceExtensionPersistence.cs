using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using Rickie.Homework.ShowcaseApp.Mappers;
using Rickie.Homework.ShowcaseApp.Persistence;
using Environment = System.Environment;

namespace Rickie.Homework.ShowcaseApp.Extensions
{
    /// <summary>
    ///     Extension class to add persistence infrastructure to current service
    /// </summary>
    public static class ServiceExtensionPersistence
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var mapper = new ModelMapper();


            mapper.AddMappings(typeof(UserMap).Assembly.ExportedTypes);

            var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            var connectionString = configuration.GetConnectionString("MSSQLConnectionLocal");

            // TODO: use AWS SSM and KMS instead for connection string
            if (configuration.GetValue<bool>("IsCloudDeployment"))
                connectionString = Environment.GetEnvironmentVariable("CloudMSSQLConnection");

            var nhConfiguration = new Configuration();
            nhConfiguration.Configure("hibernate.cfg.xml");


            nhConfiguration.DataBaseIntegration(c =>
            {
                c.Dialect<MsSql2012Dialect>();
                c.ConnectionString = connectionString;
                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                c.LogFormattedSql = true;
                c.LogSqlInConsole = true;
            });

            // nhConfiguration.NamedSQLQueries.Add("MakePayment", new NamedSQLQueryDefinition(""));

            nhConfiguration.AddMapping(domainMapping);
            var sessionFactory = nhConfiguration.BuildSessionFactory();

            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());

            // Repositories
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IUserRepositoryAsync, UserRepositoryAsync>();
            services.AddTransient<IUserTokenRepositoryAsync, UserTokenRepositoryAsync>();
            services.AddTransient<IUserBalanceRepositoryAsync, UserBalanceRepositoryAsync>();
            services.AddTransient<IPaymentStatusRepositoryAsync, PaymentStatusRepositoryAsync>();
            services.AddTransient<IPaymentRepositoryAsync, PaymentRepositoryAsync>();
        }
    }
}