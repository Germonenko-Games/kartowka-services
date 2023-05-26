﻿// <auto-generated />
using System;
using Kartowka.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kartowka.Migrations
{
    [DbContext(typeof(CoreContext))]
    partial class CoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Kartowka.Core.Models.Pack", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer")
                        .HasColumnName("author_id");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_packs");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_packs_name");

                    b.ToTable("packs", (string)null);
                });

            modelBuilder.Entity("Kartowka.Core.Models.Question", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("character varying(400)")
                        .HasColumnName("content");

                    b.Property<int>("ContentType")
                        .HasColumnType("integer")
                        .HasColumnName("content_type");

                    b.Property<long>("QuestionCategoryId")
                        .HasColumnType("bigint")
                        .HasColumnName("question_category_id");

                    b.Property<int>("QuestionType")
                        .HasColumnType("integer")
                        .HasColumnName("question_type");

                    b.Property<int>("Score")
                        .HasColumnType("integer")
                        .HasColumnName("score");

                    b.HasKey("Id")
                        .HasName("pk_questions");

                    b.ToTable("questions", (string)null);
                });

            modelBuilder.Entity("Kartowka.Core.Models.QuestionsCategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int>("Order")
                        .HasColumnType("integer")
                        .HasColumnName("order");

                    b.Property<long>("RoundId")
                        .HasColumnType("bigint")
                        .HasColumnName("round_id");

                    b.HasKey("Id")
                        .HasName("pk_questions_categories");

                    b.ToTable("questions_categories", (string)null);
                });

            modelBuilder.Entity("Kartowka.Core.Models.Round", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int>("Order")
                        .HasColumnType("integer")
                        .HasColumnName("order");

                    b.Property<long>("PackId")
                        .HasColumnType("bigint")
                        .HasColumnName("pack_id");

                    b.HasKey("Id")
                        .HasName("pk_rounds");

                    b.HasIndex("PackId", "Order")
                        .IsUnique()
                        .HasDatabaseName("ix_rounds_pack_id_order");

                    b.ToTable("rounds", (string)null);
                });

            modelBuilder.Entity("Kartowka.Core.Models.Question", b =>
                {
                    b.HasOne("Kartowka.Core.Models.QuestionsCategory", null)
                        .WithMany("Questions")
                        .HasForeignKey("QuestionCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_questions_questions_categories_questions_category_id");
                });

            modelBuilder.Entity("Kartowka.Core.Models.QuestionsCategory", b =>
                {
                    b.HasOne("Kartowka.Core.Models.Round", null)
                        .WithMany("Categories")
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_questions_categories_rounds_round_id");
                });

            modelBuilder.Entity("Kartowka.Core.Models.Round", b =>
                {
                    b.HasOne("Kartowka.Core.Models.Pack", null)
                        .WithMany("Rounds")
                        .HasForeignKey("PackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_rounds_packs_pack_id");
                });

            modelBuilder.Entity("Kartowka.Core.Models.Pack", b =>
                {
                    b.Navigation("Rounds");
                });

            modelBuilder.Entity("Kartowka.Core.Models.QuestionsCategory", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("Kartowka.Core.Models.Round", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
