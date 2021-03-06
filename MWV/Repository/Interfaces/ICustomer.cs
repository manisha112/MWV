﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface ICustomer
    {
        List<Customer> CustListbyAgentId();
        Boolean CreateCustomer(Customer customer);
        string GetProductionPlannerName();
        string GetEmail();
    };
}
