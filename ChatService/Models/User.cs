﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ChatService.Models;

public class User : IdentityUser
{
    
    [MaxLength(100)]
    public string Nome { get; set; }
    
    
    public User() : base() { }
}