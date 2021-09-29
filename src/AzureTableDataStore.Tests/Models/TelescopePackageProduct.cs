using System;
using System.Collections.Generic;

namespace AzureTableDataStore.Tests.Models
{
    public interface ITelescopePackageProduct
    {
        DateTime AddedToInventory { get; set; }
        string CategoryId { get; set; }
        string Description { get; set; }
        Guid InternalReferenceId { get; set; }
        LargeBlob MainImage { get; set; }
        string Name { get; set; }
        int PackageDepthMm { get; set; }
        int PackageHeightMm { get; set; }
        int PackageWidthMm { get; set; }
        string ProductId { get; set; }
        List<string> SearchNames { get; set; }
        long SoldItems { get; set; }
        ProductSpec Specifications { get; set; }
    }

    public class TelescopePackageProduct : ITelescopePackageProduct
    {
        [TableRowKey]
        public string ProductId { get; set; }

        [TablePartitionKey]
        public string CategoryId { get; set; }


        public string Name { get; set; }
        public List<string> SearchNames { get; set; }
        public DateTime AddedToInventory { get; set; }
        public Guid InternalReferenceId { get; set; }
        public long SoldItems { get; set; }

        public int PackageWidthMm { get; set; }
        public int PackageHeightMm { get; set; }
        public int PackageDepthMm { get; set; }

        public string Description { get; set; }
        public LargeBlob MainImage { get; set; }

        public ProductSpec Specifications { get; set; }


    }

    public class TelescopePackageProduct_WithIgnoreProperty
    {
        [TableRowKey]
        public string ProductId { get; set; }
        [TablePartitionKey]
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public List<string> SearchNames { get; set; }
        public DateTime AddedToInventory { get; set; }
        public Guid InternalReferenceId { get; set; }
        public long SoldItems { get; set; }

        public int PackageWidthMm { get; set; }
        public int PackageHeightMm { get; set; }
        public int PackageDepthMm { get; set; }

        [TableIgnoreProperty]
        public string Description { get; set; }
        public LargeBlob MainImage { get; set; }

        [TableIgnoreProperty]
        public ProductSpec Specifications { get; set; }

     
        public ProductSpec_WithIgnoreProperty Specifications2 { get; set; }


        public static TelescopePackageProduct_WithIgnoreProperty Create(TelescopePackageProduct item)
        {
            var newItem = new TelescopePackageProduct_WithIgnoreProperty();
            newItem.AddedToInventory = item.AddedToInventory;
            newItem.CategoryId = item.CategoryId;
            newItem.Description = item.Description;
            newItem.InternalReferenceId = item.InternalReferenceId;
            newItem.MainImage = item.MainImage;
            newItem.Name = item.Name;
            newItem.PackageDepthMm = item.PackageDepthMm;
            newItem.PackageHeightMm = item.PackageHeightMm;
            newItem.PackageWidthMm = item.PackageWidthMm;
            newItem.ProductId = item.ProductId;
            newItem.SearchNames = item.SearchNames;
            newItem.SoldItems = item.SoldItems;
            newItem.Specifications = item.Specifications;
            return newItem;
        }

        public static TelescopePackageProduct_WithIgnoreProperty Create2(TelescopePackageProduct item)
        {
            var newItem = new TelescopePackageProduct_WithIgnoreProperty();
            newItem.AddedToInventory = item.AddedToInventory;
            newItem.CategoryId = item.CategoryId;
            newItem.Description = item.Description;
            newItem.InternalReferenceId = item.InternalReferenceId;
            newItem.MainImage = item.MainImage;
            newItem.Name = item.Name;
            newItem.PackageDepthMm = item.PackageDepthMm;
            newItem.PackageHeightMm = item.PackageHeightMm;
            newItem.PackageWidthMm = item.PackageWidthMm;
            newItem.ProductId = item.ProductId;
            newItem.SearchNames = item.SearchNames;
            newItem.SoldItems = item.SoldItems;
            newItem.Specifications = item.Specifications;
            newItem.Specifications2 = ProductSpec_WithIgnoreProperty.Create(item.Specifications);
            return newItem;
        }



    }

    public interface IProductSpec
    {
        string ApplicationDescription { get; set; }
        bool ForAstrophotography { get; set; }
        bool ForVisualObservation { get; set; }
        MountSpec Mount { get; set; }
        OpticsSpec Optics { get; set; }
        TripodSpec Tripod { get; set; }
    }

    public class ProductSpec : IProductSpec
    {
        public string ApplicationDescription { get; set; }
        public bool ForAstrophotography { get; set; }
        public bool ForVisualObservation { get; set; }

        public OpticsSpec Optics { get; set; }
        public MountSpec Mount { get; set; }
        public TripodSpec Tripod { get; set; }
    }

    public class ProductSpec_WithIgnoreProperty
    {
        [TableIgnoreProperty]
        public string ApplicationDescription { get; set; }
        public bool ForAstrophotography { get; set; }
        public bool ForVisualObservation { get; set; }

        [TableIgnoreProperty]
        public OpticsSpec Optics { get; set; }
        public MountSpec Mount { get; set; }
        public TripodSpec Tripod { get; set; }

        public static ProductSpec_WithIgnoreProperty Create(ProductSpec item)
        {
            var newItem = new ProductSpec_WithIgnoreProperty();
            newItem.ApplicationDescription = item.ApplicationDescription;
            newItem.ForAstrophotography = item.ForAstrophotography;
            newItem.ForVisualObservation = item.ForVisualObservation;
            newItem.Mount = item.Mount;
            newItem.Optics = item.Optics;
            newItem.Tripod = item.Tripod;
            return newItem;
        }

    }


    public class OpticsSpec
    {
        public string Type { get; set; }
        public int ApertureMm { get; set; }
        public int FocalLengthMm { get; set; }
        public double ApertureRatioF { get; set; }
    }

    public enum MountingType
    {
        Azimuthal,
        GermanEquatorial,
        AltAz,
        Dobsonian
    }

    public class MountSpec
    {
        public string Type { get; set; }
        public MountingType Mounting { get; set; }
        public bool GotoControl { get; set; }
        public bool Tracking { get; set; }
    }

    public class TripodSpec
    {
        public string HeightDescription { get; set; }
        public string Material { get; set; }
        public double WeightKg { get; set; }
    }


}