﻿// <auto-generated />
using System;
using JobeSharp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace JobeSharp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20211031083722_IndexOnCreationDateTimeOfTheRunForDeletionTask")]
    partial class IndexOnCreationDateTimeOfTheRunForDeletionTask
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("JobeSharp.Model.Run", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreationDateTimeUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("JobId")
                        .HasColumnType("text");

                    b.Property<string>("LanguageName")
                        .HasColumnType("text");

                    b.Property<string>("SerializedExecutionResult")
                        .HasColumnType("text");

                    b.Property<string>("SerializedTask")
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "CreationDateTimeUtc" }, "IX_CreationDateTimeUtc");

                    b.ToTable("Runs");
                });
#pragma warning restore 612, 618
        }
    }
}
