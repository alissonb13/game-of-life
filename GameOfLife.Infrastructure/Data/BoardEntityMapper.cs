using GameOfLife.Business.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GameOfLife.Infrastructure.Data;

public class BoardEntityMapper : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.HasKey(board => board.Id);

        builder.Property(board => board.Rows)
            .IsRequired();

        builder.Property(board => board.Columns)
            .IsRequired();

        builder.Property(board => board.History)
            .HasColumnType("jsonb")
            .IsRequired()
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<BoardState>>(v)!
            );

        builder.ToTable("boards");
    }
}