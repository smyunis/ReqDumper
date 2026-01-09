using Microsoft.AspNetCore.Http.Extensions;

namespace ReqDumper
{
    public class FileLogger
    {
        private readonly string _logPath;
        public FileLogger(IConfiguration configuration)
        {
            _logPath = configuration["DumpPath"] ?? ".";
        }



        public async Task WriteRequestBodyToFileAsync(HttpRequest request, string prefix = "dump")
        {
            var body = await GetRequestBodyStringAsync(request);
            await WriteRequestToFileAsync(body, prefix);
        }
        public async Task WriteRequestToFileAsync(HttpRequest request, string prefix = "dump")
        {
            using var responseWriter = new StringWriter();
            await responseWriter.WriteAsync("************************\n");
            await responseWriter.WriteAsync($"Method :  {request.Method.ToUpper()} \n");
            await responseWriter.WriteAsync($"Path :  {request.GetEncodedPathAndQuery()} \n");


            await responseWriter.WriteAsync("\n******* Headers ********\n");
            foreach (var header in request.Headers)
            {
                await responseWriter.WriteAsync($"{header.Key} : {header.Value} \n");
            }

            await responseWriter.WriteAsync("\n********* Body *********\n");

            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            await responseWriter.WriteAsync(requestBody);


            await responseWriter.WriteAsync("\n********* Time *********\n");
            await responseWriter.WriteAsync(DateTime.Now.ToString());
            await responseWriter.WriteAsync("\n************************\n");

            await WriteRequestToFileAsync(responseWriter.ToString(), prefix);
        }


        private async Task<string> GetRequestBodyStringAsync(HttpRequest request)
        {
            using var requestBodyStream = new StreamReader(request.Body);
            return await requestBodyStream.ReadToEndAsync();
        }

        private async Task WriteRequestToFileAsync(string requestBody, string prefix = "dump")
        {
            string timeStamp = DateTime.Now.ToString("yyyy-MM-dd.HH.mm.ss.fff");
            string uniqueId = Guid.NewGuid().ToString("N");
            string dateStamp = DateTime.Now.ToString("yyyy-MM-dd");
            string directoryPath = Path.Combine(_logPath, "reqdump", prefix, dateStamp);
            Directory.CreateDirectory(directoryPath);
            string fileName = $"dump_{timeStamp}.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            await File.WriteAllTextAsync(filePath, requestBody);
        }
    }
}