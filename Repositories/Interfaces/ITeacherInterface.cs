using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ITeacherInterface : IBaseInterface<Teacher>
    {
        public Task<int> Register(Teacher teacher);
        public Task<List<StudentProgress>> GetStudentProgress();
        public Task<List<StudentRating>> GetStudentRatings();
        public Task<Teacher> GetTeacherBySubject(int c_subjectID);
        public Task<List<TimeTable>> GetTimeTable();
        public Task<int> AddTimeTable(TimeTable timeTable);

        public Task<int> Approve(int tid);
        public Task<int> UnApprove(int tid);
    }
}