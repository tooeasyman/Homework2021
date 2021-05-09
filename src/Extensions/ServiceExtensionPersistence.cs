using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Dialect;
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
        /// <summary>
        ///     Config NHibernate and inject storage access objects
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var mapper = new ModelMapper();


            mapper.AddMappings(typeof(UserMap).Assembly.ExportedTypes);

            var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            
            // Use local development connection string by default
            var connectionString = configuration.GetConnectionString("MSSQLConnectionLocal");
            
            // Use AWS Lambda runtime config for production
            if (configuration.GetValue<bool>("IsCloudDeployment"))
                // TODO: use AWS SSM and KMS instead for connection string
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