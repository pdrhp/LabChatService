using System.ComponentModel.DataAnnotations;
using ChatService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Data;

public class ChatServiceDbContext : IdentityDbContext<User>
{
    public ChatServiceDbContext(DbContextOptions<ChatServiceDbContext> options) : base(options) {}
    
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatGroup> ChatGroups { get; set; }
    public DbSet<ChatRequest> ChatRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ChatRequest>()
            .HasOne<User>(cr => cr.Requester)
            .WithMany(u => u.RequestsAsRequester)
            .HasForeignKey(cr => cr.RequesterId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ChatRequest>()
            .HasOne<User>(cr => cr.Requested)
            .WithMany(u => u.RequestsAsRequested)
            .HasForeignKey(cr => cr.RequestedId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}