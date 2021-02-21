﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NotifcationService;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NotifcationService.Migrations
{
    [DbContext(typeof(NotificationContext))]
    [Migration("20210221121200_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Models.Models.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Head")
                        .HasColumnType("text");

                    b.Property<string>("IncludedSegment")
                        .HasColumnType("text");

                    b.Property<List<Guid>>("IncludedUsersIds")
                        .HasColumnType("uuid[]");

                    b.Property<bool>("IsDelevried")
                        .HasColumnType("boolean");

                    b.Property<int>("NotificationType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });
#pragma warning restore 612, 618
        }
    }
}