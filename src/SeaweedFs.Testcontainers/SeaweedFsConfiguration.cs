namespace SeaweedFs.Testcontainers;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class SeaweedFsConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsConfiguration" /> class.
    /// </summary>
    /// <param name="accessKey">The S3 access key.</param>
    /// <param name="secretKey">The S3 secret key.</param>
    /// <param name="buckets">Buckets to create during startup.</param>
    public SeaweedFsConfiguration(
        string accessKey = null,
        string secretKey = null,
        IEnumerable<string> buckets = null)
    {
        AccessKey = accessKey;
        SecretKey = secretKey;
        Buckets = buckets?.ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SeaweedFsConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SeaweedFsConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SeaweedFsConfiguration(SeaweedFsConfiguration resourceConfiguration)
        : this(new SeaweedFsConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public SeaweedFsConfiguration(SeaweedFsConfiguration oldValue, SeaweedFsConfiguration newValue)
        : base(oldValue, newValue)
    {
        AccessKey = BuildConfiguration.Combine(oldValue.AccessKey, newValue.AccessKey);
        SecretKey = BuildConfiguration.Combine(oldValue.SecretKey, newValue.SecretKey);
        Buckets = (oldValue.Buckets ?? Array.Empty<string>())
            .Concat(newValue.Buckets ?? Array.Empty<string>())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Gets the S3 access key.
    /// </summary>
    public string AccessKey { get; }

    /// <summary>
    /// Gets the S3 secret key.
    /// </summary>
    public string SecretKey { get; }

    /// <summary>
    /// Gets the buckets created during startup.
    /// </summary>
    public IReadOnlyList<string> Buckets { get; } = Array.Empty<string>();
}
