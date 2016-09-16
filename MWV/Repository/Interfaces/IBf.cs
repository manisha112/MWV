using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface IBf
    {
        List<Bf> GetBfs();
        bool AddBF(string BFCode, string BFDescription, string created_by);
        bool EditBF(string BFCodeToEdit, string BFCode, string BFDescription, string modified_by);
        bool DeleteBF(string BFCode);
    }
}
