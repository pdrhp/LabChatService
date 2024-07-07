using ChatService.Configuration;
using ChatService.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace ChatService.Services;

public class MinioService : IS3Service
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _minioSettings;
    

    public MinioService(IOptions<MinioSettings> settings)
    {
        _minioSettings = settings.Value;

        _minioClient = new MinioClient()
            .WithEndpoint(_minioSettings.Endpoint)
            .WithCredentials(_minioSettings.AccessKey, _minioSettings.SecretKey)
            .Build();
    }
    
    public async Task<string> UploadFileAsync(string path ,string objectName, Stream data, long size, string contentType)
    {
        try
        {
            bool bucketFound = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_minioSettings.BucketName));
            
            if (!bucketFound)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_minioSettings.BucketName));
            }

            string filePath = string.IsNullOrEmpty(path) ? objectName : $"{path}/{objectName}";

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_minioSettings.BucketName)
                .WithObject(filePath)
                .WithStreamData(data)
                .WithObjectSize(size)
                .WithContentType(contentType));

            string fileUrl = $"{_minioSettings.Endpoint}/{_minioSettings.BucketName}/{filePath}";

            return fileUrl;
        }
        catch (Exception e)
        {
            string debug = e.Message;
            throw;
        }
    }
}