using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Asp.UpgradeTest
{
    public class TestMiddleware
    {
        private readonly RequestDelegate _next;

        public TestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var upgradeFeature = context.Features.Get<IHttpUpgradeFeature>();
            if (upgradeFeature?.IsUpgradableRequest == true)
            {
                var stream = await upgradeFeature.UpgradeAsync();

                var buffer = Encoding.UTF8.GetBytes("Hello World");
                await stream.WriteAsync(buffer, 0, buffer.Length);

                await Task.Delay(100000); //just dont end the request
            }

            await _next(context);
        }
    }
}
