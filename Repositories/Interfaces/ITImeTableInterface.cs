using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ITimeTableInterface : IBaseInterface<TimeTable>
    {
        public Task<List<TimeTable>> GetAllByStandardGroupByDayOfWeek(int standardID);   // dayOfWeek, List<TimeTable>
    }
}