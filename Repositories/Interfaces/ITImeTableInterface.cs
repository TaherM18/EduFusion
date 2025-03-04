using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ITimeTableInterface : IBaseInterface<TimeTable>
    {
        public Task<List<TimeTable>> GetAllByStandard(int standardID);
        public Task<List<TimeTable>> GetAllByTeacher(int teacherID);
    }
}