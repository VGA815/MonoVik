using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.ChatMembers;
using MonoVik.WebApi.Chats;
using MonoVik.WebApi.MediaFiles;
using MonoVik.WebApi.Messages;
using MonoVik.WebApi.UserBlocks;
using MonoVik.WebApi.UserPreferences;
using MonoVik.WebApi.Users;
using System;

namespace MonoVik.WebApi.Database
{
    public class ApplicationContext : DbContext
    {
#pragma warning disable
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public virtual DbSet<Chat> Chats { get; set; }

        public virtual DbSet<ChatMember> ChatMembers { get; set; }

        public virtual DbSet<MediaFile> MediaFiles { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserBlock> UserBlocks { get; set; }

        public virtual DbSet<UserPreference> UserPreferences { get; set; }

        public virtual DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
#pragma warning enable
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
