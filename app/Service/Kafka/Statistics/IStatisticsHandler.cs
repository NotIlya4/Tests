using Service.Model;

namespace Service;

/// <summary>
/// 
/// </summary>
public interface IStatisticsHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="statistics"></param>
    void Publish(Statistics statistics);
}