using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LandOfRails_Website.Models
{
    public partial class landofrails_websiteContext : DbContext
    {
        public landofrails_websiteContext()
        {
        }

        public landofrails_websiteContext(DbContextOptions<landofrails_websiteContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Download> Downloads { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql($"server=landofrails.net;uid=landofrails_website;pwd={File.ReadAllLines("Sensitive-data")[1]};database=landofrails_website", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.12-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Download>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.BackgroundImageLink).HasColumnType("text");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DownloadLink)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("text");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
