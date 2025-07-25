﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;

namespace Simple.Ecommerce.Infra.Mapping
{
    public class ProductPhotoMapping : IEntityTypeConfiguration<ProductPhoto>
    {
        public void Configure(EntityTypeBuilder<ProductPhoto> builder)
        {
            builder.ToTable("ProdutosFotos");

            builder.Property(pp => pp.Id).ValueGeneratedOnAdd();
            builder.HasKey(pp => pp.Id);

            builder.Property(pp => pp.ProductId).IsRequired();

            builder.Property(pp => pp.Deleted).IsRequired();
        
            builder.OwnsOne(pp => pp.Photo, p => 
            {  
                p.Property(p => p.FileName).HasColumnName("FileName").IsRequired();
                p.WithOwner();
            });
        }
    }
}
