using System.ComponentModel;
using System.Runtime.Serialization;
using Service.Enums;
using Spam;

namespace Service;

public class DbContextStrategyOptionsView
{
    public DbContextStrategyType DbContextStrategyType { get; set; } = DbContextStrategyType.StringEntity;
    public DataCreationStrategyOptionsView DataCreationStrategyOptions { get; set; }
}

