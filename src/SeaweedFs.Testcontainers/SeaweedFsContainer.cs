namespace SeaweedFs.Testcontainers;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class SeaweedFsContainer : DockerContainer
{
    private readonly SeaweedFsConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public SeaweedFsContainer(SeaweedFsConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the configured S3 access key.
    /// </summary>
    /// <returns>The S3 access key.</returns>
    public string GetAccessKey()
    {
        return _configuration.AccessKey;
    }

    /// <summary>
    /// Gets the configured S3 secret key.
    /// </summary>
    /// <returns>The S3 secret key.</returns>
    public string GetSecretKey()
    {
        return _configuration.SecretKey;
    }

    /// <summary>
    /// Gets the buckets configured for startup.
    /// </summary>
    /// <returns>An immutable view of the configured buckets.</returns>
    public IReadOnlyList<string> GetBuckets()
    {
        return new ReadOnlyCollection<string>(_configuration.Buckets.ToList());
    }

    /// <summary>
    /// Gets the mapped S3 endpoint.
    /// </summary>
    /// <returns>The S3 endpoint as a <see cref="Uri" />.</returns>
    public Uri GetS3Endpoint()
    {
        return new Uri(GetS3Url());
    }

    /// <summary>
    /// Gets the mapped S3 endpoint.
    /// </summary>
    /// <returns>The S3 endpoint as text.</returns>
    public string GetS3Url()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetS3Port()).ToString();
    }

    /// <summary>
    /// Gets the mapped host port for the S3 gateway.
    /// </summary>
    /// <returns>The mapped S3 port.</returns>
    public ushort GetS3Port()
    {
        return (ushort)GetMappedPublicPort(SeaweedFsBuilder.SeaweedFsS3Port);
    }
}
