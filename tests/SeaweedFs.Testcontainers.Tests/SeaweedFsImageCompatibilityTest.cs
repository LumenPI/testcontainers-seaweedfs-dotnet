namespace SeaweedFs.Testcontainers.Tests;

/// <summary>
/// Proves that <see cref="SeaweedFsBuilder" /> works unmodified against both the
/// open-source and Enterprise SeaweedFS images: same "mini" command, same
/// environment variables, same startup banner, same S3 gateway behavior.
/// </summary>
public sealed class SeaweedFsImageCompatibilityTest
{
    [Theory]
    [InlineData("chrislusf/seaweedfs:4.40")]
    [InlineData("chrislusf/seaweedfs-enterprise:4.40")]
    public async Task CreatesConfiguredBucketAndAcceptsS3Traffic(string image)
    {
        // Given
        await using var seaweedFs = new SeaweedFsBuilder(image)
            .WithBucket("compat-bucket")
            .Build();

        await seaweedFs.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var config = new AmazonS3Config
        {
            ServiceURL = seaweedFs.GetS3Url(),
            AuthenticationRegion = "us-east-1",
            ForcePathStyle = true,
        };

        using var client = new AmazonS3Client(seaweedFs.GetAccessKey(), seaweedFs.GetSecretKey(), config);

        // When
        var buckets = await client.ListBucketsAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Contains(buckets.Buckets, b => b.BucketName == "compat-bucket");
    }
}
