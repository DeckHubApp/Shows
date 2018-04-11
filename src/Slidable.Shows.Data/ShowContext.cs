using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Slidable.Shows.Data
{
    public class ShowContext : DbContext
    {
        public ShowContext([NotNull] DbContextOptions options) : base(options ?? throw new ArgumentNullException(nameof(options)))
        {
        }

        public DbSet<Show> Shows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Show>()
                .HasIndex(s => new { s.Presenter, s.Slug })
                .IsUnique();
        }

        public async Task UpdateHighestShown(string place, string presenter, string slug, int shown)
        {
            await Database.ExecuteSqlCommandAsync(
                "UPDATE Shows SET HighestSlideShown = {0} WHERE Place = {1} AND Presenter = {2} AND Slug = {3} AND HighestSlideShown < {0}",
                shown, place, presenter, slug);
        }
    }
}