using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Slidable.Shows.Data
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddShowData(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsCallback) =>
            services.AddShowContextPool(optionsCallback)
                .AddScoped<IShowData, ShowData>();
        
        public static IServiceCollection AddShowContextPool(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsCallback) =>
            services.AddDbContextPool<ShowContext>(optionsCallback);
    }
}