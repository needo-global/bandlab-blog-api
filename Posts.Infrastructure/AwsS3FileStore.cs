using System.IO;
using System.Net;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Amazon.S3;
using Amazon.S3.Model;
using Posts.Infrastructure.Abstract;

namespace Posts.Infrastructure;

public class AwsS3FileStore : IStorage
{
    private readonly IAmazonS3 _client = new AmazonS3Client();

    public async Task<string> WriteAsync(string fileName, byte[] content)
    {
        var bucketName = "bandlab-post-dev-data"; // TODO - Move to configuration options
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
                throw new Exception($"The content for file {fileName} is not saved in S3");
            }

            return $"https://s3.amazonaws.com/{bucketName}{fileName}";
        }
        catch (Exception ex)
        {
            throw new Exception($"The content for file {fileName} is not saved in S3");
        }
    }
}