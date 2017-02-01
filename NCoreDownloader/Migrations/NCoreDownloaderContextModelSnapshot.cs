using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NCoreDownloader;

namespace NCoreDownloader.Migrations
{
    [DbContext(typeof(NCoreDownloaderContext))]
    partial class NCoreDownloaderContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("NCoreDownloader.RssItemModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Category");

                    b.Property<string>("Description");

                    b.Property<string>("Link");

                    b.Property<string>("Source");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("RssItems");
                });
        }
    }
}
