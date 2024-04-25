using Microsoft.EntityFrameworkCore;
using Spammer;

namespace Service;

public class DbContextSpammerOptions
{
    public required bool HotConnectionsBeforeSpam { get; set; }
    public required IDbContextFactory<AppDbContext> DbContextFactory { get; set; }
}