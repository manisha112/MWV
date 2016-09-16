using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface IPaperMill
    {
        List<Papermill> GetListOfPaperMill();
        bool AddPaperMill(decimal Capacity, decimal min_width, decimal max_width, string location, decimal deckle_min, decimal deckle_max,
                          int max_cuts, decimal min_diameter, decimal max_diameter, decimal max_weight_child, decimal min_weight_jumbo,
                          decimal max_weight_jumbo, string name, string address,  string created_by);

        bool EditPaperMill(int PaperMillIDToEdit, decimal Capacity, decimal min_width, decimal max_width, string location, decimal deckle_min,
                          decimal deckle_max, int max_cuts, decimal min_diameter, decimal max_diameter, decimal max_weight_child,
                          decimal min_weight_jumbo, decimal max_weight_jumbo, string name, string address,  string modified_by);

        bool DeletePaperMill(int PaperMillIDToDel);


    }
}
