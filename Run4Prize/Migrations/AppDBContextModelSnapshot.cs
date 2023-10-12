﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Run4Prize.Models.DBContexts.AppContext;

#nullable disable

namespace Run4Prize.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.AccessTokenEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AthleteId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("EntityCreateDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EntityUpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("expiresat")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("expiresin")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("refreshtoken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("utcexpireat")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AthleteId")
                        .IsUnique();

                    b.ToTable("Tokens", (string)null);
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.ActivityEntity", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int>("AchievementCount")
                        .HasColumnType("int");

                    b.Property<int>("AthleteCount")
                        .HasColumnType("int");

                    b.Property<long>("AthleteId")
                        .HasColumnType("bigint");

                    b.Property<float>("AverageCadence")
                        .HasColumnType("real");

                    b.Property<float>("AverageHeartrate")
                        .HasColumnType("real");

                    b.Property<float>("AveragePower")
                        .HasColumnType("real");

                    b.Property<float>("AverageSpeed")
                        .HasColumnType("real");

                    b.Property<float>("AverageTemperature")
                        .HasColumnType("real");

                    b.Property<float>("Calories")
                        .HasColumnType("real");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CommentCount")
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Distance")
                        .HasColumnType("real");

                    b.Property<int>("ElapsedTime")
                        .HasColumnType("int");

                    b.Property<float>("ElevationGain")
                        .HasColumnType("real");

                    b.Property<DateTime>("EntityCreateDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EntityUpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GearId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("HasKudoed")
                        .HasColumnType("bit");

                    b.Property<bool>("HasPowerMeter")
                        .HasColumnType("bit");

                    b.Property<bool>("IsCommute")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFlagged")
                        .HasColumnType("bit");

                    b.Property<bool>("IsManual")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTrainer")
                        .HasColumnType("bit");

                    b.Property<float>("Kilojoules")
                        .HasColumnType("real");

                    b.Property<int>("KudosCount")
                        .HasColumnType("int");

                    b.Property<float>("MaxHeartrate")
                        .HasColumnType("real");

                    b.Property<float>("MaxSpeed")
                        .HasColumnType("real");

                    b.Property<int>("MovingTime")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PhotoCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDateLocal")
                        .HasColumnType("datetime2");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("SufferScore")
                        .HasColumnType("float");

                    b.Property<string>("TimeZone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Truncated")
                        .HasColumnType("int");

                    b.Property<int>("WeightedAverageWatts")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AthleteId");

                    b.ToTable("Activities", (string)null);
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.AthleteEntity", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool>("ApproveFollowers")
                        .HasColumnType("bit");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ColorCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedAt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EntityCreateDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EntityUpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Follower")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Friend")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPremium")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Profile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileMedium")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ResourceState")
                        .HasColumnType("int");

                    b.Property<string>("Sex")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdatedAt")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Athletes", (string)null);
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.JobParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ExecuteEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExecuteStartDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsError")
                        .HasColumnType("bit");

                    b.Property<bool>("IsExecuted")
                        .HasColumnType("bit");

                    b.Property<string>("JobName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Parameter")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("JobParameters", (string)null);
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.LogEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("EntityCreateDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EntityUpdateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Logs", (string)null);
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.WeekEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Weeks", (string)null);
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.WeekUserDistanceEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AthleteId")
                        .HasColumnType("bigint");

                    b.Property<float>("Distance")
                        .HasColumnType("real");

                    b.Property<long>("WeekId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AthleteId");

                    b.HasIndex("WeekId");

                    b.ToTable("WeekUserDistances", (string)null);
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.AccessTokenEntity", b =>
                {
                    b.HasOne("Run4Prize.Models.DBContexts.AppContext.AthleteEntity", "Athlete")
                        .WithOne("AccessToken")
                        .HasForeignKey("Run4Prize.Models.DBContexts.AppContext.AccessTokenEntity", "AthleteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Athlete");
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.ActivityEntity", b =>
                {
                    b.HasOne("Run4Prize.Models.DBContexts.AppContext.AthleteEntity", "Athlete")
                        .WithMany("Activities")
                        .HasForeignKey("AthleteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Athlete");
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.WeekUserDistanceEntity", b =>
                {
                    b.HasOne("Run4Prize.Models.DBContexts.AppContext.AthleteEntity", "Athlete")
                        .WithMany("WeekUserDistances")
                        .HasForeignKey("AthleteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Run4Prize.Models.DBContexts.AppContext.WeekEntity", "Week")
                        .WithMany("WeekUserDistances")
                        .HasForeignKey("WeekId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Athlete");

                    b.Navigation("Week");
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.AthleteEntity", b =>
                {
                    b.Navigation("AccessToken");

                    b.Navigation("Activities");

                    b.Navigation("WeekUserDistances");
                });

            modelBuilder.Entity("Run4Prize.Models.DBContexts.AppContext.WeekEntity", b =>
                {
                    b.Navigation("WeekUserDistances");
                });
#pragma warning restore 612, 618
        }
    }
}
