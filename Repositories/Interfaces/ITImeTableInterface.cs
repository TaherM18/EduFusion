using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ITimeTableInterface : IBaseInterface<TimeTable>
    {
        public Task< Dictionary<string, List<TimeTable>> > GetAllByStandardGroupByDayOfWeek(int standardID);   // dayOfWeek, List<TimeTable>
    }
}