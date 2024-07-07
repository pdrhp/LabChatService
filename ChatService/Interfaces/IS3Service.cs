namespace ChatService.Interfaces;

public interface IS3Service
{
    Task<string> UploadFileAsync(string path ,string objectName, Stream data, long size, string contentType);
    
}