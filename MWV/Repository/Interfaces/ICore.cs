using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface ICore
    {
        List<Core> GetCore();
        bool AddCore(string CoreCode, string CoreDescription, string created_by);
        bool EditCore(string CoreCodeToEdit, string CoreCode, string CoreDescription, string modified_by);
        bool DeleteCore(string CoreCode);
    }
}
