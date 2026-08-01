namespace SeaweedFs.Testcontainers;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SeaweedFsBuilder : ContainerBuilder<SeaweedFsBuilder, SeaweedFsContainer, SeaweedFsConfiguration>
{
    /// <summary>
    /// The SeaweedFS S3 gateway container port.
    /// </summary>
    public const ushort SeaweedFsS3Port = 8333;

    /// <summary>
    /// The default S3 access key.
    /// </summary>
    public const string DefaultAccessKey = "admin";

    /// <summary>
    /// The default S3 secret key.
    /// </summary>
    public const string DefaultSecretKey = "secret";

    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>chrislusf/seaweedfs:4.40</c>). Always pin the tag.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/chrislusf/seaweedfs/tags" />.
    /// </remarks>
    public SeaweedFsBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/chrislusf/seaweedfs/tags" />.
    /// </remarks>
    public SeaweedFsBuilder(IImage image)
        : this(new SeaweedFsConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeaweedFsBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private SeaweedFsBuilder(SeaweedFsConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override SeaweedFsConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Configures both S3 credentials.
    /// </summary>
    /// <param name="accessKey">The S3 access key.</param>
    /// <param name="secretKey">The S3 secret key.</param>
    /// <returns>A configured instance of <see cref="SeaweedFsBuilder" />.</returns>
    public SeaweedFsBuilder WithCredentials(string accessKey, string secretKey)
    {
        return WithAccessKey(accessKey).WithSecretKey(secretKey);
    }

    /// <summary>
    /// Overrides the default S3 access key.
    /// </summary>
    /// <param name="accessKey">The S3 access key.</param>
    /// <returns>A configured instance of <see cref="SeaweedFsBuilder" />.</returns>
    public SeaweedFsBuilder WithAccessKey(string accessKey)
    {
        return Merge(DockerResourceConfiguration, new SeaweedFsConfiguration(accessKey: accessKey))
            .WithEnvironment("AWS_ACCESS_KEY_ID", accessKey);
    }

    /// <summary>
    /// Overrides the default S3 secret key.
    /// </summary>
    /// <param name="secretKey">The S3 secret key.</param>
    /// <returns>A configured instance of <see cref="SeaweedFsBuilder" />.</returns>
    public SeaweedFsBuilder WithSecretKey(string secretKey)
    {
        return Merge(DockerResourceConfiguration, new SeaweedFsConfiguration(secretKey: secretKey))
            .WithEnvironment("AWS_SECRET_ACCESS_KEY", secretKey);
    }

    /// <summary>
    /// Adds a bucket for SeaweedFS to create during startup.
    /// </summary>
    /// <param name="bucket">The bucket to create.</param>
    /// <returns>A configured instance of <see cref="SeaweedFsBuilder" />.</returns>
    public SeaweedFsBuilder WithBucket(string bucket)
    {
        if (string.IsNullOrWhiteSpace(bucket))
        {
            throw new ArgumentException("Bucket must not be blank.", nameof(bucket));
        }

        if (bucket.IndexOf(',') >= 0)
        {
            throw new ArgumentException("Bucket must not contain a comma.", nameof(bucket));
        }

        var builder = Merge(DockerResourceConfiguration, new SeaweedFsConfiguration(buckets: new[] { bucket }));
        var buckets = builder.DockerResourceConfiguration.Buckets;
        return builder.WithEnvironment("S3_BUCKET", string.Join(",", buckets));
    }

    /// <summary>
    /// Adds buckets for SeaweedFS to create during startup.
    /// </summary>
    /// <param name="buckets">The buckets to create.</param>
    /// <returns>A configured instance of <see cref="SeaweedFsBuilder" />.</returns>
    public SeaweedFsBuilder WithBuckets(params string[] buckets)
    {
        if (buckets == null)
        {
            throw new ArgumentNullException(nameof(buckets));
        }

        var builder = this;
        foreach (var bucket in buckets)
        {
            builder = builder.WithBucket(bucket);
        }

        return builder;
    }

    /// <inheritdoc />
    public override SeaweedFsContainer Build()
    {
        Validate();
        return new SeaweedFsContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override SeaweedFsBuilder Init()
    {
        return base.Init()
            .WithPortBinding(SeaweedFsS3Port, true)
            .WithCommand("mini", "-dir=/data")
            .WithAccessKey(DefaultAccessKey)
            .WithSecretKey(DefaultSecretKey)
            // The S3 gateway accepts connections before startup buckets (S3_BUCKET) are
            // created, so waiting on the HTTP endpoint alone is a race. The startup banner
            // is only printed once every component, including bucket creation, is done.
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("All enabled components are running and ready to use"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.AccessKey, nameof(DockerResourceConfiguration.AccessKey))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.SecretKey, nameof(DockerResourceConfiguration.SecretKey))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override SeaweedFsBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SeaweedFsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SeaweedFsBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SeaweedFsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SeaweedFsBuilder Merge(SeaweedFsConfiguration oldValue, SeaweedFsConfiguration newValue)
    {
        return new SeaweedFsBuilder(new SeaweedFsConfiguration(oldValue, newValue));
    }
}
