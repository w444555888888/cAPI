using BookingApi.Utils;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace BookingApi.Utils
{
    // Middleware 負責統一處理 API 錯誤，將 Exception 轉成 JSON 回傳
    public class ErrorHandlingMiddleware
    {
        // 儲存下一個 Middleware 的委派
        private readonly RequestDelegate _next;

        // 建構子：把下一個 Middleware 存起來
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // 每次 HTTP Request 都會呼叫此方法
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // 執行下一個 Middleware / Controller
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        // 將 Exception 轉成標準 JSON 回應
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // 預設狀態碼 500
            var statusCode = (int)HttpStatusCode.InternalServerError;

            // 如果是自訂 AppException，使用它的 StatusCode
            if (ex is AppException appEx)
            {
                statusCode = appEx.StatusCode;
            }

            // JSON 格式回傳
            var response = new
            {
                status = statusCode,
                message = ex.Message ?? "伺服器錯誤",
                success = false,
                data = (object?)null 
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
