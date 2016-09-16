using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface IProductTimeLine
    {
        List<ProductionTimeline> GetListOfProductTimeLines();
        bool AddProductTimeLine(int PaperMillID, string BFCode, string GSMCode, string ShadeCode, int Speed, decimal TonPerHr, decimal TimePerTon, string created_by);
        bool EditProductTimeLine(int ProductTLIDToEdit, int Speed, decimal TonPerHr, decimal TimePerTon, string modified_by);
        bool DeleteProductTimeLine(int ProductTLIDToDel, int PaperMillID, string BFCode, string GSMCode, string ShadeCode);
    }
}
