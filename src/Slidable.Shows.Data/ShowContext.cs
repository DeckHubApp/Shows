using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace ShtikLive.Shows.Data
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
    }
}