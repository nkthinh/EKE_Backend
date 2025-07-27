using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.SwipeActions
{
    public interface ISwipeActionRepository : IBaseRepository<SwipeAction>
    {
        Task<IEnumerable<long>> GetSwipedTutorIdsByStudentAsync(long studentId);
        Task<SwipeAction?> GetByStudentAndTutorAsync(long studentId, long tutorId);
        Task<bool> HasStudentSwipedTutorAsync(long studentId, long tutorId);
    }
}
