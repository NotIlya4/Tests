using Microsoft.EntityFrameworkCore;

namespace Spam;

public class AppDbContextConfigurator
{
    private Action<DbContextOptionsBuilder>? _action;

    public void Configure(Action<DbContextOptionsBuilder> action)
    {
        _action = action;
    }

    internal void ApplyToDbContextOptionsBuilder(DbContextOptionsBuilder builder)
    {
        _action?.Invoke(builder);
    }
}