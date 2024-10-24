﻿using CliWrap;
using CliWrap.Buffered;
using GUIForCLI.Api.Entities.Zpool;
using Microsoft.AspNetCore.Mvc;
using PRS = GUIForCLI.Parsers;

namespace GUIForCLI.Api.Controllers
{
    [Route("api/zpool")]
    [ApiController]
    public class ZpoolController : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetZpools()
        {
            var result = await Cli.Wrap("sudo")
               .WithArguments("zpool list")
               .ExecuteBufferedAsync();

            var ret = new
            {
                disks = PRS.ZpoolParser.ParseZPool(result.StandardOutput),
                // disks = PRS.ZpoolParser.ParseZPool(@"NAME     SIZE  ALLOC   FREE  CKPOINT  EXPANDSZ   FRAG    CAP  DEDUP    HEALTH  ALTROOT
                // mypool   928G   764K   928G        -         -     0%     0%  1.00x    ONLINE  -"),
                res = result,
            };

            return Ok(ret);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create(AddZpoolReuqest request)
        {
            var result = await Cli.Wrap("sudo")
               .WithArguments($"zpool create {request.PoolName} {request.LogicalName}")
               .ExecuteBufferedAsync();

            if (result.ExitCode == 0)
            {
                var ret = new
                {
                    message = "Pool created successfully",
                    res = result,
                };

                return Ok(ret);
            }
            else
            {
                var ret = new
                {
                    message = "Pool created successfully",
                    res = result,
                };

                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating zpool");
            }
        }

        [HttpPost("remove")]
        public async Task<IActionResult> Remove(AddZpoolReuqest request)
        {
            var result = await Cli.Wrap("sudo")
               .WithArguments($"zpool destroy -f {request.PoolName}")
               .ExecuteBufferedAsync();

            if (result.ExitCode == 0)
            {
                var ret = new
                {
                    message = $"Pool: {request.PoolName} deleted successfully",
                    res = result,
                };

                return Ok(ret);
            }
            else
            {
                var ret = new
                {
                    message = "Pool created successfully",
                    res = result,
                };

                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating zpool");
            }
        }
    }
}
