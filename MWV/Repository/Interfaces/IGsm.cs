using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface IGsm
    {
        List<Gsm> GetGSMs();
        bool AddGsm(string GsmCode, string GsmDescription, string created_by);
        bool EditGsm(string GsmCodeToEdit, string GsmCode, string GsmDescription, string modified_by);
        bool DeleteGsm(string GsmCode);
    }
}
