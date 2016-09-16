using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MWV.Models;

namespace MWV.DBContext
{
    public class MWVDBContext: DbContext
    {
        public MWVDBContext() : base("DefaultConnection") { }
        // Database.SetInitializer<ElyticsDBContext>(new CreateDatabaseIfNotExists<ElyticsDBContext>());
    
        public static MWVDBContext Create()
        {
            return new MWVDBContext();
        }

        public DbSet<Agent> Agents { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Bf> Bfs { get; set; }
        public DbSet<Gsm> Gsms { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_Products> Order_products { get; set; }
        public DbSet<Papermill> Papermills { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Product_papermill> Product_papermills { get; set; }
        public DbSet<Product_prices> Product_prices { get; set; }
        public DbSet<Shade> Shades { get; set; }
        public DbSet<Truck_dispatches> Truck_dispatches { get; set; }
        public DbSet<Truck_dispatch_details> Truck_dispatch_details { get; set; }
        public DbSet<deckle_approvals> deckle_approvals { get; set; }
        public DbSet<ProductionChild> ProductionChild { get; set; }
        public DbSet<ProductionJumbo> ProductionJumbo { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<ProductionRun> ProductionRun { get; set; }
        public DbSet<ProductionTimeline> ProductionTimeline { get; set; }
        public DbSet<papermill_logs> papermill_logs { get; set; }
        public DbSet<Core> Cores { get; set; }
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<Alerts> Alerts { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public DbSet<Reports> Reports { get; set; }
    }
}