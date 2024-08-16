using Amazon.S3;
using Amazon.S3.Model;
using Spam;

namespace Service;

public class S3Strategy(IAmazonS3 s3, string bucketName, string content) : ISpammerStrategy
{
    public async Task Execute(RunnerExecutionContext context, CancellationToken cancellationToken)
    {
        await s3.PutObjectAsync(new PutObjectRequest()
        {
            BucketName = bucketName,
            Key = Guid.NewGuid().ToString(),
            ContentBody = content
        }, cancellationToken);
    }
}