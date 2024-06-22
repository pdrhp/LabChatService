using System.ComponentModel.DataAnnotations;
using ChatService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Data;

public class ChatServiceDbContext : IdentityDbContext<User>
{
    public ChatServiceDbContext(DbContextOptions<ChatServiceDbContext> options) : base(options) {}
    
    public DbSet<ChatMessage> ChatMessages { get; set; }
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

        modelBuilder.Entity<ChatMessage>()
            .HasOne<User>(cm => cm.Sender)
            .WithMany(u => u.MessagesAsSender)
            .HasForeignKey(cm => cm.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ChatMessage>()
            .HasOne<User>(cm => cm.Receiver)
            .WithMany(u => u.MessagesAsReceiver)
            .HasForeignKey(cm => cm.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ChatMessage>()
            .HasOne<ChatRequest>(cm => cm.ChatRequest)
            .WithMany(cr => cr.Messages)
            .HasForeignKey(cm => cm.ChatRequestId);
    }
}