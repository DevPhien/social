using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace social.Entites;

public partial class SocialContext : DbContext
{
    public SocialContext()
    {
    }

    public SocialContext(DbContextOptions<SocialContext> options) : base(options)
    {
    }

    public virtual DbSet<social_profile> social_profiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("Server=103.226.251.47;Initial Catalog=SmartOfficeBic;Persist Security Info=False;User ID=sa;Password=#Os1234567BiBi;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<social_profile>(entity =>
        {
            entity.HasKey(e => e.key_id);
            entity.ToTable("social_profile");
            entity.Property(e => e.key_id).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.profile_type).HasMaxLength(25).IsUnicode(false);
            entity.Property(e => e.api_key).HasMaxLength(500).IsUnicode(false);
            entity.Property(e => e.page_id).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.id).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.email).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.birthday).HasColumnType("date");
            entity.Property(e => e.first_name).HasMaxLength(50);
            entity.Property(e => e.last_name).HasMaxLength(50);
            entity.Property(e => e.friend_count);
            entity.Property(e => e.access_token).HasMaxLength(500);
            entity.Property(e => e.refresh_token).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
