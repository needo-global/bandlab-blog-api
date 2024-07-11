using System.IO;
using System.Net;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Posts.Infrastructure.Abstract;

namespace Posts.Infrastructure;

public class AwsS3FileStore : IStorage
{
    private readonly ILogger<AwsS3FileStore> _logger;
    private readonly IAmazonS3 _client = new AmazonS3Client();

    private readonly string bucketName = "bandlab-post-dev-data"; // TODO - Move to configuration options

    public AwsS3FileStore(ILogger<AwsS3FileStore> logger)
    {
        _logger = logger;
    }

    public async Task<string> WriteAsync(string fileName, byte[] content)
    {
        try
        {
            PutObjectResponse response;

            await using (var stream = new MemoryStream(content))
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                    InputStream = stream
                };
                response = await _client.PutObjectAsync(putRequest);
            }

            if (response is not {HttpStatusCode: HttpStatusCode.OK})
            {
                Console.WriteLine($"Response status code - {response.HttpStatusCode}");
                _logger.LogWarning($"Response status code - {response.HttpStatusCode}");
                throw new Exception($"The content for file {fileName} is not saved in S3");
            }

            return $"https:{bucketName}.s3.ap-southeast-2.amazonaws.com/{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error - {ex.Message}");
            throw new Exception($"The content for file {fileName} is not saved in S3");
        }
    }

    public async Task<byte[]> ReadAsync(string fileName)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = fileName,
        };

        try
        {
            GetObjectResponse response;
            using (response = await _client.GetObjectAsync(request))
            {
                if (response is not { HttpStatusCode: HttpStatusCode.OK })
                {
                    _logger.LogWarning($"Response status code - {response.HttpStatusCode}");
                    throw new Exception($"The content for file {fileName} cannot be read from S3");
                }

                await using (var stream = new MemoryStream())
                {
                   await response.ResponseStream.CopyToAsync(stream);
                   return stream.ToArray();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error - {ex.Message}");
            throw new Exception($"The content for file {fileName} cannot be read from S3");
        }
    }
}