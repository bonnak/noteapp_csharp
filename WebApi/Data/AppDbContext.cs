using System;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
