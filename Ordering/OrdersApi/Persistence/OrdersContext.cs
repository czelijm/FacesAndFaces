﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrdersApi.Models;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistence
{
    public class OrdersContext:DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options):base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new EnumToStringConverter<Status>();
           
            modelBuilder
                .Entity<Order>()
                .Property(p => p.Status)
                .HasConversion(converter);

            //composite key for OrderDetails
            modelBuilder
                .Entity<OrderDetails>()
                .HasKey(k=>new { k.OrderId, k.OrderDetailId});

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }


        public void MigrateDB() 
        {
            Policy.Handle<Exception>().WaitAndRetry(10, r => TimeSpan.FromSeconds(10)).Execute(() => Database.Migrate());
        }
    }
}
