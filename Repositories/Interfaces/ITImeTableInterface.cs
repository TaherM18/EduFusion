using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ITImeTableInterface : IBaseInterface<TimeTable>
    {
        public Task< Dictionary<string, List<TimeTable>> > GetAllGroupByDayOfWeek();   // dayOfWeek, List<TimeTable>
    }
}