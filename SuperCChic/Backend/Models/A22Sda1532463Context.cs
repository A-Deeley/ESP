using System;
using System.Collections.Generic;
using Backend.TransactionBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backend.Models;

public partial class A22Sda1532463Context : DbContext
{
    public A22Sda1532463Context()
    {
    }

    public A22Sda1532463Context(DbContextOptions<A22Sda1532463Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionRow> TransactionRows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseLazyLoadingProxies()
            .UseMySQL("Server=sql.decinfo-cchic.ca;Port=33306;Database=a22_sda_1532463;Uid=dev-1532463;Pwd=Info2020");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("companies");

            entity.HasIndex(e => e.Id, "companies_index_1");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departments");

            entity.HasIndex(e => e.Id, "departments_index_2");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.CompanyId, "company_id");

            entity.HasIndex(e => e.Cup, "cup").IsUnique();

            entity.HasIndex(e => e.DepartmentId, "department_id");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.HasIndex(e => new { e.Id, e.Cup }, "products_index_0");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplyTps)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("apply_tps");
            entity.Property(e => e.ApplyTvq)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)")
                .HasColumnName("apply_tvq");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Cup)
                .HasMaxLength(12)
                .HasColumnName("cup");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DiscountAmt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("discount_amt");
            entity.Property(e => e.DiscountType)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)")
                .HasColumnName("discount_type");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasDefaultValueSql("'0'")
                .HasColumnName("price");
            entity.Property(e => e.Qty)
                .HasDefaultValueSql("'0'")
                .HasColumnName("qty");
            entity.Property(e => e.UnitType)
                .HasMaxLength(50)
                .HasDefaultValueSql("'unit'")
                .HasColumnName("unit_type");

            entity.HasOne(d => d.Company).WithMany(p => p.Products)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("products_ibfk_1");

            entity.HasOne(d => d.Department).WithMany(p => p.Products)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("products_ibfk_2");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transactions");

            entity.HasIndex(e => e.Id, "transactions_index_3");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
        });

        modelBuilder.Entity<TransactionRow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transaction_rows");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.HasIndex(e => e.TransactionId, "transaction_id");

            entity.HasIndex(e => new { e.Id, e.TransactionId }, "transaction_rows_index_4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiscountAmtUnit).HasColumnName("discount_amt_unit");
            entity.Property(e => e.PriceUnit).HasColumnName("price_unit");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QtyUnit).HasColumnName("qty_unit");
            entity.Property(e => e.TpsUnit).HasColumnName("tps_unit");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.TvqUnit).HasColumnName("tvq_unit");

            entity.HasOne(d => d.Product).WithMany(p => p.TransactionRows)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("transaction_rows_ibfk_1");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionRows)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("transaction_rows_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
