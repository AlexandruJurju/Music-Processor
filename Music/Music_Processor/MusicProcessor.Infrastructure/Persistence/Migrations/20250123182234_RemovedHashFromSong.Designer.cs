﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicProcessor.Infrastructure.Persistence;

#nullable disable

namespace MusicProcessor.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250123182234_RemovedHashFromSong")]
    partial class RemovedHashFromSong
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("MusicProcessor.Domain.Entities.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_modified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_albums");

                    b.ToTable("albums", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.Entities.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_modified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_artists");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_artists_name");

                    b.ToTable("artists", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.Entities.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_modified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_genres");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_genres_name");

                    b.ToTable("genres", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.Entities.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int?>("AlbumId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("album_id");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT")
                        .HasColumnName("comment");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_modified");

                    b.Property<double>("Duration")
                        .HasColumnType("REAL")
                        .HasColumnName("duration");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("file_path");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT")
                        .HasColumnName("file_type");

                    b.Property<int?>("GenreId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("genre_id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT")
                        .HasColumnName("title");

                    b.Property<int>("TrackNumber")
                        .HasColumnType("INTEGER")
                        .HasColumnName("track_number");

                    b.Property<int?>("Year")
                        .HasColumnType("INTEGER")
                        .HasColumnName("year");

                    b.HasKey("Id")
                        .HasName("pk_songs");

                    b.HasIndex("AlbumId")
                        .HasDatabaseName("ix_songs_album_id");

                    b.HasIndex("GenreId")
                        .HasDatabaseName("ix_songs_genre_id");

                    b.HasIndex("Title")
                        .HasDatabaseName("ix_songs_title");

                    b.ToTable("songs", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.Entities.Style", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_created");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("TEXT")
                        .HasColumnName("date_modified");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<bool>("RemoveFromSongs")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false)
                        .HasColumnName("remove_from_songs");

                    b.HasKey("Id")
                        .HasName("pk_styles");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_styles_name");

                    b.ToTable("styles", (string)null);
                });

            modelBuilder.Entity("genre_styles", b =>
                {
                    b.Property<int>("GenresId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("genres_id");

                    b.Property<int>("StylesId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("styles_id");

                    b.HasKey("GenresId", "StylesId")
                        .HasName("pk_genre_styles");

                    b.HasIndex("StylesId")
                        .HasDatabaseName("ix_genre_styles_styles_id");

                    b.ToTable("genre_styles", (string)null);
                });

            modelBuilder.Entity("song_artists", b =>
                {
                    b.Property<int>("ArtistsId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("artists_id");

                    b.Property<int>("SongsId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("songs_id");

                    b.HasKey("ArtistsId", "SongsId")
                        .HasName("pk_song_artists");

                    b.HasIndex("SongsId")
                        .HasDatabaseName("ix_song_artists_songs_id");

                    b.ToTable("song_artists", (string)null);
                });

            modelBuilder.Entity("song_styles", b =>
                {
                    b.Property<int>("SongsId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("songs_id");

                    b.Property<int>("StylesId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("styles_id");

                    b.HasKey("SongsId", "StylesId")
                        .HasName("pk_song_styles");

                    b.HasIndex("StylesId")
                        .HasDatabaseName("ix_song_styles_styles_id");

                    b.ToTable("song_styles", (string)null);
                });

            modelBuilder.Entity("MusicProcessor.Domain.Entities.Song", b =>
                {
                    b.HasOne("MusicProcessor.Domain.Entities.Album", "Album")
                        .WithMany("Songs")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_songs_albums_album_id");

                    b.HasOne("MusicProcessor.Domain.Entities.Genre", null)
                        .WithMany("Songs")
                        .HasForeignKey("GenreId")
                        .HasConstraintName("fk_songs_genres_genre_id");

                    b.Navigation("Album");
                });

            modelBuilder.Entity("genre_styles", b =>
                {
                    b.HasOne("MusicProcessor.Domain.Entities.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_genre_styles_genres_genres_id");

                    b.HasOne("MusicProcessor.Domain.Entities.Style", null)
                        .WithMany()
                        .HasForeignKey("StylesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_genre_styles_styles_styles_id");
                });

            modelBuilder.Entity("song_artists", b =>
                {
                    b.HasOne("MusicProcessor.Domain.Entities.Artist", null)
                        .WithMany()
                        .HasForeignKey("ArtistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song_artists_artists_artists_id");

                    b.HasOne("MusicProcessor.Domain.Entities.Song", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song_artists_songs_songs_id");
                });

            modelBuilder.Entity("song_styles", b =>
                {
                    b.HasOne("MusicProcessor.Domain.Entities.Song", null)
                        .WithMany()
                        .HasForeignKey("SongsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song_styles_songs_songs_id");

                    b.HasOne("MusicProcessor.Domain.Entities.Style", null)
                        .WithMany()
                        .HasForeignKey("StylesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song_styles_styles_styles_id");
                });

            modelBuilder.Entity("MusicProcessor.Domain.Entities.Album", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("MusicProcessor.Domain.Entities.Genre", b =>
                {
                    b.Navigation("Songs");
                });
#pragma warning restore 612, 618
        }
    }
}
