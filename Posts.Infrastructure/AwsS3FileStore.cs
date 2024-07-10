using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Posts.Infrastructure.Abstract;

namespace Posts.Infrastructure;

public class AwsS3FileStore : IStorage
{
    //private readonly IAmazonS3 _client = new AmazonS3Client();

    public async Task WriteAsync(string bucketName, string fileName, byte[] content)
    {
        //try
        //{
        //    PutObjectResponse response;

        //    await using (Stream stream = new MemoryStream(content))
        //    {
        //        var putRequest = new PutObjectRequest
        //        {
        //            BucketName = bucketName,
        //            Key = fileName,
        //            InputStream = stream
        //        };
        //        response = await _client.PutObjectAsync(putRequest);
        //    }

        //    if (response is not {HttpStatusCode: HttpStatusCode.OK})
        //    {
        //        throw new Exception($"The content for file {fileName} is not saved in S3");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    throw new Exception($"The content for file {fileName} is not saved in S3");
        //}
    }
}