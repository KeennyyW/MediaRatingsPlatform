using System;
using Mrp.Data.Interfaces;
using Mrp.Models;
using Npgsql;

namespace Mrp.Data.Repositories;

public class AdoNetMediaRepository : IMediaRepository
{
    private readonly string ConnectionString;

    public AdoNetMediaRepository(string connectionString) {
    ConnectionString = connectionString;
    }
    public MediaEntry Create(MediaEntry entry)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(@"INSERT INTO media (title, description, media_type, release_year, age_restriction, creator_id)
        VALUES (@title, @description, @mediaType, @releaseYear, @ageRestriction, @creatorId)
        RETURNING id", connection);

        cmd.Parameters.AddWithValue("title", entry.Title);
        cmd.Parameters.AddWithValue("description", entry.Description);
        cmd.Parameters.AddWithValue("mediaType", entry.MediaType);
        cmd.Parameters.AddWithValue("releaseYear", entry.ReleaseYear);
        cmd.Parameters.AddWithValue("ageRestriction", entry.AgeRestriction);
        cmd.Parameters.AddWithValue("creatorId", entry.CreatorId);

        object? result = cmd.ExecuteScalar();
        int newId = (int)result!;
        entry.Id = newId;

        return entry;
    }
    public MediaEntry? GetById(int id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(@"SELECT id, title, description, media_type,
        release_year, age_restriction, creator_id FROM media WHERE id = @id", connection);

        cmd.Parameters.AddWithValue("id", id);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return ReadEntry(reader);
        }
        return null;
    }

    public List<MediaEntry> GetAll()
    {
        var list = new List<MediaEntry>();

        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(@"SELECT id, title, description, media_type,
        release_year, age_restriction, creator_id FROM media", connection);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(ReadEntry(reader));
        }
        return list;
    }

    public bool Update(MediaEntry entry)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand(@"UPDATE media SET title = @title,
        description = @description, media_type = @mediaType, release_year = @releaseYear,
        age_restriction = @ageRestriction WHERE id = @id", connection);

        cmd.Parameters.AddWithValue("id", entry.Id);
        cmd.Parameters.AddWithValue("title", entry.Title);
        cmd.Parameters.AddWithValue("description", entry.Description);
        cmd.Parameters.AddWithValue("mediaType", entry.MediaType);
        cmd.Parameters.AddWithValue("releaseYear", entry.ReleaseYear);
        cmd.Parameters.AddWithValue("ageRestriction", entry.AgeRestriction);

        int rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Delete(int id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand("DELETE FROM media WHERE id = @id", connection);
        cmd.Parameters.AddWithValue("id", id);

        int rows = cmd.ExecuteNonQuery();
        if (rows > 0)
        {
            return true;
        }
        return false;
    }

    private MediaEntry ReadEntry(NpgsqlDataReader reader)
    {
        var entry = new MediaEntry();
        entry.Id = reader.GetInt32(0);
        entry.Title = reader.GetString(1);
        entry.Description = reader.GetString(2);
        entry.MediaType = reader.GetString(3);
        entry.ReleaseYear = reader.GetInt32(4);
        entry.AgeRestriction = reader.GetInt32(5);
        entry.CreatorId = reader.GetInt32(6);
        return entry;
    }
}