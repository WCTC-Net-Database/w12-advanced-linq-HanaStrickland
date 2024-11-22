﻿// <auto-generated />
using System;
using ConsoleRpgEntities.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    [DbContext(typeof(GameContext))]
    partial class GameContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.35")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AbilityPlayer", b =>
                {
                    b.Property<int>("AbilitiesId")
                        .HasColumnType("int");

                    b.Property<int>("PlayersId")
                        .HasColumnType("int");

                    b.HasKey("AbilitiesId", "PlayersId");

                    b.HasIndex("PlayersId");

                    b.ToTable("PlayerAbilities", (string)null);
                });

            modelBuilder.Entity("ConsoleRpgEntities.Models.Abilities.PlayerAbilities.Ability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AbilityType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Abilities");

                    b.HasDiscriminator<string>("AbilityType").HasValue("Ability");
                });

            modelBuilder.Entity("ConsoleRpgEntities.Models.Characters.Monsters.Monster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AggressionLevel")
                        .HasColumnType("int");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<string>("MonsterType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Monsters");

                    b.HasDiscriminator<string>("MonsterType").HasValue("Monster");
                });

            modelBuilder.Entity("ConsoleRpgEntities.Models.Characters.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<int>("Health")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("ConsoleRpgEntities.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Attack")
                        .HasColumnType("int");

                    b.Property<int>("Defense")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PlayerId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(5,2)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("ConsoleRpgEntities.Models.Abilities.PlayerAbilities.ShoveAbility", b =>
                {
                    b.HasBaseType("ConsoleRpgEntities.Models.Abilities.PlayerAbilities.Ability");

                    b.Property<int>("Damage")
                        .HasColumnType("int");

                    b.Property<int>("Distance")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("ShoveAbility");
                });

            modelBuilder.Entity("ConsoleRpgEntities.Models.Characters.Monsters.Goblin", b =>
                {
                    b.HasBaseType("ConsoleRpgEntities.Models.Characters.Monsters.Monster");

                    b.Property<int>("Sneakiness")
                        .HasColumnType("int");

                    b.HasDiscriminator().HasValue("Goblin");
                });

            modelBuilder.Entity("AbilityPlayer", b =>
                {
                    b.HasOne("ConsoleRpgEntities.Models.Abilities.PlayerAbilities.Ability", null)
                        .WithMany()
                        .HasForeignKey("AbilitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConsoleRpgEntities.Models.Characters.Player", null)
                        .WithMany()
                        .HasForeignKey("PlayersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ConsoleRpgEntities.Models.Item", b =>
                {
                    b.HasOne("ConsoleRpgEntities.Models.Characters.Player", "Player")
                        .WithMany("Items")
                        .HasForeignKey("PlayerId");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("ConsoleRpgEntities.Models.Characters.Player", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
