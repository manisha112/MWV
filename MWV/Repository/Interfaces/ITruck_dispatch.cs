using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;
using MWV.Repository.Implementation;

namespace MWV.Repository.Interfaces
{
    interface ITruck_dispatch
    {
        int AddDispatch(Truck_dispatchRepository.tempTruckDispatch Truck_dispatch, List<Truck_dispatchRepository.tempTruckDispatchDetails> lstCargo);
        List<Truck_dispatches> searchResultTransportation(string selectVichleValue, DateTime fromDate, DateTime toDate);
        List<Truck_dispatches> GetTruckInward();
        List<Truck_dispatches> GetTruckOutward();
        Boolean SaveTruckOutward(int id);
        Boolean SaveArrivedVehicle(int id);
        void DeleteVehicles(int tdId);
        List<Truck_dispatches> SearchVehicleByDaysAndVehicleStatus(DateTime dt, string selectedValue);
        List<Truck_dispatches> SearchVehicles(string selectVichleText, DateTime fromDate, DateTime toDate);
        Truck_dispatches GetDispatchBytdid(int? truckdispatchid);
    }
}
