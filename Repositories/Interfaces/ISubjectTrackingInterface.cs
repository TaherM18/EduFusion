using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ISubjectTrackingInterface : IBaseInterface<SubjectTracking>
    {
        public Task<List<SubjectTracking>> GetAllByStandard(int standardID);
        public Task<List<SubjectTracking>> GetAllByTeacher(int teacherID);
    }
}