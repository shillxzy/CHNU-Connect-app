using CHNU_Connect.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        Task<IEnumerable<Schedule>> GetByGroupIdAsync(int groupId);
        Task<IEnumerable<Schedule>> GetBySubGroupIdAsync(int subGroupId);
    }

}
