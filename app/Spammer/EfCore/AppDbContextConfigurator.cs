using Microsoft.EntityFrameworkCore;

namespace Spammer;

public class AppDbContextConfigurator
{
    private Action<DbContextOptionsBuilder>? _action;

    internal void Configure(DbContextOptionsBuilder builder)
    {
        _action?.Invoke(builder);
    }

    public void Configure(Action<DbContextOptionsBuilder> action)
    {
        _action = action;
    }
}