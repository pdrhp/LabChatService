﻿using System.ComponentModel.DataAnnotations;
using ChatService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Data;

public class ChatServiceDbContext : IdentityDbContext<User>
{
    public ChatServiceDbContext(DbContextOptions<ChatServiceDbContext> options) : base(options) {}
    
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatGroup> ChatGroups { get; set; }
    
    
}