namespace SeaweedFs.Testcontainers.Tests;

public sealed class SeaweedFsContainerTest : IAsyncLifetime
{
    private const string Image = "chrislusf/seaweedfs:4.40";

    private readonly SeaweedFsContainer _seaweedFsContainer = new SeaweedFsBuilder(Image)
        .WithBucket("test-bucket")
        .Build();

    public async ValueTask InitializeAsync()
    {
        await _seaweedFsContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _seaweedFsContainer.DisposeAsync();
    }

    [Fact]
    public async Task StoresAndReadsAnObjectUsingS3()
    {
        // Given
        const string bucket = "test-bucket";
        const string key = "hello.txt";
        const string content = "Hello SeaweedFS";

        using var client = CreateClient(_seaweedFsContainer);

        // When
        var buckets = await client.ListBucketsAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using (var inputStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
        {
            await client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = inputStream,
            }, TestContext.Current.CancellationToken).ConfigureAwait(true);
        }

        using var response = await client.GetObjectAsync(bucket, key, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);
        using var reader = new StreamReader(response.ResponseStream);
        var downloaded = await reader.ReadToEndAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Contains(buckets.Buckets, b => b.BucketName == bucket);
        Assert.Equal(content, downloaded);
    }

    [Fact]
    public async Task AuthenticatesWithCustomCredentials()
    {
        // Given
        await using var customContainer = new SeaweedFsBuilder(Image)
            .WithCredentials("custom-access-key", "custom-secret-key")
            .WithBucket("custom-bucket")
            .Build();

        await customContainer.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var client = CreateClient(customContainer);

        // When
        var buckets = await client.ListBucketsAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Contains(buckets.Buckets, b => b.BucketName == "custom-bucket");
    }

    private static AmazonS3Client CreateClient(SeaweedFsContainer container)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = container.GetS3Url(),
            AuthenticationRegion = "us-east-1",
            ForcePathStyle = true,
        };

        return new AmazonS3Client(container.GetAccessKey(), container.GetSecretKey(), config);
    }
}
