using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;


namespace MWV.Repository.Interfaces
{
    interface IShade
    {
        List<Shade> GetShades();
        bool AddShade(string ShadeCode, string ShadeDescription, string created_by);
        bool EditShade(string ShadeCodeToEdit, string ShadeCode, string ShadeDescription, string modified_by);
        bool DeleteShade(string ShadeCode);
    }
}
